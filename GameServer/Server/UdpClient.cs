using System.Net.Sockets;
using GameSever.Utility;
using GameSever.Manager;
using GameSever.Protocol;
using GameSever.Controller;
using GameSever.PlayerSpace;
using GameSever.Room;
using System.Net;
using System.Text;
using System.Diagnostics;


namespace GameSever.Server
{
    public class UdpClient
    {
        public Socket udpSocket;
        //客户端endpoint
        public EndPoint endPoint;
        private EndPoint sendendPoint;
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
        public UdpClient(Socket udpSocket)
        {
            player = new Player();
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            sendendPoint = new IPEndPoint(IPAddress.Any, 0);

            this.udpSocket = udpSocket;

            //开始接受
            Thread thread = new Thread(StartReceive);
            thread.Start();

            //发送队列
            m_CheckSendQueue = OnCheckSendQueueCallBack;
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
        public void SendMsg(byte[] data,EndPoint endPoint)
        {
            sendendPoint = endPoint;
            byte[] sendBuffer = MakeDataPackage(data);

            lock (m_SendQueue)
            {
                //加入队列
                m_SendQueue.Enqueue(sendBuffer);
                //启动委托
                m_CheckSendQueue?.Invoke();
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            udpSocket.EndSend(ar);
            //继续检查队列
            OnCheckSendQueueCallBack();
        }

        //委托检查
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
        //开始发送
        private void Send(byte[] buffer)
        {
            udpSocket.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, sendendPoint, SendCallBack, udpSocket);
        }

        #endregion

        #region 接受
        public void StartReceive()
        {
            udpSocket.BeginReceiveFrom(memoryBuffer, 0, memoryBuffer.Length, SocketFlags.None, ref endPoint, UdpReceiveCallback, udpSocket);
        }
        //udp接受
        public void UdpReceiveCallback(IAsyncResult result)
        {
            int len = udpSocket.EndReceiveFrom(result, ref endPoint);
            
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
                                UdpRequestController.Instance.Dispatch(protoCode, this, protoContent);
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
                                break;
                        }
                    }
                    StartReceive();
                }
                else
                {

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