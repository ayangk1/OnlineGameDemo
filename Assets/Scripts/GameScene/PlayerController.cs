using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameSever.Protocol;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private new Rigidbody rigidbody;

    private Character character;
    public GameObject bullet;

    public void Init()
    {
        character = GetComponent<Character>();
        playerInput = new PlayerInput();
        rigidbody = GetComponent<Rigidbody>();

        if (GlobalConfig.Instance.operation == Operation.keyboard)
        {
            GameUI.Instance.KeyboardButtonOperation();
            playerInput.Keyboard.Enable();
            playerInput.Xbox.Disable();
        }
        else if (GlobalConfig.Instance.operation == Operation.xbox)
        {
            playerInput.Xbox.Enable();
            playerInput.Keyboard.Disable();
            GameUI.Instance.XboxButtonOperation();
        }
            
        playerInput.Keyboard.Attack.started += Attack;
        playerInput.Keyboard.Setting.started += OnSetting;

        playerInput.Xbox.Attack.started += Attack;
        playerInput.Xbox.Setting.started += OnSetting;
        
        
        GameUI.Instance.xboxButton.onClick.AddListener(() =>
        {
            playerInput.Xbox.Enable();
            GlobalConfig.Instance.operation = Operation.xbox;
            playerInput.Keyboard.Disable();
            GameUI.Instance.XboxButtonOperation();
        });
        GameUI.Instance.keyboardButton.onClick.AddListener(() =>
        {
            playerInput.Keyboard.Enable();
            GlobalConfig.Instance.operation = Operation.keyboard;
            playerInput.Xbox.Disable();
            GameUI.Instance.KeyboardButtonOperation();
        });
    }

    private void OnSetting(InputAction.CallbackContext obj)
    {
        if (obj.started)
            GameUI.Instance.SettingButton();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.Keyboard.Attack.started -= Attack;
            playerInput.Keyboard.Setting.started -= OnSetting;
            
            playerInput.Xbox.Attack.started -= Attack;
            playerInput.Xbox.Setting.started -= OnSetting;
        }
        
    }

    /// <summary>
    /// 同步位置
    /// </summary>
    public void SendPosition()
    {
        if(!GetComponent<NetGameObject>().isMine) return;
        
        MovePacket movePacket;
        movePacket.x = transform.position.x;
        movePacket.y = transform.position.y;
        movePacket.z = transform.position.z;
        movePacket.r_y = transform.eulerAngles.y;
        Room_SyncPlayerDataProto proto = new Room_SyncPlayerDataProto(0,UserDataManager.Instance.ownerInfo.admin,UserDataManager.Instance.ownerInfo.username,movePacket);
        //NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        NetworkManager.Instance.SendUdpMsg(proto.ToArray());
    }
    
    private void  SendActionStatus(ActionStatus actionStatus)
    {
        if(!GetComponent<NetGameObject>().isMine) return;

        Room_SyncPlayerActionStatusProto proto = new Room_SyncPlayerActionStatusProto
        {
            admin = UserDataManager.Instance.ownerInfo.admin,
            status = actionStatus
        };

        //NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        NetworkManager.Instance.SendUdpMsg(proto.ToArray());
    }

    private void Move()
    {
        if(!GetComponent<NetGameObject>().isMine) return;
        
        if (playerInput.Xbox.enabled)
        {
            if(playerInput.Xbox.Move.ReadValue<Vector2>() == Vector2.zero) return;
            
            var lookInput = playerInput.Xbox.Move.ReadValue<Vector2>();
            var dir = new Vector3(lookInput.x, 0, lookInput.y);
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.deltaTime * character.rotaionSpeed);
            transform.position += transform.forward  * Time.deltaTime * character.moveSpeed;
        }
        else if (playerInput.Keyboard.enabled)
        {
            var dir = playerInput.Keyboard.Move.ReadValue<Vector2>();
            transform.Translate(new Vector3(dir.x, 0, dir.y) * Time.deltaTime * character.moveSpeed);
        }
        
         
    }
    
    /// <summary>
    /// 玩家朝向
    /// </summary>
    public void Look()
    {
        if(!GetComponent<NetGameObject>().isMine) return;
        if (playerInput.Xbox.enabled)
        {
            if(playerInput.Xbox.Look.ReadValue<Vector2>() == Vector2.zero || playerInput.Xbox.Move.ReadValue<Vector2>() != Vector2.zero) return;
            var lookInput = playerInput.Xbox.Look.ReadValue<Vector2>();
            var dir = new Vector3(lookInput.x, 0, lookInput.y);
            Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.deltaTime * character.rotaionSpeed);
        }
        else if (playerInput.Keyboard.enabled)
        {
            transform.LookAt(new Vector3(GetMouseWorld().x, transform.position.y, GetMouseWorld().z));
        }
    }

    /// <summary>
    /// 玩家攻击
    /// </summary>
    private void Attack(InputAction.CallbackContext obj)
    {
        if(!GetComponent<NetGameObject>().isMine || EventSystem.current.IsPointerOverGameObject()) return;
        if(GameUI.Instance.isSetting) return;
        
            var bulletObj = Instantiate(bullet, transform.position, transform.rotation);
            bulletObj.GetComponent<Bullet>().Init(true);
            SendActionStatus(ActionStatus.Attack);
    }

    private Vector3 GetMouseWorld()
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out RaycastHit raycastHit))
            {
                return raycastHit.point;
            }
        }
        return Vector3.zero;
    }
    
    private void FixedUpdate()
    {
        if (GetComponent<Character>().isDead) return;
        
        if (!NetworkManager.Instance.IsConnect()) return;
        SendPosition();
        
        if (GameUI.Instance.isSetting) return;
        Move();
        Look();
    }
}
