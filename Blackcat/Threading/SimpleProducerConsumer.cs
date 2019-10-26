using System;
using System.Threading.Tasks.Dataflow;

namespace Blackcat.Threading
{
    public class SimpleProducerConsumer<T> : IDisposable
    {
        private readonly BufferBlock<T> bufferBlock = new BufferBlock<T>();

        public void Post(T data)
        {
            bufferBlock.Post(data);
        }

        public async void WaitAndConsume(Action<T> whenAvailable)
        {
            while (await bufferBlock.OutputAvailableAsync())
            {
                var data = bufferBlock.Receive();
                whenAvailable?.Invoke(data);
            }
        }

        public void Dispose()
        {
            bufferBlock.Complete();
        }
    }
}