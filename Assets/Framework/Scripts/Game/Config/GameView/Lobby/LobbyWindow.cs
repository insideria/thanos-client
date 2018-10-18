using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using GameDefine;
using Thanos.Tools;
using Thanos.GameData;
using GameDefine;
using UICommon;
using Thanos.GameData;
using Thanos;
using Thanos.GameEntity;
using Thanos.GuideDate;
using Thanos.Resource;

using Thanos.Ctrl;
using Thanos.Model;

namespace Thanos.View
{
    public class LobbyWindow : BaseWindow
    {
        public LobbyWindow()
        {
            mScenesType = EScenesType.EST_Login;
            mResName = GameConstDefine.LoadGameLobbyUI;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LobbyEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LobbyExit, Hide); 
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LobbyEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LobbyExit, Hide); 

//#if UNITY_STANDALONE_WIN || UNITY_EDITOR|| SKIP_SDK
//#else
//             SdkConector.HideToolBar();
//#endif
        }

        //窗口控件初始化   获取组件或对象，此时已经生成了UI控件  此方法在广播时，加载的资源
        protected override void InitWidget()
        {           
            mHomepage = mRoot.Find("StartMenuManager/StartMenuBtn/Homepage").GetComponent<UIToggle>();// 主页 
            UIGuideCtrl.Instance.AddUiGuideEventBtn(mHomepage.gameObject);
            mBattle = mRoot.Find("StartMenuManager/StartMenuBtn/Battle").GetComponent<UIToggle>();//战斗
            mMarket = mRoot.Find("StartMenuManager/StartMenuBtn/Market").GetComponent<UIToggle>();//商城
            mInteraction = mRoot.Find("StartMenuManager/StartMenuBtn/Interaction").GetComponent<UIToggle>();//社交

            UIGuideCtrl.Instance.AddUiGuideEventBtn(mMarket.gameObject);
            mDiamondText = mRoot.Find("Status/Diamond/Label").GetComponent<UILabel>();
            mGoldText = mRoot.Find("Status/Gold/Label").GetComponent<UILabel>();

            mMailBtn = mRoot.Find("Status/Mail").gameObject;
            mMailBtnBg = mRoot.Find("Status/Mail/Dark").gameObject;

            mSettingBtn = mRoot.Find("Status/Setting").GetComponent<UIButton>();

            mChatBtn = mRoot.Find("StartMenuManager/Chat").gameObject;
            NewChatTask = mChatBtn.transform.Find("Tip").GetComponent<UISprite>();
            mInerPage = mRoot.Find("InerPage");

            mLevel = mRoot.Find("Status/Level").GetComponent<UILabel>();
            mHeadIcon = mRoot.Find("Status/Head/Icon").GetComponent<UISprite>();
            mNickName = mRoot.Find("Status/Name").GetComponent<UILabel>();
            mExp = mRoot.Find("Status/EXP/Num").GetComponent<UILabel>();
            mGold = mRoot.Find("Status/Gold/Label").GetComponent<UILabel>();
            mDiamond = mRoot.Find("Status/Diamond/Label").GetComponent<UILabel>();
            VipSignLevel = mRoot.Find("Status/VipSign/Label").GetComponent<UILabel>();

            //大厅GM命令
            mGMInput = mRoot.Find("InputArea/Input").GetComponent<UIInput>();
            mGMBtn = mRoot.Find("InputArea/SendMsg").gameObject;

            UIGuideCtrl.Instance.AddUiGuideEventBtn(mBattle.gameObject);

            EventDelegate.Add(mHomepage.onChange, OnHomePageChange);//切换窗口
            EventDelegate.Add(mBattle.onChange, OnBattleChange);//切换战斗窗口
            EventDelegate.Add(mMarket.onChange, OnMarketChange);// 切换商店窗口
            EventDelegate.Add(mInteraction.onChange, OnInteractionChange);//切换社交窗口
            UIEventListener.Get(mHeadIcon.transform.parent.gameObject).onClick += InfoPersonPress;

            //战斗匹配
            // BattleWindow.Instance.SetParent(mInerPage);

            UIEventListener.Get(mChatBtn).onClick += ChatTaskBtn;

            UIEventListener.Get(mMailBtn).onClick += MailListBtn;

            //大厅GM命令
            UIEventListener.Get(mGMBtn).onClick += AddNewGMCmd;

            UIEventListener.Get(mSettingBtn.gameObject).onClick += GameSetting;
        }

        private void GameSetting(GameObject go)
        {
            GameSettingCtrl.Instance.Enter();
        }

        private void InfoPersonPress(GameObject go)
        {
            GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_PersonalInfo);

            LobbyCtrl.Instance.AskPersonInfo();
          
        }
        //////////////////
        /// 大厅GM命令///
        /// ////////////
        private void AddNewGMCmd(GameObject obj)
        {
            string cmd = mGMInput.value;
            if (cmd.Length > 0)
            {
                Debug.Log(cmd);
                LobbyCtrl.Instance.AskNewGMCmd(cmd);

                mGMInput.value = "";
            }
        }
        
        /// <summary>
        /// 聊天按钮
        /// </summary>
        /// <param name="go"></param>
        private void ChatTaskBtn(GameObject go)
        {
            GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_Chat);

            NewChat(false);
            ChatTaskCtrl.Instance.Enter(0);
        }
        //窗口控件释放
        protected override void RealseWidget()
        {
        }

        //游戏事件注册  创建窗体时调用，
        protected override void OnAddListener()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeMoney, RefreshMoney);//属刷新金币
            EventCenter.AddListener<bool>((Int32)GameEventEnum.GameEvent_ReceiveLobbyMsg, NewChat);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_AddNewMailReq, NoticeNewMail);//邮件
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeNickName,ChangeNickName);//改昵称
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeHeadID, ChangeHeadID);//改头像
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeUserLevel, ChangeLevel);//改变等级
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeUserLevel, ChangeLevel);//改变等级
        }

        private void ChangeLevel()
        {
            mLevel.text = GameUserModel.Instance.UserLevel.ToString();
        }
        private void ChangeNickName()
        {
            MsgInfoManager.Instance.ShowMsg(10039);
            mNickName.text = GameUserModel.Instance.GameUserNick;
        }

        private void ChangeHeadID()
        {
            mHeadIcon.spriteName = GameUserModel.Instance.GameUserHead.ToString();
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ChangeMoney, RefreshMoney);
            EventCenter.RemoveListener<bool>((Int32)GameEventEnum.GameEvent_ReceiveLobbyMsg, NewChat);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_AddNewMailReq, NoticeNewMail);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ChangeNickName, ChangeNickName);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ChangeHeadID, ChangeHeadID);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ChangeUserLevel, ChangeLevel);
        }

        private void NewChat(bool isVib)
        {
            NewChatTask.gameObject.SetActive(FriendManager.Instance.AllTalkDic.Values.Count != 0);
            if (FriendManager.Instance.AllTalkDic.Values.Count != 0)
            {
                foreach (var item in FriendManager.Instance.AllTalkDic.Values)
                    NewChatTask.gameObject.SetActive(item.TalkState == GameDefine.MsgTalkEnum.UnReadMsg);
            }
        }

        //显示
        public override void OnEnable()
        {
            Debug.Log("GameUserData.Instance.GameUserGold.ToString()  " + GameUserModel.Instance.mGameUserGold.ToString());
            mGoldText.text = GameUserModel.Instance.mGameUserGold.ToString();
            mDiamondText.text = GameUserModel.Instance.mGameUserDmd.ToString();

            //初始化
            mBattle.value = true;
            
        }

        //隐藏
        public override void OnDisable()
        {

        }

        public void ChangeUserLevel()
        {
            mLevel.text = GameUserModel.Instance.UserLevel.ToString();
        }

        /////////////////////////////////////////////////////////////////////////////
        //主页切换
        public void OnHomePageChange()
        {
    
            mLevel.text = GameUserModel.Instance.UserLevel.ToString();//等级
            mNickName.text = GameUserModel.Instance.GameUserNick;//昵称
            mHeadIcon.spriteName = GameUserModel.Instance.GameUserHead.ToString();//头像名称
            mGold.text = GameUserModel.Instance.mGameUserGold.ToString();//金币
            mDiamond.text = GameUserModel.Instance.mGameUserDmd.ToString();//钻石
            VipSignLevel.text = "VIP "+GameUserModel.Instance.GameUserVipLevel.ToString();//VIP
            int level = GameUserModel.Instance.UserLevel;
            mLevel.text = level.ToString();     //VIP等级       
            LevelConfigInfo leveinfo = ConfigReader.GetLevelInfo(level);
            if (leveinfo != null)
            {
                mExp.text = GameUserModel.Instance.GameUserExp + "/" + leveinfo.LevelUpExp;//经验值
                mExp.transform.parent.GetComponent<UISprite>().fillAmount = GameUserModel.Instance.GameUserExp / leveinfo.LevelUpExp;//经验条
                if (level >= 29 && GameUserModel.Instance.GameUserExp >= leveinfo.LevelUpExp)
                {
                    level = 30;//最大等级
                    mLevel.text = level.ToString();
                    mExp.gameObject.SetActive(false);
                    mExp.transform.parent.GetComponent<UISprite>().fillAmount = 1f;
                }
            }          
            if (mHomepage.value)
            {
                GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_HomePage);
                HomePageCtrl.Instance.Enter();//显示UI
            }
            else
            {
                HomePageCtrl.Instance.Exit();
            }
        }

        //战斗按钮绑定事件
        public void OnBattleChange()
        {
            if (mBattle.value)
            {
                GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_Battle);
                BattleCtrl.Instance.Enter();//广播进入BattleWindow事件
            }
            else
            {
                BattleCtrl.Instance.Exit();
            }
        }

        public void OnMarketChange()
        {
            if (mMarket.value)
            {
                GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_Market);
                MarketCtrl.Instance.Enter();
            }
            else
            {
                MarketCtrl.Instance.Exit();
            }
        }

        public void OnInteractionChange()
        {
            if (mInteraction.value)
            {
                GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_Friend);
                SocialCtrl.Instance.Enter();
            }
            else
            {
                SocialCtrl.Instance.Exit();
            }
        }

        public void MailListBtn(GameObject obj)
        {
            GameLog.Instance.AddUIEvent(GameLog.UIEventType.UIEventType_Mail);

            mMailBtnBg.SetActive(false);

            MailCtrl.Instance.Enter();
        }

        //新邮件按钮状态闪烁
        void NoticeNewMail()
        {
            mMailBtnBg.SetActive(true); 
        }

        /// <summary>
        /// 刷新帐号的货币
        /// </summary>
        private void RefreshMoney()
        {
            mGoldText.text = GameUserModel.Instance.mGameUserGold.ToString();
            mDiamondText.text = GameUserModel.Instance.mGameUserDmd.ToString();
        }

        private UITweener uit;
        private UIToggle mHomepage;  //主页
        private UIToggle mBattle;    //战斗
        private UIToggle mMarket;    //商城
        private UIToggle mInteraction;  //社交

        private UILabel mDiamondText;   //钻石
        private UILabel mGoldText;      //金币

        private GameObject mMailBtnBg;        //邮件按钮背景
        private GameObject mMailBtn;      //邮箱
        private UIButton mSettingBtn;   //设置
        private GameObject mChatBtn;      //聊天
        private UISprite NewChatTask;
        
        //大厅GM输入框
        private UIInput mGMInput;
        private GameObject mGMBtn; 
        

        //个人简单信息
        UILabel mLevel = null;
        UISprite mHeadIcon = null;
        UILabel mNickName = null;
        UILabel mExp = null;
        GameObject mInfoBtn = null;
        UILabel mGold = null;
        UILabel mDiamond = null;
        UILabel VipSignLevel = null;

        private Transform mInerPage;
        List<string> TempChat = new List<string>();
       
    }

}
