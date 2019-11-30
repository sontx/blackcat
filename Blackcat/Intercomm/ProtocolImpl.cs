using Blackcat.Internal;
using Blackcat.Types;
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
        private readonly Stream stream;
        private bool disponsed;

        public IContentAdapter ContentAdapter { get; set; } = new JsonContentAdapter();

        public ProtocolImpl(Stream stream)
        {
            Precondition.ArgumentNotNull(stream, nameof(stream));
            this.stream = stream;
        }

        public void Send(object data)
        {
            var adapter = ContentAdapter;
            Precondition.PropertyNotNull(adapter, nameof(ContentAdapter));

            var encoded = adapter.ToString(data);
            var contentBytes = Encoding.UTF8.GetBytes(encoded);
            var headerBytes = contentBytes.Length.ToBytes<int>();
            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(contentBytes, 0, contentBytes.Length);
        }

        public T Receive<T>()
        {
            var adapter = ContentAdapter;
            Precondition.PropertyNotNull(adapter, nameof(ContentAdapter));

            var headerBytes = new byte[4];
            var readByteCount = stream.Read(headerBytes, 0, headerBytes.Length);
            if (readByteCount == headerBytes.Length)
            {
                var contentByteCount = headerBytes.ToInt32();
                var contentBytes = new byte[contentByteCount];
                readByteCount = stream.Read(contentBytes, 0, contentBytes.Length);
                if (readByteCount == contentByteCount)
                {
                    var encoded = Encoding.UTF8.GetString(contentBytes);
                    return adapter.ToObject<T>(encoded);
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