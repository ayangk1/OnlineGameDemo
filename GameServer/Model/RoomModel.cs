using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Controller;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Room;
using GameSever.Server;

namespace GameSever.Model
{
    public class RoomModel : Singleton<RoomModel>
    {
        public void Init()
        {
            //玩家请求离开房间
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_PlayerLeaveProto, OnRequireLeaveRoom);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_PlayerLeaveProto, OnRequireLeaveRoom);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SuccessEnterProto, OnSuccessEnterRoom);
        }

        
        private void OnSuccessEnterRoom(ref UdpClient client, byte[] buffer)
        {
            Room_SuccessEnterProto proto = Room_SuccessEnterProto.GetProto(buffer);

            Console.WriteLine("Udp用户{0}通过协议{1}发送：账号{2}", client.endPoint
            , ProtoCodeConf.Room_SuccessEnterProto, proto.admin);
            
            Room_OtherPlayerEnterProto protoOther = new Room_OtherPlayerEnterProto(proto.admin);
            UdpRoomManager.Instance.SendRoomOtherPlayer(client,client.player.roomId,protoOther.ToArray());
        }

        /// <summary>
        /// udp当请求离开房间
        /// </summary>
        private void OnRequireLeaveRoom(ref UdpClient client, byte[] buffer)
        {
            Room_PlayerLeaveProto proto = Room_PlayerLeaveProto.GetProto(buffer);

            Console.WriteLine("Udp用户{0}通过协议{1}发送：离开房间号{2}", client.endPoint
            , ProtoCodeConf.Room_PlayerLeaveProto, client.player.roomId);

            Room_SyncLeaveRoomProto onProto = new Room_SyncLeaveRoomProto
            {
                admin = proto.admin
            };
            UdpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, onProto.ToArray());
            UdpRoomManager.Instance.PlayerExit(client, client.player.roomId);
        }

        /// <summary>
        /// tcp当请求离开房间
        /// </summary>
        private void OnRequireLeaveRoom(TcpClient client, byte[] buffer)
        {
            Room_PlayerLeaveProto proto = Room_PlayerLeaveProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：离开房间号{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.Room_PlayerLeaveProto, client.player.roomId);

            Room_SyncLeaveRoomProto onProto = new Room_SyncLeaveRoomProto
            {
                admin = proto.admin
            };
            TcpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, onProto.ToArray());
            TcpRoomManager.Instance.PlayerExit(client, client.player.roomId);
        }


    }
}