using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
using System.Linq;
using Thanos.Model;

namespace Thanos.GuideDate
{
    public class IGuideMidMatchTip : Singleton<IGuideMidMatchTip>
    {
        private const string path = "Guide/TPS1"; 
        private const float timeLimit = 5f;
        GameObject objTip = null; 
        int count = 0;
        public void RegisterListerner()
        {
            if ((MAPTYPE)GameUserModel.Instance.GameMapID == MAPTYPE.MIDDLEMAP)
            {
                EventCenter.AddListener<long>((Int32)GameEventEnum.GameEvent_BattleTimeDownEnter, OnEvent);
                EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ScenseChange, ChangeScense);
            }
        }   
        public void OnUpdate() {
           if (objTip != null) {
                TimeSpan span = GameTimeData.Instance.GetGameTime(); 
                if (span.TotalSeconds >= timeLimit) { 
                    LoadUiResource.DestroyLoad(path);
                    objTip = null;
                }
            }
        }
        void OnEvent(long i) {
            EventCenter.RemoveListener<long>((Int32)GameEventEnum.GameEvent_BattleTimeDownEnter, OnEvent);    
            TimeSpan span = GameTimeData.Instance.GetGameTime();

            if (span.TotalSeconds < timeLimit)
            {
                objTip = LoadUiResource.LoadRes(GameMethod.GetUiCamera.transform, path);
            }
        }

        void ChangeScense() {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ScenseChange, ChangeScense);
            if (objTip != null)
            {
                LoadUiResource.DestroyLoad(path);
            }
        }
    }
}
