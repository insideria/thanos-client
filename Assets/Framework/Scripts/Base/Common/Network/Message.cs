using System;
using System.Text;

namespace Thanos.Network
{
        public class Message
        {
            struct Header
            {
                public Int32 mSize;
                public Int32 mID;
            }

            public static Int32 mMsgHeaderSize = 8;

            private MemBuffer mBuffer = null;
            private Int32 mMsgSize = 0;

            public Message(Int32 n32Size)
            {
                this.ConstructMsg(n32Size);
            }

            public Int32 GetReadPos()
            {
                return this.mBuffer.ReadPos;
            }

            public void SetReadPos(Int32 n32Pos)
            {
                this.mBuffer.ReadPos = n32Pos;
            }

            public Int32 GetWritePos()
            {
                return this.mBuffer.WritePos;
            }

            public void SetWritePos(Int32 n32Pos)
            {
                this.mBuffer.WritePos = n32Pos;
            }

            public Boolean Reset()
            {
                this.mBuffer.SetReadPos(0);
                this.mBuffer.SetWritePos(0);
                Header myHeader;
                myHeader.mSize = 0;
                myHeader.mID = 0;
                this.Add(myHeader.mSize);
                this.Add(myHeader.mID);
                this.mBuffer.SetReadPos(mMsgHeaderSize);
                this.mBuffer.SetWritePos(mMsgHeaderSize);
                return true;
            }

            private bool ConstructMsg(Int32 n32Size)
            {
                this.mBuffer = new MemBuffer(n32Size);
                mMsgSize = n32Size;

                Header myHeader;
                myHeader.mSize = 0;
                myHeader.mID = 0;

                this.Add(myHeader.mSize);
                this.Add(myHeader.mID);
                this.mBuffer.SetReadPos(mMsgHeaderSize);
                this.mBuffer.SetWritePos(mMsgHeaderSize);
                return true;
            }

            private void UpdateMsgSize()
            {
                Int32 n32MsgSize = this.mBuffer.GetWritePos();
                Int32 n32ProtocalIDSize = sizeof(Int32);
                byte[] arrByteStream = BitConverter.GetBytes(n32MsgSize);
                this.mBuffer.ReWrite(0, arrByteStream, (Int32)n32ProtocalIDSize);
            }

            public byte[] GetMsgBuffer()
            {
                return this.mBuffer.GetMemBuffer();
            }

            public Int32 GetMsgSize()
            {
                return BitConverter.ToInt32(this.mBuffer.GetMemBuffer(), 0);
            }

            public Int32 GetProtocalID()
            {
                return BitConverter.ToInt32(this.mBuffer.GetMemBuffer(), sizeof(Int32));
            }

            public Int32 GetMsgTime()
            {
                return BitConverter.ToInt32(this.mBuffer.GetMemBuffer(), sizeof(Int32) * 2);
            }

            public Int32 GetMsgVerifyCode()
            {
                return BitConverter.ToInt32(this.mBuffer.GetMemBuffer(), sizeof(Int32) * 3);
            }

            public void SetProtocalID(Int32 n32ProtocalID)
            {
                Int32 n32ProtocalIDSize = sizeof(Int32);
                byte[] arrByteStream = BitConverter.GetBytes(n32ProtocalID);
                this.mBuffer.ReWrite((Int32)n32ProtocalIDSize, arrByteStream, (Int32)n32ProtocalIDSize);
            }

            public void Add(Boolean blValue)
            {
                this.mBuffer.Add(blValue);
                this.UpdateMsgSize();
            }

            public void Add(byte bValue)
            {
                this.mBuffer.Add(bValue);
                this.UpdateMsgSize();
            }

            public void Add(char nchValue)
            {
                this.mBuffer.Add(nchValue);
                this.UpdateMsgSize();
            }

            public void Add(UInt16 un16Value)
            {
                this.mBuffer.Add(un16Value);
                this.UpdateMsgSize();
            }

            public void Add(Int16 n16Value)
            {
                this.mBuffer.Add(n16Value);
                this.UpdateMsgSize();
            }

            public void Add(UInt32 un32Value)
            {
                this.mBuffer.Add(un32Value);
                this.UpdateMsgSize();
            }

            public void Add(Int32 n32Value)
            {
                this.mBuffer.Add(n32Value);
                this.UpdateMsgSize();
            }

            public void Add(UInt64 un64Value)
            {
                this.mBuffer.Add(un64Value);
                this.UpdateMsgSize();
            }

            public void Add(Int64 n64Value)
            {
                this.mBuffer.Add(n64Value);
                this.UpdateMsgSize();
            }

            public void Add(float fValue)
            {
                this.mBuffer.Add(fValue);
                this.UpdateMsgSize();
            }

            public void Add(double dValue)
            {
                this.mBuffer.Add(dValue);
                this.UpdateMsgSize();
            }

            public void Add(string strValue)
            {
                byte[] abFromByte = Encoding.Unicode.GetBytes(strValue);
                byte[] abToByte = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, abFromByte);
                Int32 n32UTF8BytesCount = Encoding.UTF8.GetCharCount(abToByte);
                this.mBuffer.Add(abToByte, 0, n32UTF8BytesCount);
                this.UpdateMsgSize();
            }

            public void Add(string strValue, UInt32 un32FixSize)
            {
                byte[] abFromByte = Encoding.Unicode.GetBytes(strValue);
                byte[] abToByte = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, abFromByte);
                Int32 n32UTF8BytesCount = Encoding.UTF8.GetCharCount(abToByte);
                if (n32UTF8BytesCount > (Int32)un32FixSize)
                {
                    n32UTF8BytesCount = (Int32)un32FixSize;
                }
                this.mBuffer.Add(abToByte, 0, n32UTF8BytesCount);
                Int32 n32AddByteNum = (Int32)un32FixSize - n32UTF8BytesCount;
                if (n32AddByteNum > 0)
                {
                    for (Int32 i = 0; i < n32AddByteNum; i++)
                    {
                        this.mBuffer.Add((byte)0);
                    }
                }
                this.UpdateMsgSize();
            }

            public void Add(char[] arrchValue, Int32 n32Size)
            {
                byte[] abFromByte = Encoding.Unicode.GetBytes(arrchValue);
                byte[] abToByte = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, abFromByte, 0, n32Size);
                Int32 n32UTF8BytesCount = Encoding.UTF8.GetCharCount(abToByte);
                if (n32UTF8BytesCount > n32Size)
                {
                    n32UTF8BytesCount = n32Size;
                }
                this.mBuffer.Add(abToByte, 0, n32UTF8BytesCount);
                Int32 n32AddByteNum = n32Size - n32UTF8BytesCount;
                if (n32AddByteNum > 0)
                {
                    for (Int32 i = 0; i < n32AddByteNum; i++)
                    {
                        this.mBuffer.Add((byte)0);
                    }
                }
                this.UpdateMsgSize();
            }

            public void Add(byte[] arrbData, Int32 n32Offset, Int32 n32Size)
            {
                this.mBuffer.Add(arrbData, n32Offset, n32Size);
                this.UpdateMsgSize();
            }

            public void Add(ref UInt64 rsGUID)
            {
                this.mBuffer.Add(rsGUID);
                this.UpdateMsgSize();
            }

            public Boolean GetBoolean()
            {
                return this.mBuffer.GetBoolean();
            }

            public byte GetByte()
            {
                return this.mBuffer.GetByte();
            }

            public char GetChar()
            {
                return this.mBuffer.GetChar();
            }

            public UInt16 GetUInt16()
            {
                return this.mBuffer.GetUInt16();
            }

            public Int16 GetInt16()
            {
                return this.mBuffer.GetInt16();
            }

            public UInt32 GetUInt32()
            {
                return this.mBuffer.GetUInt32();
            }

            public Int32 GetInt32()
            {
                return this.mBuffer.GetInt32();
            }

            public UInt64 GetUInt64()
            {
                return this.mBuffer.GetUInt64();
            }

            public Int64 GetInt64()
            {
                return this.mBuffer.GetInt64();
            }

            public float GetFloat()
            {
                return this.mBuffer.GetFloat();
            }

            public double GetDouble()
            {
                return this.mBuffer.GetDouble();
            }

            public bool GetByteArray(byte[] arrch, Int32 n32CharArrayLen)
            {
                if (!this.mBuffer.GetBytesArray(arrch, n32CharArrayLen))
                {
                    return false;
                }
                return true;
            }

            public bool GetCharArray(char[] arrch, Int32 n32CharArrayLen)
            {
                byte[] pTempUTF8ByteArray = new byte[n32CharArrayLen];
                if (!this.mBuffer.GetBytesArray(pTempUTF8ByteArray, n32CharArrayLen))
                {
                    return false;
                }

                byte[] pTempUnicodeByteArray = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, pTempUTF8ByteArray);

                Array.Copy(pTempUnicodeByteArray, arrch, n32CharArrayLen);

                return true;
            }

            public string GetString(UInt32 un32StrLen)
            {
                byte[] pTempUTF8ByteArray = new byte[(Int32)un32StrLen];
                if (!this.mBuffer.GetBytesArray(pTempUTF8ByteArray, (Int32)un32StrLen))
                {
                    return null;
                }

                byte[] pTempUnicodeByteArray = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, pTempUTF8ByteArray);
                return Encoding.Unicode.GetString(pTempUnicodeByteArray);
            }

            public string GetString()
            {
                return this.mBuffer.GetString();
            }

            public UInt64 GetGUID()
            {
                UInt64 sGUID;
                sGUID = this.mBuffer.GetUInt64();
                return sGUID;
            }

            public static Int32 PopMsgFromMemQueue(MemQueue cMsgQueue, Message cMsg)
            {
                Int32 n32Get = cMsgQueue.GetFront(cMsg.GetMsgBuffer(), (UInt32)mMsgHeaderSize);
                if (0 != n32Get)
                {
                    return n32Get;
                }

                Int32 n32MsgSize = cMsg.GetMsgSize();
                if (n32MsgSize >= cMsg.mMsgSize)
                {
                    return -2;
                }

                Int32 n32Pop = cMsgQueue.PopFront(cMsg.GetMsgBuffer(), (UInt32)n32MsgSize);
                if (0 != n32Pop)
                {
                    return -3;
                }

                cMsg.SetReadPos(mMsgHeaderSize);
                cMsg.SetWritePos(n32MsgSize);
                return 0;
            }
        }
}