using Blackcat.Types;
using Blackcat.Utils;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class Receiver : IDisposable
    {
        private readonly TcpListener listener;
        private readonly JsonSerializerSettings jsonSettings;
        private TcpClient client;
        private bool started;
        private bool disposed;

        public Receiver(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCaseNamingContractResolver() };
        }

        public Task<T> ReceiveAsync<T>()
        {
            return Task.Run(() =>
            {
                StartIfNeeded();
                WaitForClientIfNeeded();
                return ReceiveData<T>();
            });
        }

        public Task SendAsync(object data)
        {
            if (!started || client == null)
                throw new IntercommonIOException("You must receive a request first");

            return Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(data, jsonSettings);
                var contentBytes = Encoding.UTF8.GetBytes(json);
                var headerBytes = contentBytes.Length.ToBytes<int>();
                var stream = client.GetStream();
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(contentBytes, 0, contentBytes.Length);
            });
        }

        private T ReceiveData<T>()
        {
            var stream = client.GetStream();
            var headerBytes = new byte[4];
            var readByteCount = stream.Read(headerBytes, 0, headerBytes.Length);
            if (readByteCount == headerBytes.Length)
            {
                var contentByteCount = headerBytes.ToInt32();
                var contentBytes = new byte[contentByteCount];
                readByteCount = stream.Read(contentBytes, 0, contentBytes.Length);
                if (readByteCount == contentByteCount)
                {
                    var json = Encoding.UTF8.GetString(contentBytes);
                    return JsonConvert.DeserializeObject<T>(json, jsonSettings);
                }
                throw new IntercommonIOException("Missing body content bytes");
            }
            throw new IntercommonIOException("Missing header content bytes");
        }

        private void WaitForClientIfNeeded()
        {
            if (client == null)
                client = listener.AcceptTcpClient();
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
                client?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}