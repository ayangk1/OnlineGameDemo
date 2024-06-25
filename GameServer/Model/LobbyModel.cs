using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Controller;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Room;
using GameSever.Server;

namespace GameSever.Model
{
    public class LobbyModel : Singleton<LobbyModel>
    {
        public void Init()
        {
            //玩家请求进入房间
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomProto, OnRequireRoomCallBack);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomProto, OnRequireRoomCallBack);
            //玩家进入大厅
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_ClientEnterProto, OnLobbyClientEnter);
            //玩家发送本地时间
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.clientSendLocalTimeProto, ClientSendLocalTimeCallBack);
        }

        /// <summary>
        /// udp请求进入房间
        /// </summary>
        private void OnRequireRoomCallBack(ref UdpClient client, byte[] buffer)
        {
            Lobby_RequireEnterRoomProto proto = Lobby_RequireEnterRoomProto.GetProto(buffer);

            Console.WriteLine("Udp用户{0}通过协议{1}发送：进入房间号{2}", client.endPoint
            , ProtoCodeConf.Lobby_RequireEnterRoomProto, proto.roomId);

            SocketManager.Instance.ConnectedUdpClients.Add(client);
            SocketManager.Instance.RoomUdpClients.Add(client.endPoint);

            //如果没有房间则直接创建
            if (UdpRoomManager.Instance.roomMap.Count == 0)
            {
                UdpRoomManager.Instance.CreateRoom(proto.roomId, ref client);
                Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto()
                {
                    success = true
                };
                client.SendMsg(protoBack.ToArray(),client.endPoint);
            }
            else
            {
                //输入不在范围内
                if (proto.roomId < UdpRoomManager.Instance.MinRoomId || proto.roomId >= UdpRoomManager.Instance.MaxRoomId)
                {
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto
                    {
                        success = false,
                        errCode = "请输入" + UdpRoomManager.Instance.MinRoomId + "~" + UdpRoomManager.Instance.MaxRoomId + "房间号"
                    };
                    client.SendMsg(protoBack.ToArray(),client.endPoint);
                    return;
                }
                //如果没有 则创建
                else if (!UdpRoomManager.Instance.roomMap.ContainsKey(proto.roomId))
                {
                    UdpRoomManager.Instance.CreateRoom(proto.roomId, ref client);
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto
                    {
                        success = true
                    };
                    client.SendMsg(protoBack.ToArray(),client.endPoint);
                    return;
                }
                else
                {
                    //进入房间
                    UdpRoomManager.Instance.EnterRoom(proto.roomId, ref client);
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto
                    {
                        currRoomFrame = UdpRoomManager.Instance.roomMap[proto.roomId].frame,
                        success = true
                    };
                    client.SendMsg(protoBack.ToArray(),client.endPoint);
                }

            }
        }

        /// <summary>
        /// 玩家发送本地时间
        /// </summary>
        private void ClientSendLocalTimeCallBack(TcpClient client, byte[] buffer)
        {
            ClientSendLocalTimeProto proto = ClientSendLocalTimeProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.clientSendLocalTimeProto, proto.localTime);

            ServerReturnTimeProto protoBack = new ServerReturnTimeProto(proto.localTime,DateTime.Now.ToUniversalTime().Ticks / 1000);
            client.SendMsg(protoBack.ToArray());

        }

        /// <summary>
        /// 当请求进入房间
        /// </summary>
        private void OnRequireRoomCallBack(TcpClient client, byte[] buffer)
        {
            Lobby_RequireEnterRoomProto proto = Lobby_RequireEnterRoomProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：进入房间号{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.Lobby_RequireEnterRoomProto, proto.roomId);

            //如果没有房间则直接创建
            if (TcpRoomManager.Instance.roomMap.Count == 0)
            {
                TcpRoomManager.Instance.CreateRoom(proto.roomId, ref client);
                Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto()
                {
                    success = true
                };
                client.SendMsg(protoBack.ToArray());
            }
            else
            {
                //输入不在范围内
                if (proto.roomId < TcpRoomManager.Instance.MinRoomId || proto.roomId >= TcpRoomManager.Instance.MaxRoomId)
                {
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto
                    {
                        success = false,
                        errCode = "请输入" + TcpRoomManager.Instance.MinRoomId + "~" + TcpRoomManager.Instance.MaxRoomId + "房间号"
                    };
                    client.SendMsg(protoBack.ToArray());
                    return;
                }
                //如果没有 则创建
                else if (!TcpRoomManager.Instance.roomMap.ContainsKey(proto.roomId))
                {
                    TcpRoomManager.Instance.CreateRoom(proto.roomId, ref client);
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto();
                    protoBack.success = true;
                    client.SendMsg(protoBack.ToArray());
                    return;
                }
                else
                {
                    //进入房间
                    TcpRoomManager.Instance.EnterRoom(proto.roomId, ref client);
                    Lobby_RequireEnterRoomCallBackProto protoBack = new Lobby_RequireEnterRoomCallBackProto
                    {
                        currRoomFrame = TcpRoomManager.Instance.roomMap[proto.roomId].frame,
                        success = true
                    };
                    client.SendMsg(protoBack.ToArray());
                }

            }
        }

        /// <summary>
        /// 当玩家进入大厅
        /// </summary>
        private void OnLobbyClientEnter(TcpClient client, byte[] buffer)
        {
            Lobby_ClientEnterProto proto = Lobby_ClientEnterProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.Lobby_ClientEnterProto, proto.admin);

            Lobby_SyncOnlineClientProto syncProto = new Lobby_SyncOnlineClientProto
            {
                count = SocketManager.Instance.LoginedTcpClients.Count,
                clientList = new List<ClientInfo>()
            };
            //获取在线玩家列表
            for (int i = 0; i < syncProto.count; i++)
                syncProto.clientList.Add(SocketManager.Instance.LoginedTcpClients[i].clientInfo);
            foreach (var m_client in SocketManager.Instance.LoginedTcpClients)
                m_client.SendMsg(syncProto.ToArray());
        }
    }
}