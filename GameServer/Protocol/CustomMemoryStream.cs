using System;
using System.IO;
using System.Text;
namespace GameSever.Utility
{
    public class CustomMemoryStream : MemoryStream
    {
        public CustomMemoryStream()
        {
            
        }
        public CustomMemoryStream(byte[] buffer): base(buffer)
        {

        }


        #region Short
        //从流中读取一个short数据
        public short ReadShort()
        {
            byte[] bytes = new byte[2];
            base.Read(bytes, 0, 2);
            return BitConverter.ToInt16(bytes,0);
        }
        public void WriteShort(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr,0,arr.Length);
        }
        #endregion

        #region UShort
        //从流中读取一个UShort数据
        public ushort ReadUShort()
        {
            byte[] bytes = new byte[2];
            base.Read(bytes, 0, 2);
            return BitConverter.ToUInt16(bytes,0);
        }
        public void WriteUShort(ushort value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr,0,arr.Length);
        }
        #endregion

        #region Int
        //从流中读取一个Int数据
        public int ReadInt()
        {
            byte[] bytes = new byte[4];
            base.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes,0);
        }
        public void WriteInt(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr,0,arr.Length);
        }
        #endregion

        #region Float
        //从流中读取一个Float数据
        public float ReadFloat()
        {
            byte[] bytes = new byte[4];
            base.Read(bytes, 0, 4);
            return BitConverter.ToSingle(bytes,0);
        }
        public void WriteFloat(float value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr,0,arr.Length);
        }
        #endregion

        #region Long
        //从流中读取一个Float数据
        public long ReadLong()
        {
            byte[] bytes = new byte[8];
            base.Read(bytes, 0, 8);
            return BitConverter.ToInt64(bytes,0);
        }
        public void WriteLong(long value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr,0,arr.Length);
        }
        #endregion

        #region Bool
        //从流中读取一个Bool数据
        public bool ReadBool()
        {
            return base.ReadByte() == 1;
        }
        public void WriteBool(bool value)
        {
            base.WriteByte((byte)(value?1:0));
        }
        #endregion

        #region UTF8String
        //从流中读取一个UTF8String数据
        public string ReadUTF8String()
        {
            ushort len = this.ReadUShort();
            byte[] arr = new byte[len];
            base.Read(arr, 0,len);
            return Encoding.UTF8.GetString(arr);
        }
        
        public void WriteUTF8String(string value)
        {
            byte[] arr = Encoding.UTF8.GetBytes(value);
            if(arr.Length > 65535)
            {
                throw new InvalidCastException("字符串超出范围！");
            }
            this.WriteUShort((ushort)arr.Length);
            base.Write(arr,0,arr.Length);
        }
        #endregion



    }
}