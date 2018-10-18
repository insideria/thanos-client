using System;

namespace Thanos.Ctrl
{
    public class RuneCombineCtrl : Singleton<RuneCombineCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RuneCombineWindowEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RuneCombineWindowExit);
        }

        public RuneCombineCtrl()
        {
        }
    }
}
