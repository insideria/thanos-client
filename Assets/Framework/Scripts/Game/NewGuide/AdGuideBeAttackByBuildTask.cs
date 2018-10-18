using System;
using UnityEngine;
using Thanos.GameEntity;

namespace Thanos.GuideDate
{
    public class AdGuideBeAttackByBuildTask : GuideTaskBase
    {
        private AdvancedGuideInfo mGuideInfo;
        public AdGuideBeAttackByBuildTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            if (!ConfigReader.AdvancedGuideInfoDict.TryGetValue(this.mTaskId, out mGuideInfo))
            {
                return;
            }
            EventCenter.AddListener<UInt64, uint, UInt64>((Int32)GameEventEnum.GameEvent_BroadcastBeAtk, OnWarningEvent);
            mTaskCDtime = mGuideInfo.CDTime / 1000f;
        }

        public override void ExcuseTask()
        {
            base.ExcuseTask();
        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<UInt64, uint, UInt64>((Int32)GameEventEnum.GameEvent_BroadcastBeAtk, OnWarningEvent);
            base.ClearTask();
        }

        /// <summary>
        /// 收到攻击事件
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="skillID"></param>
        /// <param name="targetID"></param>
        private void OnWarningEvent(UInt64 ownerId, uint skillID, UInt64 targetID)
        {
            if (mIsTaskCoolDown)
            {
                return;
            }
            if (PlayerManager.Instance.LocalPlayer.GameObjGUID != targetID)
            {
                return;
            }
            //Ientity ownerEntity;
            IEntity targetEntity;
            if (EntityManager.AllEntitys.TryGetValue(ownerId, out targetEntity))
            {
                if (!CheckIfAttackerIdBuilding(targetEntity))
                {
                    return;
                }
                mIsTaskCoolDown = true;
                mTaskTime = Time.realtimeSinceStartup;
                EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_AdvancedGuideShowTip, this.mTaskId);
            }
        }

        /// <summary>
        /// 检测攻击者是是否是建筑
        /// </summary>
        /// <param name="attacker"></param>
        /// <returns></returns>
        private bool CheckIfAttackerIdBuilding(IEntity attacker)
        {
            if (attacker.realObject == null)
            {
                return false;
            }
            if (attacker.entityType == EntityTypeEnum.Building)
            {
                return true;
            }
            return false;
        }

    }


}
