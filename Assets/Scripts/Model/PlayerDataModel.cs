using System.Collections.Generic;
using System.Linq;
using GameSever.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataModel : SingletonMonoBehaviour<PlayerDataModel>
{
    public int syncFrame = 0;
    
    public GameObject bullet;
    public bool first;

    public Dictionary<string, float> initDatas = new();
    /// <summary>
    /// 玩家血条GameObject
    /// </summary>
    public Dictionary<string, GameObject> healthBars = new();
    /// <summary>
    /// 当前其他玩家
    /// </summary>
    public Dictionary<string,GameObject> currRoomOtherPlayer = new();
    /// <summary>
    /// 死亡玩家
    /// </summary>
    public List<string> deadPlayer = new();
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        //玩家离开房间
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncLeaveRoomProto, OnCastLeaveRoom);
        //数据同步
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerDataProto, CastPlayerCallBack);
        //状态同步
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Room_SyncPlayerActionStatusProto, CastPlayerActionStatusSyncCallBack);
        RequestController.Instance.AddRequestListener(ProtoCodeConf.serverReturnTimeProto,ServerReturnTimeCallBack);
    }

    private void ServerReturnTimeCallBack(byte[] buffer)
    {
        ServerReturnTimeProto proto = ServerReturnTimeProto.GetProto(buffer);
        
        Debug.Log($"服务器通过协议{ProtoCodeConf.serverReturnTimeProto}发送");

        float localTime = proto.localTime;
        long serverTime = proto.serverTime;

        GlobalConfig.Instance.PingValue = (int)((Time.realtimeSinceStartup * 1000 - localTime) * 0.5f);
        GlobalConfig.Instance.GameServerTime = serverTime - GlobalConfig.Instance.PingValue;
    }

    /// <summary>
    /// 同步状态
    /// </summary>
    private void CastPlayerActionStatusSyncCallBack(byte[] buffer)
    {
        Room_SyncPlayerActionStatusProto proto = Room_SyncPlayerActionStatusProto.GetProto(buffer);
        
        if (proto.frame < syncFrame) return;
        syncFrame = proto.frame;
        
        if (proto.status == ActionStatus.Attack)
        {
            if (deadPlayer.Contains(proto.admin)) return;
            //如果（其他玩家列表）中包含广播的这个玩家
            if (currRoomOtherPlayer.TryGetValue(proto.admin, out var player))
            {
                var bulletObj = Instantiate(bullet,player.transform.position, player.transform.rotation);
                bulletObj.GetComponent<Bullet>().Init(false);
            }
        }
        else if (proto.status == ActionStatus.Idel)
        {
            if (currRoomOtherPlayer.TryGetValue(proto.admin, out var player))
            {
                player.GetComponent<Character>().Health = proto.health;
            }
        }
        else if (proto.status == ActionStatus.Hit)
        {
            //如果（其他玩家列表）中包含广播的这个玩家
            if (currRoomOtherPlayer.TryGetValue(proto.admin, out var player))
            {
                player.GetComponent<Character>().Health = proto.health;
            }
        }
        else if (proto.status == ActionStatus.Dead)
        {
            //如果（其他玩家列表）中包含广播的这个玩家
            if (currRoomOtherPlayer.TryGetValue(proto.admin, out var player))
            {
                if(player != null)
                    player.GetComponent<Character>().Health = proto.health;
            }
        }
    }
    
    /// <summary>
    /// 收到玩家离开房间
    /// </summary>
    private void OnCastLeaveRoom(byte[] buffer)
    {
        Room_SyncLeaveRoomProto proto = Room_SyncLeaveRoomProto.GetProto(buffer);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Room_SyncLeaveRoomProto}发送");
        
        //移除玩家
        RemovePlayer(proto.admin);
        //如果死的玩家离开则移除
        if (deadPlayer.Contains(proto.admin))
            deadPlayer.Remove(proto.admin);
    }
    
    /// <summary>
    /// 移除玩家
    /// </summary>
    public void RemovePlayer(string admin)
    {
        //先移除血条
        if (healthBars.ContainsKey(admin))
        {
            Destroy(healthBars[admin]);
            healthBars.Remove(admin);
        }
        //再移除人物
        if (currRoomOtherPlayer.ContainsKey(admin))
        {
            Destroy(currRoomOtherPlayer[admin]);
            currRoomOtherPlayer.Remove(admin);
        }
    }
    
    /// <summary>
    /// 同步位置
    /// </summary>
    private void CastPlayerCallBack(byte[] buffer)
    {
        Room_SyncPlayerDataProto proto = Room_SyncPlayerDataProto.GetProto(buffer);

        if (proto.frame < syncFrame) return;
        syncFrame = proto.frame;
        
        //如果是死亡 且没有退出则返回
        if (deadPlayer.Contains(proto.admin)) return;
        
        
        //如果（其他玩家列表）中包含广播的这个玩家 就直接修改位置和旋转
        if (currRoomOtherPlayer.ContainsKey(proto.admin) && currRoomOtherPlayer[proto.admin] != null)
        {
            currRoomOtherPlayer[proto.admin].transform.position = new Vector3(proto.movePacket.x, proto.movePacket.y, proto.movePacket.z);
            currRoomOtherPlayer[proto.admin].transform.eulerAngles = new Vector3(0, proto.movePacket.r_y,0);
        }
        else
        {
            if( proto.admin == UserDataManager.Instance.ownerInfo.admin) return;
            
            //如果不包含这个玩家 则生成这个玩家
            var obj = GameController.Instance.SpwanOtherPlayer();
            obj.transform.position = new Vector3(proto.movePacket.x, proto.movePacket.y, proto.movePacket.z);
            obj.GetComponent<Character>().admin = proto.admin;
            currRoomOtherPlayer.Add(proto.admin,obj);
            //生成血条
            GameController.Instance.SpwanHealthBar(obj.transform,proto.admin,proto.username);

        }
    }
    
    
    private void OnDestroy()
    {
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Room_SyncLeaveRoomProto, OnCastLeaveRoom);
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Room_SyncPlayerDataProto, CastPlayerCallBack);
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Room_SyncPlayerActionStatusProto, CastPlayerActionStatusSyncCallBack);
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.serverReturnTimeProto,ServerReturnTimeCallBack);
    }
}