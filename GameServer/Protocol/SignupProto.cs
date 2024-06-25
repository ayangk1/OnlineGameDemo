using GameSever.Utility;
namespace GameSever.Protocol
{
public struct SignupProto : IProto
{
    public ushort ProtoId => 1100;

    public string usrname;
    public string admin;
    public string password;

    public SignupProto(string usrname,string admin,string password)
    {
        this.usrname = usrname;
        this.admin = admin;
        this.password = password;
    }

    public byte[] ToArray()
    {
        using(CustomMemoryStream ms = new CustomMemoryStream())
        {
            ms.WriteUShort(ProtoId);
            ms.WriteUTF8String(usrname);
            ms.WriteUTF8String(admin);
            ms.WriteUTF8String(password);
            return ms.ToArray();
        }
    }

    public static SignupProto GetProto(byte[] buffer)
    {
        SignupProto proto = new SignupProto();
        using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
        {
            proto.usrname = ms.ReadUTF8String();
            proto.admin = ms.ReadUTF8String();
            proto.password = ms.ReadUTF8String();
        }
        return proto;
    }
}
}