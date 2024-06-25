using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct Lobby_ClientEnterProto : IProto
    {
        public ushort ProtoId => 1200;
        
        public string admin;
        
        public Lobby_ClientEnterProto(string admin)
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

        public static Lobby_ClientEnterProto GetProto(byte[] buffer)
        {
            Lobby_ClientEnterProto proto = new Lobby_ClientEnterProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}