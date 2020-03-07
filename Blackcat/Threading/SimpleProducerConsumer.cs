using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Blackcat.Threading
{
    public class SimpleProducerConsumer<T> : IDisposable
    {
        private readonly BlockingCollection<T> _collection = new BlockingCollection<T>();

        public void Post(T data)
        {
            _collection.Add(data);
        }

        public void WaitAndConsume(Action<T> whenAvailable)
        {
            foreach (var item in _collection.GetConsumingEnumerable())
            {
                whenAvailable(item);
            }
        }

        public Task WaitAndConsumeAsync(Action<T> whenAvailable)
        {
            return Task.Run(() =>
            {
                WaitAndConsume(whenAvailable);
            });
        }

        public void Dispose()
        {
            _collection.CompleteAdding();
        }
    }
}