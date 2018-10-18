using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
using System.Linq;
using Thanos.GameData;
namespace Thanos.GuideDate
{
    public class GuideBuildingTips : Singleton<GuideBuildingTips>
    {
        private Dictionary<int, IEntity> buildingDic = new Dictionary<int, IEntity>();
        public int[] npcIdArray = { 21021, 21019, 21023, 21022, 21018, 21020 };
        public string[] pathArray = { "Guide/Tips_wenz", "Guide/Tips_wenz2", "Guide/Tips_wenz1", "Guide/Tips_wenz3"
                                    ,"Guide/Tips_wenz4","Guide/Tips_wenz5"};
		public int[] tipHeight = {5,5,5,5,5,5};
        public void AddBuildingTips(IEntity entity) {
            if (SceneGuideTaskManager.Instance().IsNewsGuide() == SceneGuideTaskManager.SceneGuideType.NoGuide)
                return;
            if (!npcIdArray.Contains(entity.NpcGUIDType))
                return;
            if (buildingDic.ContainsKey(entity.NpcGUIDType))
            {
                buildingDic[entity.NpcGUIDType] = entity;
            }
            else {
                buildingDic.Add(entity.NpcGUIDType, entity);
            }
        }

        public Dictionary<int, IEntity> GetTipTargetDic() {
            return buildingDic;
        }
    }
}
