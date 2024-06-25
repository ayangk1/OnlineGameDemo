using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Room_SuccessEnterProto : IProto
    {
        public ushort ProtoId => 1603;

        public string admin;
        public Room_SuccessEnterProto(string admin)
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

        public static Room_SuccessEnterProto GetProto(byte[] buffer)
        {
            Room_SuccessEnterProto proto = new Room_SuccessEnterProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}