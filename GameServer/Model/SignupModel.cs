using GameSever.Controller;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Server;
using MySql.Data.MySqlClient;

namespace GameSever.Model
{
    public class SignupModel : Singleton<SignupModel>
    {
        public void Init()
        {
            TcpRequestController.Instance.AddRequestListener(ProtoCodeConf.signupProto, SignupCallBack);
        }

        private void SignupCallBack(TcpClient client, byte[] buffer)
        {
            SignupProto proto = SignupProto.GetProto(buffer);

            Console.WriteLine("用户{0}通过协议{1}发送：{2}", client.tcpSocket.RemoteEndPoint
            , ProtoCodeConf.signupProto, proto.usrname + ":" + proto.admin + ":" + proto.password);

            #region 连接MySql数据库
            var mySqlConnection = MySqlManager.Instance.ConnectMysql(client);
            //注册 先看是否admin已被注册
            //string sql = "select * from user where admin='" + proto.admin + "'";
            string sql1 = "select admin from userdata";
            MySqlCommand command1 = new MySqlCommand(sql1, mySqlConnection);
            MySqlDataReader dataReader1 = command1.ExecuteReader();
            while (dataReader1.Read())
            {
                //如果已经存在
                if (dataReader1[0].ToString() == proto.admin)
                {
                    SignupCallBackProto callback1 = new SignupCallBackProto(false)
                    {
                        errCode = "账号已注册"
                    };
                    client.SendMsg(callback1.ToArray());
                    dataReader1.Close();
                    return;
                }
            }
            dataReader1.Close();
            //如果不存在
            string sql2 = "insert into userdata (username,admin,password) values (@username,@admin,@password)";
            MySqlCommand command2 = new MySqlCommand(sql2, mySqlConnection);
            command2.Parameters.AddWithValue("@username", proto.usrname);
            command2.Parameters.AddWithValue("@admin", proto.admin);
            command2.Parameters.AddWithValue("@password", proto.password);
            command2.ExecuteNonQuery();
            //注册成功
            SignupCallBackProto callback = new SignupCallBackProto(true);
            client.SendMsg(callback.ToArray());
            #endregion
        }
    }
}