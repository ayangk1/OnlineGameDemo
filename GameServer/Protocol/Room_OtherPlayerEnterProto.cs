using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Room_OtherPlayerEnterProto :IProto
    {
        public ushort ProtoId => 1600;

        public string admin;

        public Room_OtherPlayerEnterProto(string admin)
        {
            this.admin = admin;
        }

        public byte[] ToArray()
        {
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteUTF8String(admin);
                return ms.ToArray();
            }
        }

        public static Room_OtherPlayerEnterProto GetProto(byte[] buffer)
        {
            Room_OtherPlayerEnterProto proto = new Room_OtherPlayerEnterProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}