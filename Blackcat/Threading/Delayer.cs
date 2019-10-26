using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Blackcat.Threading
{
    public class Delayer
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly int atLeast;

        public Delayer(int atLeast)
        {
            this.atLeast = atLeast;
            stopwatch.Start();
        }

        public async Task Delay()
        {
            if (stopwatch.ElapsedMilliseconds < atLeast)
                await Task.Delay((int)Math.Max(0, atLeast - stopwatch.ElapsedMilliseconds));
        }
    }
}