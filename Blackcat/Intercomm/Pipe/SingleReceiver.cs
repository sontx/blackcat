using Blackcat.Internal;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Pipe
{
    public sealed class SingleReceiver : AbstractReceiver, ISingleReceiver
    {
        private readonly object lockObj = new object();
        private readonly string pipeName;

        private NamedPipeServerStream server;
        private ISession session;

        public SingleReceiver(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(async () =>
            {
                lock (lockObj)
                {
                    StartIfNeeded();
                }
                return await session.ReceiveAsync<T>();
            });
        }

        public Task SendAsync(object data)
        {
            lock (lockObj)
            {
                if (server == null || session == null)
                    throw new IntercommonIOException("You must receive a request first");
                return session.SendAsync(data);
            }
        }

        private void StartIfNeeded()
        {
            var sessionFactory = SessionFactory;
            Precondition.PropertyNotNull(sessionFactory, nameof(SessionFactory));
            var protocolFactory = ProtocolFactory;
            Precondition.PropertyNotNull(protocolFactory, nameof(ProtocolFactory));

            if (server == null)
            {
                server = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.WriteThrough);
                server.WaitForConnection();
                var protocol = protocolFactory(server);
                session = sessionFactory(protocol);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    session?.Dispose();
                    server?.Close();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}