using System;

namespace Thanos.Ctrl
{
    public class ChatTaskCtrl : Singleton<ChatTaskCtrl>
    {
        public bool isOpen = false;
        public void Enter(UInt64 sGUID)
        {
            if (!isOpen){
                isOpen = true;
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ChatTaskEnter, sGUID);
            }
            else
            {
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ReadyChatTaskEnter, sGUID);
            }

        }

        public void Exit()
        {
            isOpen = false;
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ChatTaskExit);
        }
        public void SendMsg(UInt64 sGUID, uint length, byte[] talkMsg)
        {
            HolyGameLogic.Instance.EmsgTocs_AskSendMsgToUser(sGUID, length, talkMsg);
        }
        public void ShowChatTask()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ShowChatTaskFriend);
        }
        public void SetNewChat()
        {
            EventCenter.Broadcast<bool>((Int32)GameEventEnum.GameEvent_ReceiveLobbyMsg,false);
        }

        internal void SetDestoy(UInt64 sGUID)
        {
            EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameEvent_RemoveChatTask, sGUID);
        }
    }
}
