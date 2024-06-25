using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct LoginCallBackProto: IProto
    {
        public ushort ProtoId => 1001;

        public bool success;
        public string errCode;

        public LoginCallBackProto(bool success)
        {
            this.success = success;
            errCode = "";
        }

        public byte[] ToArray()
        {
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteBool(success);
                if (!success)
                {
                    ms.WriteUTF8String(errCode);
                }
                return ms.ToArray();
            }
        }

        public static LoginCallBackProto GetProto(byte[] buffer)
        {
            LoginCallBackProto proto = new LoginCallBackProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.success = ms.ReadBool();
                if (!proto.success)
                    proto.errCode = ms.ReadUTF8String();
            }
            return proto;
        }
    }
}