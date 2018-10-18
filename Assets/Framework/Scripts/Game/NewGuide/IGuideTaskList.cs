using System.Collections.Generic;
using System.Linq;

namespace Thanos.GuideDate
{
    public class IGuideTaskList
    {
        public enum TaskState
        {
            InActiveState = 0,
            OpenState,
            CloseState,
        }

        protected int taskId = -1;

        protected IGuideManagerData taskData = null;

        protected TaskState curState = TaskState.InActiveState;

        private List<IGuideTask> sonTaskList = new List<IGuideTask>();

        protected IGuideTask currentTask = null;

        protected int count = 0; 

        public void OnEnter()
        {
            EventCenter.AddListener<FEvent>(taskData.TriggerInterface, OnInterfaceTrigger);
            for (int i = 0; i < taskData.SonTaskList.Count; i++) {
                IGuideTask task = new IGuideTask(this,taskData.SonTaskList.ElementAt(i));
                sonTaskList.Add(task);
            }
        }

        protected void SetTaskState(TaskState state)
        {
            curState = state;
        }

        public void OnExecute()
        {
            if (curState != TaskState.OpenState || currentTask == null)
                return;
            currentTask.OnExecute();
        }

        public void CheckNextTask() {
            if (curState != TaskState.OpenState || currentTask == null)
                return;

            if (count == sonTaskList.Count)
            {
                OnEnd();
            }
            else {
                StartTask(count);
            }
        } 

        protected virtual void OnEnd()
        {
            IGuideTaskManager.Instance().NextTask();
        }

        public void Clean() {
            currentTask = null;
            SetTaskState(TaskState.CloseState);
            EventCenter.RemoveListener<FEvent>(taskData.TriggerInterface, OnInterfaceTrigger);
        }

        public IGuideTaskList(int id)
        {         
            SetTaskData(id);           
        }

        protected virtual void SetTaskData(int id) {
            taskId = id;
            taskData = ConfigReader.GetIGuideManagerInfo(taskId);
        }
        public IGuideManagerData GetIGuideTaskData()
        {
            return taskData;
        }

        public int GetTaskId() {
            return taskId;
        }
        
        protected virtual void OnInterfaceTrigger(FEvent eve)
        { 
            if (curState != TaskState.InActiveState)
                return; 
            TriggerTask();
        }


        void TriggerTask() {
            IGuideTaskManager.Instance().SetCurTask(this);
            count = 0; 
            StartTask(count);
            SetTaskState(TaskState.OpenState);
        }

        protected virtual void StartTask(int index) {
            currentTask = null;
            count += 1;
            currentTask = sonTaskList.ElementAt(index);
            currentTask.OnEnter(); 
        } 
    }
}
