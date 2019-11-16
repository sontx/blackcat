using Blackcat.Types;
using Blackcat.Utils;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public sealed class Sender : IDisposable
    {
        private readonly TcpClient client;
        private readonly JsonSerializerSettings jsonSettings;
        private bool disposed;

        public string HostName { get; set; }

        public int Port { get; set; }

        public Sender() : this(0, "localhost")
        {
        }

        public Sender(int port, string hostname)
        {
            client = new TcpClient();
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCaseNamingContractResolver() };
            Port = port;
            HostName = hostname;
        }

        public Task<T> SendAsync<T>(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                SendData(data);
                return ReceiveData<T>();
            });
        }

        public Task SendAsync(object data)
        {
            return Task.Run(() =>
            {
                ConnectIfNeeded();
                SendData(data);
            });
        }

        private void ConnectIfNeeded()
        {
            if (!client.Connected)
                client.Connect(HostName, Port);
        }

        private void SendData(object data)
        {
            var json = JsonConvert.SerializeObject(data, jsonSettings);
            var contentBytes = Encoding.UTF8.GetBytes(json);
            var headerBytes = contentBytes.Length.ToBytes<int>();// 4 bytes header
            var sendBytes = headerBytes.CombineWith(contentBytes);
            var stream = client.GetStream();
            stream.Write(sendBytes, 0, sendBytes.Length);
        }

        private T ReceiveData<T>()
        {
            var headerBytes = new byte[4];
            var stream = client.GetStream();
            var readByteCount = stream.Read(headerBytes, 0, headerBytes.Length);
            if (readByteCount == headerBytes.Length)
            {
                var readBytes = new byte[headerBytes.ToInt32()];
                if (readBytes.Length > 0)
                {
                    readByteCount = stream.Read(readBytes, 0, readBytes.Length);
                    if (readByteCount == readBytes.Length)
                    {
                        var json = Encoding.UTF8.GetString(readBytes);
                        return JsonConvert.DeserializeObject<T>(json, jsonSettings);
                    }
                }
            }
            return default;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                client.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}