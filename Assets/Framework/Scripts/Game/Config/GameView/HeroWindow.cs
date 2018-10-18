using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Thanos.GameData;
using Thanos.GameEntity;
using GameDefine;
using UICommon;
using Thanos.GuideDate;
using Thanos.Resource;
using Thanos.Ctrl;
using Thanos.Model;

namespace Thanos.View
{
    public class HeroWindow : BaseWindow
    {
        public HeroWindow()
        {
            mScenesType = EScenesType.EST_Login;
            mResName = GameConstDefine.HeroSelectUI;
            mResident = false;
        }

        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_HeroEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_HeroExit, Hide);
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_HeroFirstTime,  SetFirstTime);
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_HeroSecondTime, SetSecondTime);
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_HeroEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_HeroExit, Hide);

            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_HeroFirstTime, SetFirstTime);
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_HeroSecondTime, SetSecondTime);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            mMidShow = mRoot.Find("Animator_Area"); //中间区域
            mLockList.Clear();
            mHeroShowList.Clear();
            mSpriteBlackList.Clear();
            for (int i = 0; i < HeroCtrl.Instance.heroInfoList.Count; i++)
            {
                #region  加载选择英雄显示预设
                GameObject objLoad = null;        
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.HeroShowBoxUI, ResourceType.PREFAB);
                objLoad = GameObject.Instantiate(objUnit.Asset) as GameObject;

                objLoad.transform.parent = mMidShow.Find("HeroBox").Find("Grid");
                objLoad.transform.localScale = Vector3.one;
                objLoad.transform.localPosition = Vector3.zero;
                objLoad.name = "HeroBox" + (i + 1).ToString();

                UIToggle toggle = objLoad.GetComponent<UIToggle>();
                toggle.group = mHeroShowGroup;
                mHeroShowList.Add(toggle);
                #endregion

                #region 显示英雄选择ICON图片
                //black
                mShowAlpha = mRoot.Find("Animator_Area");
                Transform selecParent = objLoad.transform.Find("HeroThumbnails");
                mSpriteBlackList.Add(selecParent.Find("Clip").GetComponent<UISprite>());//黑色透明图片
                //head  头像 HeroThumbnails/head
                UISprite sprite = selecParent.Find("Head").GetComponent<UISprite>();
                sprite.spriteName = HeroCtrl.Instance.heroInfoList[i].HeroSelectHead.ToString();
                //lock   锁图标
                Transform lockT = objLoad.transform.Find("Lock");
                lockT.gameObject.SetActive(false);
                mLockList.Add(lockT);
                #endregion

            }
          
            #region 获得描述预设
            Transform desParent = mMidShow.Find("HeroDestribe");
            mHeroName = desParent.Find("Name").Find("Label").GetComponent<UILabel>();
            Transform skillTran = desParent.Find("Skill");
            mSkillDes = skillTran.Find("Instruction").GetComponent<UILabel>();
            mSkillDes2 = skillTran.Find("Instruction2").GetComponent<UILabel>();
            mSkillDes3 = skillTran.Find("Instruction3").GetComponent<UILabel>();
            mSpriteDes = desParent.Find("Portrait").Find("Sprite").GetComponent<UISprite>();
            mSkilltex = skillTran.Find("Icon1").GetComponent<UISprite>();
            mSkilltex1 = skillTran.Find("Icon2").GetComponent<UISprite>();
            mSkilltex2 = skillTran.Find("Icon3").GetComponent<UISprite>();
            #endregion

            #region 获得英雄头像预设
            mTeamLeft = mRoot.Find("TeamSelectInfo");
            mTeamRight = mRoot.Find("EnemySelectInfo");
            int index = 1;
            for (int j = 0; j < mHeroCount; j++)
            {

                Transform teamParent = null;
                if (j % 2 == 0)
                {  //左边三个 j = 0,2,4 , i = 1,2,3			 
                    teamParent = mTeamLeft;   //幽月精灵
                }
                else
                {//右边三个 j = 1,3,5, i = 1,2,3				 
                    teamParent = mTeamRight;  //嗜热炎魔
                }

                Transform parent = teamParent.Find("Player" + index.ToString()); //玩家
                mSpriteDic[j] = parent.Find("Thumbnail").GetComponent<UISprite>();//玩家选定的英雄头像
                mLabelName[j] = parent.Find("Name").GetComponent<UILabel>();//玩家名称
                mLabelName[j].text = "";
                mSpriteSelectDic[j] = parent.Find("Frame").GetComponent<UISprite>();//外围边框
                mLabelDic[j] = parent.Find("ConfirmBar");//  已选定图像



                if (j % 2 != 0)
                {
                    index += 1;
                }
            }
            #endregion

            #region 确定
            mBtnSure = mMidShow.Find("ConfirmButton").GetComponent<ButtonOnPress>();
            mObjHightLight = mBtnSure.transform.Find("Highlight").gameObject;
            mObjCanPress = mBtnSure.transform.Find("Show").gameObject;
            #endregion

            #region 购买界面
            mBuyParent = mRoot.Find("SureToBuyHero");
            Transform buyParentHero = mBuyParent.Find("Hero");
            mSpriteBuyDes =  buyParentHero.Find("Portrait").GetComponent<UISprite>();
            mLabelBuyName = buyParentHero.Find("Name").GetComponent<UILabel>(); ;
            mLabelBuySkill = buyParentHero.Find("Skill").Find("Instruction").GetComponent<UILabel>();
            mLabelBuyDes = buyParentHero.Find("Story").GetComponent<UILabel>();
            mBtnCloseBuy = mBuyParent.Find("Background").GetComponent<ButtonOnPress>();
            mBtnBuy = mBuyParent.Find("Buy").GetComponent<ButtonOnPress>();

            //符文
            mSkinsToggle = mRoot.Find("TurnPage/SelectSkins").GetComponent<UIToggle>();
            mSkinsDisable = mRoot.Find("TurnPage/SelectSkins/Disable").gameObject;
            mSkinsSubmit = mRoot.Find("SkinsInterface/ConfirmButton").GetComponent<UIButton>();
            mRunePages = mRoot.Find("SkinsInterface/RunePages").GetComponent<UIPopupList>();

             mSkinsHeroName = mRoot.Find("SkinsInterface/DefaultSkin/NamePlate/Name").GetComponent<UILabel>();
            mSkinsHeroIcon = mRoot.Find("SkinsInterface/DefaultSkin/Portrait").GetComponent<UISprite>();

            EventDelegate.Add(mRunePages.onChange, OnRunePageChange);
            #endregion

            #region 倒计时
            for (int i = 0; i < 2; i++)
            {
                mSpriteTens[i] = mRoot.Find("TopBar" + (i + 1).ToString()).Find("Minute1").GetComponent<UISprite>();//十位
                mSpriteUnits[i] = mRoot.Find("TopBar" + (i + 1).ToString()).Find("Minute2").GetComponent<UISprite>();//各位
            }

            SetTime(0, mFirstCountTime);
            SetTime(1, mSecondCountTime);

            #endregion

            mArrowUpDown[0] = mMidShow.transform.Find("Arrow/Left");
            mArrowUpDown[1] = mMidShow.transform.Find("Arrow/Right");
            mScrollView = mMidShow.transform.Find("HeroBox").GetComponent<UIScrollView>();

            //这里发送服务器记录的ui点击事件消息
            SendUIEventMsg();
        }

        //窗口控件释放
        protected override void RealseWidget()
        {

        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_HeroPreSelect, UpdateHeroListSelect);//更新英雄选择列表
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_HeroRealSelect, HeroSelect);//英雄选择
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_HeroAddSelect, HeroAddSelect);//预选英雄显示
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_HeroEnterRandom, RandomSelectHero);//随机选择英雄
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_HeroReconnectPreSelect, UpdateReconnectPreSelect);//更新预选英雄描述
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_HeroPreSelect, UpdateHeroListSelect);
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_HeroRealSelect, HeroSelect);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_HeroAddSelect, HeroAddSelect);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_HeroEnterRandom, RandomSelectHero);
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_HeroReconnectPreSelect, UpdateReconnectPreSelect);
        }

        //显示
        public override void OnEnable()
        {
            //设置选择状态为InitSelect
            SetSelectHeroState(SelectHeroState.InitSelect);
            ShowPlayerNickName(); //显示玩家昵称 
            mArrowUpDown[0].gameObject.SetActive(false);
            mArrowUpDown[1].gameObject.SetActive(true);
            mScrollView.onDragFinished += OnDragFinished;//添加滚动事件
            IGuideTaskManager.Instance().SendTaskEnd((Int32)GameEventEnum.GameEvent_UIGuideRoomBeginBtnEnd);

        }

        //隐藏
        public override void OnDisable()
        {
            IGuideTaskManager.Instance().RemoveTaskStartListerner((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroHeadStart, StartIGuideTask);
            IGuideTaskManager.Instance().RemoveTaskStartListerner((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroCommitStart, StartIGuideTask);

            mScrollView.onDragFinished -= OnDragFinished;
            mFirstCountTime = 60;
            mSecondCountTime = 10;
        }

        public enum SelectHeroState
        {
            InitSelect,//初始化选择
            EnterSelect,//进入选择
            InSelect,//选择中
            RandomSelect,//随机选择
            OutSelect,//退出选择
        }

        private SelectHeroState mSeletState = SelectHeroState.InitSelect;

        #region 可供选择
        private List<UIToggle> mHeroShowList = new List<UIToggle>(); //点击选择框
        private List<UISprite> mSpriteBlackList = new List<UISprite>();
        private List<Transform> mLockList = new List<Transform>();
        private int mSelectIndex = -1;
        #endregion

        #region 英雄头像
        Transform mTeamLeft;
        Transform mTeamRight;
        private UISprite[] mSpriteDic = new UISprite[mHeroCount];       //玩家选定的英雄头像显示 
        private UISprite[] mSpriteSelectDic = new UISprite[mHeroCount]; //玩家英雄头像外围边框
        private Transform[] mLabelDic = new Transform[mHeroCount];      //已选定  图像
        private UILabel[] mLabelName = new UILabel[mHeroCount];

        UITweener mTween1;
        UITweener mTween2;
        UITweener mTweenAlpha;

        private UISprite mSkilltex;
        private UISprite mSkilltex1;
        private UISprite mSkilltex2;
        #endregion

        #region 描述
        private UISprite mSpriteDes;
        private UILabel mHeroName;
        private UILabel mSkillDes;
        private UILabel mSkillDes2;
        private UILabel mSkillDes3;
        #endregion

        #region 按钮
        private ButtonOnPress mBtnSure;
        private ButtonOnPress mBtnCloseBuy;
        private ButtonOnPress mBtnBuy;
        private GameObject mObjHightLight;
        private GameObject mObjCanPress;

        private UIToggle mSkinsToggle;
        private GameObject mSkinsDisable;
        private UIButton mSkinsSubmit;
        private UIPopupList mRunePages;

        private UILabel mSkinsHeroName;
        private UISprite mSkinsHeroIcon;

        #endregion

        #region 购买英雄界面
        UISprite mSpriteBuyDes;
        UILabel mLabelBuyName;
        UILabel mLabelBuySkill;
        UILabel mLabelBuyDes;
        Transform mBuyParent;
        #endregion

        #region Top
        UISprite[] mSpriteTens = new UISprite[2];
        UISprite[] mSpriteUnits = new UISprite[2];
        #endregion


        private IPlayer mLocalPlayer;
        private bool mStartCount = false;

        #region 倒计时的时间
        public static int mFirstCountTime = 60;
        public static int mSecondCountTime =10;
        private Vector3 mTweenScaleVector = new Vector3(1.5f, 1.5f, 1f);//倒计时缩放
        private DateTime mStartTime;
        #endregion

        #region const
        private const int mTopBarTotalCount = 2;
        private const int mHeroCount = 6;
        private const int mHeroShowGroup = 2;
        private const float mMove = 50f;
        #endregion


        #region 中路顯示
        Transform mMidShow;
        Transform mShowAlpha;
        #endregion


        Transform[] mArrowUpDown = new Transform[2];
        UIScrollView mScrollView;
        void SetBtnSure(bool isVlb)
        {
            mBtnSure.gameObject.SetActive(isVlb);
            mBtnSure.enabled = isVlb;
        }
       
        
        #region 设置英雄选择界面状态
        public void SetSelectHeroState(SelectHeroState state)
        {
            mSeletState = state;
            switch (mSeletState)
            {
                case SelectHeroState.InitSelect://初始化选择状态
                    InitSelectHero();
                    #region
                    //for (int j = 0; j < mLabelDic.Length; j++)
                    //{
                    //    mLabelDic[j].gameObject.SetActive(false);//已选定图像  禁用
                    //}
                    //EnabledSurePress(false);//确定按钮初始化  禁用

                    //for (int i = 0; i < mHeroShowList.Count; i++)
                    //{
                    //    EventDelegate.Add(mHeroShowList[i].onChange, OnSelectHero);// 为可选英雄添加监听器

                    //}
                    //mBtnSure.AddListener(OnCommitHero);//为确定按钮添加事件
                    //mBtnCloseBuy.AddListener(CloseBuy);//为背景添加事件 （点击可关闭商店）
                    //mBtnBuy.AddListener(BuyHero);    //为购买按钮添加事件
                    //mLocalPlayer = PlayerManager.Instance.LocalAccount;
                    //InitFlash();
                    //InitRunePageList();
                    #endregion
                    break;
                case SelectHeroState.EnterSelect://进入选择状态
                    EnterSelectHero();
                    break;
                case SelectHeroState.RandomSelect://随机选择状态
                    RandomSelectHero();
                    break;
                case SelectHeroState.OutSelect://退出随机选择
                    OutSelectHero();
                    break;
            }
        }
        #endregion

        #region 预选择英雄显示
        public void UpdateHeroListSelect()
        {
            //刷新选择列表    mSpriteBlackList == 黑色透明图片
            foreach (var pic in mSpriteBlackList)
            {
                pic.gameObject.SetActive(false);
            }
            //heroSelectDic 指得是  玩家与预选择的英雄
            foreach (var item in HeroCtrl.Instance.heroSelectDic.Keys)
            {
                int heroId = 0;
                if (!HeroCtrl.Instance.heroSelectDic.TryGetValue(item, out heroId))
                    continue;

                //找到队友所选择的英雄，并设置为黑色代表不可用
                if (item.GameUserSeat != mLocalPlayer.GameUserSeat && mLocalPlayer.IsSameCamp(item))
                {
                    int index = GetSelectIndexBuyHeroId(heroId);//通过英雄id获取图像索引值
                    mSpriteBlackList[index].gameObject.SetActive(true);//设置遮罩
                }
                //显示头像
                mSpriteDic[(int)item.GameUserSeat - 1].spriteName = ConfigReader.GetHeroSelectInfo(heroId).HeroSelectHead.ToString();
                
                if (item == PlayerManager.Instance.LocalAccount)
                {
                    mSpriteSelectDic[(int)item.GameUserSeat - 1].spriteName = "329";
                }
                //激活可选英雄外围边框
                if (!mSpriteSelectDic[(int)item.GameUserSeat - 1].gameObject.activeInHierarchy)
                {
                    mSpriteSelectDic[(int)item.GameUserSeat - 1].gameObject.SetActive(true);
                }
            }
        }
        #endregion

        #region 显示玩家昵称
        void ShowPlayerNickName()
        {
            foreach (var item in PlayerManager.Instance.AccountDic.Values)
            {
                int index = (int)item.GameUserSeat - 1;
                if (index >= mLabelName.Length)
                    continue;
                mLabelName[index].text = item.GameUserNick;
            }
        }
        #endregion

        #region 更新左右两边队伍是否选择英雄的显示
        public void UpdateSureSelect()
        {
            //刷新左右两边队伍是否已经选择
            for (int i = 0; i < mHeroCount; i++)
            {
                mLabelDic[i].gameObject.SetActive(false);//禁用所有的已选定的图像，
            }
            foreach (var item in PlayerManager.Instance.AccountDic.Values)
            {
                if (HeroCtrl.Instance.heroSureSelectDic.ContainsKey(item))
                {
                     mLabelDic[(int)item.GameUserSeat - 1].gameObject.SetActive(true);  //显示 已选定

                    if (!mSpriteSelectDic[(int)item.GameUserSeat - 1].gameObject.activeInHierarchy)
                    {
                        mSpriteSelectDic[(int)item.GameUserSeat - 1].gameObject.SetActive(true);
                    }
                }
            }
            if (HeroCtrl.Instance.heroSureSelectDic.ContainsKey(PlayerManager.Instance.LocalAccount))
            {
                //更新英雄头像和名称，皮肤选择里面
                int id = HeroCtrl.Instance.heroSureSelectDic[PlayerManager.Instance.LocalAccount];

                //获取确定的英雄的配置文件
                HeroSelectConfigInfo config = new HeroSelectConfigInfo();
                 // HeroSelectXmlInfoDict  英雄选择表
                if (ConfigReader.HeroSelectXmlInfoDict.TryGetValue(id, out config))
                {
                    mSkinsHeroName.text = config.HeroSelectNameCh;//皮肤名称
                    mSkinsHeroIcon.spriteName = config.HeroSelectHead.ToString();//头像名称
                }
                //按钮控制
                if (!mBtnSure.enabled)
                    return;
                EnabledSurePress(false);//禁用按钮
            }

        }
        #endregion

        #region 更新英雄选择栏的英雄锁定显示
        private void UpdateLock()
        {
            //显示锁定框
            for (int i = 0; i < HeroCtrl.Instance.heroInfoList.Count; i++)
            {
                if (!GameUserModel.Instance.CanChooseHeroList.Contains(HeroCtrl.Instance.heroInfoList[i].HeroSelectNum))
                {
                    mLockList[i].gameObject.SetActive(true);
                }
                else
                {
                    mLockList[i].gameObject.SetActive(false);
                }
            }
        }
        #endregion
     
        void OnDragFinished()
        {
            Vector3 constraint = mScrollView.panel.CalculateConstrainOffset(mScrollView.bounds.min, mScrollView.bounds.max);
            if (mScrollView.RestrictWithinBounds(false, false, true))
            {
                if (constraint.y > 0f)//上到头
                {
                    mArrowUpDown[0].gameObject.SetActive(false);
                    mArrowUpDown[1].gameObject.SetActive(true);
                }
                else//左到头
                {
                    mArrowUpDown[0].gameObject.SetActive(true);
                    mArrowUpDown[1].gameObject.SetActive(false);
                }
            }
            else
            {
                mArrowUpDown[0].gameObject.SetActive(true);
                mArrowUpDown[1].gameObject.SetActive(true);
            }
        }
        #region InitFlash
        //切入动画设置  选择状态设置为进入状态
        void InitFlash()
        {
            UpdateLock();//更新英雄选择栏的英雄锁定显示
            Vector3 orginalLeft = mTeamLeft.localPosition;
            Vector3 orginalRight = mTeamRight.localPosition;

            mTeamLeft.localPosition = new Vector3(orginalLeft.x - mMove, orginalLeft.y, orginalLeft.z);
            mTeamRight.localPosition = new Vector3(orginalRight.x + mMove, orginalRight.y, orginalRight.z);

            mTween1 = TweenPosition.Begin(mTeamLeft.gameObject, 0.5f, orginalLeft);
            mTween2 = TweenPosition.Begin(mTeamRight.gameObject, 0.5f, orginalRight);
            UICommonMethod.TweenAlphaBegin(mShowAlpha.gameObject, 0f, 0f);
            mTweenAlpha = UICommonMethod.TweenAlphaBegin(mShowAlpha.gameObject, 1.7f, 1f);
            mTween1.method = UITweener.Method.EaseIn;
            mTween2.method = UITweener.Method.EaseIn;
            mSelectIndex = -1;
            SetSelectHeroState(SelectHeroState.EnterSelect);
        }
        #endregion


        #region
        void EnableToggle(bool enable)
        {
            foreach (var item in mHeroShowList)
            {
                item.enabled = enable;
            }
        }
        #endregion


        //初始化选择状态
        private  void InitSelectHero()
        {

            for (int j = 0; j < mLabelDic.Length; j++)
            {
                mLabelDic[j].gameObject.SetActive(false);//已选定图像  禁用
            }
            EnabledSurePress(false);//确定按钮初始化  禁用

            for (int i = 0; i < mHeroShowList.Count; i++)
            {
                EventDelegate.Add(mHeroShowList[i].onChange, OnSelectHero);// 为可选英雄添加监听器

            }
            mBtnSure.AddListener(OnCommitHero);//为确定按钮添加事件
            mBtnCloseBuy.AddListener(CloseBuy);//为背景添加事件 （点击可关闭商店）
            mBtnBuy.AddListener(BuyHero);    //为购买按钮添加事件
            mLocalPlayer = PlayerManager.Instance.LocalAccount;
            InitFlash();//动画设置并切换EnterSelect状态
            InitRunePageList();//初始化符文

        }

        #region 进入选择英雄阶段  
        private void EnterSelectHero()
        {
            MsgInfoManager.Instance.ShowMsg(10);//通过id读取配置文件。来显示“请选择英雄”
          //  SetBtnSure(true);//激活确定按钮  小谷注释 应该是在点击头像时激活，此时激活？
            EnableToggle(true);//激活英雄列表中的英雄头像
            FirstCountDown();//第一次倒计时
            //InSelect状态的作用仅仅是退出EnterState，但是在此状态下并没有做任何操作
            SetSelectHeroState(SelectHeroState.InSelect);
            //UIGuide
            IGuideTaskManager.Instance().AddTaskStartListerner((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroHeadStart, StartIGuideTask);
            IGuideTaskManager.Instance().AddTaskStartListerner((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroCommitStart, StartIGuideTask);
            IGuideTaskManager.Instance().SendTaskTrigger((Int32)GameEventEnum.GameEvent_UIGuideTriggerSelectHero);

        }
        #endregion

        void StartIGuideTask(Int32 taskId)
        {
            IGuideTaskManager.Instance().SendTaskEffectShow(taskId);
        }



        #region 进入最后随机阶段   进入选择皮肤阶段 倒计时10秒开始
        private void RandomSelectHero() 
        {
            IGuideTaskManager.Instance().SendTaskEnd((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroHeadEnd);
            IGuideTaskManager.Instance().SendTaskEnd((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroCommitEnd);

            EnabledSurePress(false);//禁用确定按钮
            TweenRotation obj = mSpriteUnits[0].transform.parent.GetComponent<TweenRotation>();//十位动画组件
            obj.enabled = true;
          
            obj.method = UITweener.Method.EaseOut;
            EventDelegate.Add(obj.onFinished, LastCountDown);

            mSkinsDisable.gameObject.SetActive(false); //禁用选择皮肤的灰色图片  来显示绿色图
            mSkinsToggle.value = true;
        }
        #endregion

        #region 结束选择英雄
        private void OutSelectHero()
        {
            for (int i = 0; i < mHeroShowList.Count; i++)
            {
                //EventDelegate.Add(mHeroShowList[i].onChange, OnSelectHero);//小谷注释  结束后不应该移除事件吗？
                EventDelegate.Remove(mHeroShowList[i].onChange, OnSelectHero);//小谷改的
            }
            mBtnSure.RemoveListener(OnCommitHero);
            mBtnCloseBuy.RemoveListener(CloseBuy);
            mBtnBuy.RemoveListener(BuyHero);
            mStartCount = false;
        }
        #endregion


        /// //////////////////////////////////////////////////////////倒计时？？？？？？？

        #region 倒计时
        private void SetTime(int step, int time)
        {
            //    54 =  15  +  14  第一次倒计时每张图片的名称与对应的数字相差10
            //    例如      紫色的4图片的名称为14
            //第二次倒计时为金色图片，图片数字与名称一一对应
            int ten = time / 10;//     5
            int unit = time % 10;//个位   4
            if (step == 0)//如果是第一次倒计时，那么个位数字加10，目的为了获得对应数字图片的名称
            {
                unit = unit + 10;   
            }
            //显示个位数字
            mSpriteUnits[step].spriteName = unit.ToString();

            if (ten == 0)//十位
            {
                //关闭十位上的数字显示
                mSpriteTens[step].gameObject.SetActive(false);
            }
            else
            {
                //打开十位上的数字显示    mSpriteTens[step]
                mSpriteTens[step].gameObject.SetActive(true);
                if (step == 0)//如果是第一次倒计时，那么十位数字加10，目的为了获得对应数字图片的名称
                {
                    ten = ten + 10;  
                }
                //显示十位数字
                mSpriteTens[step].spriteName = ten.ToString();
            }     
        }
        //选择英雄倒计时
        private void FirstCountDown()
        {
            if (mFirstCountTime == 0) return;

            SetTime(0, mFirstCountTime);//倒计时
            mStartTime = DateTime.Now;
            mStartCount = true;
            mSpriteTens[1].transform.parent.gameObject.SetActive(false);//禁用第二次倒计时专用的十位数字
        }

        //选择符文倒计时
        private void LastCountDown()
        {
            mStartTime = DateTime.Now;
            mStartCount = true;
            mSpriteTens[0].transform.parent.gameObject.SetActive(false);
            mSpriteTens[1].transform.parent.gameObject.SetActive(true);

            SetSelectHeroState(SelectHeroState.RandomSelect);
        }

        UITweener scaleTween;
        //倒计时动画
        private void TweenCountDownLabelScale(GameObject obj)
        {
            TweenAlpha.Begin(obj.gameObject, 0f, 1f);
            TweenScale.Begin(obj, 0f, mTweenScaleVector);
            scaleTween = TweenScale.Begin(obj, 0.5f, new Vector3(1f, 1f, 1f));
            scaleTween.method = UITweener.Method.EaseInOut;
            EventDelegate.Add(scaleTween.onFinished, FinshTweenScale, true);
        }

        void FinshTweenScale()
        {
            EventDelegate.Remove(scaleTween.onFinished, FinshTweenScale);
        }

        //设置第二次倒计时
        void SetSecondTimeDown()
        {
            SetTime(1, mSecondCountTime);//  10,,,,,,,
            TweenCountDownLabelScale(mSpriteUnits[1].gameObject);
        }

        public override void Update(float deltaTime)
        {
            if (mStartCount)
            {
                TimeSpan span = DateTime.Now - mStartTime;
                if (span.TotalSeconds <= 1f)//计时，不满1秒直接返回
                    return;
                mStartTime = DateTime.Now;

                if (mSeletState == SelectHeroState.RandomSelect)
                {
                    //时间基数 10,9,8，，，，，，，
                    mSecondCountTime = mSecondCountTime - (int)span.TotalSeconds;
                    if (mSecondCountTime < 0)
                    {
                        mStartCount = false;
                        return;
                    }

                    // 设置第二次倒计时
                    SetSecondTimeDown();
                }
                else
                {
                    //时间基数 59，58，，，，，，，
                    mFirstCountTime = mFirstCountTime - (int)span.TotalSeconds;
                    if (mFirstCountTime < 0)
                    {
                        mStartCount = false;
                        return;
                    }
                    //设置时间显示
                    SetTime(0, mFirstCountTime);
                }
            }
        }
        #endregion
    
      //////////////////////////////////////////////////////倒计时//////////////////////////////////////////
   

        #region 结束入场动画
        private void FinishTween()
        {
            //  SetSelectHeroState(SelectHeroState.EnterSelect);		
        }
        #endregion

        #region 点击购买英雄
        private void BuyHero(int ie, bool press)
        {
            if (press || mSeletState == SelectHeroState.InitSelect)
            {
                return;
            }
        }
        #endregion

        #region 关闭购买英雄
        private void CloseBuy(int ie, bool press)
        {
            if (press)
            {
                return;
            }
            //buyParent.gameObject.SetActive(false);
        }
        #endregion

        #region 点击确定选择英雄按钮事件
        private void OnCommitHero(int ie, bool press)
        {
            EnabledSurePress(false);//设置按钮不可显示

            if (press || mSeletState != SelectHeroState.InSelect) return;
         
            if (mSelectIndex != -1)
            {
                IGuideTaskManager.Instance().SendTaskEnd((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroCommitEnd);
                //通知服务器确定选择的英雄
                HolyGameLogic.Instance.EmsgToss_AskSelectHero((UInt32)HeroCtrl.Instance.heroInfoList.ElementAt(mSelectIndex).HeroSelectNum);

                mSkinsDisable.gameObject.SetActive(false);//选择皮肤
                mSkinsToggle.value = true;//皮肤
            }
        }
        #endregion

        #region 点击英雄栏的选择事件
        void OnSelectHero()
        {
           
            UIToggle toggle = UIToggle.current;
            if (toggle != null && toggle.group == mHeroShowGroup && toggle.value)
            {
                //mHeroShowList在 InitWight中添加了所有的toggle
                for (int i = 0; i < mHeroShowList.Count; i++)
                {
                    if (mHeroShowList[i] != toggle)
                        continue;

                    mSelectIndex = i;
                    ShowDestribe(i);//英雄描述信息 
                    if (UpdateSureBtn(i))//更新按钮状态并判断是否可点击选择按钮
                    {
                        //是否可点击
                        IGuideTaskManager.Instance().SendTaskEnd((Int32)GameEventEnum.GameEvent_UIGuideSelectHeroHeadEnd);
                        NotifyServerMyPreSelect(i);//通知服务端点击的英雄
                    }
                    break;
                }
            }
        }

        public void UpdateReconnectPreSelect(int heroId)
        {
            int index = GetSelectIndexBuyHeroId(heroId);
            mSelectIndex = index;
            ShowDestribe(index);//描述 
            SetSelectShow(index);
        }


        #endregion

        #region 更新按钮状态并判断是否可点击选择按钮
        bool UpdateSureBtn(int index)
        {
            bool canPress = true;
            int id = HeroCtrl.Instance.heroInfoList[index].HeroSelectNum;
            //如果队友没有选择，可选列表中包含此图像的id，选择状态为InSelect ，并且没有确定选择的英雄时
            if (!TeamMateHasSelectHero(id) && GameUserModel.Instance.CanChooseHeroList.Contains(id)
               && mSeletState == SelectHeroState.InSelect
               && !HeroCtrl.Instance.heroSureSelectDic.ContainsKey(PlayerManager.Instance.LocalAccount))
            {
                canPress = true;
            }
            else
            {
                canPress = false;
            }
            EnabledSurePress(canPress);
            return canPress;
        }
        #endregion

        #region 设置按钮可不可选择显示
        void EnabledSurePress(bool enable)
        {
            mBtnSure.GetComponent<BoxCollider>().enabled = enable;
            mObjHightLight.gameObject.SetActive(!enable);
            mObjCanPress.gameObject.SetActive(enable);
        }
        #endregion

        #region 通知服务端点击的英雄
        void NotifyServerMyPreSelect(int index)
        {
            int id = HeroCtrl.Instance.heroInfoList[index].HeroSelectNum;
            HolyGameLogic.Instance.EmsgTocs_TryToSelectHero((uint)id);
        }
        #endregion

        #region 通过点击的是第几个英雄显示英雄信息            英雄描述信息-------------
        void ShowDestribe(int index)
        {
            ShowHasDes(index);
            if (!GameUserModel.Instance.CanChooseHeroList.Contains(HeroCtrl.Instance.heroInfoList[index].HeroSelectNum))
            {
                ShowBuyeDes(index);
            }
        }
        #endregion

        #region 通过点击的是第几个英雄显示已购买英雄信息     英雄描述信息-------------
        public void ShowHasDes(int index)
        {
            mHeroName.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectNameCh;
            mSpriteDes.spriteName = HeroCtrl.Instance.heroInfoList[index].HeroSelectThumb;
            mSkillDes.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectDes;
            mSkillDes2.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectDes2;
            mSkillDes3.text = HeroCtrl.Instance.heroInfoList[index].HeroPassDes;
            mSkilltex.spriteName = HeroCtrl.Instance.heroInfoList[index].HeroSelectSkill1;
            mSkilltex1.spriteName = HeroCtrl.Instance.heroInfoList[index].HeroSelectSkill2;
            mSkilltex2.spriteName = HeroCtrl.Instance.heroInfoList[index].HeroSkillPass;
        }
        #endregion

        #region 通过点击的是第几个英雄显示购买描述           英雄描述信息-------------
        void ShowBuyeDes(int index)
        {
            return;
            mLabelBuyName.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectNameCh;
            mSpriteBuyDes.spriteName = HeroCtrl.Instance.heroInfoList[index].HeroSelectThumb;
            mLabelBuySkill.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectDes;
            mLabelBuyDes.text = HeroCtrl.Instance.heroInfoList[index].HeroSelectBuyDes;
            mBuyParent.gameObject.SetActive(true);
        }
        #endregion

        #region 通过英雄ID号获得选择的是第几个英雄
        public int GetSelectIndexBuyHeroId(int heroId)
        {
            for (int i = 0; i < HeroCtrl.Instance.heroInfoList.Count; i++)
            {
                if (HeroCtrl.Instance.heroInfoList[i].HeroSelectNum != heroId)
                    continue;
                return i;
            }
            return -1;
        }
        #endregion


        public void SetSelectShow(int index)
        {
            mHeroShowList[index].value = true;
        }

        #region 通过英雄ID判断队友是否有选择此英雄
        private bool TeamMateHasSelectHero(int heroId)
        {
            foreach (var item in HeroCtrl.Instance.heroSelectDic.Keys)
            {
                if (item.GameUserSeat == mLocalPlayer.GameUserSeat || !mLocalPlayer.IsSameCamp(item))//找到队友所选择的英雄
                    continue;
                int heroSelect = 0;
                if (!HeroCtrl.Instance.heroSelectDic.TryGetValue(item, out heroSelect))
                    continue;
                if (heroSelect == heroId)
                    return true;
            }
            return false;
        }
        #endregion

        //英雄选择
        public void HeroSelect(int heroid)
        {
            int itemPos = GetSelectIndexBuyHeroId(heroid);
            ShowHasDes(itemPos);
            SetSelectShow(itemPos);
            MsgInfoManager.Instance.ShowMsg((int)ErrorTypeEnum.ObtainRandomHero);
        }

        public void HeroAddSelect()
        {
            UpdateHeroListSelect();  //预选择英雄显示
            UpdateSureSelect();      //更新左右两边队伍是否选定英雄的显示
        }

        public void SetFirstTime(int time)
        {
            mFirstCountTime = time;
        }

        public void SetSecondTime(int time)
        {
            mSecondCountTime = time;
        }

        public void OnRunePageChange()
        {
            int index = mRunePages.data == null ? 0 : (int)mRunePages.data;

            HolyGameLogic.Instance.EmsgToss_AskChangeRunePage(index);
        }

        public void InitRunePageList()
        {
            mRunePages.items.Clear();
            mRunePages.itemData.Clear();
            int page = RuneEquipModel.Instance.pagenum + 1;

            for (int i = 0; i < page; i++)
            {
                mRunePages.items.Add(ConfigReader.GetMsgInfo(40050).content + (i + 1));
                mRunePages.itemData.Add(i);
            }
        }
        //这里出发发送服务器记录的ui点击事件消息
        public void SendUIEventMsg()
        {
            HolyGameLogic.Instance.EmsgTocs_ReportUIEvent();
        }
    } 
}