using GameSever.Utility;

namespace GameSever.Protocol
{
    public struct MovePacket
    {
        public float x;
        public float y;
        public float z;
        public float r_y;
    }
    public struct Room_SyncPlayerDataProto : IProto
    {
        public ushort ProtoId => 1501;

        public int frame;
        public string admin;
        public string username;
        public MovePacket movePacket;

        public Room_SyncPlayerDataProto(int frame,string admin,string username,MovePacket movePacket)
        {
            this.frame = frame;
            this.admin = admin;
            this.username = username;
            this.movePacket = movePacket;
        }

        public byte[] ToArray()
        {
            using(CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteInt(frame);
                ms.WriteUTF8String(admin);
                ms.WriteUTF8String(username);
                ms.WriteFloat(movePacket.x);
                ms.WriteFloat(movePacket.y);
                ms.WriteFloat(movePacket.z);
                ms.WriteFloat(movePacket.r_y);
                return ms.ToArray();
            }
        }

        public static Room_SyncPlayerDataProto GetProto(byte[] buffer)
        {
            Room_SyncPlayerDataProto proto = new Room_SyncPlayerDataProto();
            using(CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.frame = ms.ReadInt();
                proto.admin = ms.ReadUTF8String();
                proto.username = ms.ReadUTF8String();
                proto.movePacket.x = ms.ReadFloat();
                proto.movePacket.y = ms.ReadFloat();
                proto.movePacket.z = ms.ReadFloat();
                proto.movePacket.r_y = ms.ReadFloat();
            }
            return proto;
        }
    }
}