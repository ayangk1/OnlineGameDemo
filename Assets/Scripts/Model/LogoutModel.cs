using System;

using GameSever.Protocol;
using UnityEngine;

namespace GameSever.Model
{
    public class LogoutModel: SingletonMonoBehaviour<LogoutModel>
    {
        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            RequestController.Instance.AddRequestListener(ProtoCodeConf.logoutCallBackProto, LogoutCallBack);
        }

        private void LogoutCallBack(byte[] buffer)
        {
            LogoutCallBackProto proto = LogoutCallBackProto.GetProto(buffer);

            Debug.Log($"服务器通过协议{ProtoCodeConf.logoutCallBackProto}发送");
            
            GameManager.Instance.RemoveListMessageBlock(proto.admin);
        }
        
        private void OnDestroy()
        {
            RequestController.Instance.RemoveRequestListener(ProtoCodeConf.logoutCallBackProto, LogoutCallBack);
        }
    }
}