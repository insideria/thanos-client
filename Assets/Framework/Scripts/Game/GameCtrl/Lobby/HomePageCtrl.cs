using System;

namespace Thanos.Ctrl
{
    public class HomePageCtrl : Singleton<HomePageCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HomePageEnter);
        }
        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HomePageExit);
        }

        //请求快速战斗
        public void AskQuickPlay(int id, EBattleMatchType type)
        {
            //请求战斗匹配
            HolyGameLogic.Instance.AskMatchBattle(id, type);
            //申请匹配
            HolyGameLogic.Instance.AskStartTeamMatch();
        }
    }
}
