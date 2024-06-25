using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct ServerReturnTimeProto : IProto
    {
        public ushort ProtoId => 1701;

        public float localTime;
        public long serverTime;

        public ServerReturnTimeProto(float localTime,long serverTime)
        {
            this.localTime = localTime;
            this.serverTime = serverTime;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteFloat(localTime);
                ms.WriteLong(serverTime);
                return ms.ToArray();
            }
        }

        public static ServerReturnTimeProto GetProto(byte[] buffer)
        {
            ServerReturnTimeProto proto = new ServerReturnTimeProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.localTime = ms.ReadFloat();
                proto.serverTime = ms.ReadLong();
            }
            return proto;
        }
    }
}