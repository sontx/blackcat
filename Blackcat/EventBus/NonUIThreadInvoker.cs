using System;
using System.Threading.Tasks;

namespace Blackcat.EventBus
{
    internal class NonUIThreadInvoker : IThreadInvoker
    {
        public bool IsMainThread()
        {
            return false;
        }

        public void RunInBackground(Action job)
        {
            Task.Run(job);
        }

        public void RunInMain(Action job, bool wait)
        {
            if (wait)
                Task.Run(job).Wait();
            else
                Task.Run(job);
        }
    }
}