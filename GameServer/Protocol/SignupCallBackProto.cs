
using GameSever.Utility;
namespace GameSever.Protocol
{
public struct SignupCallBackProto : IProto
{
    public ushort ProtoId => 1101;

    public bool success;
    public string errCode;

    public SignupCallBackProto(bool success)
    {
        this.success = success;
        errCode = "";
    }

    public byte[] ToArray()
    {
        using(CustomMemoryStream ms = new CustomMemoryStream())
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

    public static SignupCallBackProto GetProto(byte[] buffer)
    {
        SignupCallBackProto proto = new SignupCallBackProto();
        using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
        {
            proto.success = ms.ReadBool();
            if (!proto.success)
                proto.errCode = ms.ReadUTF8String();
        }
        return proto;
    }
}
}