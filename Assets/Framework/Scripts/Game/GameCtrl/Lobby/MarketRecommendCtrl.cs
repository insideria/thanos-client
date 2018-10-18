using System;
using System.Collections.Generic;

namespace Thanos.Ctrl
{
    public class MarketRecommendCtrl: Singleton<MarketRecommendCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroListEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroListExit);
        }

        public List<int> GetHeroList(RecommendType type)
        {  
            switch (type)
            { 
                case RecommendType.NewKinds:
                    return heroSortByNewKind;
                case RecommendType.HotBuy:
                    return heroSortByHotKind;
                case RecommendType.Discounts:
                    return heroSortByDiscounts;
            }
            return null;
        }

        public void SetSelectHero(int heroId) {
            mHeroSelect = heroId;
        }

        public int GetHeroSelect() {
            return mHeroSelect;
        }
          
        private List<int> heroSortByNewKind = new List<int>();
        private List<int> heroSortByHotKind = new List<int>();
        private List<int> heroSortByDiscounts = new List<int>();        
        private int mHeroSelect;

        public enum RecommendType
        { 
            NewKinds = 0,
            Discounts,
            HotBuy,
        }

    }
}
