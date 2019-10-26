using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blackcat.Threading
{
    public sealed class ParallelTasks : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public int MaxParallelismCount { get; set; } = 100;

        public bool RunAllAndWait<T>(IEnumerable<T> allWorks, Action<T, ParallelLoopState> doOnEachWork)
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxParallelismCount,
                CancellationToken = cancellationTokenSource.Token
            };

            try
            {
                return Parallel.ForEach(allWorks, options, doOnEachWork).IsCompleted;
            }
            catch (OperationCanceledException) { }

            return false;
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}