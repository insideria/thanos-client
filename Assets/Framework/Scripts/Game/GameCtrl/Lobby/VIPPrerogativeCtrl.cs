using System;

namespace Thanos.Ctrl
{
    public class VIPPrerogativeCtrl : Singleton<VIPPrerogativeCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_VIPPrerogativeEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_VIPPrerogativeExit);
        }
    }
}
