using System;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using GameSever.Protocol;
using UnityEngine.SceneManagement;


public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public GameObject loadingPanel;
    public Transform messageField;
    public GameObject messageBlock;
    public GameObject listMessageBlock;
    public Transform onlineClientField;

    public Button sendButton;
    public Button exitButton;
    public Button enterRoomButton;
    public Button switchAccountButton;
    public TMP_InputField inputField;
    public TMP_InputField roomIdInputField;

    public TextMeshProUGUI globalPrompt;
    public TextMeshProUGUI enterRoomPrompt;

    private DateTime lastSend;
    private TimeSpan timeSpan;
    
    private List<GameObject> onlineClients = new ();

    protected override void Awake()
    {
        loadingPanel.SetActive(true);
        //通知进入大厅
        Lobby_ClientEnterProto proto = new Lobby_ClientEnterProto(UserDataManager.Instance.ownerInfo.admin);
        NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        base.Awake();
    }

    private void Start()
    {
        
        InitButton();
    }

    public void InitButton()
    {
        sendButton.onClick.AddListener(SendMessage);
        exitButton.onClick.AddListener(Exit);
        enterRoomPrompt.text = "";
        //进入房间按钮
        enterRoomButton.onClick.AddListener(() =>
        {
            string roomId = roomIdInputField.text;
            if (string.IsNullOrWhiteSpace(roomId))
            {
                enterRoomPrompt.text = "请输入房间号";
                return;
            }
            //请求进入房间
            Lobby_RequireEnterRoomProto proto = new Lobby_RequireEnterRoomProto(UserDataManager.Instance.ownerInfo.admin,int.Parse(roomId));
            // NetworkManager.Instance.SendTcpMsg(proto.ToArray());
            NetworkManager.Instance.SendUdpMsg(proto.ToArray());
        });

        switchAccountButton.onClick.AddListener(() =>
        {
            LogoutProto proto = new LogoutProto(UserDataManager.Instance.ownerInfo.admin);
            NetworkManager.Instance.SendTcpMsg(proto.ToArray());
            
            SceneOpenManager.Instance.LoadScene(2, 0);
        });
    }


    public void Exit()
    {
        //告诉其他玩家
        LogoutProto proto = new LogoutProto(UserDataManager.Instance.ownerInfo.admin);
        NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        //退出
        Application.Quit();
    }
    
    
    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;
        MessageBlock(inputField.text, TextAlignmentOptions.TopRight);
        Lobby_MessageProto proto = new Lobby_MessageProto(string.Format(inputField.text), UserDataManager.Instance.ownerInfo.admin);
        NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        inputField.text = "";
    }

    public void ShowGlobalPrompt(string message)
    {
        if (globalPrompt == null) return;
        globalPrompt.text = message;
        globalPrompt.GetComponent<Animator>().Play("GlobalPrompt");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SendMessage();
    }


    private void TimeBlock()
    {
        //第一次判断
        if ((DateTime.Now - lastSend).Days > 1)
        {
            lastSend = DateTime.Now;
            var obj = Instantiate(messageBlock, messageField);
            obj.GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            obj.GetComponent<TextMeshProUGUI>().fontSize = 20;
            obj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }
        else
        {
            timeSpan = DateTime.Now - lastSend;
            if (timeSpan.Seconds < 10) return;
            lastSend = DateTime.Now;
            var obj = Instantiate(messageBlock, messageField);
            obj.GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            obj.GetComponent<TextMeshProUGUI>().fontSize = 20;
            obj.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Top;
        }
    }
    /// <summary>
    /// 生成消息块
    /// </summary>
    public void MessageBlock(string message, TextAlignmentOptions align)
    {
        TimeBlock();
        var obj = Instantiate(messageBlock, messageField);
        obj.GetComponent<TextMeshProUGUI>().text = message;
        obj.GetComponent<TextMeshProUGUI>().fontSize = 30;
        obj.GetComponent<TextMeshProUGUI>().alignment = align;
    }
    /// <summary>
    /// 生成在线列表
    /// </summary>
    public void ListMessageBlock(string message, string admin)
    {
        var obj = Instantiate(listMessageBlock, onlineClientField);
        obj.GetComponent<TextMeshProUGUI>().text = message;
        obj.name = admin;
        onlineClients.Add(obj);
    }
    /// <summary>
    /// 移除在线列表
    /// </summary>
    public void RemoveListMessageBlock(string admin)
    {
        ShowGlobalPrompt($"({admin})离开房间");
        for (int i = 0; i < onlineClients.Count; i++)
        {
            if (onlineClients[i].name == admin)
            {
                Destroy(onlineClients[i]);
                onlineClients.Remove(onlineClients[i]);
            }
        }
    }
}