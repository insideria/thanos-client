using System;

namespace Thanos.Ctrl
{
    public class 
        MarketHeroListCtrl : Singleton<MarketHeroListCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroListEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MarketHeroListExit);
        }

        public void SetRootGoods(int goodId) 
        {
            mRootGood = goodId;
        }

        public void SetSelectGoods(int goodId) {
            mGoodsSelect = goodId;
        }

        public int GetRootGoods()
        {
            return mRootGood;
        }
        
        public int GetGoodsSelect() {
            return mGoodsSelect;
        }


        /// <summary>
        /// 请求购买英雄
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="tp"></param>
        public void MarketHeroAskBuyGoods(int goodsId, GameDefine.ConsumeType tp)
        {
            HolyGameLogic.Instance.EMsgToGSToCSFromGC_AskBuyGoods(goodsId, (int)tp);
        }

        private int mGoodsSelect;
        private int mRootGood;
    }
}
