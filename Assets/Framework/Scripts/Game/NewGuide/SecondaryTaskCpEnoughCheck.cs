using System;
using System.Collections.Generic;
using UnityEngine; 
using System.Linq; 
namespace Thanos.GuideDate
{
    public class SecondaryTaskCpEnoughCheck : SecondaryTaskCheckBase
    {
        const float goalCp = 1500f; 

        public override void OnEnter(SecondaryTaskInfo parent)
        {
            base.OnEnter(parent);
            AddCheckListener();
        }

        void OnEvent(FEvent eve)
        {             
            int cp = (int)eve.GetParam("Cp");
            if (cp >= goalCp)
            { 
                GuideHelpData data = ConfigReader.GetGuideHelpInfo(parentInfo.GetTaskId());
                SecondaryGuideManager.Instance.SendTaskStartTag(data);
            }
        }

        public override void AddCheckListener()
        {
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyChangeCp, OnEvent);             
        }

        public override void RemoveAddListener()
        {
            if (EventCenter.mEventTable.ContainsKey((Int32)GameEventEnum.GameEvent_NotifyChangeCp))
            {
                EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyChangeCp, OnEvent);
            }
        }

        public override void OnEnd()
        { 
            base.OnEnd();
            RemoveAddListener();
        }

    }
}
