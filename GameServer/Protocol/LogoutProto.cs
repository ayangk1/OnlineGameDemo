using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct LogoutProto: IProto
    {
        public ushort ProtoId => 1400;

        public string admin;

        public LogoutProto(string admin)
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

        public static LogoutProto GetProto(byte[] buffer)
        {
            LogoutProto proto = new LogoutProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}