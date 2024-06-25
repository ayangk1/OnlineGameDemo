using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Lobby_RequireEnterRoomProto : IProto
    {
        public ushort ProtoId => 1202;

        public string admin;
        public int roomId;
        

        public Lobby_RequireEnterRoomProto(string admin,int roomId)
        {
            this.admin = admin;
            this.roomId = roomId;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteUTF8String(admin);
                ms.WriteInt(roomId);
                return ms.ToArray();
            }
        }

        public static Lobby_RequireEnterRoomProto GetProto(byte[] buffer)
        {
            Lobby_RequireEnterRoomProto proto = new Lobby_RequireEnterRoomProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
                proto.roomId = ms.ReadInt();
            }
            return proto;
        }
    }
}