using System.Net.Sockets;

namespace Blackcat.Intercomm
{
    public interface IIOHandler
    {
        TcpClient Client { get; set; }

        T Receive<T>();

        void Send(object data);
    }
}