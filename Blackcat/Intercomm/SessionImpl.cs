using System;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    internal sealed class SessionImpl : ISession
    {
        private readonly IProtocol protocol;
        private bool disposed;

        public SessionImpl(IProtocol protocol)
        {
            this.protocol = protocol;
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(() => protocol.Receive<T>());
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() => protocol.Send(data));
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                protocol?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}