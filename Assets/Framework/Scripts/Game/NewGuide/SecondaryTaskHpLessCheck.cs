using System;
using System.Collections.Generic;
using UnityEngine; 
using System.Linq; 
namespace Thanos.GuideDate
{
    public class SecondaryTaskHpLessCheck : SecondaryTaskCheckBase
    { 
        public override void OnEnter(SecondaryTaskInfo parent)
        {
            base.OnEnter(parent);
            AddCheckListener();
        } 

        void OnEvent(FEvent eve)
        {  
            GuideHelpData data = ConfigReader.GetGuideHelpInfo(parentInfo.GetTaskId());
            bool tag = (bool)eve.GetParam("Tag");
            if (tag)
            {  
                SecondaryGuideManager.Instance.SendTaskStartTag(data);
            }
            else { 
                SecondaryGuideManager.Instance.SendTaskEndTag(data); 
            }
        }

        public override void AddCheckListener()
        {
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyHpLessWarning, OnEvent);
        }

        public override void RemoveAddListener()
        {
            if (EventCenter.mEventTable.ContainsKey((Int32)GameEventEnum.GameEvent_NotifyHpLessWarning))
            {
                EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyHpLessWarning, OnEvent);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            RemoveAddListener();
        }

    }
}
