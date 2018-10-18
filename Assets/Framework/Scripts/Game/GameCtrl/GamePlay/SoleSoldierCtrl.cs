using System;

namespace Thanos.Ctrl
{
    public class SoleSoldierCtrl : Singleton<SoleSoldierCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SoleSoldierEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SoleSoldierExit);
        }
    }
}
