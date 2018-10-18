using System;

namespace Thanos.Ctrl
{
    public class ExtraBonusCtrl : Singleton<ExtraBonusCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ExtraBonusEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ExtraBonusExit);
        }

        internal void SendMsg(string mTemp)
        {
            HolyGameLogic.Instance.EmsgTocs_CDKReq(mTemp);
        }
    }
}