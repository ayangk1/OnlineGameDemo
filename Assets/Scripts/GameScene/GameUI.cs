using System;
using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;


public class GameUI : SingletonMonoBehaviour<GameUI>
{
    public Button settingButton;
    
    
    public GameObject settingPanel;
    public Button backLobbyButton;
    
    public GameObject gameOverPanel;
    public Button gameoverBackLobbyButton;

    public Button xboxButton;
    public Button keyboardButton;

    public bool isSetting;
    
    private void Start()
    {
        settingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingButton.onClick.AddListener(SettingButton);
        
        EventSystem.current.sendNavigationEvents = false;
        
        xboxButton.GetComponent<Image>().color = Color.white;
        xboxButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        keyboardButton.GetComponent<Image>().color = Color.black;
        keyboardButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        
        backLobbyButton.onClick.AddListener(() =>
        {
            //告诉服务器退出
            Room_PlayerLeaveProto proto = new Room_PlayerLeaveProto();
            proto.admin = UserDataManager.Instance.ownerInfo.admin;
            //NetworkManager.Instance.SendTcpMsg(proto.ToArray());
            NetworkManager.Instance.SendUdpMsg(proto.ToArray());
        
            //加载场景
            SceneOpenManager.Instance.LoadScene(3,2);
        });
        gameoverBackLobbyButton.onClick.AddListener(() =>
        {
            //告诉服务器退出
            Room_PlayerLeaveProto proto = new Room_PlayerLeaveProto();
            proto.admin = UserDataManager.Instance.ownerInfo.admin;
            // NetworkManager.Instance.SendTcpMsg(proto.ToArray());
            NetworkManager.Instance.SendUdpMsg(proto.ToArray());
        
            //加载场景
            SceneOpenManager.Instance.LoadScene(3,2);
        });
    }

    public void XboxButtonOperation()
    {
        xboxButton.GetComponent<Image>().color = Color.black;
        xboxButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        keyboardButton.GetComponent<Image>().color = Color.white;
        keyboardButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
    
    public void KeyboardButtonOperation()
    {
        keyboardButton.GetComponent<Image>().color = Color.black;
        keyboardButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        xboxButton.GetComponent<Image>().color = Color.white;
        xboxButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    public void SettingButton()
    {
            if (settingPanel.activeSelf)
            {
                settingPanel.SetActive(false);
                isSetting = false;
            }
            else
            {
                settingPanel.SetActive(true);
                isSetting = true;
            }
    }

    private void Update()
    {
        
    }
}
