using System;
using UnityEngine;

namespace Thanos.GuideDate
{
    public class AdGuideCheckSkillInfoTask : GuideTaskBase
    {
        private AdvancedGuideInfo mGuideInfo;

        public AdGuideCheckSkillInfoTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            if (!ConfigReader.AdvancedGuideInfoDict.TryGetValue(this.mTaskId, out mGuideInfo))
            {
                return;
            }
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerLevelChange, OnGuideTaskEvents);
            mTaskCDtime = mGuideInfo.CDTime / 1000f;
        }

        public override void ExcuseTask()
        {
            base.ExcuseTask();
        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerLevelChange, OnGuideTaskEvents);
            base.ClearTask();
        }

        /// <summary>
        /// 引导任务事件触发
        /// </summary>
        /// <param name="level"></param>
        private void OnGuideTaskEvents(int level)
        {
            if (mIsTaskCoolDown)
            {
                return;
            }
            if (level == mGuideInfo.EventValue1[0])
            {
                mIsTaskCoolDown = true;
                mTaskTime = Time.realtimeSinceStartup;
                EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, this.mTaskId);
            }
        }

    }


}
