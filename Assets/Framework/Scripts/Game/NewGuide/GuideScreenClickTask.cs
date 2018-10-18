using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
namespace Thanos.GuideDate
{
    public class GuideScreenClickTask : GuideTaskBase
    {
      
        public GuideScreenClickTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {
           
        }

        /// <summary>
        /// 到时广播消息
        /// </summary>
        public override void EnterTask()
        {
            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_LockTarget, OnLockTarget);
        }


        public override void ExcuseTask()
        {

        }

        private void OnLockTarget(IEntity mEntity)
        {
            this.FinishTask();
        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_LockTarget, OnLockTarget);
           
        }
 
    }


}
