using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : SingletonMonoBehaviour<GameController>
{
    //left right up down
    public Transform[] pos;
    public GameObject player;
    public GameObject healthBar;
    public Transform canvas;


    void Start()
    {
        
        SpwanOwner();
    }

    public void SpwanOwner()
    {
        //生成人物
        float x = Random.Range(pos[0].position.x, pos[1].position.x);
        float y = Random.Range(pos[2].position.y, pos[3].position.y);
        var obj = Instantiate(player, new Vector3(x, y, 0), Quaternion.identity);
        obj.GetComponent<MeshRenderer>().material.color = Color.blue;
        obj.GetComponent<NetGameObject>().isMine = true;
        obj.GetComponent<Character>().admin = UserDataManager.Instance.ownerInfo.admin;
        obj.GetComponent<PlayerController>().Init();
        UserDataManager.Instance.localPlayer = obj;
        //生成血条
        SpwanHealthBar(obj.transform,UserDataManager.Instance.ownerInfo.admin,UserDataManager.Instance.ownerInfo.username);
    }
    /// <summary>
    /// 生成血条
    /// </summary>
    public void SpwanHealthBar(Transform target,string admin,string username)
    {
        var healthObj = Instantiate(healthBar, Vector3.zero, Quaternion.identity, canvas);
        //目标
        healthObj.GetComponent<HealthBar>().target = target;
        healthObj.GetComponent<Slider>().value = 1;
        //名字
        healthObj.transform.GetComponentInChildren<TextMeshProUGUI>().text = username;
        PlayerDataModel.Instance.healthBars.Add(admin,healthObj);
    }

    public GameObject SpwanOtherPlayer()
    {
        var obj = Instantiate(player);
        obj.GetComponent<NetGameObject>().isMine = false;
        return obj;
        
        
    }
}
