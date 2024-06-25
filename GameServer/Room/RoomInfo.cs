using System.Net;
using GameSever.Server;

namespace GameSever.Room
{
    public class RoomInfo
    {
        public int id;
        public int frame;
        public int playerCount;
        //房间人数列表
        public List<EndPoint> udpEndPointPlayers = new();
        public EndPoint? udpEndPointCreater;
        public List<TcpClient> tcpPlayers = new List<TcpClient>();
        public TcpClient? tcpCreater;
    }
}