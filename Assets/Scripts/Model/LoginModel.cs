using System;
using UnityEngine;

namespace GameSever.Protocol
{
    public class LoginModel : SingletonMonoBehaviour<LoginModel>
    {
        private void Start()
        {
            Init();
        }

        

        public void Init()
        {
            RequestController.Instance.AddRequestListener(ProtoCodeConf.loginCallBackProto, LoginCallBack);
            RequestController.Instance.AddRequestListener(ProtoCodeConf.signupCallBackProto, SignupCallBack);
        }

        private void LoginCallBack(byte[] buffer)
        {
            LoginCallBackProto proto = LoginCallBackProto.GetProto(buffer);

            Debug.Log($"服务器通过协议{ProtoCodeConf.loginCallBackProto}发送");

            if (proto.success)
            {
                //登陆成功
                Debug.Log("登陆成功");
                Main.Instance.loginPromptText.text = "登陆成功";
                Main.Instance.loadingPanel.SetActive(false);
                SceneOpenManager.Instance.LoadScene(0,2);
            }
            else
            {
                //登陆失败
                Debug.Log(proto.errCode);
                Main.Instance.loadingPanel.SetActive(false);
                Main.Instance.loginPromptText.text = proto.errCode;
                //重新添加监听
                Main.Instance.loginButton.onClick.RemoveAllListeners();
                Main.Instance.loginButton.onClick.AddListener(Main.Instance.Login);
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        private void SignupCallBack(byte[] buffer)
        {
            SignupCallBackProto proto = SignupCallBackProto.GetProto(buffer);
            Debug.Log($"服务器通过协议{ProtoCodeConf.signupCallBackProto}发送");
            if (proto.success)
            {
                //注册成功
                Debug.Log("注册成功");
                Main.Instance.signupPromptText.text = "注册成功";
            }
            else
            {
                //注册失败
                Debug.Log(proto.errCode);
                Main.Instance.signupPromptText.text = proto.errCode;
                //重新添加监听
                Main.Instance.signupButton.onClick.RemoveAllListeners();
                Main.Instance.signupButton.onClick.AddListener(Main.Instance.Register);
            }
            
        }
        
        
        
        private void OnDestroy()
        {
            RequestController.Instance.RemoveRequestListener(ProtoCodeConf.loginCallBackProto, LoginCallBack);
            RequestController.Instance.RemoveRequestListener(ProtoCodeConf.signupCallBackProto, SignupCallBack);
        }
    }
}