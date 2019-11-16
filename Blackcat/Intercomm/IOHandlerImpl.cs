using Blackcat.Types;
using Blackcat.Utils;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Blackcat.Intercomm
{
    public sealed class IOHandlerImpl : IIOHandler
    {
        private readonly JsonSerializerSettings jsonSettings;

        public TcpClient Client { get; set; }

        public IOHandlerImpl()
        {
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCaseNamingContractResolver() };
        }

        public void Send(object data)
        {
            var json = JsonConvert.SerializeObject(data, jsonSettings);
            var contentBytes = Encoding.UTF8.GetBytes(json);
            var headerBytes = contentBytes.Length.ToBytes<int>();
            var stream = Client.GetStream();
            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(contentBytes, 0, contentBytes.Length);
        }

        public T Receive<T>()
        {
            var stream = Client.GetStream();
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
    }
}