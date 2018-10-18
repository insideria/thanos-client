using System;
using UnityEngine;
using System.Collections.Generic;

using GameDefine;
using Thanos.Model;
using Thanos.Ctrl;
using Thanos.GuideDate;

namespace Thanos.View
{
    public class GamePlayGuideWindow : BaseWindow
    {
        public GamePlayGuideWindow()
        {

            mScenesType = EScenesType.EST_None;
            mResName = GameConstDefine.UIGuideRestPath;
            mResident = true;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化

        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_PlayGuideEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_PlayGuideExit, Hide);
            //EventCenter.AddListener<GameObject>((Int32)GameEventEnum.GameEvent_UIGuideEvent, OnUiGuideAddButtonEvent);
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_PlayGuideEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_PlayGuideExit, Hide);
            //EventCenter.RemoveListener<GameObject>((Int32)GameEventEnum.GameEvent_UIGuideEvent, OnUiGuideAddButtonEvent);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            
        }

        protected override void RealseWidget()
        {

        }

        public override void OnEnable()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_PlayTaskModelFinish, ExecuteNextGuide);
            EventCenter.AddListener<GuideTaskType, int>((Int32)GameEventEnum.GameEvent_PlayChildTaskFinish, OnFinishChildTask);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);
            if (GamePlayGuideModel.Instance.GuideTaskExecuteList.Count == 0)
            {
                ExecuteNextGuide();
            }
        }

        public override void OnDisable()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_PlayTaskModelFinish, ExecuteNextGuide);
            EventCenter.RemoveListener<GuideTaskType, int>((Int32)GameEventEnum.GameEvent_PlayChildTaskFinish, OnFinishChildTask);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);
        }

        //每帧更新
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            for (int i = GamePlayGuideModel.Instance.GuideTaskExecuteList.Count - 1; i >= 0; i--)
            {
                GamePlayGuideModel.Instance.GuideTaskExecuteList[i].ExcuseTask();
            }
        }

        //游戏事件注册
        protected override void OnAddListener()
        {

        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {

        }

        /// <summary>
        /// 界面打开需要表现引导
        /// </summary>
        /// <param name="gobj"></param>
        private void OnUiGuideAddButtonEvent(GameObject gobj)
        {
            if (!ConfigReader.GuideTaskXmlInfoDict.ContainsKey(mCurrentTaskId))
            {
                return;
            }
            if (ConfigReader.GuideTaskXmlInfoDict[mCurrentTaskId].BtnName == gobj.name)
            {
                DeltGuideInfos(mCurrentTaskId);
            }
        }

        private void SdkLogOff()
        {
            GamePlayGuideModel.Instance.GuideClearTask();
            GamePlayGuideModel.Instance.ClearModelData();
            GamePlayGuideCtrl.Instance.Exit();
        }

        /// <summary>
        /// 显示当前要出现的UI引导
        /// </summary>
        /// <param name="taskId"></param>
        private void DeltGuideInfos(int taskId)
        {
            //UIGuideType type = (UIGuideType)ConfigReader.GuideTaskXmlInfoDict[taskId].GuideType;
            //foreach (var item in mTypeGameObject)
            //{
            //    if (item.Key == type)
            //    {
            //        item.Value.SetActive(true);
            //        continue;
            //    }
            //    item.Value.SetActive(false);
            //}
            //switch (type)
            //{
            //    case UIGuideType.BackGroundBtn: InitGuideGroundBtn(taskId); break;
            //}
        }

        /// <summary>
        /// 初始化有强制的UI引导
        /// </summary>
        private void InitGuideGroundBtn(int taskId)
        {
            //GuideTaskInfo info = ConfigReader.GuideTaskXmlInfoDict[taskId];
            //string name = info.BtnName;
            //GuideEventButton = UIGuideModel.Instance.GetUiGuideButtonGameObject(name);
            //if (GuideEventButton == null)
            //{
            //    return;
            //}
            //LocalParent = GuideEventButton.transform.parent.gameObject;
            //GuideEventButton.transform.parent = mRoot.transform;
            //GuideEventButton.SetActive(false);
            //GuideEventButton.SetActive(true);

            //GameObject obj = LoadUiResource.LoadRes(mRoot, "Guide/" + info.PrefabName);
            //obj.transform.FindChild("Label").GetComponent<UILabel>().text = info.Text;
            //obj.transform.localPosition = info.PosXYZ;
            //ButtonOnPress ck = GuideEventButton.AddComponent<ButtonOnPress>();
            //ck.AddListener(taskId, OnUiGuideBtnFinishEvent, ButtonOnPress.EventType.ClickType);
        }

        /// <summary>
        /// 新手引导事件触发
        /// </summary>
        /// <param name="gobj"></param>
        private void OnUiGuideBtnFinishEvent(int taskId, bool presses)
        {
            if (GuideEventButton == null)
            {
                return;
            }
            GuideEventButton.transform.parent = LocalParent.transform;
            GuideEventButton.SetActive(false);
            GuideEventButton.SetActive(true);
            //TaskIdList.Remove(taskId);
            //if (TaskIdList.Count <= 0)
            //{
            //    return;
            //}
            //mCurrentTaskId = TaskIdList[0];
            //DeltGuideInfos(mCurrentTaskId);
        }

        /// <summary>
        /// 完成子任务
        /// </summary>
        /// <param name="taskId"></param>
        private void OnFinishChildTask(GuideTaskType type , int taskId)
        {
            GamePlayGuideModel.Instance.OnFinishChildTask(type, taskId);
        }

        /// <summary>
        /// 或许模块要执行的Id
        /// </summary>
        private void ExecuteNextGuide()
        {
            int taskId = GamePlayGuideModel.Instance.NowTaskId;
            if (!ConfigReader.GuideTaskMgrInfoDict.ContainsKey(taskId))
            {
                return;
            }
            List<int> idList = ConfigReader.GuideTaskMgrInfoDict[taskId].ChildTaskId;
            List<int> TypeList = ConfigReader.GuideTaskMgrInfoDict[taskId].ChildTaskType;
            for (int tp = 0; tp < TypeList.Count; tp++)
            {
                GuideTaskBase task = null;
                GuideTaskType type = (GuideTaskType)TypeList[tp];
                switch (type)
                {
                    case GuideTaskType.ClickButtonTask:
                        task = new GuideClickButtonTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.PathTask:
                        task = new GuidePathTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.TimeCtrlTask:
                        task = new GuideTimeCtrlTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.MoveCameraTask:
                        task = new GuideCameraTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.TipTask:
                        task = new GuideTipTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.PopTipTask:
                        task = new GuidePopTipTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.ObstructTask:
                        task = new GuideObstructTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.VoiceTask:
                        task = new GuideVoiceTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.ObjFlashTask:
                        task = new GuideFlashTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.ObjShowTask:
                        task = new GuideShowObjTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.AbsorbTask:
                        task = new GuideAbsorbTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.SenderSoldierTask:
                        task = new GuideSendNpcTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.SenderHeroTask:
                        task = new GuideSendHeroTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.KillTask:
                        task = new GuideKillTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.RewardTipTask:
                        task = new GuideRewardTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.ForceClickTask:
                        task = new GuideForceClick(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.ScreenClickTask:
                        task = new GuideScreenClickTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.KillHeroTask:
                        task = new GuideKillHeroTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.GetHeroTask:
                        task = new GuideGetHeroTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.GetGuideToAdGuide:
                        task = new GuideToAdGuideTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.LevelToBuyRunes:
                        task = new GuideLevelToBuyRuneTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.GetRuneTask:
                        task = new GuideGetRuneTask(idList[tp], type, mRoot.gameObject);
                        break;
                    case GuideTaskType.EquipRuneTask:
                        task = new GuideEquipRuneTask(idList[tp], type, mRoot.gameObject);
                        break;

                }
                task.EnterTask();
                GamePlayGuideModel.Instance.GuideTaskExecuteList.Add(task);
            }
        }

        private Dictionary<UIGuideType, GameObject> mTypeGameObject = new Dictionary<UIGuideType, GameObject>();

        private GameObject GuideEventButton;
        private GameObject LocalParent;
        private int mCurrentTaskId;

        public enum UIGuideType
        {
            BackGroundBtn = 1,
            TypeCheckBox,
            TypeBubble,
        }

        //public List<GuideTaskBase> GuideTaskExecuteList = new List<GuideTaskBase>();

    }


}
