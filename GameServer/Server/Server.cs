using System.Net.Sockets;
using System.Net;
using GameSever.Manager;
using System.Text;

namespace GameSever.Server
{
    public class Server
    {
        private static Socket? tcpSocket;
        private static Socket? udpSocket;

        //构造函数
        public Server(string ip,int port)
        {
            #region TCP
            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            #endregion

            #region UDP
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            #endregion
       
            tcpSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            tcpSocket.Listen(3000);
            udpSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            //udpSocket.Listen(3000);
            
            //tcp连接
            if (!tcpSocket.Connected)
            {
                Thread thread = new Thread(TcpStartAccept);
                thread.Start();
            }
            Console.WriteLine("tcp服务器" + ip + "已开启，开始监听端口：" + port);
            //udp连接
            if (!udpSocket.Connected)
            {
                Thread thread = new Thread(UdpStartServer);
                thread.Start();
            }
            Console.WriteLine("udp服务器" + ip + "已开启，开始监听端口：" + port);
            //Init
            ModelManager.Instance.Init();
        }
        //tcp开始接受
        private static void TcpStartAccept()
        {
            while (true)
            {
                if (tcpSocket != null)
                {
                    Socket socket = tcpSocket.Accept();
                    //玩家
                    TcpClient client = new TcpClient(socket);

                    SocketManager.Instance.ConnectedTcpClients.Add(client);
                    
                    Console.WriteLine("客户端{0}连接", socket.RemoteEndPoint?.ToString());
                    Console.WriteLine("当前客户端数量:" + SocketManager.Instance.ConnectedTcpClients.Count);
                }
            }
        }
        //udp开始接受
        private static void UdpStartServer()
        {
            if(udpSocket != null)
            {
                // EndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 6677);
                // byte[] buffer = new byte[1024];
                // int len = udpSocket.ReceiveFrom(buffer,ref endPoint);
                // Console.WriteLine(Encoding.UTF8.GetString(buffer,0,len));

                UdpClient client = new UdpClient(udpSocket);
            }
                
        }

    }
}