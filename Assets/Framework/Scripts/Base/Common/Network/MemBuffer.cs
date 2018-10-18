using System;
using System.IO;
using System.Text;

namespace Thanos.Network
{
    public class MemBuffer
    {
        private Int32 mBufferSize = 0;
        private MemoryStream mStream = null;
        private Int32 mReadPos = 0;
        private Int32 mWritePos = 0;

        public MemBuffer(Int32 n32Size)
        {
            this.mStream = new MemoryStream((Int32)n32Size);
            this.mBufferSize = n32Size;
        }

        public Int32 ReadPos
        {
            get { return mReadPos; }
            set { mReadPos = value; }
        }

        public Int32 WritePos
        {
            get { return mWritePos; }
            set { mWritePos = value; }
        }

        public byte[] GetMemBuffer()
        {
            return this.mStream.GetBuffer();
        }

        public Int32 GetBufferSize()
        {
            return this.mBufferSize;
        }

        public Int32 GetWritePos()
        {
            return this.mWritePos;
        }

        public Int32 GetReadPos()
        {
            return this.mReadPos;
        }

        public Int32 AddWritePos(Int32 n32AddSize)
        {
            if (this.mBufferSize >= this.mWritePos + n32AddSize)
            {
                this.mWritePos += n32AddSize;
            }
            return this.mWritePos;
        }

        public Int32 AddReadPos(Int32 n32AddSize)
        {
            if (this.mBufferSize >= this.mReadPos + n32AddSize)
            {
                this.mReadPos += n32AddSize;
            }
            return this.mReadPos;
        }

        public void SetReadPos(Int32 n32NewPos)
        {
            this.mReadPos = n32NewPos;
        }

        public void SetWritePos(Int32 n32NewPos)
        {
            this.mWritePos = n32NewPos;
        }

        public Int32 Resize(Int32 n32NewSize)
        {
            //check parameter.
            if (n32NewSize <= 0)
            {
                return 0;
            }
            if (this.mWritePos >= n32NewSize)
            {
                return 0;
            }

            //really resize the buffer.
            //this.m_msStream.SetLength(n32NewSize);

            MemoryStream msOld = this.mStream;
            this.mStream = new MemoryStream((Int32)(n32NewSize));
            this.mBufferSize = n32NewSize;
            //msOld.Seek(0 , SeekOrigin.Begin);
            this.mStream.Seek(0, SeekOrigin.Begin);
            this.mStream.Write(msOld.GetBuffer(), 0, (Int32)this.mWritePos);

            return n32NewSize;
        }

        public Boolean ReWrite(Int32 n32Pos, byte[] arrbData, Int32 n32Size)
        {
            //check parameters.
            if (n32Size + n32Pos > this.mBufferSize)
            {
                return false;
            }

            //copy data.
            this.mStream.Seek(n32Pos, SeekOrigin.Begin);
            this.mStream.Write(arrbData, 0, (Int32)n32Size);
            return true;
        }

        private void AddData(byte[] arrByteStream, Int32 n32Size)
        {
            //check the stream size.
            if (this.mBufferSize <= this.mWritePos + n32Size)
            {
                Int32 n32IncrementSize = n32Size * 2;
                this.Resize(this.mBufferSize + n32IncrementSize);
            }

            //write the data.
            this.mStream.Seek(this.mWritePos, SeekOrigin.Begin);
            this.mStream.Write(arrByteStream, 0, (Int32)n32Size);
            this.mWritePos += n32Size;
        }

        public void Add(Boolean blValue)
        {
            Int32 n32TypeSize = sizeof(Boolean);
            byte[] arrByteStream = BitConverter.GetBytes(blValue);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(byte bValue)
        {
            Int32 n32TypeSize = sizeof(byte);
            byte[] arrByteStream = BitConverter.GetBytes(bValue);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(char nchValue)
        {
            Int32 n32TypeSize = sizeof(char);

            byte[] arrByteStream = BitConverter.GetBytes(nchValue);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(UInt16 n16Value)
        {
            Int32 n32TypeSize = sizeof(UInt16);
            byte[] arrByteStream = BitConverter.GetBytes(n16Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(Int16 n16Value)
        {
            Int32 n32TypeSize = sizeof(Int16);
            byte[] arrByteStream = BitConverter.GetBytes(n16Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(UInt32 un32Value)
        {
            Int32 n32TypeSize = sizeof(UInt32);
            byte[] arrByteStream = BitConverter.GetBytes(un32Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(Int32 n32Value)
        {
            Int32 n32TypeSize = sizeof(Int32);
            byte[] arrByteStream = BitConverter.GetBytes(n32Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(UInt64 n64Value)
        {
            Int32 n32TypeSize = sizeof(UInt64);
            byte[] arrByteStream = BitConverter.GetBytes(n64Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(Int64 n64Value)
        {
            Int32 n32TypeSize = sizeof(Int64);
            byte[] arrByteStream = BitConverter.GetBytes(n64Value);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(float fValue)
        {
            Int32 n32TypeSize = sizeof(float);
            byte[] arrByteStream = BitConverter.GetBytes(fValue);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(double dValue)
        {
            Int32 n32TypeSize = sizeof(double);
            byte[] arrByteStream = BitConverter.GetBytes(dValue);
            this.AddData(arrByteStream, (Int32)n32TypeSize);
        }

        public void Add(string strValue)
        {
            for (Int32 i = 0; i < strValue.Length; i++)
            {
                Add(strValue[i]);
            }
            Add('\0');
        }

        public void Add(string strValue, UInt32 un32FixSize)
        {
            for (UInt32 i = 0; i < un32FixSize; i++)
            {
                if (i < strValue.Length)
                {
                    Add(strValue[(Int32)i]);
                }
                else
                {
                    Add('\0');
                }
            }
        }

        public void Add(char[] arrchValue, Int32 n32Size)
        {
            //this.AddData((byte[])arrchValue , n32Size);
            for (Int32 i = 0; i < n32Size; i++)
            {
                this.Add(arrchValue[i]);
            }
            //this.Add('\0');
        }

        public void Add(byte[] arrbData, Int32 n32Offset, Int32 n32Size)
        {
            // this.AddData(arrbData[i + n32Offset] , n32Size);

            for (Int32 i = 0; i < n32Size; i++)
            {
                if (i >= arrbData.Length)
                {
                    this.Add((byte)0);
                }
                else
                {
                    this.Add(arrbData[i + n32Offset]);
                }

            }
        }

        public Boolean GetBoolean()
        {
            Boolean bValue = BitConverter.ToBoolean(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(Boolean);
            return bValue;
        }

        public byte GetByte()
        {
            this.mStream.Seek(this.mReadPos, SeekOrigin.Begin);
            byte bValue = Convert.ToByte(this.mStream.ReadByte());
            this.mReadPos++;
            return bValue;
        }

        public char GetChar()
        {
            char chValue = BitConverter.ToChar(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(char);
            return chValue;
        }

        public UInt16 GetUInt16()
        {
            UInt16 n16Value = BitConverter.ToUInt16(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(UInt16);
            return n16Value;
        }

        public Int16 GetInt16()
        {
            Int16 n16Value = BitConverter.ToInt16(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(Int16);
            return n16Value;
        }

        public UInt32 GetUInt32()
        {
            UInt32 un32Value = BitConverter.ToUInt32(this.mStream.GetBuffer(), this.mReadPos);
            this.mReadPos += sizeof(UInt32);
            return un32Value;
        }

        public Int32 GetInt32()
        {
            Int32 n32Value = BitConverter.ToInt32(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(Int32);
            return n32Value;
        }

        public UInt64 GetUInt64()
        {
            UInt64 n64Value = BitConverter.ToUInt64(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(UInt64);
            return n64Value;
        }

        public Int64 GetInt64()
        {
            Int64 n64Value = BitConverter.ToInt64(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(Int64);
            return n64Value;
        }

        public float GetFloat()
        {
            float nValue = BitConverter.ToSingle(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(float);
            return nValue;
        }

        public double GetDouble()
        {
            double dValue = BitConverter.ToDouble(this.mStream.GetBuffer(), (Int32)this.mReadPos);
            this.mReadPos += sizeof(double);
            return dValue;
        }

        public bool GetBytesArray(byte[] arrBData, Int32 n32GetSize)
        {
            if (this.mReadPos + n32GetSize > this.mWritePos)
            {
                return false;
            }
            this.mStream.Seek(this.mReadPos, SeekOrigin.Begin);
            this.mStream.Read(arrBData, 0, (Int32)n32GetSize);
            this.mReadPos += n32GetSize;
            return true;
        }

        public bool GetCharArray(char[] arrch, Int32 n32CharArrayLen)
        {
            //char[] arrch = new Char[n32CharArrayLen];
            if (this.mReadPos + n32CharArrayLen * sizeof(char) > this.mWritePos)
            {
                return false;
            }
            for (Int32 i = 0; i < n32CharArrayLen; i++)
            {
                arrch[i] = this.GetChar();
            }
            return true;
        }

        public string GetString(Int32 n32FixLen)
        {
            Int32 n32TempLen = n32FixLen;
            if (0 == n32TempLen)
            {
                n32TempLen = 100;
            }
            Int32 n32GetLen = 0;
            StringBuilder cSB = new StringBuilder(n32TempLen);
            char myChar = this.GetChar();
            n32GetLen++;
            while ('\0' != myChar)
            {
                cSB.Append(myChar);
                myChar = this.GetChar();
                n32GetLen++;
                if (n32FixLen > 0 && n32GetLen >= n32FixLen)
                {
                    break;
                }
            }

            if (n32FixLen > 0 && n32GetLen < n32FixLen)
            {
                Int32 n32AddWritePos = n32FixLen - n32GetLen;
                mWritePos += n32AddWritePos;
            }

            return cSB.ToString();
        }



        public string GetString()
        {
            StringBuilder cSB = new StringBuilder(10);
            char myChar = this.GetChar();

            while ('\0' != myChar)
            {
                cSB.Append(myChar);
                myChar = this.GetChar();
            }

            return cSB.ToString();
        }
    }
}