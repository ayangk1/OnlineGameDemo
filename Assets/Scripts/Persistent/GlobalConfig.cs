using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Operation
{
    keyboard,
    xbox
}
public class GlobalConfig : SingletonMonoBehaviour<GlobalConfig>
{
    public long PingValue;
    public long GameServerTime;
    public float CheckServerTime;
    public Operation operation;

    private void Start()
    {
        operation = Operation.keyboard;
    }

    public long GetCurrServerTime()
    {
        return (long)(((Time.realtimeSinceStartup - CheckServerTime) * 1000) + GameServerTime);
    }

    private void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUIStyle labelStyle = new GUIStyle();
        // 设置字体大小
        labelStyle.fontSize = 32;
        GUI.Label(new Rect(10,10,400,50),"Ping:" + PingValue,labelStyle);
    }
}
