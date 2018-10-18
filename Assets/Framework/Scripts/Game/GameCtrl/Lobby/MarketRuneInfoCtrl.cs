using System;
using UnityEngine;

namespace Thanos.Ctrl
{
    public class MarketRuneInfoCtrl : Singleton<MarketRuneInfoCtrl>
    {
        public void Enter(GameObject go)
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RuneBuyWindowEnter, go);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RuneBuyWindowExit);
        }

        public void BuyRune(int runeid, GameDefine.ConsumeType type, int num)
        {
            HolyGameLogic.Instance.EMsgToGSToCSFromGC_AskBuyGoods(runeid, (int)type, num);
        }
    }
}
