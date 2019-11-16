using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class Sender : IDisposable
    {
        private bool disposed;

        public IIOHandler IOHandler { get; set; } = new IOHandlerImpl();

        public string HostName { get; set; }

        public int Port { get; set; }

        public Sender() : this(0, "localhost")
        {
        }

        public Sender(int port, string hostname)
        {
            Port = port;
            HostName = hostname;
        }

        public Task<T> SendAsync<T>(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                IOHandler.Send(data);
                return IOHandler.Receive<T>();
            });
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                IOHandler.Send(data);
            });
        }

        private void ConnectIfNeeded()
        {
            if (IOHandler.Client == null)
                IOHandler.Client = new TcpClient(HostName, Port);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                IOHandler.Client?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}