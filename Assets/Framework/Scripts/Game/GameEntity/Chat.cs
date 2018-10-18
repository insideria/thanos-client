using GameDefine;
using System.Collections.Generic;
using System;

namespace Thanos.GameEntity
{
    public class IChatInfo
    {
        public string mMsg;
        public bool mIsLocalPlayer;
        public string NickName
        {
            private set;
            get;
        }
        public int mHead;
        public MsgTalkEnum mTalkState;
        public void SetNickName(string nick)
        {
            NickName = nick;
        }
    }

    public class Chat
    {
        public Chat() {
            ChatInfoList = new List<IChatInfo>();
        }

        public List<IChatInfo> ChatInfoList {
            private set;
            get;
        }

        public UInt64 GUID
        {
            private set;
            get;
        }

        public string NickName
        {
            private set;
            get;
        }

        public int HeadID
        {
            private set;
            get;
        }

        public MsgTalkEnum TalkState
        {
            private set;
            get;
        }

        public void SetTalkState(MsgTalkEnum talkState)
        {
            TalkState = talkState;
        }

        public void SetMsgInfo(UInt64 sGUID ,string nickName, string msgInfo, MsgTalkEnum talkState,int headID,bool islocal)
        {
            GUID = sGUID;
            NickName = nickName;
            TalkState = talkState;
            HeadID = headID;
            IChatInfo info = new IChatInfo();
            info.mIsLocalPlayer = islocal;
            info.mMsg = msgInfo;
            info.mHead = headID;
            info.SetNickName(nickName);
            info.mTalkState = talkState;
            ChatInfoList.Add(info);
        }
    }
}
