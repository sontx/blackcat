using Blackcat.Internal;
using Blackcat.Types;
using Blackcat.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Transmission data structure: [header] [body]
    /// - Header: body length
    /// - Body: data to send which encoded as json text
    /// </summary>
    internal sealed class ProtocolImpl : IProtocol
    {
        private readonly JsonSerializerSettings jsonSettings;
        private readonly Stream stream;
        private bool disponsed;

        public ProtocolImpl(Stream stream)
        {
            Precondition.ArgumentNotNull(stream, nameof(stream));
            jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCaseNamingContractResolver() };
            this.stream = stream;
        }

        public void Send(object data)
        {
            var json = JsonConvert.SerializeObject(data, jsonSettings);
            var contentBytes = Encoding.UTF8.GetBytes(json);
            var headerBytes = contentBytes.Length.ToBytes<int>();
            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(contentBytes, 0, contentBytes.Length);
        }

        public T Receive<T>()
        {
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

        public void Dispose()
        {
            if (!disponsed)
            {
                disponsed = true;
                stream?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}