using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameDefine;

using Thanos.Ctrl;
using Thanos.GameEntity;

namespace Thanos.GuideDate
{
    public class AdGuideFullAngryTask : GuideTaskBase
    {
        private AdvancedGuideInfo mGuideInfo;

        public AdGuideFullAngryTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            if (!ConfigReader.AdvancedGuideInfoDict.TryGetValue(this.mTaskId, out mGuideInfo))
            {
                return;
            }
            EventCenter.AddListener<EFuryState>((Int32)GameEventEnum.GameEvent_FuryStateInfo, OnFuryStateChange);
            mTaskCDtime = mGuideInfo.CDTime / 1000f;
        }

        public override void ExcuseTask()
        {
            base.ExcuseTask();
        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<EFuryState>((Int32)GameEventEnum.GameEvent_FuryStateInfo, OnFuryStateChange);
            base.ClearTask();
        }

        /// <summary>
        /// 角色怒气状态改变
        /// </summary>
        /// <param name="?"></param>
        private void OnFuryStateChange(EFuryState state)
        {
            if (mIsTaskCoolDown)
            {
                return;
            }
            if (state != EFuryState.eFuryFullState)
            {
                return;
            }
            mIsTaskCoolDown = true;
            mTaskTime = Time.realtimeSinceStartup;
            EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, this.mTaskId);
        }

    }


}
