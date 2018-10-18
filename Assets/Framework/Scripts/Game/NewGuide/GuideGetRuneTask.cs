using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameDefine;

using Thanos.Ctrl;

namespace Thanos.GuideDate
{
    public class GuideGetRuneTask : GuideTaskBase
    {
        public GuideGetRuneTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            EventCenter.AddListener<uint, int, long>((Int32)GameEventEnum.GameEvent_RuneBagUpdate, GetRuneEvent);
        }

        public override void ExcuseTask()
        {

        }

        public override void ClearTask()
        {
            EventCenter.RemoveListener<uint, int, long>((Int32)GameEventEnum.GameEvent_RuneBagUpdate, GetRuneEvent);
            base.ClearTask();
        }

        private void GetRuneEvent(uint runeId, int num, long gottime)
        {
            this.FinishTask();
        }

    }


}
