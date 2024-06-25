using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GameSever.Utility
{
    public class Message
    {
        private byte[] buffer = new byte[1024];
        private int startIndex;
        public byte[] Buffer
        {
            get { return buffer; }
        }
        public int StartIndex
        {
            get { return startIndex; }
        }
        public int Remsize
        {
            get { return buffer.Length - startIndex; }
        }
        public void ReadBuffer(int len)
        {
            //包头 四个字节
            startIndex += len;
           // if (startIndex <= 4) return;
            //从0位置开始解析四个字节
            int count = BitConverter.ToInt32(buffer, 0);
            while (true)
            {
                if (startIndex >= (count + 4))
                {
                    Array.Copy(buffer, count + 4, buffer, 0, startIndex - count - 4);
                    startIndex -= (count + 4);
                }
                else
                {
                    break;
                }
            }

        }
    }
}