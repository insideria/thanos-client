using System;
using UnityEngine;
using Thanos.Model;

namespace Thanos.Ctrl
{
    public class UIGuideCtrl : Singleton<UIGuideCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayGuideEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayGuideExit);
        }

        public void InitLobbyGuideInfo()
        {
            if (!UIGuideModel.Instance.mIsGuideComp && UIGuideModel.Instance.mCurrentTaskModelId != mGuideStepInfo.GuideStepNull)
            {
                //this.Enter();
            }
        }

        /// <summary>
        /// 进入新手引导
        /// </summary>
        public void UIGuideAskEnterPrimaryGuide()
        {
            //CGLCtrl_GameLogic.Instance.AskEnterNewsGuideBattle(1002);
        }

        /// <summary>
        /// 引导的完成信息
        /// </summary>
        /// <param name="pMsg"></param>
        public void GuideRespStep(GSToGC.GuideCSStepInfo pMsg)
        {
            UIGuideModel.Instance.GuideFinishModelList(pMsg.taskid);
        }

        public void GuideBattleType(GCToCS.AskCSCreateGuideBattle.guidetype mType)
        {
            UIGuideModel.Instance.GuideType = mType;
        }

        /// <summary>
        /// 将要触发事件的新手引导的按钮放入列表
        /// </summary>
        /// <param name="gobj"></param>
        public void AddUiGuideEventBtn(GameObject gobj)
        {
            if (gobj == null || UIGuideModel.Instance.UiGuideButtonGameObjectList.Contains(gobj))
            {
                return;
            }
            UIGuideModel.Instance.UiGuideButtonGameObjectList.Add(gobj);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UIGuideEvent, gobj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gobj"></param>
        public void RemoveGuideBtnEvent(GameObject gobj)
        {
            if (gobj == null || !UIGuideModel.Instance.UiGuideButtonGameObjectList.Contains(gobj))
            {
                return;
            }
            UIGuideModel.Instance.UiGuideButtonGameObjectList.Remove(gobj);
        }

        //public List<int> Gui
        //public bool mIsGuideComp;

    }
}

