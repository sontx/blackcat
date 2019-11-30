using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    /// <summary>
    /// Sends data to <see cref="IReceiver"/>
    /// </summary>
    public interface ISender : IIntercomm
    {
        /// <summary>
        /// Sends data to <see cref="IReceiver"/>
        /// </summary>
        /// <param name="data">Data to send</param>
        Task SendAsync(object data);

        /// <summary>
        /// Sends data to <see cref="IReceiver"/> and wating for response
        /// </summary>
        /// <typeparam name="T">Type of response</typeparam>
        /// <param name="data">Data to send</param>
        /// <returns>Response from <see cref="ISender"/></returns>
        Task<T> SendAsync<T>(object data);
    }
}