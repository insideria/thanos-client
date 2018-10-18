using System;
using UnityEngine;

namespace Thanos.GuideDate
{
    public class AdGuideAbsorbSuccessTask : GuideTaskBase
    {
        private AdvancedGuideInfo mGuideInfo;

        public AdGuideAbsorbSuccessTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            if (!ConfigReader.AdvancedGuideInfoDict.TryGetValue(this.mTaskId, out mGuideInfo))
            {
                return;
            }
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, OnGuideTaskEvents);
            mTaskCDtime = mGuideInfo.CDTime / 1000f;
        }

        public override void ExcuseTask()
        {
            base.ExcuseTask();
        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, OnGuideTaskEvents);
            base.ClearTask();
        }

        /// <summary>
        /// 引导任务事件触发
        /// </summary>
        /// <param name="monsterId"></param>
        private void OnGuideTaskEvents(int monsterId)
        {
            if (mIsTaskCoolDown)
            {
                return;
            }
            mIsTaskCoolDown = true;
            mTaskTime = Time.realtimeSinceStartup;
            EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, this.mTaskId);
        }

    }


}
