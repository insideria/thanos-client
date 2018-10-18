using System;
using Thanos.Effect;

namespace Thanos.Ctrl
{
    public class BattleInfoCtrl : Singleton<BattleInfoCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleInfoEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleInfoExit);
        }

        //请求查看实时战斗信息
        public void AskBattleInfo()
        {
        }
        //请求查看自己当前个人战斗属性
        public void AskBattleMine()
        {
            HolyGameLogic.Instance.AskHeroAttributesInfo();
        }

        internal void SetEffect(bool saveState)
        {
            if (saveState)
            {
                EffectManager.Instance.SetEffectLodLevel(EffectLodLevel.High);
            }
            else {
                EffectManager.Instance.SetEffectLodLevel(EffectLodLevel.Low);
            }
        }
    }
}