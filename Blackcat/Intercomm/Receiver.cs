using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class Receiver : IDisposable
    {
        private readonly TcpListener listener;
        private bool started;
        private bool disposed;

        public IIOHandler IOHandler { get; set; } = new IOHandlerImpl();

        public Receiver(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(() =>
            {
                StartIfNeeded();
                WaitForClientIfNeeded();
                return IOHandler.Receive<T>();
            });
        }

        public Task SendAsync(object data)
        {
            if (!started || IOHandler.Client == null)
                throw new IntercommonIOException("You must receive a request first");

            return Task.Run(() =>
            {
                IOHandler.Send(data);
            });
        }

        private void WaitForClientIfNeeded()
        {
            if (IOHandler.Client == null)
                IOHandler.Client = listener.AcceptTcpClient();
        }

        private void StartIfNeeded()
        {
            if (!started)
            {
                started = true;
                listener.Start(1);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                listener.Stop();
                IOHandler.Client?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}