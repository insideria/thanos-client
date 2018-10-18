using System;
using GameDefine;

namespace Thanos.Ctrl
{
    public class SocialCtrl : Singleton<SocialCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SocialEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SocialExit);
        }
        
        internal void RemoveFriend(UInt64 sGUID, FRIENDTYPE fRIENDTYPE)
        {
            HolyGameLogic.Instance.EmsgTocs_AskRemoveFromFriendList(sGUID, (int)fRIENDTYPE);
        }
        internal void AddFriendBlackList(string CurrName, FRIENDTYPE fRIENDTYPE)
        {
            HolyGameLogic.Instance.EmsgTocs_AskAddToFriendList(CurrName, (int)fRIENDTYPE);
        }
        internal void AddFriendBlackID(ulong id, FRIENDTYPE fRIENDTYPE)
        {
            HolyGameLogic.Instance.EmsgTocs_AskAddToSNSListByID(id, (int)fRIENDTYPE);
        }

        internal void AskBlackList()
        {
            HolyGameLogic.Instance.EmsgTocs__AskBlackListOnlineInfo();
        }
    }

}