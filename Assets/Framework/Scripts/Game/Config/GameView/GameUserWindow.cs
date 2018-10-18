using System;
using UnityEngine;

namespace Thanos.View
{
    public class GameUserWindow : BaseWindow
    {
        public GameUserWindow()
        {
            mScenesType = EScenesType.EST_Login;
            //mResName = GameConstDefine.LoadGamePlayUI;
            mResident = true;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化

        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_UserGetGoodsHero, ShowGetGoodsHero);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_UserGetGoodsNml, ShowGetGoodsNml);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_UserGetMoney, ShowGetMoney);
        }

        //类对象释放
        public override void Realse()
        {
            //EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            //EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

         //窗口控件初始化
        protected override void InitWidget()
        {
            //BattleInfo = mRoot.FindChild("ExtraButton/InfoBtn").gameObject;
            //UIEventListener.Get(BattleInfo).onClick += BattlePlay;
        }

        //窗口控件释放
        protected override void RealseWidget()
        {

        }

        private void BattlePlay(GameObject go)
        {
            //GamePlayCtrl.Instance.AskBattleInfo();
        }


        //游戏事件注册
        protected override void OnAddListener()
        {

        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {

        }

        //显示
        public override void OnEnable()
        {

        }

        //隐藏
        public override void OnDisable()
        {

        }

        /// <summary>
        /// 获得英雄
        /// </summary>
        public void ShowGetGoodsHero()
        {
            Debug.Log("ShowGetGoodsHero");
        }

        /// <summary>
        /// 获得普通物品
        /// </summary>
        public void ShowGetGoodsNml()
        { 

        }

        /// <summary>
        /// 获得货币
        /// </summary>
        public void ShowGetMoney()
        {

        }

    }
}
