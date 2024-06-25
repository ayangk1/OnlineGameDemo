namespace GameSever.Protocol
{
    public class ProtoCodeConf
    {
        //登陆协议
        public const ushort loginProto = 1000;
        //登陆callback协议
        public const ushort loginCallBackProto = 1001;
        //注册协议
        public const ushort signupProto = 1100;
        //注册callback协议
        public const ushort signupCallBackProto = 1101;
        #region 大厅相关
        //玩家进入大厅协议
        public const ushort Lobby_ClientEnterProto = 1200;
        //同步大厅玩家信息协议
        public const ushort Lobby_SyncOnlineClientProto = 1201;
        //玩家请求进入房间协议
        public const ushort Lobby_RequireEnterRoomProto = 1202;
        //玩家请求进入房间CallBack协议
        public const ushort Lobby_RequireEnterRoomCallBackProto = 1203;
        //消息协议
        public const ushort Lobby_MessageProto = 1300;
        #endregion
        //登出协议
        public const ushort logoutProto = 1400;
        //登出callback协议
        public const ushort logoutCallBackProto = 1401;
        #region 房间相关
        //服务器广播玩家同步数据协议
        public const ushort Room_SyncPlayerDataProto = 1501;
        //其他玩家进入房间房间协议
        public const ushort Room_OtherPlayerEnterProto = 1600;
        //玩家请求离开房间协议
        public const ushort Room_PlayerLeaveProto = 1601;
        //服务器广播玩家离开房间协议
        public const ushort Room_SyncLeaveRoomProto = 1602;
        //玩家成功进入房间协议
        public const ushort Room_SuccessEnterProto = 1603;
        //广播玩家行动状态同步协议
        public const ushort Room_SyncPlayerActionStatusProto = 1604;
        #endregion
        //玩家发送本地时间
        public const ushort clientSendLocalTimeProto = 1700;
        //服务器返回时间
        public const ushort serverReturnTimeProto = 1701;
    }
}