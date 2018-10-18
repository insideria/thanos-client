using System;
using System.Collections.Generic;

namespace Thanos.Ctrl
{
    public class CTask
    {
        public uint mGuid;//唯一编号
        public uint mCurCount;//当前进度
        public STaskConfig mConfig;//基本配置
    }

    public class DailyBonusCtrl : Singleton<DailyBonusCtrl>
    {
        public uint mMonth = 0;
        public uint mToday = 0;
        public uint mMaxDay = 0;
        public uint mHadDay = 0;
        public bool mTodayCan = false;
        public Dictionary<uint, CTask> mDailyTaskDic = new Dictionary<uint,CTask>();//逻辑层每日任务数据
        public Dictionary<uint, CTask> mInfiniteTaskDic = new Dictionary<uint,CTask>();//逻辑层永久任务数据
        public bool mIsHadNewDailyTask = false;
        public bool mIsHadTaskFinished = false;

        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_DailyBonusEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_DailyBonusExit);
        }

        public void SetDayAwards(uint month, uint today, uint maxdays, uint haddays, bool isTodayCan)
        {
            mMonth = month;
            mToday = today;
            mMaxDay = maxdays;
            mHadDay = haddays;
            mTodayCan = isTodayCan;
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_DailyBonusUpdate);
        }

        public void AskGetAwards()
        {
            HolyGameLogic.Instance.AskGetLoginReWard();
        }

    }

}
