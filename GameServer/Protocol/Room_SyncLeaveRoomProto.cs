using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Room_SyncLeaveRoomProto :IProto
    {
        public ushort ProtoId => 1602;

        public string admin;

        public Room_SyncLeaveRoomProto(string admin)
        {
            this.admin = admin;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteUTF8String(admin);
                return ms.ToArray();
            }
        }

        public static Room_SyncLeaveRoomProto GetProto(byte[] buffer)
        {
            Room_SyncLeaveRoomProto proto = new Room_SyncLeaveRoomProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}