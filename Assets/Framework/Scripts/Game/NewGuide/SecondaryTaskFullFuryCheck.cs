using System;
using System.Collections.Generic;
using UnityEngine; 
using System.Linq;
using GameDefine;
namespace Thanos.GuideDate
{
    public class SecondaryTaskFullFuryCheck : SecondaryTaskCheckBase
    {
        EFuryState curState = EFuryState.eFuryNullState;
        
        
        public override void OnEnter(SecondaryTaskInfo parent)
        {
            base.OnEnter(parent);
            AddCheckListener();
        } 

        void OnEvent(FEvent eve)
        {
            EFuryState state = (EFuryState)eve.GetParam("State");
            GuideHelpData data = ConfigReader.GetGuideHelpInfo(parentInfo.GetTaskId()); 
            if (state == EFuryState.eFuryFullState && curState != EFuryState.eFuryFullState)
            {              
                SecondaryGuideManager.Instance.SendTaskStartTag(data);
            }
            else
            { 
                SecondaryGuideManager.Instance.SendTaskEndTag(data); 
            }
            curState = state;
        }

        public override void AddCheckListener()
        {            
            //EventCenter.AddListener<CEvent>((Int32)GameEventEnum.GameEvent_NotifySelfPlayerFuryStateChange, OnEvent);             
        }

        public override void RemoveAddListener()
        {
            //if (EventCenter.mEventTable.ContainsKey((Int32)GameEventEnum.GameEvent_NotifySelfPlayerFuryStateChange))
            //{
            //    EventCenter.RemoveListener<CEvent>((Int32)GameEventEnum.GameEvent_NotifySelfPlayerFuryStateChange, OnEvent);
            //}
        }

        public override void OnEnd()
        { 
            base.OnEnd();
            RemoveAddListener();
        }

    }
}
