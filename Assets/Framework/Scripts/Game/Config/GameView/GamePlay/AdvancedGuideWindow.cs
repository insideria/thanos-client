using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GuideDate;
using Thanos.Resource;

namespace Thanos.View
{
    public class AdvancedGuideWindow : BaseWindow
    {
        public AdvancedGuideWindow()
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.AdVancedGuideRestPath;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_AdvancedGuideEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_AdvancedGuideExit, Hide);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            Transform mAnchor = mRoot.Find("Anchor");
            for (int mcd = 0; mcd < mAnchor.childCount; mcd++)
            {
                GuideInfoWidnow infoWd = new GuideInfoWidnow(this);
                infoWd.InfoWindowGameObject = mAnchor.GetChild(mcd).gameObject;
                InfoWindowList.Add(infoWd);
                infoWd.InfoWindowGameObject.SetActive(false);
                Transform bg = infoWd.InfoWindowGameObject.transform.Find("BG");
                infoWd.DynamicLabel = infoWd.InfoWindowGameObject.transform.Find("BG/DynamicLabel").GetComponent<UILabel>();
                UIEventListener.Get(bg.gameObject).onClick += infoWd.GuideInfoOnClick;
            }

            GuideInfoWindow = mRoot.Find("UIGuideWindow").gameObject;
            mSmallTitle = GuideInfoWindow.transform.Find("TopText").GetComponent<UILabel>();
            mInfoContent = GuideInfoWindow.transform.Find("BottomText").GetComponent<UILabel>();
            mBtnContinue = GuideInfoWindow.transform.Find("Button").GetComponent<UIButton>();
            mCenterPic = GuideInfoWindow.transform.Find("CenterPic").GetComponent<UISprite>();
            EventDelegate.Add(mBtnContinue.onClick, OnButtonContinueOnClick);
            GuideInfoWindow.SetActive(false);
            InitAdGuideTask();
        }

        protected override void RealseWidget()
        {

        }

        public override void OnEnable()
        {

        }
        //隐藏
        public override void OnDisable()
        {
        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, OnEnterGuideTask);
            //EventCenter.AddListener((Int32)GameEventEnum.GameEvent_BatttleFinished, BatttleFinished);
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, OnEnterGuideTask);
            //EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_BatttleFinished, BatttleFinished);
        }

        public override void Realse()
        {
        }

        //每帧更新
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            for (int i = AdGuideTaskList.Count - 1; i >= 0; i--)
            {
                AdGuideTaskList[i].ExcuseTask();
            }
        }

        private void BatttleFinished()
        {
            Thanos.Ctrl.AdvancedGuideCtrl.Instance.Exit();
        }

        private void OnButtonContinueOnClick()
        {
            int[] TaskArray = AdGuideShowTaskQueue.ToArray();
            List<int> taskList = new List<int>();
            foreach (int task in TaskArray)
            {
                int mtk = AdGuideShowTaskQueue.Dequeue();
                if (mtk != mShowGuideTaskId)
                {
                    taskList.Add(mtk);
                }
            }
            foreach (int mtask in taskList)
            {
                AdGuideShowTaskQueue.Enqueue(mtask);
            }
            GuideInfoWindow.SetActive(false);
            DeltGuideInfoList();
        }

        /// <summary>
        /// 显示引导的信息
        /// </summary>
        /// <param name="mTaskId"></param>
        public void ShowGuideWindow(int mTaskId)
        {
            mShowGuideTaskId = mTaskId;
            AdvancedGuideInfo mInfo = ConfigReader.AdvancedGuideInfoDict[mShowGuideTaskId];
            GuideInfoWindow.SetActive(true);
            mSmallTitle.text = mInfo.SmallTitle;
            mInfoContent.text = mInfo.Content;
            ResourceItem Unit = ResourcesManager.Instance.loadImmediate("Guide/" + mInfo.PrefabID, ResourceType.PREFAB);
            mCenterPic.atlas = (Unit.Asset as GameObject).GetComponent<UIAtlas>();
            mCenterPic.spriteName = mInfo.PicID;
        }

        /// <summary>
        /// 进入引导任务
        /// </summary>
        /// <param name="mTaskId"></param>
        private void OnEnterGuideTask(int mTaskId)
        {
            if (AdGuideShowTaskQueue.Count <= AdGuideTaskMaxNum)
            {
                AdGuideShowTaskQueue.Enqueue(mTaskId);
            }
            else
            {
                //AdGuideShowTaskQueue.Peek
                int lastId = AdGuideShowTaskQueue.Dequeue();
                AdGuideShowTaskQueue.Enqueue(mTaskId);
            }
            DeltGuideInfoList();
        }

        /// <summary>
        /// 刷新引导显示按钮列表
        /// </summary>
        private void DeltGuideInfoList()
        {
            int[] TaskArray = AdGuideShowTaskQueue.ToArray();
            for (int i = 0; i < InfoWindowList.Count; i++)
            {
                GuideInfoWidnow infoWd = InfoWindowList[i];
                if (i >= AdGuideShowTaskQueue.Count)
                {
                    infoWd.InfoWindowGameObject.SetActive(false);
                }
                else
                {
                    infoWd.mGuideInfo = ConfigReader.AdvancedGuideInfoDict[TaskArray[i]];
                    infoWd.ShowGuideInfo();
                }
            }
        }

        /// <summary>
        /// 初始化引导任务
        /// </summary>
        private void InitAdGuideTask()
        {
            foreach (var item in ConfigReader.AdvancedGuideInfoDict)
            {
                GuideTaskBase mTask = null;
                switch ((GuideTaskType)item.Key)
                {
                    case GuideTaskType.mGuideBuyEquip:
                        mTask = new AdGuideBuyEquipTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideGetCp:
                        mTask = new AdGuideGetCpTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideBeAttackByBuilding:
                        mTask = new AdGuideBeAttackByBuildTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideFullAngry:
                        mTask = new AdGuideFullAngryTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideAbsorbMonster:
                        mTask = new AdGuideAbsorbSuccessTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuidePlayerReborn:
                        mTask = new AdGuidePlayerRebornTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideEnterAltar:
                        mTask = new AdGuideEnterAltarTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideCheckSkillInfo:
                        mTask = new AdGuideCheckSkillInfoTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuidePromptToAbsorb:
                        mTask = new AdGuidePromptToAbsorbTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                    case GuideTaskType.mGuideBornStrongSoldier:
                        mTask = new AdGuideBornStrongSoldierTask(item.Key, GuideTaskType.mGuideBuyEquip, mRoot.gameObject);
                        break;
                }
                if (mTask != null)
                {
                    mTask.EnterTask();
                    AdGuideTaskList.Add(mTask);
                }
            }
        }

        Queue<int> AdGuideShowTaskQueue = new Queue<int>(AdGuideTaskMaxNum);
        private const int AdGuideTaskMaxNum = 3;
        private List<GuideTaskBase> AdGuideTaskList = new List<GuideTaskBase>();
        private List<GuideInfoWidnow> InfoWindowList = new List<GuideInfoWidnow>();

        private int mShowGuideTaskId;

        //提示小窗口        
        private GameObject GuideInfoWindow;
        private UILabel mSmallTitle;
        private UILabel mInfoContent;
        private UISprite mCenterPic;
        private UIButton mBtnContinue;

    }

    public class GuideInfoWidnow
    {
        public GameObject InfoWindowGameObject;
        public UILabel DynamicLabel;
        public AdvancedGuideInfo mGuideInfo;
        public AdvancedGuideWindow ParentWindow;

        public GuideInfoWidnow(AdvancedGuideWindow mRoot)
        {
            ParentWindow = mRoot;
        }

        /// <summary>
        /// 操作提示说明窗口点击
        /// </summary>
        /// <param name="gObj"></param>
        public void GuideInfoOnClick(GameObject gObj)
        {
            if (mGuideInfo == null)
            {
                return;
            }
            ParentWindow.ShowGuideWindow(mGuideInfo.Taskid);
        }

        public void ShowGuideInfo()
        {
			if (InfoWindowGameObject != null) {
				InfoWindowGameObject.SetActive(true);			
			}
            
            DynamicLabel.text = mGuideInfo.Title;
        }

        //public void 
    }
}
