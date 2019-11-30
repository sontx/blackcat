using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace Blackcat.Intercomm.Pipe
{
    internal sealed class PipeServers
    {
        private readonly object lockObj = new object();
        private readonly string pipeName;
        private readonly int initPoolSize;

        private List<PipeServer> servers = new List<PipeServer>();

        public PipeServers(string pipeName, int initPoolSize)
        {
            this.pipeName = pipeName;
            this.initPoolSize = initPoolSize;
        }

        public void Start()
        {
            lock (lockObj)
            {
                for (var i = 0; i < initPoolSize; i++)
                {
                    var server = CreateServer();
                    servers.Add(server);
                }
            }
        }

        private PipeServer CreateServer()
        {
            var server = new NamedPipeServerStream(
                pipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Byte,
                PipeOptions.WriteThrough);
            return new PipeServer { Server = server };
        }

        public async Task<NamedPipeServerStream> WaitForConnectionAsync()
        {
            var clonedServers = GetSnaphost();
            var tasks = new List<Task>();
            foreach (var server in clonedServers)
            {
                if (!server.Server.IsConnected)
                {
                    server.Used = false;
                    tasks.Add(server.Server.WaitForConnectionAsync());
                }
            }
            if (tasks.Count == 0)
            {
                IncreasePoolSize();
                return await WaitForConnectionAsync();
            }
            await Task.WhenAny(tasks);
            foreach (var server in clonedServers)
            {
                if (server.Server.IsConnected && !server.Used)
                {
                    server.Used = true;
                    return server.Server;
                }
            }
            return null;
        }

        private void IncreasePoolSize()
        {
            var clonedServers = GetSnaphost();
            clonedServers.Add(CreateServer());
            servers = clonedServers;
        }

        private List<PipeServer> GetSnaphost()
        {
            List<PipeServer> clonedServers;
            lock (lockObj)
            {
                clonedServers = new List<PipeServer>(servers);
            }
            return clonedServers;
        }

        public void Close()
        {
            lock (lockObj)
            {
                var clonedServers = GetSnaphost();
                foreach (var server in clonedServers)
                {
                    server.Server.Close();
                }
            }
        }

        private class PipeServer
        {
            public NamedPipeServerStream Server { get; set; }
            public bool Used { get; set; }
        }
    }
}