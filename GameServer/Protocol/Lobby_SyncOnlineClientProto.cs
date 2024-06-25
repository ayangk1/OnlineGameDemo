using GameSever.Utility;
using System.Collections.Generic;

namespace GameSever.Protocol
{
    public struct ClientInfo
    {
        public string username;
        public string admin;
    }
    public struct Lobby_SyncOnlineClientProto : IProto
    {
        public ushort ProtoId => 1201;

        public int count;
        public List<ClientInfo> clientList;

        public byte[] ToArray()
        {
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort(ProtoId);
                ms.WriteInt(count);
                for (int i = 0; i < count; i++)
                {
                    ms.WriteUTF8String(clientList[i].username);
                    ms.WriteUTF8String(clientList[i].admin);
                }
                return ms.ToArray();
            }
        }

        public static Lobby_SyncOnlineClientProto GetProto(byte[] buffer)
        {
            Lobby_SyncOnlineClientProto proto = new Lobby_SyncOnlineClientProto();
            using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
            {
                proto.count = ms.ReadInt();
                proto.clientList = new List<ClientInfo>();
                for (int i = 0; i < proto.count; i++)
                {
                    ClientInfo clientInfo = new ClientInfo
                    {
                        username = ms.ReadUTF8String(),
                        admin = ms.ReadUTF8String()
                    };
                    proto.clientList.Add(clientInfo);
                }
            }
            return proto;
        }
    }
}