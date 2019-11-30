using Blackcat.Internal;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Tcp
{
    public sealed class Sender : AbstractIntercomm, ISender
    {
        private readonly object lockObj = new object();
        private readonly int port;
        private readonly string hostname;
        private IProtocol protocol;

        public Sender()
            : this(0, "localhost")
        {
        }

        public Sender(int port, string hostname)
        {
            this.port = port;
            this.hostname = hostname;
        }

        public Task<T> SendAsync<T>(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                protocol.Send(data);
                return protocol.Receive<T>();
            });
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                protocol.Send(data);
            });
        }

        private void ConnectIfNeeded()
        {
            var factory = ProtocolFactory;
            Precondition.PropertyNotNull(factory, nameof(ProtocolFactory));

            lock (lockObj)
            {
                if (protocol == null)
                {
                    var client = new TcpClient(hostname, port);
                    var stream = client.GetStream();
                    protocol = factory(stream);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    protocol?.Dispose();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}