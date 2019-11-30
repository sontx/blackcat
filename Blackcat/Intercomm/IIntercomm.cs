using System;
using System.IO;

namespace Blackcat.Intercomm
{
    public interface IIntercomm : IDisposable
    {
        Func<Stream, IProtocol> ProtocolFactory { get; set; }
    }
}