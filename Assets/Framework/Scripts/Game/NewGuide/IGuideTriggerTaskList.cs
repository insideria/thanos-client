using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
using System.Linq;
namespace Thanos.GuideDate
{
    public class IGuideTriggerTaskList:IGuideTaskList
    {

        public IGuideTriggerTaskList(int id) : base(id) {
           
        }

        protected override void SetTaskData(int id)
        {
            taskId = id; 
            taskData = ConfigReader.GetITriggerGuideManagerInfo(taskId);
        }

        protected override void OnEnd()
        {
            IGuideTaskManager.Instance().RemoveTriggerTask(this);
            // total task end;     
            IGuideTaskManager.Instance().AddHasTriggerTask(taskId);

            HolyGameLogic.Instance.EmsgTocsAskFinishUIGuideTask(2, taskId, 1); 
        }

        protected override void OnInterfaceTrigger(FEvent eve)
        {
            if (curState != TaskState.InActiveState)
                return; 
            count = GetFirstNoTrigerTaskId(); 
            StartTask(count);
            SetTaskState(TaskState.OpenState);
        }

        protected override void StartTask(int index) {
            base.StartTask(index); 
            IGuideData data = ConfigReader.GetIGuideInfo(currentTask.GetTaskId());
            IGuideTaskManager.Instance().SendTaskEffectShow(data.EndTaskEvent);
            IGuideTaskManager.Instance().AddHasTriggerTask(data.TaskId); 
            HolyGameLogic.Instance.EmsgTocsAskFinishUIGuideTask(2, taskId, 0);
        }

        private int GetFirstNoTrigerTaskId() {
            for (int i = 0; i < taskData.SonTaskList.Count; i++) {
                int id = taskData.SonTaskList.ElementAt(i);
                if (IGuideTaskManager.Instance().IsTaskTriggered(id)) {
                    continue;
                }
                return i;
            }
            return -1;
        }
    }
}
