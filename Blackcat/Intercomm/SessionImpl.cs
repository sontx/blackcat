using System;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class SessionImpl : ISession
    {
        private readonly IIOHandler handler;
        private bool disposed;

        public SessionImpl(IIOHandler handler)
        {
            this.handler = handler;
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(() => handler.Receive<T>());
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() => handler.Send(data));
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                handler.Client?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}