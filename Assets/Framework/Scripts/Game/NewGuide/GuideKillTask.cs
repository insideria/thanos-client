using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using GameDefine;
using Thanos.Tools;
using Thanos.GameEntity;

namespace Thanos.GuideDate

{
    public class GuideKillTask : GuideTaskBase
    {
        private int killTimes = 0; 
        private CKillTask killTask = null;
        private bool CheckIsAllMeet(UInt64 kill,int deadType,int deadId,int reason) {
            if (!CheckDeadIsAsignedId(deadId))
                return false;
            if (!CheckDeadIsAsignedReason(reason))
                return false;
            if (!CheckDeadIsAsignedType((DeadType)deadType))
                return false;
            return true;
        }

        /// <summary>
        /// 检测死亡原因
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        private bool CheckDeadIsAsignedReason(int reason) {
            if (killTask.KillReason.Contains(-1))
            {
                return true;
            }
            else if (killTask.KillReason.Contains(reason))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测野怪和小兵的npcId
        /// </summary>
        /// <param name="dead"></param>
        /// <returns></returns>
        private bool CheckDeadIsAsignedId(int deadId) {
            if (killTask.DeadId == -1) {
                return true;
            }
            else {
                if (deadId == killTask.DeadId)
                    return true;
            } 
            return false;
        }

        /// <summary>
        /// 检测死亡的类型
        /// </summary>
        /// <param name="dead"></param>
        /// <returns></returns>
        private bool CheckDeadIsAsignedType(DeadType deadType) {

            if (killTask.DeadType == deadType)
            {
                    return true;
            }
            return false;
        }

        public GuideKillTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {
            //读取数据
            killTask = ConfigReader.GetKillTaskInfo(task);
            if (killTask == null)
            {
                //this.FinishTask();
                Debug.LogError("GuideKillTask 找不到任務 Id" + task);
            }
        }

        public override void EnterTask()
        { 
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_GuideKillTask, GetKillEvent);
            //if()
        }

        public override void ExcuseTask()
        {

        }

        public override void ClearTask() 
        {
            if (EventCenter.mEventTable.ContainsKey((Int32)GameEventEnum.GameEvent_GuideKillTask))
            {
                EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_GuideKillTask, GetKillEvent);
            }
            base.ClearTask();
        }


        private int mKillCount;
        private void GetKillEvent(int mType)
        {
            if (killTask.DeadId == mType)
            {
                mKillCount++;
                if (mKillCount >= killTask.DeadTimes)
                {
                    this.FinishTask();
                }
            }
        }


    }


}
