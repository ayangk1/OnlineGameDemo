using System.Net.Sockets;
using GameSever.Utility;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Controller;
using GameSever.PlayerSpace;
using GameSever.Room;


namespace GameSever.Server
{
    public class TcpClient
    {
        public Socket tcpSocket { get; set; }
        public ClientInfo clientInfo;
        public Player player;

        #region 接受数据参数
        //接受数据包的缓冲流
        private CustomMemoryStream customMemoryStream = new();
        //接受数据缓冲数组
        private byte[] memoryBuffer = new byte[1024];
        #endregion

        #region 发送数据参数
        private Queue<byte[]> m_SendQueue = new Queue<byte[]>();
        private Action? m_CheckSendQueue;
        #endregion

        #region 构造函数
        public TcpClient(Socket tcpSocket)
        {
            this.tcpSocket = tcpSocket;

            player = new Player();
            Thread thread = new Thread(StartReceive);
            thread.Start();

            //发送队列
            m_CheckSendQueue = OnCheckSendQueueCallBack;

            //发给客户端
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUTF8String(string.Format("欢迎来到服务器：" + DateTime.Now.ToString()));
                SendMsg(ms.ToArray());
            }
        }
        #endregion

        #region 发送
        /// <summary>
        /// 封装数据包
        /// </summary>
        private byte[] MakeDataPackage(byte[] data)
        {
            byte[] retbuffer;
            using (CustomMemoryStream ms = new CustomMemoryStream())
            {
                ms.WriteUShort((ushort)data.Length);
                ms.Write(data, 0, data.Length);
                retbuffer = ms.ToArray();
            }
            return retbuffer;
        }

        /// <summary>
        /// 把消息加入消息队列
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsg(byte[] data)
        {
            byte[] sendBuffer = MakeDataPackage(data);

            lock (m_SendQueue)
            {
                //加入队列
                m_SendQueue.Enqueue(sendBuffer);
                //启动委托
                m_CheckSendQueue?.Invoke();
            }
        }

        private void Send(byte[] buffer)
        {
            tcpSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, tcpSocket);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            tcpSocket.EndSend(ar);
            //继续检查队列
            OnCheckSendQueueCallBack();
        }

        private void OnCheckSendQueueCallBack()
        {
            lock (m_SendQueue)
            {
                if (m_SendQueue.Count > 0)
                {
                    //消息队列有数据就发送
                    Send(m_SendQueue.Dequeue());
                }
            }
        }
        #endregion

        #region 接受
        public void StartReceive()
        {
            tcpSocket.BeginReceive(memoryBuffer, 0, memoryBuffer.Length, SocketFlags.None, TcpReceiveCallback, tcpSocket);
        }
        //Tcp接受
        public void TcpReceiveCallback(IAsyncResult result)
        {
            int len = tcpSocket.EndReceive(result);
            try
            {
                if (len > 0)
                {
                    //把接受的数据写入缓冲流尾部
                    customMemoryStream.Position = customMemoryStream.Length;
                    //把指定长度的字节 写入数据流
                    customMemoryStream.Write(memoryBuffer, 0, len);
                    //协议代码ushort占两个字节
                    if (customMemoryStream.Length > 2)
                    {
                        while (true)
                        {
                            customMemoryStream.Position = 0;
                            //获取包体长度
                            int curLen = customMemoryStream.ReadUShort();
                            int curFullLen = 2 + curLen;

                            //如果数据流的长度>=整包的长度 说明至少收到了一个完整的包
                            if (customMemoryStream.Length >= curFullLen)
                            {
                                //开始获取
                                byte[] buffer = new byte[curLen];
                                //从2开始是因为前两个字节是协议
                                customMemoryStream.Position = 2;
                                customMemoryStream.Read(buffer, 0, curLen);

                                #region 解析消息
                                //协议代码
                                ushort protoCode = 0;
                                byte[] protoContent = new byte[buffer.Length - 2];

                                using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
                                {
                                    protoCode = ms.ReadUShort();
                                    ms.Read(protoContent, 0, protoContent.Length);
                                }
                                TcpRequestController.Instance.Dispatch(protoCode, this, protoContent);
                                #endregion

                                //处理剩余字节
                                int remainLen = (int)customMemoryStream.Length - curFullLen;
                                if (remainLen > 0)
                                {
                                    customMemoryStream.Position = curFullLen;

                                    byte[]? remainBuffer = new byte[remainLen];
                                    customMemoryStream.Read(remainBuffer, 0, remainLen);

                                    //清空数据流
                                    customMemoryStream.Position = 0;
                                    customMemoryStream.SetLength(0);

                                    //剩余字节重新写入数据流
                                    customMemoryStream.Write(remainBuffer, 0, remainBuffer.Length);

                                    remainBuffer = null;
                                }
                                else
                                {
                                    //没有剩余字节
                                    //清空数据流
                                    customMemoryStream.Position = 0;
                                    customMemoryStream.SetLength(0);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    StartReceive();
                }
                else
                {
                    SocketManager.Instance.ConnectedTcpClients.Remove(this);

                    TcpRoomManager.Instance.PlayerExit(this, player.roomId);
                    if (TcpRoomManager.Instance.roomMap.ContainsKey(player.roomId))
                        Console.WriteLine("客户端{0}离开房间{1},房间剩余人数:{2}", tcpSocket.RemoteEndPoint?.ToString()
                        , player.roomId, TcpRoomManager.Instance.roomMap[player.roomId].playerCount);

                    foreach (var me in SocketManager.Instance.ConnectedTcpClients)
                    {
                        if (me == this)
                        {
                            me.tcpSocket.Disconnect(true);
                            me.tcpSocket.Dispose();
                        }
                    }
                    SocketManager.Instance.LoginedTcpClients.Remove(this);
                    Console.WriteLine("客户端{0}断开连接,当前客户端数:{1}", tcpSocket.RemoteEndPoint?.ToString()
                             , SocketManager.Instance.ConnectedTcpClients.Count);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
    }
}