using System;

namespace Blackcat.EventBus
{
    public interface IThreadInvoker
    {
        bool IsMainThread();

        void RunInBackground(Action job);

        void RunInMain(Action job, bool wait);
    }
}