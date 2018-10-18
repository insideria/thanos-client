using System.Collections.Generic;
using Thanos.GameEntity;
using System;
using Thanos.Model;

namespace Thanos.Ctrl
{
    public class ScoreCtrl : Singleton<ScoreCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ScoreEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ScoreExit);
        }

        public void AskBackRoom()
        {
            ScoreCtrl.Instance.Exit();
            HolyGameLogic.Instance.EmsgToss_AskReEnterRoom();
        }

        public long mGotGole;
        public int mExp;
        public int mGotExp;
        public int mLevel;
        public int mGotLevel;
        public int mUpLevelExt;//升级后的经验

        public void SetScoreInfo(long gotGole, int oldExp, int newExp,short oldLevel, short newLevel,int upLevelExp)
        {
            mGotGole = gotGole;
            mExp = oldExp;//上次经验
            mGotExp = newExp;//当前获得经验
            mLevel = oldLevel;//上次等级
            mGotLevel = newLevel;//最新等级
            mUpLevelExt = upLevelExp;
            //Debug.LogError(gotGole + " oldExp = " + oldExp + " newExp = " + newExp + "  oldLevel = " + oldLevel + " newLevel = " + newLevel + " upLevelExp = " + upLevelExp);
            GameUserModel.Instance.SetGameUserLv((short)mGotLevel);//设置升级后等级和经验
            GameUserModel.Instance.SetGameUserExp(mUpLevelExt);
        }

        //showInfo
        public void SetSettlementInfo(UInt64 objGUID, int headID, string nickName, int kills, int death, int asstimes, int level,int totalcp, int lasthit, EntityCampTypeEnum camp)
        {
            SettlementData ssd = new SettlementData();
            ssd.Init(objGUID, headID, nickName, level, kills, death, asstimes, totalcp, lasthit, camp);
            mListMentDate.Add(ssd);
        }
        
        public List<SettlementData> GetSettlementList()
        {
            return mListMentDate;
        }

        private List<SettlementData> mListMentDate = new List<SettlementData>();

        public class SettlementData
        {
            public UInt64 mObjGUID;
            public int mSeat;
            public int mHeadID;
            public string mNickName;
            public int mLevel, mKills, mDeath, mAsstimes, mTotalcp, mLasthit;
            public EntityCampTypeEnum mCamp;

            public void Init(UInt64 objGUID, int headID, string nickName, int level, int kills, int death, int asstimes, int totalcp, int lasthit, EntityCampTypeEnum camp)
            {
                mObjGUID = objGUID;
                mHeadID = headID;
                mNickName = nickName;
                mLevel = level;
                mKills = kills;
                mDeath = death;
                mAsstimes = asstimes;
                mTotalcp = totalcp;
                mLasthit = lasthit;
                mCamp = camp;
            }
        }
    }
}
