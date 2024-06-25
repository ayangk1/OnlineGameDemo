using System.Collections.Generic;
using UnityEngine;

namespace GameSever.Protocol
{
    public class RequestController : SingletonMonoBehaviour<RequestController>
    {
        public delegate void OnActionHandler(byte[] buffer);
        private Dictionary<ushort, List<OnActionHandler>> dic = new();
        //添加请求监听
        public void AddRequestListener(ushort protoCode, OnActionHandler handler)
        {
            if (dic.ContainsKey(protoCode)) dic[protoCode].Add(handler);
            else
            {
                List<OnActionHandler> lstHandler = new();
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
        public void Dispatch(ushort protoCode, byte[] buffer)
        {
            if (dic.TryGetValue(protoCode, out var lstHandler))
            {
                if (lstHandler.Count > 0)
                {
                    for (int i = 0; i < lstHandler.Count; i++)
                    {
                        if (lstHandler[i] != null)
                            lstHandler[i](buffer);
                    }
                }
            }
        }
    }
}