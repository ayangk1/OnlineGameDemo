using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameSever.Protocol;
using GameSever.Utility;
using TMPro;
using UnityEngine;

public class NetworkManager : SingletonMonoBehaviour<NetworkManager>
{
    public string ip;
    public int port;
    
    private static Socket tcpSocket;
    private static Socket udpSocket;
    public EndPoint udpServerEndPoint;

    #region 发送数据参数

    private readonly Queue<byte[]> m_SendTcpQueue = new ();
    private readonly Queue<byte[]> m_SendUdpQueue = new ();
    private Action m_CheckSendQueue;

    #endregion

    #region 接受数据参数

    //接受数据包的缓冲流
    private readonly CustomMemoryStream customMemoryStream = new();

    //接受数据缓冲数组
    private readonly byte[] memoryBuffer = new byte[1024];

    //接受消息的队列
    private readonly Queue<byte[]> m_ReceiveQueue = new();
    private int m_ReceiveCount = 0;

    #endregion

    #region Start
    private void Start()
    {
        
    }
    #endregion

    #region Update
    private void Update()
    {
        while (true)
        {
            if (m_ReceiveCount <= 5)
            {
                m_ReceiveCount++;
                lock (m_ReceiveQueue)
                {
                    if (m_ReceiveQueue.Count > 0)
                    {
                        #region 解析消息
                        byte[] buffer = m_ReceiveQueue.Dequeue();
                        //协议代码
                        ushort protoCode;
                        byte[] protoContent = new byte[buffer.Length - 2];

                        using (CustomMemoryStream ms = new CustomMemoryStream(buffer))
                        {
                            protoCode = ms.ReadUShort();
                            ms.Read(protoContent, 0, protoContent.Length);
                        }
                        RequestController.Instance.Dispatch(protoCode, protoContent);
                        #endregion
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                m_ReceiveCount = 0;
                break;
            }
        }
    }
    #endregion

    #region 连接
    
    //循环连接
    public IEnumerator CheckConnect()
    {
        Connect_TCP();
        Connect_UDP();
        yield return new WaitForSeconds(5);
        StartCoroutine(CheckConnect());
    }

    public void Connect_TCP()
    {
        if (tcpSocket != null && tcpSocket.Connected) return;
        tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            //tcp连接
            tcpSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            StartTcpReceive();
            m_CheckSendQueue = OnCheckSendQueueCallBack;
            Debug.Log("Tcp连接成功");
        }
        catch (Exception e)
        {
            Debug.Log("连接失败" + e);
        }
    }
    
    public void Connect_UDP()
    {
        if (udpSocket != null && udpSocket.Connected) return;
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        try
        {
            udpServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            // udpSocket.SendTo(Encoding.UTF8.GetBytes("udp请求"+ip+"连接"), udpServerEndPoint);
            StartUdpReceive();
            m_CheckSendQueue = OnCheckSendQueueCallBack;
            Debug.Log("Udp连接"+ip+"成功");
        }
        catch (Exception e)
        {
            Debug.Log("连接失败" + e);
        }
    }
    
    public bool IsConnect()
    {
        if (tcpSocket != null && tcpSocket.Connected) return true;
        return false;
    }

    #endregion

    #region 发送

    private void OnCheckSendQueueCallBack()
    {
        lock (m_SendTcpQueue)
        {
            
            if (m_SendTcpQueue.Count > 0)
                //消息队列有数据就发送
                SendTcp(m_SendTcpQueue.Dequeue());
                
        }
        lock (m_SendUdpQueue)
        {
            if (m_SendUdpQueue.Count > 0)
                //消息队列有数据就发送
                SendUdp(m_SendUdpQueue.Dequeue());
        }
    }

    private void SendTcp(byte[] _buffer)
    {
        tcpSocket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, SendTcpCallBack, tcpSocket);
    }
    private void SendUdp(byte[] _buffer)
    {
        udpSocket.BeginSendTo(_buffer, 0, _buffer.Length, SocketFlags.None,udpServerEndPoint, SendUdpCallBack, udpSocket);
    }
    private void SendTcpCallBack(IAsyncResult ar)
    {
        tcpSocket.EndSend(ar);
        //继续检查队列
        OnCheckSendQueueCallBack();
    }
    private void SendUdpCallBack(IAsyncResult ar)
    {
        udpSocket.EndSendTo(ar);
        //继续检查队列
        OnCheckSendQueueCallBack();
    }

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
    /// 把消息加入tcp消息队列
    /// </summary>
    public void SendTcpMsg(byte[] data)
    {
        
        byte[] sendBuffer = MakeDataPackage(data);
        lock (m_SendTcpQueue)
        {
            //加入队列
            m_SendTcpQueue.Enqueue(sendBuffer);
            //启动委托
            m_CheckSendQueue?.Invoke();
        }
    }
    
    /// <summary>
    /// 把消息加入udp消息队列
    /// </summary>
    public void SendUdpMsg(byte[] data)
    {
        byte[] sendBuffer = MakeDataPackage(data);
        lock (m_SendUdpQueue)
        {
            //加入队列
            m_SendUdpQueue.Enqueue(sendBuffer);
            //启动委托
            m_CheckSendQueue?.Invoke();
        }
    }

    #endregion

    #region 接受

    public void StartTcpReceive()
    {
        tcpSocket.BeginReceive(memoryBuffer, 0, memoryBuffer.Length, SocketFlags.None, TcpReceiveCallback, tcpSocket);
    }
    public void StartUdpReceive()
    {
        udpSocket.BeginReceiveFrom(memoryBuffer, 0, memoryBuffer.Length, SocketFlags.None,ref udpServerEndPoint, UdpReceiveCallback, udpSocket);
    }

    private void UdpReceiveCallback(IAsyncResult result)
    {
        int len = udpSocket.EndReceiveFrom(result,ref udpServerEndPoint);

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
                            customMemoryStream.Position = 2;
                            customMemoryStream.Read(buffer, 0, curLen);
                            lock (m_ReceiveQueue)
                            {
                                m_ReceiveQueue.Enqueue(buffer);
                            }
                            
                            //处理剩余字节
                            int remainLen = (int)customMemoryStream.Length - curFullLen;
                            if (remainLen > 0)
                            {
                                customMemoryStream.Position = curFullLen;

                                byte[] remainBuffer = new byte[remainLen];
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

                StartUdpReceive();
            }
            else
            {
                Debug.Log($"服务器{tcpSocket.RemoteEndPoint}断开连接");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("服务器{0}断开连接"+ex, tcpSocket.RemoteEndPoint);
        }
    }
    /// <summary>
    /// Tcp接受
    /// </summary>
    private void TcpReceiveCallback(IAsyncResult result)
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
                            customMemoryStream.Position = 2;
                            customMemoryStream.Read(buffer, 0, curLen);
                            lock (m_ReceiveQueue)
                            {
                                m_ReceiveQueue.Enqueue(buffer);
                            }
                            
                            //处理剩余字节
                            int remainLen = (int)customMemoryStream.Length - curFullLen;
                            if (remainLen > 0)
                            {
                                customMemoryStream.Position = curFullLen;

                                byte[] remainBuffer = new byte[remainLen];
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

                StartTcpReceive();
            }
            else
            {
                Debug.Log($"服务器{tcpSocket.RemoteEndPoint}断开连接");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("服务器{0}断开连接"+ex, tcpSocket.RemoteEndPoint);
        }
    }

    #endregion

    private void OnDestroy()
    {
        if (tcpSocket != null && tcpSocket.Connected)
        {
            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }
    }
}