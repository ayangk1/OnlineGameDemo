using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct LogoutCallBackProto: IProto
    {
        public ushort ProtoId => 1401;

        public string admin;

        public LogoutCallBackProto(string admin)
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

        public static LogoutCallBackProto GetProto(byte[] buffer)
        {
            LogoutCallBackProto proto = new LogoutCallBackProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}