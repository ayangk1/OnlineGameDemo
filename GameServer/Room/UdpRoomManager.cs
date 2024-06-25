using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Server;
using GameSever.Room;
using System.Dynamic;
using GameSever.PlayerSpace;

namespace GameSever.Room
{
    public class UdpRoomManager : Singleton<UdpRoomManager>
    {
        //id:房间信息
        public Dictionary<int, RoomInfo> roomMap = new();
        public Dictionary<int, Timer> roomTimer = new();
        public Dictionary<int, Player> playerFrame = new();
        public int MaxRoomId = 10001;
        public int MinRoomId = 100;

        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom(int roomId, ref UdpClient Creater)
        {
            var room = new RoomInfo();
            if (roomId < MinRoomId || roomId >= MaxRoomId)
            {
                Random random = new Random();
                room.id = random.Next(MinRoomId, MaxRoomId);
            }
            else
                room.id = roomId;

            room.playerCount++;
            if(!room.udpEndPointPlayers.Contains(Creater.endPoint))
                room.udpEndPointPlayers.Add(Creater.endPoint);
            room.udpEndPointCreater = Creater.endPoint;
            room.frame = 1;
            Creater.player.roomId = roomId;
            roomMap.Add(room.id, room);

            var _timer = StartTimer(_ =>
             {
                 if (roomMap.ContainsKey(room.id))
                 {
                     roomMap[room.id].frame++;
                 }

             }, 0, 1000 / 60);
            roomTimer.Add(room.id, _timer);
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        public void EnterRoom(int roomId, ref UdpClient client)
        {
            //如果没有包含房间
            if (!roomMap.ContainsKey(roomId))
                CreateRoom(roomId, ref client);
            else
            {
                if (roomId < MinRoomId || roomId >= MaxRoomId) return;

                var room = roomMap[roomId];
                client.player.roomId = roomId;
                if(!room.udpEndPointPlayers.Contains(client.endPoint))
                    room.udpEndPointPlayers.Add(client.endPoint);
                room.playerCount++;
            }
        }
        /// <summary>
        /// 发送给房间其他玩家
        /// </summary>
        public void SendRoomOtherPlayer(UdpClient client, int roomId, byte[] data)
        {
            //如果有这个房间
            if (!roomMap.ContainsKey(roomId)) return;

            foreach (var endPoint in roomMap[roomId].udpEndPointPlayers)
            {
                if (endPoint.ToString() != client.endPoint.ToString())
                    client.SendMsg(data, endPoint);
            }
        }
        /// <summary>
        /// 玩家退出房间
        /// </summary>
        public void PlayerExit(UdpClient client, int roomId)
        {
            //没有房间退出
            if (!roomMap.ContainsKey(roomId)) return;
            //如果房间不包含这个玩家 则返回
            if (!roomMap[roomId].udpEndPointPlayers.Contains(client.endPoint)) return;

            roomMap[roomId].udpEndPointPlayers.Remove(client.endPoint);
            roomMap[roomId].playerCount--;

            if (roomMap[roomId].playerCount > 0) return;
            //如果房间没人则移除房间
            var closeRoom = Task.Run(async () =>
            {
                await Task.Delay(1000);
                lock (roomMap)
                {
                    if (roomMap.ContainsKey(roomId))
                    {
                        Console.WriteLine($"移除房间{roomId}");
                        roomMap[roomId].frame = 1;
                        roomMap.Remove(roomId);
                    }

                    lock (roomTimer)
                    {
                        //移除计时器
                        if (roomTimer.ContainsKey(roomId))
                        {
                            roomTimer[roomId].Dispose();
                            roomTimer.Remove(roomId);
                        }
                    }
                }
            });
        }

        //开启计时器 执行传入方法
        public Timer StartTimer(TimerCallback action, int delay, int interval)
        {
            return new Timer(action, null, delay, interval);

        }
    }
}