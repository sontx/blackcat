using Blackcat.Internal;
using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Pipe
{
    public sealed class MultiReceiver : AbstractReceiver, IMultiReceiver
    {
        private readonly object lockObj = new object();
        private readonly PipeServers servers;

        private bool started;

        public MultiReceiver(string pipeName)
            : this(pipeName, 10)
        { }

        public MultiReceiver(string pipeName, int initPoolSize)
        {
            servers = new PipeServers(pipeName, initPoolSize);
        }

        private ISession CreateSession(NamedPipeServerStream client)
        {
            var sessionFactory = SessionFactory;
            Precondition.PropertyNotNull(sessionFactory, nameof(SessionFactory));
            var protocolFactory = ProtocolFactory;
            Precondition.PropertyNotNull(protocolFactory, nameof(ProtocolFactory));

            var protocol = protocolFactory(client);
            return sessionFactory(protocol);
        }

        private void StartIfNeeded()
        {
            lock (lockObj)
            {
                if (!started)
                {
                    started = true;
                    servers.Start();
                }
            }
        }

        public async Task<ISession> WaitForSessionAsync()
        {
            StartIfNeeded();
            NamedPipeServerStream stream = null;
            while (!disposed && stream == null)
            {
                stream = await servers.WaitForConnectionAsync();
            }
            return stream != null ? CreateSession(stream) : null;
        }

        public async Task WaitForSessionAsync(Func<ISession, Task> acceptSession)
        {
            while (!disposed)
            {
                using (var session = await WaitForSessionAsync())
                {
                    if (session == null) return;
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
                    servers.Close();
                }
                base.Dispose(disposing);
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}