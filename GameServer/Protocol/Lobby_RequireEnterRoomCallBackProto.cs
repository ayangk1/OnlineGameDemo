using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Lobby_RequireEnterRoomCallBackProto : IProto
    {
        public ushort ProtoId => 1203;

        public bool success;
        public int currRoomFrame;
        public string errCode;

        public byte[] ToArray()
        {
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteBool(success);
                 ms.WriteInt(currRoomFrame);
                if (!success)
                {
                    ms.WriteUTF8String(errCode);
                }
                return ms.ToArray();
            }
        }

        public static Lobby_RequireEnterRoomCallBackProto GetProto(byte[] buffer)
        {
            Lobby_RequireEnterRoomCallBackProto proto = new Lobby_RequireEnterRoomCallBackProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.success = ms.ReadBool();
                proto.currRoomFrame = ms.ReadInt();
                if (!proto.success)
                    proto.errCode = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}