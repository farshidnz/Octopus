using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Deployment.Tests
{
    public class AsyncHelper
    {
        private static readonly TaskFactory SyncTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunNow<TResult>(Func<Task<TResult>> func)
        {
            return SyncTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        }

        public static void RunNow(Func<Task> func)
        {
            SyncTaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}
