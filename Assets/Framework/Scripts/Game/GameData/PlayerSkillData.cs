using UnityEngine;
using System.Collections;
using Thanos.Tools;
using Thanos.GameEntity;
using GameDefine;

namespace Thanos.GameData
{
    public class PlayerSkillData
    {
        private static PlayerSkillData instance = null;
        public static PlayerSkillData Instance {
            get {
                if (instance == null)
                {
                    instance = new PlayerSkillData();
                }
                return instance;
            }
        }
        public IPlayer Player
        {
            private set;
            get;
        }

        public int Slot{
            private set;
            get;
        }
        public int SkillId{
            private set;
            get;
        }
        public float TotalTime
        {
            private set;
            get;
        }
        public EFuryState FuryState
        {
            private set;
            get;
        }
        public int Slot1
        {
            private set;
            get;
        }
        public int SkillId1
        {
            private set;
            get;
        }
        public void SetPlayerSkillInfo(IPlayer player, int slot, int skillId)
        {
            Player = player;
            Slot = slot;
            SkillId = skillId;
        }
        public void SetPlayerAbsSkillInfo(IPlayer player, int slot, int skillId, int slot1 ,int skillId1)
        {
            Player = player;
            Slot = slot;
            SkillId = skillId;
            Slot1 = slot1;
            SkillId1 = skillId1;
        }

        public void SetPlayerCdTime(IPlayer player, int skillId,float totalTime)
        {
            Player = player;
            SkillId = skillId;
            TotalTime = totalTime;
        }
        public void SetPlayerFuryState(IPlayer player, EFuryState state)
        {
            Player = player;
            FuryState = state;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
