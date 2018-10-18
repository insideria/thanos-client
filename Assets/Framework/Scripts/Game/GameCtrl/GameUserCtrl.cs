using Thanos.Model;
using System;

namespace Thanos.Ctrl
{
    public class GameUserCtrl : Singleton<GameUserCtrl>
    {
        public void Enter()
        {

        }

        public void Exit()
        {

        }

        /// <summary>
        /// 游戏帐号当前钻石
        /// </summary>
        /// <param name="num"></param>
        public void GameUserCurDiamond(UInt64 value)
        {
            GameUserModel.Instance.GameUserCurDiamond(value);
        }

        /// <summary>
        /// 游戏帐号当前金币
        /// </summary>
        /// <param name="num"></param>
        public void GameUserCurGold(UInt64 value)
        {
            GameUserModel.Instance.GameUserCurGold(value);
        }

        /// <summary>
        /// 帐号获得新的物品
        /// </summary>
        /// <param name="Commodityid"></param>
        public void GameUserGetNewCommodity(int Commodityid)
        {
            GameUserModel.Instance.GameUserGetNewCommodity(Commodityid);
        }

        /// <summary>
        /// 获得帐号所拥有的物品
        /// </summary>
        /// <param name="cfgInfo"></param>
        public void GameUserOwnedGoods(GSToGC.GoodsCfgInfo cfgInfo)
        {
            MarketGoodsType type;
            int gsId = cfgInfo.goodid;
            if (ConfigReader.HeroBuyXmlInfoDict.ContainsKey(gsId))
            {
                type = MarketGoodsType.GoodsTypeHero;
            }
            MarketHeroListModel.Instance.HeroListInfo(cfgInfo);
        }

        /// <summary>
        /// 设置帐号拥有的英雄信息
        /// </summary>
        /// <param name="heroid"></param>
        /// <param name="info"></param>
        public void DeltGetHeroInfoData(CommodityInfos info)
        {
            GameUserModel.Instance.DeltGetHeroInfoData(info);
        }

        public void DeleteHero(int goodId)
        {
            GameUserModel.Instance.DeleteHero(goodId);
        }

    }
}
