using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class MultiReceiver : IDisposable
    {
        private readonly TcpListener listener;
        private bool disposed;

        public Func<TcpClient, ISession> SessionFactory { get; set; } = (client) => new SessionImpl(new IOHandlerImpl { Client = client });

        public MultiReceiver(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task<ISession> WaitForSessionAsync()
        {
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();
            return SessionFactory(client);
        }

        public async Task WaitForSessionAsync(Func<ISession, Task> acceptSession)
        {
            while (!disposed)
            {
                using (var session = await WaitForSessionAsync())
                {
                    await acceptSession(session);
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                listener.Stop();
                GC.SuppressFinalize(this);
            }
        }
    }
}