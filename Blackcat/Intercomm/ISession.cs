using System;
using System.Threading.Tasks;

namespace Blackcat.Intercomm
{
    public interface ISession : IDisposable
    {
        Task SendAsync(object data);

        Task<T> ReceiveAsync<T>();
    }
}