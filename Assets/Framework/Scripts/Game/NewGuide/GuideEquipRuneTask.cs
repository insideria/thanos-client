using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameDefine;

using Thanos.Ctrl;

namespace Thanos.GuideDate
{

    ////////////////////////////////////////////////////////////////////////  装备符文 引导////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////  装备符文 引导////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////  装备符文 引导////////////////////////////////////////////////////////////////

    public class GuideEquipRuneTask : GuideTaskBase
    {
        public GuideEquipRuneTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }

        public override void EnterTask()
        {
            EventCenter.AddListener<uint, int, int>((Int32)GameEventEnum.GameEvent_RuneQuipUpdate, EquipRuneEvent);
        }

        public override void ExcuseTask()
        {

        }

        public override void ClearTask()
        {
            EventCenter.AddListener<uint, int, int>((Int32)GameEventEnum.GameEvent_RuneQuipUpdate, EquipRuneEvent);
            base.ClearTask();
        }

        private void EquipRuneEvent(uint runeid, int sPage, int slotPos)
        {
            this.FinishTask();
        }

    }


}
