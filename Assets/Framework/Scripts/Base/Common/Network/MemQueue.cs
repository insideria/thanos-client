using System;
using System.IO;

namespace Thanos.Network
{
    public class MemQueue
    {
        private MemoryStream mStream = null;
        private UInt32 mSize;
        private UInt32 mTailPos;
        private UInt32 mHeadPos;

        private UInt64 mDataPushed;
        private UInt64 mDataPoped;

        public MemQueue(UInt32 un32Size)
        {
            mStream = new MemoryStream((Int32)un32Size);
            mSize = un32Size;
            mTailPos = 0;
            mHeadPos = 0;
            mDataPushed = 0;
            mDataPoped = 0;
        }

        public UInt32 GetSize()
        {
            return mSize;
        }

        public UInt64 GetCachedDataSize()
        {
            return mDataPushed - mDataPoped;
        }

        public UInt64 GetPushedDataSize()
        {
            return mDataPushed;
        }

        public UInt64 GetPopedDataSize()
        {
            return mDataPoped;
        }

        public void Reset()
        {
            mTailPos = 0;
            mHeadPos = 0;
            mDataPushed = 0;
            mDataPoped = 0;
        }

        public Int32 PushBack(byte[] abByteStream, UInt32 un32Len)
        {
            UInt32 un32Dist = mTailPos + mSize - mHeadPos;
            UInt32 un32Used = (un32Dist >= mSize ? (un32Dist - mSize) : un32Dist);
            if (un32Len + un32Used + 1 > (UInt32)mSize)
            {
                return -1;
            }

            if (mTailPos + un32Len >= mSize)
            {
                UInt32 un32Seg1 = (UInt32)(mSize - mTailPos);
                UInt32 un32Seg2 = un32Len - un32Seg1;
                mStream.Seek(mTailPos, SeekOrigin.Begin);
                mStream.Write(abByteStream, 0, (Int32)un32Seg1);

                mStream.Seek(0, SeekOrigin.Begin);
                mStream.Write(abByteStream, (Int32)un32Seg1, (Int32)un32Seg2);
                mTailPos = un32Seg2;
            }
            else
            {
                mStream.Seek(mTailPos, SeekOrigin.Begin);
                mStream.Write(abByteStream, 0, (Int32)un32Len);
                mTailPos += un32Len;
            }
            mDataPushed += un32Len;
            return 0;
        }

        public Int32 PopFront(byte[] abByteStream, UInt32 un32Len)
        {
            UInt32 un32Dist = mTailPos + mSize - mHeadPos;
            UInt32 un32Used = (un32Dist >= mSize ? (un32Dist - mSize) : un32Dist);
            if (un32Len > un32Used)
            {
                return -1;
            }

            if (mHeadPos + un32Len >= mSize)
            {
                UInt32 un32Seg1 = (UInt32)(mSize - mHeadPos);
                UInt32 un32Seg2 = un32Len - un32Seg1;


                mStream.Seek(mHeadPos, SeekOrigin.Begin);
                mStream.Read(abByteStream, 0, (Int32)un32Seg1);

                mStream.Seek(0, SeekOrigin.Begin);
                mStream.Read(abByteStream, (Int32)un32Seg1, (Int32)un32Seg2);
                mHeadPos = un32Seg2;
            }
            else
            {
                mStream.Seek(mHeadPos, SeekOrigin.Begin);
                mStream.Read(abByteStream, 0, (Int32)un32Len);
                mHeadPos += un32Len;
            }

            mDataPoped += un32Len;
            return 0;
        }

        //get un32Len byte data from header to pBuf which do not move read pointer.
        public Int32 GetFront(byte[] abByteStream, UInt32 un32Len)
        {
            UInt32 un32Dist = mTailPos + mSize - mHeadPos;
            UInt32 un32Used = (un32Dist >= mSize ? (un32Dist - mSize) : un32Dist);
            if (un32Len > un32Used)
            {
                return -1;
            }

            if (mHeadPos + un32Len >= mSize)
            {
                UInt32 un32Seg1 = (UInt32)(mSize - mHeadPos);
                UInt32 un32Seg2 = un32Len - un32Seg1;


                mStream.Seek(mHeadPos, SeekOrigin.Begin);
                mStream.Read(abByteStream, 0, (Int32)un32Seg1);

                mStream.Seek(0, SeekOrigin.Begin);
                mStream.Read(abByteStream, (Int32)un32Seg1, (Int32)un32Seg2);
            }
            else
            {
                mStream.Seek(mHeadPos, SeekOrigin.Begin);
                mStream.Read(abByteStream, 0, (Int32)un32Len);
            }

            return 0;
        }
    }
}