using System;
using System.Collections.Generic;
using UnityEngine; 
using System.Linq;
using GameDefine;
namespace Thanos.GuideDate
{
    public class SecondaryTaskOpenShopCheck : SecondaryTaskCheckBase
    {
         
        public override void OnEnter(SecondaryTaskInfo parent)
        {
            base.OnEnter(parent);
            AddCheckListener();
        } 

        void OnEvent(FEvent eve)
        {
            bool tag = (bool)eve.GetParam("Tag");
            GuideHelpData data = ConfigReader.GetGuideHelpInfo(parentInfo.GetTaskId());       
            if(tag){
                SecondaryGuideManager.Instance.SendTaskStartTag(data);
            }
            else {
                SecondaryGuideManager.Instance.SendTaskEndTag(data); 
            } 
        }

        public override void AddCheckListener()
        {            
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyOpenShop, OnEvent);             
        }

        public override void RemoveAddListener()
        {
            if (EventCenter.mEventTable.ContainsKey((Int32)GameEventEnum.GameEvent_NotifyOpenShop))
            {
                EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.GameEvent_NotifyOpenShop, OnEvent);
            }
        }

        public override void OnEnd()
        { 
            base.OnEnd();
            RemoveAddListener();
        }

    }
}
