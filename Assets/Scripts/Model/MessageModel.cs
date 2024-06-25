using GameSever.Protocol;
using TMPro;
using UnityEngine;

public class MessageModel : SingletonMonoBehaviour<MessageModel>
{
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        RequestController.Instance.AddRequestListener(ProtoCodeConf.Lobby_MessageProto, MessageCallBack);
        
    }
    private void MessageCallBack(byte[] buffer)
    {
        Lobby_MessageProto proto = Lobby_MessageProto.GetProto(buffer);
        Debug.Log($"服务器通过协议{ProtoCodeConf.Lobby_MessageProto}发送：{proto.message + ":" + proto.admin}");
        
        GameManager.Instance.MessageBlock($"({proto.admin}):" +"\n"+proto.message,TextAlignmentOptions.TopLeft);
        GameManager.Instance.inputField.text = "";
    }
    
    private void OnDestroy()
    {
        RequestController.Instance.RemoveRequestListener(ProtoCodeConf.Lobby_MessageProto, MessageCallBack);
    }
}
