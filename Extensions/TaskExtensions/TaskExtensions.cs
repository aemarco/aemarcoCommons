using System;
using System.Threading.Tasks;

#nullable enable

namespace aemarcoCommons.Extensions.TaskExtensions
{
    public static class TaskExtensions
    {
        private static Action<Exception>? _onException;
        /// <summary>
        /// Set the default action for SafeFireAndForget to handle every exception
        /// </summary>
        /// <param name="onException">If an exception is thrown in the Task using SafeFireAndForget, <c>onException</c> will execute</param>
        public static void SetDefaultExceptionHandling(in Action<Exception> onException)
        {
            _onException = onException ?? throw new ArgumentNullException(nameof(onException));
        }
        /// <summary>
        /// Remove the default action for SafeFireAndForget
        /// </summary>
        public static void RemoveDefaultExceptionHandling() => _onException = null;
        public static bool ShouldRethrowExceptions { get; set; } 







        /// <summary>
        /// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">ValueTask.</param>
        /// <param name="onException">If an exception is thrown in the ValueTask, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        public static void SafeFireAndForget(this ValueTask task, in Action<Exception>? onException = null,
            in bool continueOnCapturedContext = false) =>
            HandleSafeFireAndForget(task, continueOnCapturedContext, onException);


        /// <summary>
        /// Safely execute the ValueTask without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">ValueTask.</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        /// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
        public static void SafeFireAndForget<TException>(this ValueTask task, in Action<TException>? onException = null,
            in bool continueOnCapturedContext = false) where TException : Exception =>
            HandleSafeFireAndForget(task, continueOnCapturedContext, onException);


        /// <summary>
        /// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        public static void SafeFireAndForget(this Task task, in Action<Exception>? onException = null,
            in bool continueOnCapturedContext = false) =>
            HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

        /// <summary>
        /// Safely execute the Task without waiting for it to complete before moving to the next line of code; commonly known as "Fire And Forget". Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c>, continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c>, continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        /// <typeparam name="TException">Exception type. If an exception is thrown of a different type, it will not be handled</typeparam>
        public static void SafeFireAndForget<TException>(this Task task, in Action<TException>? onException = null,
            in bool continueOnCapturedContext = false) where TException : Exception =>
            HandleSafeFireAndForget(task, continueOnCapturedContext, onException);

       

       
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void

        private static async void HandleSafeFireAndForget<TException>(ValueTask valueTask, bool continueOnCapturedContext,
            Action<TException>? onException) where TException : Exception
        {
            try
            {
                await valueTask.ConfigureAwait(continueOnCapturedContext);
            }
            catch (TException ex) when (_onException != null || onException != null)
            {
                HandleException(ex, onException);

                if (ShouldRethrowExceptions)
                    throw;
            }
        }

        private static async void HandleSafeFireAndForget<TException>(Task task, bool continueOnCapturedContext,
            Action<TException>? onException) where TException : Exception
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (TException ex) when (_onException != null || onException != null)
            {
                HandleException(ex, onException);

                if (ShouldRethrowExceptions)
                    throw;
            }
        }

#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

        private static void HandleException<TException>(in TException exception, in Action<TException>? onException)
            where TException : Exception
        {
            _onException?.Invoke(exception);
            onException?.Invoke(exception);
        }
    }
}
