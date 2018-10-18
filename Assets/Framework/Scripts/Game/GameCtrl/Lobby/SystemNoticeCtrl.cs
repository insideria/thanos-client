using System;

namespace Thanos.Ctrl
{
    public class SystemNoticeCtrl : Singleton<SystemNoticeCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SystemNoticeEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SystemNoticeExit);
        }
    }
}