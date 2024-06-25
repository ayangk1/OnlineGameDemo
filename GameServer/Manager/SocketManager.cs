using System.Net;
using GameSever.Server;

namespace GameSever.Manager
{
    public class SocketManager : Singleton<SocketManager>
    {
        public SocketManager()
        {
            roomUdpClients = new ();
            connectedUdpclients = new List<UdpClient>();
            connectedTcpClients = new List<TcpClient>();
            loginedTcpClients = new List<TcpClient>();
        }

        /// <summary>
        /// udp连接人数
        /// </summary>
        private List<UdpClient> connectedUdpclients;
        public List<UdpClient> ConnectedUdpClients => connectedUdpclients;
        /// <summary>
        /// udp连接人数
        /// </summary>
        private List<EndPoint> roomUdpClients;
        public List<EndPoint> RoomUdpClients => roomUdpClients;
        
        /// <summary>
        /// tcp连接人数
        /// </summary>
        private List<TcpClient> connectedTcpClients;
        public List<TcpClient> ConnectedTcpClients => connectedTcpClients;

        /// <summary>
        /// tcp在线人数
        /// </summary>
        private List<TcpClient> loginedTcpClients;
        public List<TcpClient> LoginedTcpClients => loginedTcpClients;


    }
}