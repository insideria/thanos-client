using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
namespace Thanos.GuideDate
{
    public class GuideAbsorbTask : GuideTaskBase
    {
        CAbsorbTask absorbTask = null;

        public GuideAbsorbTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {
            //读取数据
            absorbTask = ConfigReader.GetAbsorbTaskInfo(task);
            if (absorbTask == null)
            {
                Debug.LogError("GuideAbsorbTask 找不到任務 Id" + task);
            }
        }

        /// <summary>
        /// 到时广播消息
        /// </summary>
        public override void EnterTask()
        {
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, GetAbsorbEvent);
        }


        public override void ExcuseTask()
        {

        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, GetAbsorbEvent);
            base.ClearTask();
        }

        private void GetAbsorbEvent(int objId)
        {
            if (objId == 0)
            {
                return;
            }
            base.FinishTask();
        }

    }


}
