using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct LoginProto : IProto
    {
        public ushort ProtoId => 1000;

        public string admin;
        public string password;

        public LoginProto(string admin,string password)
        {
            this.admin = admin;
            this.password = password;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteUTF8String(admin);
                ms.WriteUTF8String(password);
                return ms.ToArray();
            }
        }

        public static LoginProto GetProto(byte[] buffer)
        {
            LoginProto proto = new LoginProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.admin = ms.ReadUTF8String();
                proto.password = ms.ReadUTF8String();
            }
            return proto;
        }

    }
}