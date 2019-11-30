using Blackcat.Internal;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Pipe
{
    public sealed class Sender : AbstractIntercomm, ISender
    {
        private readonly object lockObj = new object();
        private readonly string serverName;
        private readonly string pipeName;
        private readonly int connectTimeout;

        private NamedPipeClientStream client;
        private IProtocol protocol;

        public Sender(string pipeName)
            : this(".", pipeName, 10000)
        {
        }

        public Sender(string serverName, string pipeName, int connectTimeout)
        {
            this.serverName = serverName;
            this.pipeName = pipeName;
            this.connectTimeout = connectTimeout;
        }

        private void ConnectIfNeeded()
        {
            var factory = ProtocolFactory;
            Precondition.PropertyNotNull(factory, nameof(ProtocolFactory));

            lock (lockObj)
            {
                if (client == null)
                {
                    client = new NamedPipeClientStream(
                        serverName,
                        pipeName,
                        PipeDirection.InOut,
                        PipeOptions.WriteThrough,
                        TokenImpersonationLevel.None);
                    if (!client.IsConnected)
                        client.Connect(connectTimeout);
                    protocol = factory(client);
                }
            }
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                protocol.Send(data);
            });
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

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    protocol?.Dispose();
                    client?.Close();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}