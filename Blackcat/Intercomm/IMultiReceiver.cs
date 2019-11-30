using System;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Supports receiving from multiple clients.
    /// </summary>
    public interface IMultiReceiver : IReceiver
    {
        /// <summary>
        /// Waits for a connection from <see cref="ISender"/> and returns
        /// an <see cref="ISession"/> to communicate with this <see cref="ISender"/>
        /// </summary>
        /// <returns>An <see cref="ISession"/> to communicate with the connected <see cref="ISender"/></returns>
        Task<ISession> WaitForSessionAsync();

        /// <summary>
        /// Waits for connections from <see cref="ISender"/>(es) and calls the callback with
        /// an <see cref="ISession"/> to communicate with connected <see cref="ISender"/>
        /// </summary>
        /// <param name="acceptSession">A callback to call when an <see cref="ISender"/> connects to this <see cref="IMultiReceiver"/></param>
        Task WaitForSessionAsync(Func<ISession, Task> acceptSession);
    }
}