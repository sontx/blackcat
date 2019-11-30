using System;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Represents a working session between <see cref="ISender"/> and <see cref="IReceiver"/>.
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Sends data to <see cref="ISender"/>.
        /// This only makes sense when <see cref="ISender.SendAsync{T}(object)"/> was called.
        /// </summary>
        /// <param name="data">Data to send</param>
        Task SendAsync(object data);

        /// <summary>
        /// Receives incomming data from <see cref="ISender"/>.
        /// </summary>
        /// <typeparam name="T">Type of sending data</typeparam>
        Task<T> ReceiveAsync<T>();
    }
}