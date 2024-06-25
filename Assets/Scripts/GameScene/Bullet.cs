using System;
using System.Collections;
using System.Collections.Generic;
using GameSever.Protocol;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    public void Init(bool isMine)
    {
        if (isMine)
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
            GetComponent<NetGameObject>().isMine = true;
        }
        else
        {
            GetComponent<NetGameObject>().isMine = false;
        }
    }
    
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StaticObject"))
        {
            Destroy(gameObject);
        }
        if (other.GetComponent<Character>() == null) return;
        
        //如果是自己的子弹
        if (GetComponent<NetGameObject>().isMine)
        {
            //碰到其他玩家
            if (other.GetComponent<NetGameObject>()!=null && !other.GetComponent<NetGameObject>().isMine)
            {
                other.GetComponent<Character>().Health -= GetComponent<Attack>().attack;
                Destroy(gameObject);
            }
        }
        //如果是别人的子弹
        else
        {
            //碰到自己
            if (other.GetComponent<NetGameObject>()!=null && other.GetComponent<NetGameObject>().isMine)
            {
                other.GetComponent<Character>().Health -= GetComponent<Attack>().attack;
                Destroy(gameObject);
                
                
            }
        }
        

        
    }
}
