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
    public class LogoutModel: Singleton<LogoutModel>
    {
        public void Init()
        {
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.logoutProto, LogoutCallBack);
        }
        //玩家离开
        private void LogoutCallBack(TcpClient client, byte[] buffer)
        {
            LogoutProto proto = LogoutProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.logoutProto, proto.admin);
            
            SocketManager.Instance.LoginedTcpClients.Remove(client);
            Console.WriteLine("客户端{0}退出登陆,当前登陆的客户端数:{1}", client.tcpSocket.RemoteEndPoint?.ToString()
                    , SocketManager.Instance.LoginedTcpClients.Count);
            //给其他玩家通知信息
            LogoutCallBackProto proto1 = new LogoutCallBackProto(client.clientInfo.admin);
            foreach (var other in SocketManager.Instance.LoginedTcpClients)
            {
                other.SendMsg(proto1.ToArray());
            }
        }
    }
}