using System;

namespace Thanos.Ctrl
{
    public class AdvancedGuideCtrl : Singleton<AdvancedGuideCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AdvancedGuideEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AdvancedGuideExit);
        }
    }
}

