using System;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Represents how the incomming and outcomming data will be transmitted between <see cref="ISender"/> and <see cref="IReceiver"/>
    /// </summary>
    public interface IProtocol : IDisposable
    {
        /// <summary>
        /// Receives incomming data from the other side.
        /// </summary>
        /// <typeparam name="T">Type of sending data</typeparam>
        /// <returns>Received data</returns>
        T Receive<T>();

        /// <summary>
        /// Sends data to the other side.
        /// </summary>
        /// <param name="data">Data to send</param>
        void Send(object data);
    }
}