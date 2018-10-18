using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using GameDefine;

namespace Thanos.GuideDate
{
    public class GuideTaskBase  
    { 
        public int mTaskId
        {
            private set;
            get;
        }

        protected GameObject mRoot;

        protected bool isFinish = false;

        public GuideTaskType taskType;

        protected bool mIsTaskCoolDown;
        protected float mTaskCDtime;
        protected float mTaskTime;

        public GuideTaskBase(int id, GuideTaskType type, GameObject mParent)
        {
            mTaskId = id;
            taskType = type;
            mRoot = mParent;
        }

        public bool IsFinish() 
        {
            return isFinish;
        }

        /// <summary>
        /// 进入任务
        /// </summary>
        public virtual void EnterTask() 
        { 

        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public virtual void ExcuseTask() 
        {
            if (!mIsTaskCoolDown)
            {
                return;
            }
            if (Time.realtimeSinceStartup - mTaskTime >= mTaskCDtime)
            {
                mIsTaskCoolDown = false;
            }
        }

        //任务结束广播消息
        public virtual void FinishTask() 
        {
            if (isFinish)
            {
                return;
            }
            isFinish = true;
            EventCenter.Broadcast<GuideTaskType, int>((Int32)GameEventEnum.GameEvent_PlayChildTaskFinish, this.taskType, this.mTaskId);
        }

        //清理任务
        public virtual void ClearTask() 
        { 
            isFinish = true;
        }
    }


}
