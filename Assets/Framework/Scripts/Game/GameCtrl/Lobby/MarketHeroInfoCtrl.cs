using UnityEngine;
using System.Collections;
using Thanos;
using Thanos.GameData;
using Thanos.GameData;
using Thanos.Network;
using LSToGC;
using System.IO;
using System.Linq;
using System;
using Thanos.Model;

namespace Thanos.Ctrl
{
    public class MarketHeroInfoCtrl : Singleton<MarketHeroInfoCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroInfoEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroInfoExit);
        }

        public class HeroGoodsInfo
        {
            public enum CostType
            {
                GoldType = 0,
                DiamondType,
            }
           public int mGoodType;
           public int mGoodId;
           public CostType mCostType;
           public int mCost;
           public bool mIsDiscount;
        }
    }
}
