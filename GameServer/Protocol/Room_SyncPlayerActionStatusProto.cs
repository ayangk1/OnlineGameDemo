using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Utility;

namespace GameSever.Protocol
{
    public enum ActionStatus
    {
        Idel,
        Move,
        Attack,
        Hit,
        Recovery,
        Skill,
        Dead
    }
    public struct Room_SyncPlayerActionStatusProto :IProto
    {
        public ushort ProtoId => 1604;
        public int frame;

        public string admin;
        public ActionStatus status;
        public float health;

        public Room_SyncPlayerActionStatusProto(int frame,string admin,ActionStatus status)
        {
            this.frame = frame;
            this.admin = admin;
            this.status = status;
            this.health = 0;
        }

        public byte[] ToArray()
        {
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteInt(frame);
                ms.WriteUTF8String(admin);
                ms.WriteInt((int)status);
                if (status == ActionStatus.Hit)
                {
                    ms.WriteFloat(health);
                }
                else if (status == ActionStatus.Recovery)
                {
                    ms.WriteFloat(health);
                }
                else if (status == ActionStatus.Idel)
                {
                    ms.WriteFloat(health);
                }
                
                return ms.ToArray();
            }
        }

        public static Room_SyncPlayerActionStatusProto GetProto(byte[] buffer)
        {
            Room_SyncPlayerActionStatusProto proto = new Room_SyncPlayerActionStatusProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.frame = ms.ReadInt();
                proto.admin = ms.ReadUTF8String();
                proto.status = (ActionStatus)ms.ReadInt();
                if (proto.status == ActionStatus.Hit)
                {
                    proto.health = ms.ReadFloat();
                }
                else if (proto.status == ActionStatus.Recovery)
                {
                    proto.health = ms.ReadFloat();
                }
                else if (proto.status == ActionStatus.Idel)
                {
                    proto.health = ms.ReadFloat();
                }
            }
            return proto;
        }
    }
}