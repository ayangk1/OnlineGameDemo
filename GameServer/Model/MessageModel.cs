using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Controller;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Server;

namespace GameSever.Model
{
    public class MessageModel : Singleton<MessageModel>
    {
        public void Init()
        {
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_MessageProto, MessageCallBack);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_MessageProto, MessageCallBack);
        }

        private void MessageCallBack(ref UdpClient client, byte[] buffer)
        {
            Lobby_MessageProto proto = Lobby_MessageProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.udpSocket.RemoteEndPoint
            , ProtoCodeConf.Lobby_MessageProto, proto.message);

            foreach(var endPoint in SocketManager.Instance.ConnectedUdpClients)
            {
                Lobby_MessageProto proto1 = new Lobby_MessageProto(proto.message, proto.admin);
                client.SendMsg(proto1.ToArray(),client.endPoint);
            }
        }

        private void MessageCallBack(TcpClient client, byte[] buffer)
        {
            Lobby_MessageProto proto = Lobby_MessageProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.Lobby_MessageProto, proto.message);


            for (int i = 0; i < SocketManager.Instance.LoginedTcpClients.Count; i++)
            {
                if (SocketManager.Instance.LoginedTcpClients[i] != client)
                {
                    Lobby_MessageProto proto1 = new Lobby_MessageProto(proto.message, proto.admin);
                    SocketManager.Instance.LoginedTcpClients[i].SendMsg(proto1.ToArray());
                }
            }

        }
    }
}