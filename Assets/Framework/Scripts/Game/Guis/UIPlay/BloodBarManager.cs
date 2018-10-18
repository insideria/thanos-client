using UnityEngine;
using System;
using Thanos.GameEntity;
using Thanos.GuideDate;

public enum XueTiaoType{
	PlayerTypeRed,
	PlayerTypeBlue,
	SoldierTypeRed,
	SoldierTypeBlue,
	BuildingTypeRed,
	BuildingTypeBlue,
	Monster,
}

public class BloodBarManager: MonoBehaviour 
{ 
	
	#region 血条路径
    private static string pathPlayerXuetiao = "HeroLifePlate{0}";
    private static string pathBuildingXuetiao = "TowerLifePlate{0}";
    private static string pathSummonedXuetiao = "Summoned{0}";
    private static string pathSoldierXuetiao = "SoldierLifePlate{0}";
    public static string pathMonster = "CreepsLifePlate";

    public static string xueTiaoName = "XueTiao_";
	#endregion


	public static BloodBarManager Instance{
		private set;
		get;
	}

	void OnEnable(){
		if(Instance == null){
			Instance = this;
		}
	}

	private string GetXueTiaoPrefabPath(IEntity entity){
        string XuetiaoColor = "Green";

        if (PlayerManager.Instance.LocalPlayer == null || PlayerManager.Instance.LocalPlayer.realObject == null)
        {
            int campEntity = (int)entity.EntityCamp % 2;
            int playerCamp = (int)PlayerManager.Instance.LocalAccount.GameUserSeat % 2;
            if (playerCamp != campEntity) {
                XuetiaoColor = "Red";
            }
        }
        else if (entity.EntityCamp == EntityCampTypeEnum.Bad || (PlayerManager.Instance.LocalPlayer != null && entity.EntityCamp != PlayerManager.Instance.LocalPlayer.EntityCamp))
        {            
            XuetiaoColor = "Red";
        }
        
        if (PlayerManager.Instance.LocalAccount.ObjType == GameDefine.ObPlayerOrPlayer.PlayerObType && entity.EntityCamp != PlayerManager.Instance.LocalAccount.EntityCamp)
        {
            XuetiaoColor = "Red";
        }
        string path = "";
        switch (entity.entityType)
        { 
            case EntityTypeEnum.Building:
                path = String.Format(pathBuildingXuetiao, XuetiaoColor);
                break;
            case EntityTypeEnum.Player:
                path = String.Format(pathPlayerXuetiao, XuetiaoColor);
                break;
            case EntityTypeEnum.Soldier:
            case EntityTypeEnum.AltarSoldier:
                path = String.Format(pathSoldierXuetiao, XuetiaoColor);
                //Debug.LogError("path = " + path);
                break;
            case EntityTypeEnum.Monster:
                path = pathMonster;
                break;
        }
        if (entity.NpcGUIDType == 21017 || entity.NpcGUIDType == 21025 || entity.NpcGUIDType == 21024)
        { //新手引导假的英雄
            path = String.Format(pathPlayerXuetiao, XuetiaoColor);
            SceneGuideTaskManager.Instance().AddFakeHero(entity);
        }
        else if (entity.NPCCateChild == NPCCateChildEnum.BUILD_Summon)
        {
            path = String.Format(pathSummonedXuetiao, XuetiaoColor);
        }
        return GameDefine.GameConstDefine.GuisPlay + path;
	}

	public BloodBarUI CreateXueTiao(IEntity entity){
        string path = GetXueTiaoPrefabPath(entity);
		BloodBarUI xueTiao = LoadXueTiaoPrefab(entity,path);
		xueTiao.Init(entity);
		xueTiao.ResetBloodBarValue();
		xueTiao.UpdatePosition(entity.realObject.transform);
		xueTiao.gameObject.transform.parent = transform;
        xueTiao.transform.localScale = Vector3.one;
		xueTiao.gameObject.transform.name = xueTiaoName + entity.ModelName;
		return xueTiao;
	}

	public BloodBarUI LoadXueTiaoPrefab(IEntity entity,string path)
    {        
        //ResourceItem pathLoadUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.PREFAB);
        //GameObject pathLoad = pathLoadUnit.Asset as GameObject;
        GameObject obj = ObjectPool.Instance.GetGO(path);

//         if (pathLoad == null)
//             Debug.LogError("error Not found  BloodBarUI =  null");
//         GameObject obj = GameObject.Instantiate(pathLoad) as GameObject;
        if (obj == null)
            Debug.LogError("obj = null");
		BloodBarUI xueTiao = obj.GetComponent<BloodBarUI>();
        xueTiao.mResName = path;

		return xueTiao;
	} 
}
