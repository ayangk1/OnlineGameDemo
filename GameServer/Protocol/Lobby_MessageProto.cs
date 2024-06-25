using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Lobby_MessageProto : IProto
    {
        public ushort ProtoId => 1300;

        public string message;
        public string admin;

        public Lobby_MessageProto(string message,string admin)
        {
            this.message = message;
            this.admin = admin;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteUTF8String(message);
                ms.WriteUTF8String(admin);
                return ms.ToArray();
            }
        }

        public static Lobby_MessageProto GetProto(byte[] buffer)
        {
            Lobby_MessageProto proto = new Lobby_MessageProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.message = ms.ReadUTF8String();
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}