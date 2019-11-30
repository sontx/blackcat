using System;
using System.IO;

namespace Blackcat.Intercomm
{
    public abstract class AbstractIntercomm : IIntercomm
    {
        protected bool disposed;

        public Func<Stream, IProtocol> ProtocolFactory { get; set; }

        public AbstractIntercomm()
        {
            ProtocolFactory = DefaultProtocolFactory;
        }

        private IProtocol DefaultProtocolFactory(Stream stream)
        {
            return new ProtocolImpl(stream);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        ~AbstractIntercomm()
        {
            Dispose(false);
        }
    }
}