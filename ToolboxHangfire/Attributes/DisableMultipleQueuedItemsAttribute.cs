using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable IDE0130

//based on https://discuss.hangfire.io/t/recurring-jobs-dont-queue-when-already-in-queue/1969/3
//pimped by aemarco

namespace aemarcoCommons.ToolboxHangfire;

public class DisableMultipleQueuedItemsAttribute : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter
{
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(10);

    private readonly TimeSpan _jobLockTimeout;
    private readonly bool _ignoreParameters;
    private readonly PreventionMode _preventionMode;


    /// <summary>
    /// Prevent planning another job instance until initial one proceeded.
    /// By default, prevention will
    /// - last one hour, unless defined by "jobLockTimeoutHours"
    /// - will prevent only with same parameter values, unless "ignoreParameters"
    /// - will prevent until performed first time, unless defined otherwise by "preventionMode"
    /// For fingerprinting, parameters are considered, unless JsonFingerAttribute is used on parameters
    /// </summary>
    /// <param name="jobLockTimeoutHours">Hours till lock is being released, defaults to 1</param>
    /// <param name="ignoreParameters">Defines if prevention should ignore parameters, defaults to false</param>
    /// <param name="preventionMode">Defines the start and end of prevention, defaults to CreatedTillPerformed</param>
    public DisableMultipleQueuedItemsAttribute(int jobLockTimeoutHours = 1, bool ignoreParameters = false, PreventionMode preventionMode = PreventionMode.CreatedTillPerformed)
    {
        _jobLockTimeout = TimeSpan.FromHours(jobLockTimeoutHours);
        _ignoreParameters = ignoreParameters;
        _preventionMode = preventionMode;
    }


    public void OnCreating(CreatingContext filterContext)
    {
        if (_preventionMode == PreventionMode.None)
            return;

        if (!AddFingerprintIfNotExists(filterContext.Connection, filterContext.Job))
        {
            filterContext.Canceled = true;
        }
    }
    void IClientFilter.OnCreated(CreatedContext filterContext) { }
    void IServerFilter.OnPerforming(PerformingContext filterContext) { }
    public void OnPerformed(PerformedContext filterContext)
    {
        if (_preventionMode == PreventionMode.CreatedTillPerformed)
        {
            RemoveFingerprint(filterContext.Connection, filterContext.BackgroundJob.Job);
        }
    }
    public void OnStateElection(ElectStateContext context)
    {
        //a deleted job never reaches OnPerformed - release it here, whatever the mode
        if (context.CandidateState is DeletedState ||
            (_preventionMode == PreventionMode.CreatedTillFinal && context.CandidateState.IsFinal))
        {
            RemoveFingerprint(context.Connection, context.BackgroundJob.Job);
        }
    }



    private bool AddFingerprintIfNotExists(IStorageConnection connection, Job job)
    {
        var finger = new HangFinger(job, _ignoreParameters);
        using (connection.AcquireDistributedLock(finger.FingerLock, LockTimeout))
        {
            var fingerprint = connection.GetAllEntriesFromHash(finger.FingerKey);

            if (fingerprint != null &&
                fingerprint.ContainsKey(HangFinger.TimestampKey) &&
                DateTimeOffset.TryParse(fingerprint[HangFinger.TimestampKey], null, DateTimeStyles.RoundtripKind, out var timestamp) &&
                DateTimeOffset.UtcNow <= timestamp.Add(_jobLockTimeout))
            {
                // Actual fingerprint found, returning.
                return false;
            }

            // Fingerprint
            // does not exist,
            // it is invalid (no `Timestamp` key),
            // or it is not actual (timeout expired).
            connection.SetRangeInHash(finger.FingerKey, finger.ToFingerDictionary());
            return true;
        }
    }

    private void RemoveFingerprint(IStorageConnection connection, Job job)
    {
        var finger = new HangFinger(job, _ignoreParameters);
        using IDisposable distributedLock = connection.AcquireDistributedLock(finger.FingerLock, LockTimeout);
        using var transaction = connection.CreateWriteTransaction();
        transaction.RemoveHash(finger.FingerKey);
        transaction.Commit();
    }


    private class HangFinger
    {
        public const string TimestampKey = "Timestamp";

        public HangFinger(Job job, bool ignoreParameters)
        {
            var jobType = job.Type is null
                ? "noJobType"
                : job.Type.Name;
            var jobMethod = job.Method is null
                ? "noJobMethod"
                : job.Method.Name;


            if (ignoreParameters)
            {
                Fingerprint = $"{jobType}.{jobMethod}";
                return;
            }

            var jobParameters = job.Args is null
                ? "noJobParameters"
                : string.Join(".", GetParameterSignatures(job.Args, job.Method!));
            Fingerprint = $"{jobType}.{jobMethod}.{jobParameters}";
        }

        //build up parameter signature
        private static IEnumerable<string> GetParameterSignatures(IReadOnlyList<object> args, MethodInfo method)
        {
            //if (method == null) return args.Select(x => x.ToString());
            var result = new List<string>();
            var parameters = method.GetParameters();
            for (var i = 0; i < parameters.Length; i++)
            {
                var item = args[i];
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (item is null)
                {
                    result.Add("null");
                    continue;
                }

                var fingerIgnore = (FingerIgnoreAttribute?)parameters[i]
                    .GetCustomAttributes(typeof(FingerIgnoreAttribute), true)
                    .FirstOrDefault();
                if (fingerIgnore is not null)
                {
                    continue;
                }

                var jsonIgnoreAttribute = (JsonFingerAttribute?)parameters[i]
                    .GetCustomAttributes(typeof(JsonFingerAttribute), true)
                    .FirstOrDefault();
                if (jsonIgnoreAttribute is not null)
                {
                    result.Add(jsonIgnoreAttribute.CreateSignature(item));
                    continue;
                }
                result.Add(item.ToString() ?? "null");
            }
            return result;
        }


        //readable Fingerprint, used for locking while adding or removing the finger
        public string FingerLock => $"{nameof(FingerLock)}:{GetMd5Hash(Fingerprint)}";

        //unreadable Fingerprint, used as Key in Database because of length limitation
        public string FingerKey => $"{nameof(FingerKey)}:{GetMd5Hash(Fingerprint)}";

        //readable Fingerprint
        public string Fingerprint { get; }

        private static string GetMd5Hash(string text)
        {
            var hash = MD5.HashData(Encoding.Default.GetBytes(text));
            return BitConverter.ToString(hash);
        }

        //fingers values... timestamp for expiration, fingerprint for readability
        public Dictionary<string, string> ToFingerDictionary()
        {
            var result = new Dictionary<string, string>
            {
                { TimestampKey, DateTimeOffset.UtcNow.ToString("o") },
                { nameof(Fingerprint), Fingerprint}
            };
            return result;
        }
    }

}

public enum PreventionMode
{
    None,
    CreatedTillPerformed,
    CreatedTillFinal
}
