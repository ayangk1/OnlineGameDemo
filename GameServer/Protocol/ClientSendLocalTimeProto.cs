using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct ClientSendLocalTimeProto : IProto
    {
        public ushort ProtoId => 1700;

        public float localTime;

        public ClientSendLocalTimeProto(float localTime)
        {
            this.localTime = localTime;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteFloat(localTime);
                return ms.ToArray();
            }
        }

        public static ClientSendLocalTimeProto GetProto(byte[] buffer)
        {
            ClientSendLocalTimeProto proto = new ClientSendLocalTimeProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.localTime = ms.ReadFloat();
            }
            return proto;
        }
    }
}