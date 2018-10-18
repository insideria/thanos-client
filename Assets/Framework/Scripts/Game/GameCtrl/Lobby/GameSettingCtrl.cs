using System;

namespace Thanos.Ctrl
{
    public class GameSettingCtrl : Singleton<GameSettingCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_GameSettingEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_GameSettingExit);
        }
    }
}