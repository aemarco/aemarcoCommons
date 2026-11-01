using aemarcoCommons.ToolboxHangfire;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using System.Linq;
#pragma warning disable IDE0130

namespace ToolboxHangfireTests;

public class DisableMultipleQueuedItemsAttributeTests
{

    private const string TimestampKey = "Timestamp";

    private Dictionary<string, Dictionary<string, string>> _hashes = null!;
    private IStorageConnection _connection = null!;
    private JobStorage _storage = null!;

    [SetUp]
    public void SetUp()
    {
        _hashes = new Dictionary<string, Dictionary<string, string>>();
        _storage = Substitute.For<JobStorage>();

        //in memory stand-in for the hash part of the storage
        _connection = Substitute.For<IStorageConnection>();
        _connection
            .AcquireDistributedLock(Arg.Any<string>(), Arg.Any<TimeSpan>())
            .Returns(Substitute.For<IDisposable>());
        _connection
            .GetAllEntriesFromHash(Arg.Any<string>())
            .Returns(x => _hashes.GetValueOrDefault(x.Arg<string>()));
        _connection
            .When(x => x.SetRangeInHash(Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, string>>>()))
            .Do(x => _hashes[x.ArgAt<string>(0)] = x.ArgAt<IEnumerable<KeyValuePair<string, string>>>(1)
                .ToDictionary(y => y.Key, y => y.Value));

        var transaction = Substitute.For<IWriteOnlyTransaction>();
        transaction
            .When(x => x.RemoveHash(Arg.Any<string>()))
            .Do(x => _hashes.Remove(x.Arg<string>()));
        _connection.CreateWriteTransaction().Returns(transaction);
    }

    [TearDown]
    public void TearDown()
    {
        _connection.Dispose();
    }


    //OnCreating
    [Test]
    public void OnCreating_NoFingerprint_AddsFingerprintAndDoesNotCancel()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        var context = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));

        //act
        attribute.OnCreating(context);

        //assert
        context.Canceled.ShouldBeFalse();
        _hashes.Count.ShouldBe(1);
    }

    [Test]
    public void OnCreating_SameJobWhileFingerprintActive_Cancels()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeTrue();
        _hashes.Count.ShouldBe(1);
    }

    [Test]
    public void OnCreating_DifferentParameters_DoesNotCancel()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(2, "a")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeFalse();
        _hashes.Count.ShouldBe(2);
    }

    [Test]
    public void OnCreating_IgnoreParameters_DifferentParameters_Cancels()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute(ignoreParameters: true);
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(2, "b")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeTrue();
    }

    [Test]
    public void OnCreating_DifferentJobMethods_DoesNotCancel()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute(ignoreParameters: true);
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.RunOther(1, "a")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeFalse();
    }

    [Test]
    public void OnCreating_FingerIgnoreParameter_DifferingValueIsIgnored()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.RunIgnoring(1, 5))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.RunIgnoring(2, 5)));
        var third = GetCreatingContext(Job.FromExpression(() => JobTarget.RunIgnoring(2, 6)));

        //act
        attribute.OnCreating(second);
        attribute.OnCreating(third);

        //assert
        second.Canceled.ShouldBeTrue();
        third.Canceled.ShouldBeFalse();
    }

    [Test]
    public void OnCreating_JsonFingerWithProperty_OnlyThatPropertyCounts()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.RunJson(new Payload(1, "a")))));
        var sameId = GetCreatingContext(Job.FromExpression(() => JobTarget.RunJson(new Payload(1, "b"))));
        var otherId = GetCreatingContext(Job.FromExpression(() => JobTarget.RunJson(new Payload(2, "a"))));

        //act
        attribute.OnCreating(sameId);
        attribute.OnCreating(otherId);

        //assert
        sameId.Canceled.ShouldBeTrue();
        otherId.Canceled.ShouldBeFalse();
    }

    [Test]
    public void OnCreating_NullParameter_IsFingerprinted()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, null!))));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, null!)));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeTrue();
    }

    [Test]
    public void OnCreating_FingerprintOlderThanLockTimeout_DoesNotCancel()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute(jobLockTimeoutHours: 1);
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        _hashes.Values.Single()[TimestampKey] = DateTimeOffset.UtcNow.AddHours(-2).ToString("o");
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeFalse();
    }

    [Test]
    public void OnCreating_FingerprintWithoutTimestamp_DoesNotCancel()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute();
        attribute.OnCreating(GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a"))));
        _hashes.Values.Single().Remove(TimestampKey);
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));

        //act
        attribute.OnCreating(second);

        //assert
        second.Canceled.ShouldBeFalse();
    }

    [Test]
    public void OnCreating_PreventionModeNone_NeitherCancelsNorStoresFingerprint()
    {
        //arrange
        var attribute = new DisableMultipleQueuedItemsAttribute(preventionMode: PreventionMode.None);
        var first = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));
        var second = GetCreatingContext(Job.FromExpression(() => JobTarget.Run(1, "a")));

        //act
        attribute.OnCreating(first);
        attribute.OnCreating(second);

        //assert
        first.Canceled.ShouldBeFalse();
        second.Canceled.ShouldBeFalse();
        _hashes.ShouldBeEmpty();
    }


    //OnPerformed
    [TestCase(PreventionMode.CreatedTillPerformed, true)]
    [TestCase(PreventionMode.CreatedTillFinal, false)]
    public void OnPerformed_ReleasesFingerprintOnlyForCreatedTillPerformed(PreventionMode mode, bool released)
    {
        //arrange
        var job = Job.FromExpression(() => JobTarget.Run(1, "a"));
        var attribute = new DisableMultipleQueuedItemsAttribute(preventionMode: mode);
        attribute.OnCreating(GetCreatingContext(job));

        //act
        attribute.OnPerformed(GetPerformedContext(job));

        //assert
        _hashes.Count.ShouldBe(released ? 0 : 1);
    }


    //OnStateElection
    [Test]
    public void OnStateElection_DeletedState_ReleasesFingerprintInEveryMode(
        [Values(PreventionMode.CreatedTillPerformed, PreventionMode.CreatedTillFinal)] PreventionMode mode)
    {
        //arrange
        var job = Job.FromExpression(() => JobTarget.Run(1, "a"));
        var attribute = new DisableMultipleQueuedItemsAttribute(preventionMode: mode);
        attribute.OnCreating(GetCreatingContext(job));

        //act
        attribute.OnStateElection(GetElectStateContext(job, new DeletedState()));

        //assert
        _hashes.ShouldBeEmpty();
    }

    [TestCase(PreventionMode.CreatedTillFinal, true, true)]
    [TestCase(PreventionMode.CreatedTillFinal, false, false)]
    [TestCase(PreventionMode.CreatedTillPerformed, true, false)]
    public void OnStateElection_FinalState_ReleasesFingerprintOnlyForCreatedTillFinal(
        PreventionMode mode,
        bool isFinalState,
        bool released)
    {
        //arrange
        var job = Job.FromExpression(() => JobTarget.Run(1, "a"));
        var attribute = new DisableMultipleQueuedItemsAttribute(preventionMode: mode);
        attribute.OnCreating(GetCreatingContext(job));
        IState state = isFinalState
            ? new SucceededState(null, 0, 0)
            : new EnqueuedState();

        //act
        attribute.OnStateElection(GetElectStateContext(job, state));

        //assert
        _hashes.Count.ShouldBe(released ? 0 : 1);
    }


    private CreatingContext GetCreatingContext(Job job) =>
        new(new CreateContext(_storage, _connection, job, new EnqueuedState()));

    private PerformedContext GetPerformedContext(Job job) =>
        new(
            new PerformContext(
                _storage,
                _connection,
                new BackgroundJob("1", job, DateTime.UtcNow),
                Substitute.For<IJobCancellationToken>()),
            null,
            false,
            null);

    private ElectStateContext GetElectStateContext(Job job, IState state) =>
        new(
            new ApplyStateContext(
                _storage,
                _connection,
                Substitute.For<IWriteOnlyTransaction>(),
                new BackgroundJob("1", job, DateTime.UtcNow),
                state,
                null));

}

public record Payload(int Id, string Name);

public static class JobTarget
{
    public static void Run(int number, string text) { }
    public static void RunOther(int number, string text) { }
    public static void RunIgnoring([FingerIgnore] int ignored, int counted) { }
    public static void RunJson([JsonFinger("Id")] Payload payload) { }
}
