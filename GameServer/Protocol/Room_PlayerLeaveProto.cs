using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Room_PlayerLeaveProto : IProto
    {
        public ushort ProtoId => 1601;

        public string admin;

        public Room_PlayerLeaveProto(string admin)
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

        public static Room_PlayerLeaveProto GetProto(byte[] buffer)
        {
            Room_PlayerLeaveProto proto = new Room_PlayerLeaveProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}