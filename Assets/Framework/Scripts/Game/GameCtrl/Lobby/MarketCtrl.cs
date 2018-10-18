using System;

namespace Thanos.Ctrl
{
    public class MarketCtrl : Singleton<MarketCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketExit);
        }

    }
}
