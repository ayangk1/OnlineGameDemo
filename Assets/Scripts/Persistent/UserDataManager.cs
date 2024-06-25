using System.Collections.Generic;
using GameSever.Protocol;
using UnityEngine;


public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
{
    /// <summary>
    /// 本地玩家
    /// </summary>
    public GameObject localPlayer;
    /// <summary>
    /// 当前客户端信息
    /// </summary>
    public ClientInfo ownerInfo;
    
    /// <summary>
    /// 是否是刚打开游戏
    /// </summary>
    public bool first;
}
