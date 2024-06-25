using System;
using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using UnityEngine;
using UnityEngine.UI;
using XLua;


public class Character : MonoBehaviour
{
    public const float maxHealth = 100;
    public string admin;
    public float moveSpeed;
    public float rotaionSpeed;
    public bool isDead;
    [SerializeField]
    private float health;
    public float Health
    {
        get => health;
        set
        {
            //如果生命值改变
            if (Math.Abs(health - value) > 0.1f)
            {
                if (PlayerDataModel.Instance != null && PlayerDataModel.Instance.healthBars.ContainsKey(admin))
                {
                    var obj = PlayerDataModel.Instance.healthBars[admin];
                    obj.GetComponent<Slider>().value = value / maxHealth;
                    
                    //告诉服务器自身当前血量
                    if (admin == UserDataManager.Instance.ownerInfo.admin)
                    {
                        Room_SyncPlayerActionStatusProto proto = new Room_SyncPlayerActionStatusProto
                        {
                            admin = UserDataManager.Instance.ownerInfo.admin,
                            status = ActionStatus.Hit,
                            health = value
                        };
                        NetworkManager.Instance.SendUdpMsg(proto.ToArray());
                        
                        //如果自身死亡
                        if (value <= 0)
                            Dead();
                    }
                    
                    //如果是其他人
                    if (admin != UserDataManager.Instance.ownerInfo.admin)
                    {
                        if (value <= 0)
                        {
                            if (!PlayerDataModel.Instance.deadPlayer.Contains(admin))
                                PlayerDataModel.Instance.deadPlayer.Add(admin);
                            Destroy(PlayerDataModel.Instance.healthBars[admin]);
                            Destroy(PlayerDataModel.Instance.currRoomOtherPlayer[admin]);
                        }
                    }
                }
            }
            health = value;
        }
    }

    public void SendServer()
    {
            Room_SyncPlayerActionStatusProto proto = new Room_SyncPlayerActionStatusProto
            {
                admin = UserDataManager.Instance.ownerInfo.admin,
                status = ActionStatus.Idel,
                health = Health
            };
            NetworkManager.Instance.SendUdpMsg(proto.ToArray());
    }

    private void Dead()
    {
            GameUI.Instance.gameOverPanel.SetActive(true);
            isDead = true;
            Destroy(PlayerDataModel.Instance.healthBars[UserDataManager.Instance.ownerInfo.admin]);
            Destroy(this.gameObject);
            
    }

    private void OnEnable()
    {
        isDead= false;
        Health = maxHealth;
    }
}
