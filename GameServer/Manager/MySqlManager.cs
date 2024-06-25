using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GameSever.Protocol;
using GameSever.Server;
using MySql.Data.MySqlClient;

namespace GameSever.Manager
{
    public class MySqlManager : Singleton<MySqlManager>
    {
        private MySqlConnection? mySqlConnection;
        public MySqlConnection ConnectMysql(TcpClient client)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                UserID = "root",
                Password = "123456",
                Port = 3306,
                Server = "1.14.18.29",
                Database = "UserData"
            };
            try
            {
                mySqlConnection = new MySqlConnection(builder.ConnectionString);
                mySqlConnection.Open();

                return mySqlConnection;
            }
            catch (Exception e)
            {
                Console.WriteLine("数据库连接失败：" + e.Message);

                //返回登陆失败
                LoginCallBackProto loginProto = new LoginCallBackProto(false)
                {
                    errCode = "连接数据库失败"
                };
                client.SendMsg(loginProto.ToArray());
                //返回注册失败
                SignupCallBackProto signupProto = new SignupCallBackProto(false)
                {
                    errCode = "连接数据库失败"
                };
                client.SendMsg(signupProto.ToArray());

                return new MySqlConnection();
            }
            finally{
               mySqlConnection = new MySqlConnection(builder.ConnectionString);
                mySqlConnection.Open();
            }
        }
        public string GetUserName(string admin,TcpClient client)
        {
            var mySqlConnection = ConnectMysql(client);
            //登陆 先看账号是否存在
            string sql = "select * from userdata where admin = @admin";
            MySqlCommand command = new MySqlCommand(sql, mySqlConnection);
            command.Parameters.AddWithValue("admin", admin);
            using (MySqlDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var username = dataReader.GetString("username");
                    mySqlConnection.Close();
                    return username;
                }
            }
            return "";
        }
    }
}