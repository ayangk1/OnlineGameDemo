using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using UnityEngine;

public class LobbyModel : SingletonMonoBehaviour<LobbyModel>
{
    private bool isUpdateOwner;
    /// <summary>
    /// 在线客户端列表
    /// </summary>
    public List<ClientInfo> onlineClients = new();

    private void Start()
    {
        isUpdateOwner = false;
        Init();
    }


    public void Init()
    {
        //
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_SyncOnlineClientProto,OnSyncOnlineClient);
        //请求进入房间
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomCallBackProto,OnEnterRoom);
    }
    /// <summary>
    /// 请求进入房间
    /// </summary>
    private void OnEnterRoom(byte[] buffer)
    {
        Lobby_RequireEnterRoomCallBackProto proto = Lobby_RequireEnterRoomCallBackProto.GetProto(buffer);
        
        Debug.Log($"服务器通过协议{ProtoCodeConf.Lobby_RequireEnterRoomCallBackProto}发送");
        
        if (proto.success)
            SceneOpenManager.Instance.LoadScene(2, 3);
        else
        {
            Debug.Log(proto.errCode);
            GameManager.Instance.enterRoomPrompt.text = proto.errCode;
        }
    }
    
    /// <summary>
    /// 同步大厅在线玩家
    /// </summary>
    private void OnSyncOnlineClient(byte[] buffer)
    {
        Lobby_SyncOnlineClientProto proto = Lobby_SyncOnlineClientProto.GetProto(buffer);
        GameManager.Instance.loadingPanel.SetActive(false);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Lobby_SyncOnlineClientProto}发送：{proto.count}");
        
        for (int i = 0; i < proto.count; i++)
        {
            //如果是自己 并且没更新过自己
            if (proto.clientList[i].admin == UserDataManager.Instance.ownerInfo.admin && !isUpdateOwner)
            {
                isUpdateOwner = true;
                UserDataManager.Instance.ownerInfo.username = proto.clientList[i].username;
                GameManager.Instance.ListMessageBlock(proto.clientList[i].username + "(我)", proto.clientList[i].admin);
                GameManager.Instance.ShowGlobalPrompt("(我)进入大厅");
            }
        }

        for (int i = 0; i < proto.count; i++)
        {
            //如果不是自己 
            lock (onlineClients)
            {
                if (proto.clientList[i].admin != UserDataManager.Instance.ownerInfo.admin && !onlineClients.Contains(proto.clientList[i]))
                {
                    onlineClients.Add(proto.clientList[i]);
                    GameManager.Instance.ListMessageBlock(proto.clientList[i].username + "(在线)", proto.clientList[i].admin);
                    GameManager.Instance.ShowGlobalPrompt($"({proto.clientList[i].username})" + "进入大厅");
                }
            }
        }
    }

    private void OnDestroy()
    {
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Lobby_SyncOnlineClientProto,OnSyncOnlineClient);
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Lobby_RequireEnterRoomCallBackProto,OnEnterRoom);
    }
}