using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using UnityEngine;

public class RoomModel : SingletonMonoBehaviour<RoomModel>
{
    private void Start()
    {
        Init();
        //成功进入房间
        Room_SuccessEnterProto proto = new Room_SuccessEnterProto();
        proto.admin = UserDataManager.Instance.ownerInfo.admin;
        NetworkManager.Instance.SendUdpMsg(proto.ToArray());
    }
    public void Init()
    {
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomProto, OnRequireEnterRoom);
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Room_OtherPlayerEnterProto, OnOtherPlayerEnterRoom);
    }
    
    /// <summary>
    /// 其他玩家进入房间
    /// </summary>
    private void OnOtherPlayerEnterRoom(byte[] buffer)
    {
        Room_OtherPlayerEnterProto proto = Room_OtherPlayerEnterProto.GetProto(buffer);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Room_OtherPlayerEnterProto}发送");
        
        UserDataManager.Instance.localPlayer.GetComponent<Character>().SendServer();
    }

    private void OnRequireEnterRoom(byte[] buffer)
    {
        Lobby_RequireEnterRoomProto proto = Lobby_RequireEnterRoomProto.GetProto(buffer);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Lobby_RequireEnterRoomProto}发送");
        
        //进入房间后发送自己本地时间
        ClientSendLocalTimeProto timeProto = new ClientSendLocalTimeProto();
        timeProto.localTime = Time.realtimeSinceStartup * 1000;//秒转毫秒
        GlobalConfig.Instance.CheckServerTime = Time.realtimeSinceStartup;
        NetworkManager.Instance.SendTcpMsg(timeProto.ToArray());
    }

    private void OnDestroy()
    {
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomProto, OnRequireEnterRoom);
        
    }
}
