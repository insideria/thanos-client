using System;
using UnityEngine;
using Thanos.GameEntity;

namespace Thanos.GuideDate
{
    public class AdGuideBuyEquipTask : GuideTaskBase
    {
        private AdvancedGuideInfo mGuideInfo;
        private bool ToBuyEquip = true;

        public AdGuideBuyEquipTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            if (!ConfigReader.AdvancedGuideInfoDict.TryGetValue(this.mTaskId, out mGuideInfo))
            {
                return;
            }
            mTaskCDtime = mGuideInfo.CDTime / 1000f;
        }

        public override void ExcuseTask()
        {
            base.ExcuseTask();
            if (Time.frameCount % 5 != 0)
            {
                return;
            }
            if (PlayerManager.Instance.LocalPlayer == null || mIsTaskCoolDown)
            {
                return;
            }
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
            if(player.realObject == null)
            {
                return;
            }
            float mDisToBorn = Vector3.Distance(player.realObject.transform.position, mGuideInfo.EventValue0);
            if (ToBuyEquip && mDisToBorn <= mGuideInfo.EventValue2)
            {
                ToBuyEquip = false;
            }
            if (!ToBuyEquip && mDisToBorn > mGuideInfo.EventValue2)
            {
                ToBuyEquip = true;
                mIsTaskCoolDown = true;
                mTaskTime = Time.realtimeSinceStartup;
                EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, this.mTaskId);
            }
        }

        public override void ClearTask()
        {
            base.ClearTask();
        }



    }


}
