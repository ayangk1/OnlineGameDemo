using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameSever.Protocol;
using UnityEngine;

public class UDPConnect : MonoBehaviour
{
    public UdpClient udpSocket;
    public string serverIp;
    public int port;
    public string sendStr;

    public string message;

    private void Start()
    {
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_MessageProto, LoginCallBack);
        NetworkManager.Instance.Connect_UDP();
    }

    private void LoginCallBack(byte[] buffer)
    {
        Lobby_MessageProto proto = Lobby_MessageProto.GetProto(buffer);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Lobby_MessageProto}发送：{proto.message + ":" + proto.admin}");
        message = proto.message;
    }
    
    private void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUIStyle labelStyle = new GUIStyle();
        // 设置字体大小
        labelStyle.fontSize = 46;
        GUI.Label(new Rect(10,10,400,50),message,labelStyle);
    }

    public void Connect()
    {
        Lobby_MessageProto proto = new Lobby_MessageProto(sendStr,"admin");
        NetworkManager.Instance.SendUdpMsg(proto.ToArray());
    }

}
