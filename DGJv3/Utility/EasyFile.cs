using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGJv3
{
    public class EasyFile
    {
        private FileStream fs = null;

        /// <summary>
        /// 写入一个字节。
        /// </summary>
        /// <param name="bytes"></param>
        public void WriteBytes(byte b)
        {
            fs.WriteByte(b);
        }
        /// <summary>
        /// 写入字节数组。
        /// </summary>
        public void WriteBytes(byte[] bytes)
        {
            fs.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// 写入一个EasyFile块。
        /// </summary>
        public void WriteBlock(byte[] bytes)
        {
            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            WriteBytes(lengthBytes);

            WriteBytes(bytes);
        }
        /// <summary>
        /// 写入一个字符串。
        /// </summary>
        /// <param name="str">所写入的数据。</param>
        public void WriteString(string str)
        {
            WriteBlock(Encoding.UTF8.GetBytes(str));
        }
        /// <summary>
        /// 写入一个32位整数。
        /// </summary>
        /// <param name="i">所写入的数据。</param>
        public void WriteInt32(int i)
        {
            WriteBlock(BitConverter.GetBytes(i));
        }



        /// <summary>
        /// 读取一个字节。
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return (byte)fs.ReadByte();
        }
        /// <summary>
        /// 读取指定数量字节。（不一定读了length字节。）
        /// </summary>
        /// <param name="length">预期读取字节。</param>
        /// <returns>读取结果。</returns>
        public byte[] ReadBytes(int length)
        {
            byte[] temp = new byte[length];
            int fact = fs.Read(temp, 0, length);
            return temp.Take(fact).ToArray();
        }
        /// <summary>
        /// 读取一个EasyFile块。
        /// </summary>
        /// <returns>所读到的数据。</returns>
        public byte[] ReadBlock()
        {
            byte[] lengthBytes = BitConverter.GetBytes((int)0);
            fs.Read(lengthBytes, 0, lengthBytes.Length);

            byte[] temp = new byte[BitConverter.ToInt32(lengthBytes, 0)];
            fs.Read(temp, 0, temp.Length);
            return temp;
        }
        /// <summary>
        /// 读取一个字符串。
        /// </summary>
        /// <returns>所读到的数据。</returns>
        public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadBlock());
        }
        /// <summary>
        /// 读取一个32位整数。
        /// </summary>
        /// <returns>所读到的数据。</returns>
        public int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBlock(), 0);
        }

        public EasyFile(ref FileStream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException("The file stream can not be null.");
            }
            this.fs = fileStream;
        }
    }
}
