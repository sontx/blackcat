using System;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Receives incomming data from <see cref="ISender"/>
    /// </summary>
    public interface IReceiver : IIntercomm
    {
        Func<IProtocol, ISession> SessionFactory { get; set; }
    }
}