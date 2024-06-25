using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Model;
using GameSever.Utility;

namespace GameSever.Manager
{
    public class ModelManager : Singleton<ModelManager>
    {
        public void Init()
        {
            LoginModel.Instance.Init();
            SignupModel.Instance.Init();
            MessageModel.Instance.Init();
            LobbyModel.Instance.Init();
            LogoutModel.Instance.Init();
            PlayerDataSyncModel.Instance.Init();
            RoomModel.Instance.Init();
            Console.WriteLine("Model初始化!");
        }
    }
}