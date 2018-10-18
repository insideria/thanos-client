using GameDefine;
using System;
using System.Collections.Generic;

namespace Thanos.GameEntity
{
   public class Friend
    {
       public UInt64 GUID
       {
           private set;
           get;
       }
        public bool IsOnline
        {
            private set;
            get;
        }
        public string NiceName
        {
            private set;
            get;
        }

        public int HeadId
        {
            private set;
            get;
        }

        public uint isVip
        {
            private set;
            get;
        }

        public FRIENDTYPE FriendType
        {
            private set;
            get;
        }

        public List<Chat> Chat = new List<Chat>();
        public void SetFriendInfo(UInt64 sGUID,string niceName, int headId, bool isonline = false,uint vipLevel = 0)
        {
            this.GUID = sGUID;
            this.NiceName = niceName;
            this.HeadId = headId;
            this.IsOnline = isonline;
            this.isVip = vipLevel;
        }
    }
}
