using Blackcat.Internal;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Tcp
{
    public sealed class SingleReceiver : AbstractReceiver, ISingleReceiver
    {
        private readonly TcpListener listener;
        private readonly object lockObj = new object();
        private ISession session;
        private bool started;

        public SingleReceiver(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(async () =>
            {
                lock (lockObj)
                {
                    StartIfNeeded();
                    WaitForClientIfNeeded();
                }
                return await session.ReceiveAsync<T>();
            });
        }

        public Task SendAsync(object data)
        {
            if (!started || session == null)
                throw new IntercommonIOException("You must receive a request first");
            return session.SendAsync(data);
        }

        private void WaitForClientIfNeeded()
        {
            var sessionFactory = SessionFactory;
            Precondition.PropertyNotNull(sessionFactory, nameof(SessionFactory));
            var protocolFactory = ProtocolFactory;
            Precondition.PropertyNotNull(protocolFactory, nameof(ProtocolFactory));

            if (session == null)
            {
                var client = listener.AcceptTcpClient();
                var protocol = protocolFactory(client.GetStream());
                session = sessionFactory(protocol);
            }
        }

        private void StartIfNeeded()
        {
            if (!started)
            {
                started = true;
                listener.Start(1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    listener.Stop();
                    session?.Dispose();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}