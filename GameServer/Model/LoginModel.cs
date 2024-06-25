using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameSever.Controller;
using GameSever.Manager;
using GameSever.PlayerSpace;
using GameSever.Protocol;
using GameSever.Server;
using MySql.Data.MySqlClient;

namespace GameSever.Model
{
    public class LoginModel : Singleton<LoginModel>
    {
        public void Init()
        {
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.loginProto, LoginCallBack);
        }
        //登陆CallBack
        private void LoginCallBack(TcpClient client, byte[] buffer)
        {
            LoginProto proto = LoginProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.loginProto, proto.admin + ":" + proto.password);

            #region 连接MySql数据库
            var mySqlConnection = MySqlManager.Instance.ConnectMysql(client);
            //登陆 先看账号是否存在
            string sql1 = "select * from userdata where admin = @admin";
            MySqlCommand command1 = new MySqlCommand(sql1, mySqlConnection);
            command1.Parameters.AddWithValue("admin", proto.admin);
            using (MySqlDataReader dataReader = command1.ExecuteReader())
            {
                if (!dataReader.HasRows)
                {
                    LoginCallBackProto callback1 = new LoginCallBackProto(false)
                    {
                        errCode = "账号不存在"
                    };
                    client.SendMsg(callback1.ToArray());
                    mySqlConnection.Close();
                    return;
                }
            }
            //登陆 检查密码是否正确
            string sql2 = "select * from userdata where admin = @admin and password = @password";
            MySqlCommand command2 = new MySqlCommand(sql2, mySqlConnection);
            command2.Parameters.AddWithValue("admin", proto.admin);
            command2.Parameters.AddWithValue("password", proto.password);
            using (MySqlDataReader dataReader = command2.ExecuteReader())
            {
                if (dataReader.HasRows)
                {
                    //登陆 看是否admin已登陆
                    for (int i = 0; i < SocketManager.Instance.LoginedTcpClients.Count; i++)
                    {
                        if (SocketManager.Instance.LoginedTcpClients[i].clientInfo.admin == proto.admin)
                        {
                            LoginCallBackProto callback2 = new LoginCallBackProto(false)
                            {
                                errCode = "账号已登陆"
                            };
                            client.SendMsg(callback2.ToArray());
                            mySqlConnection.Close();
                            return;
                        }
                    }
                    //登陆成功
                    LoginCallBackProto callback1 = new LoginCallBackProto(true);
                    client.SendMsg(callback1.ToArray());
                    //添加到登陆列表
                    client.clientInfo.username = MySqlManager.Instance.GetUserName(proto.admin,client);
                    client.clientInfo.admin = proto.admin;
                    SocketManager.Instance.LoginedTcpClients.Add(client);
                    mySqlConnection.Close();
                    return;
                }
                else
                {
                    LoginCallBackProto callback1 = new LoginCallBackProto(false)
                    {
                        errCode = "密码错误"
                    };
                    client.SendMsg(callback1.ToArray());
                    mySqlConnection.Close();
                    return;
                }
            }


            #endregion
        }
    }
}