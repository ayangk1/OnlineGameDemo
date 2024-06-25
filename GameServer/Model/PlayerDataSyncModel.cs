using GameSever.Controller;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Room;
using GameSever.Server;

namespace GameSever.Model
{
    public class PlayerDataSyncModel : Singleton<PlayerDataSyncModel>
    {
        public void Init()
        {
            //同步房间玩家位置信息
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerDataProto, PlayerDataSyncCallBack);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerDataProto, PlayerDataSyncCallBack);
            //同步房间玩家状态信息
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerActionStatusProto, PlayerActionStatusSyncCallBack);
            UdpRequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerActionStatusProto, PlayerActionStatusSyncCallBack);
        }
        /// <summary>
        /// udp状态同步
        /// </summary>
        private void PlayerActionStatusSyncCallBack(ref UdpClient client, byte[] buffer)
        {
            Room_SyncPlayerActionStatusProto proto = Room_SyncPlayerActionStatusProto.GetProto(buffer);

            //如果没有有这个房间
            if (!UdpRoomManager.Instance.roomMap.ContainsKey(client.player.roomId)) return;

            Room_SyncPlayerActionStatusProto protoBack;
            //如果受伤并且血量为0 直接返回死亡消息
            if (proto.status == ActionStatus.Hit && proto.health <= 0)
            {
                protoBack = new Room_SyncPlayerActionStatusProto(UdpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, ActionStatus.Dead);
            }
            else if (proto.status == ActionStatus.Idel)
            {
                protoBack = new Room_SyncPlayerActionStatusProto(UdpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, proto.status)
                {
                    health = proto.health
                };
            }
            else if (proto.status == ActionStatus.Hit && proto.health > 0)
            {
                protoBack = new Room_SyncPlayerActionStatusProto(UdpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, proto.status)
                {
                    health = proto.health
                };
            }
            else 
            {
                protoBack = new Room_SyncPlayerActionStatusProto(UdpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, proto.status);
            }

            UdpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, protoBack.ToArray());
        }

        /// <summary>
        /// udp数据同步
        /// </summary>
        private void PlayerDataSyncCallBack(ref UdpClient client, byte[] buffer)
        {
            Room_SyncPlayerDataProto proto = Room_SyncPlayerDataProto.GetProto(buffer);

            if (!UdpRoomManager.Instance.roomMap.ContainsKey(client.player.roomId))
                return;

            client.player.syncFrame = proto.frame;
            client.player.x = proto.movePacket.x;
            client.player.y = proto.movePacket.y;
            client.player.z = proto.movePacket.z;
            client.player.r_y = proto.movePacket.r_y;


            try
            {
                Room_SyncPlayerDataProto protoCallback = new Room_SyncPlayerDataProto
                {
                    frame = UdpRoomManager.Instance.roomMap[client.player.roomId].frame,
                    username = proto.username,
                    admin = proto.admin
                };
                MovePacket movePacket = new MovePacket
                {
                    x = proto.movePacket.x,
                    y = proto.movePacket.y,
                    z = proto.movePacket.z,
                    r_y = proto.movePacket.r_y,
                };
                protoCallback.movePacket = movePacket;
                //广播所有数据
                UdpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, protoCallback.ToArray());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //广播状态同步
        private void PlayerActionStatusSyncCallBack(TcpClient client, byte[] buffer)
        {
            Room_SyncPlayerActionStatusProto proto = Room_SyncPlayerActionStatusProto.GetProto(buffer);

            //如果没有有这个房间
            if (!TcpRoomManager.Instance.roomMap.ContainsKey(client.player.roomId)) return;

            Room_SyncPlayerActionStatusProto protoBack;
            //如果受伤并且血量为0 直接返回死亡消息
            if (proto.status == ActionStatus.Hit && proto.health <= 0)
            {
                protoBack = new Room_SyncPlayerActionStatusProto(TcpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, ActionStatus.Dead);
            }
            else
            {
                protoBack = new Room_SyncPlayerActionStatusProto(TcpRoomManager.Instance.roomMap[client.player.roomId].frame, proto.admin, proto.status)
                {
                    health = proto.health
                };
            }
            TcpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, protoBack.ToArray());
        }

        //广播数据同步
        private void PlayerDataSyncCallBack(TcpClient client, byte[] buffer)
        {
            Room_SyncPlayerDataProto proto = Room_SyncPlayerDataProto.GetProto(buffer);

            if (!TcpRoomManager.Instance.roomMap.ContainsKey(client.player.roomId))
                return;

            try
            {
                Room_SyncPlayerDataProto protoCallback = new Room_SyncPlayerDataProto
                {
                    frame = TcpRoomManager.Instance.roomMap[client.player.roomId].frame,
                    username = proto.username,
                    admin = proto.admin
                };
                MovePacket movePacket = new MovePacket
                {
                    x = proto.movePacket.x,
                    y = proto.movePacket.y,
                    z = proto.movePacket.z,
                    r_y = proto.movePacket.r_y,
                };
                protoCallback.movePacket = movePacket;
                //广播所有数据
                TcpRoomManager.Instance.SendRoomOtherPlayer(client, client.player.roomId, protoCallback.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}