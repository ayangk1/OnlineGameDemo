using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.PlayerSpace;
using GameSever.Server;
using Newtonsoft.Json;


namespace GameSever.Controller
{
    public class TcpRequestController : Singleton<TcpRequestController>
    {
        public delegate void OnActionHandler(TcpClient client, byte[] buffer);
        private Dictionary<ushort, List<OnActionHandler>> dic = new();
        //添加请求监听
        public void AddRequestListener(ushort protoCode, OnActionHandler handler)
        {
            if (dic.ContainsKey(protoCode)) dic[protoCode].Add(handler);
            else
            {
                List<OnActionHandler> lstHandler = new List<OnActionHandler>();
                lstHandler.Add(handler);
                dic[protoCode] = lstHandler;
            }
        }
        //移除请求监听
        public void RemoveRequestListener(ushort protoCode, OnActionHandler handler)
        {
            if (dic.ContainsKey(protoCode))
            {
                List<OnActionHandler> lstHandler = dic[protoCode];
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0) dic.Remove(protoCode);
            }
        }
        //派发请求
        public void Dispatch(ushort protoCode, TcpClient client, byte[] buffer)
        {
            if (dic.ContainsKey(protoCode))
            {
                List<OnActionHandler> lstHandler = dic[protoCode];
                if (lstHandler.Count > 0 && lstHandler != null)
                {
                    for (int i = 0; i < lstHandler.Count; i++)
                    {
                        if (lstHandler[i] != null)
                            lstHandler[i](client, buffer);
                    }
                }
            }
        }
    }
}