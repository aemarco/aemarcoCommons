using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.SchedulerTools
{
    /// <summary>
    /// class for time-scheduling tasks to run
    /// kind of scheduling:
    /// - 
    /// 
    /// </summary>
    public class Scheduler
    {

        #region fields

        //protected static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        protected List<TaskObject> _openTasks = new List<TaskObject>();

        protected bool _loopRunning = false;
        protected CancellationTokenSource _abortWaiting = new CancellationTokenSource();
        protected Task _mainLoopTask = null;



        #endregion

        #region log

        public event EventHandler<NewLogEventArgs> NewLog;
        protected virtual void OnNewLog(string prio, string message, Exception ex = null)
        {
            NewLog?.Invoke(this, new NewLogEventArgs(prio, message, ex));
        }

        #endregion

        #region Mainloop

        private void EnsureLoopRunning()
        {
            //start worker loop if not already running.
            if (!_loopRunning)
            {
                _loopRunning = true;
                _mainLoopTask = Task.Run(() => MainLoop());
            }
            else if (!_abortWaiting.IsCancellationRequested)
            {
                _abortWaiting.Cancel();
            }
        }

        protected virtual async Task MainLoop()
        {
            while (_loopRunning)
            {
                //renew _abortWaiting
                if (_abortWaiting.IsCancellationRequested)
                {
                    _abortWaiting = new CancellationTokenSource();
                }

                //break if nothing to do
                var task = _openTasks.OrderBy(x => x.PlannedFor).FirstOrDefault();
                if (task == null)
                {
                    _loopRunning = false;
                    break;
                }


                //means we have some planned stuff
                var now = DateTimeOffset.Now;
                if (task.PlannedFor <= now) //due
                {

                    //maybe replan if we are outside working hours
                    if (task.SchedulingOptions.HasFlag(SchedulingOptions.DelayUntilNextTimeWindow) &&
                        UpdateToWaitTillServiceHours(task))
                    {
                        OnNewLog("Info", $"Task {task.Name} replanned for {task.PlannedFor.LocalDateTime.ToShortDateString()} {task.PlannedFor.LocalDateTime.ToShortTimeString()}");
                        continue;
                    }

                    _openTasks.Remove(task);
                    OnNewLog("Debug", $"Task \"{task.Name}\" starting.");
                    //perform
                    try
                    {
                        //Execute
                        task.LastStarted = DateTimeOffset.Now;
                        task.PlannedAction();

                        //happy :)
                        task.RetryCount = 0;
                        OnNewLog("Debug", $"Task \"{task.Name}\" done.");
                    }
                    catch (Exception ex)
                    {
                        OnNewLog("Error", $"Scheduled Task \"{task.Name}\" ended with Exception", ex);

                        if (task.SchedulingOptions.HasFlag(SchedulingOptions.RetryOnException))
                        {
                            if (task.RetryCount < task.MaxRetries)
                            {
                                task.PlannedFor = DateTimeOffset.MinValue;
                                task.RetryCount++;
                                _openTasks.Add(task);

                                OnNewLog("Warn", $"{ task.RetryCount}. from {task.MaxRetries} Retries scheduled");
                            }
                            else
                            {
                                OnNewLog("Warn", $"\"{task.Name}\": Maximum Retries reached");
                            }
                            continue;
                        }
                    }

                    //replan
                    if (task.SchedulingOptions.HasFlag(SchedulingOptions.RecurringByInterval) ||
                        task.SchedulingOptions.HasFlag(SchedulingOptions.RecurringByDynamicInterval))
                    {
                        PlanSingle(task, true);
                    }
                }
                else if (!_abortWaiting.IsCancellationRequested) //not due, wait till first one´s due
                {
                    var waitFor = task.PlannedFor - now;
                    try
                    {
                        await Task.Delay(waitFor, _abortWaiting.Token);
                    }
                    catch (OperationCanceledException) { }
                }
            }
        }

        public virtual void EndScheduler()
        {
            _loopRunning = false;
            if (!_abortWaiting.IsCancellationRequested)
            {
                _abortWaiting.Cancel();
            }
            if (_mainLoopTask != null)
            {
                _mainLoopTask.GetAwaiter().GetResult();
            }
        }


        #endregion


        /// <summary>
        /// By default the Scheduler will start the given Tasks and perform its PlannedAction on 
        /// given PlannedFor Timestamp if no other job is running already. If so, the PlannedAction
        /// will be started once already running job is completed. 
        /// By setting SchedulingOptions in the Task, certain behaviours can be modified.
        /// </summary>
        /// <param name="taskObject">specified Task to add</param>
        public virtual void PlanFor(params TaskObject[] taskObjects)
        {
            foreach (var taskObject in taskObjects)
            {
                PlanSingle(taskObject);
            }
            //start worker loop if not already running.
            EnsureLoopRunning();
        }

        protected virtual void PlanSingle(TaskObject taskObject, bool recurring = false)
        {
            if (_openTasks.FirstOrDefault(x => x.Name == taskObject.Name) is TaskObject planned)
            {//already planned
                OnNewLog("Warn", $"Task \"{planned.Name}\" already planned.");
                return;
            }

            //planning
            if (taskObject.SchedulingOptions.HasFlag(SchedulingOptions.RecurringByDynamicInterval))
            {
                taskObject.PlannedFor = taskObject.LastStarted.Add(taskObject.DynamicInterval());
            }
            else if (recurring && taskObject.SchedulingOptions.HasFlag(SchedulingOptions.RecurringByInterval))
            {
                //only add interval after first performed
                taskObject.PlannedFor += taskObject.Interval;
                //no repeating for low PlannedFor´s
                if (taskObject.PlannedFor <= DateTimeOffset.Now)
                {
                    taskObject.PlannedFor = DateTimeOffset.Now.Add(taskObject.Interval);
                }
            }

            //WaitTillServiceHours
            if (taskObject.SchedulingOptions.HasFlag(SchedulingOptions.DelayUntilNextTimeWindow))
            {
                UpdateToWaitTillServiceHours(taskObject);
            }

            //adding
            _openTasks.Add(taskObject);
            OnNewLog("Info", $"Task {taskObject.Name} scheduled at {taskObject.PlannedFor.LocalDateTime.ToShortDateString()} {taskObject.PlannedFor.LocalDateTime.ToShortTimeString()}");
        }

        protected virtual bool UpdateToWaitTillServiceHours(TaskObject taskObject)
        {
            bool result = false;
            var now = DateTimeOffset.Now;
            if (taskObject.PlannedFor < now) taskObject.PlannedFor = now;

            //ServiceHours
            TimeSpan currentTarget = taskObject.PlannedFor.ToLocalTime().TimeOfDay;

            if (currentTarget < taskObject.TimeWindowStart || currentTarget > taskObject.TimeWindowEnd)
            {
                result = true;
                //out of Timewindow
                TimeSpan haveToWait = taskObject.TimeWindowStart - currentTarget;
                if (currentTarget > taskObject.TimeWindowEnd)
                {
                    haveToWait += TimeSpan.FromDays(1);
                }
                taskObject.PlannedFor = taskObject.PlannedFor.Add(haveToWait);
            }
            return result;
        }

        public virtual void ReplanFor(string name, DateTimeOffset start)
        {
            if (_openTasks.FirstOrDefault(x => x.Name == name) is TaskObject task)
            {
                task.PlannedFor = start;
                OnNewLog("Info", $"Task {task.Name} replaned for {task.PlannedFor.LocalDateTime.ToShortDateString()} {task.PlannedFor.LocalDateTime.ToShortTimeString()}");
            }

            //start worker loop if not already running.
            EnsureLoopRunning();
        }






        public virtual void RemoveFromPlanning(string name)
        {
            if (_openTasks.FirstOrDefault(x => x.Name == name) is TaskObject task)
            {
                _openTasks.Remove(task);
                OnNewLog("Info", $"Task {task.Name} removed from planning");
            }
        }

    }
}
