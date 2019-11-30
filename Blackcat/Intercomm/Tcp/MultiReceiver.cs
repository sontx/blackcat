using Blackcat.Internal;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Tcp
{
    public sealed class MultiReceiver : AbstractReceiver, IMultiReceiver
    {
        private readonly TcpListener listener;
        private readonly object lockObj = new object();
        private bool started;

        public MultiReceiver(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        private void StartServerIfNeeded()
        {
            lock (lockObj)
            {
                if (!started)
                {
                    started = true;
                    listener.Start();
                }
            }
        }

        private ISession CreateSession(TcpClient client)
        {
            var sessionFactory = SessionFactory;
            Precondition.PropertyNotNull(sessionFactory, nameof(SessionFactory));
            var protocolFactory = ProtocolFactory;
            Precondition.PropertyNotNull(protocolFactory, nameof(ProtocolFactory));

            var protocol = protocolFactory(client.GetStream());
            return sessionFactory(protocol);
        }

        public async Task<ISession> WaitForSessionAsync()
        {
            StartServerIfNeeded();
            var client = await listener.AcceptTcpClientAsync();
            return CreateSession(client);
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

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    listener.Stop();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}