using System;
using System.Collections.Generic;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Tools;
using System.Linq;
using Thanos.GameData;
using Thanos.Resource;

namespace Thanos.GuideDate
{
    public class BuildingAtkWarning 
    {        
        private bool isInWarning = false;

        private float showTime = 6f;

        private float totalTime = 30f;

        private DateTime startTime;

        private IEntity target = null;

        private GameObject objEffect = null;

        private string AudioPath = "/Audio/UIAudio/JTsj"; 

        public IEntity GetTarget() {
            return target;
        }

        public BuildingAtkWarning() {
            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_NotifyBuildingDes, OnBuildingDead);
        }

        ~BuildingAtkWarning() {
            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_NotifyBuildingDes, OnBuildingDead);
        }

        //当建筑销毁时
        private void OnBuildingDead(IEntity entity)
        {
            if (entity == target)
            {
                isInWarning = false;
                BuildingAtkWarningManager.Instance.RemoveWarning(this);//移除警告
            }
        }

        public void OnUpdate()
        {
            if (!isInWarning)
                return;
            TimeSpan span = DateTime.Now - startTime;
            if (span.TotalSeconds > showTime && objEffect != null && objEffect.activeInHierarchy) {
                objEffect.SetActive(false);
            }
            if (span.TotalSeconds > totalTime) {
                isInWarning = false;
                BuildingAtkWarningManager.Instance.RemoveWarning(this);
            }
        } 

        public void CreateCircleRes(IEntity entity)
        {
            target = entity;
            string path = GameConstDefine.LoadGameOtherEffectPath;            
            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(path + "guangquan_jianta", ResourceType.PREFAB);
            GameObject obj = GameObject.Instantiate(objUnit.Asset) as GameObject;
            obj.transform.parent = entity.realObject.transform;
            obj.transform.position = entity.realObject.transform.position + new Vector3(0.0f, 0.2f, 0.0f);
            obj.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            int skillID = ConfigReader.NpcXmlInfoDict[entity.NpcGUIDType].NpcSkillType1;
            float range = ConfigReader.GetSkillManagerCfg(skillID).range;
            float rate = 1f / 16f;
            obj.transform.localScale = new Vector3(range * rate / entity.RealEntity.transform.localScale.x, 1.0f, range * rate / entity.RealEntity.transform.localScale.z);
            startTime = DateTime.Now;            
            objEffect = obj;
            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioPath, ResourceType.PREFAB);
            AudioClip clip = clipUnit.Asset as AudioClip;
            AudioManager.Instance.PlayEffectAudio(clip);
            isInWarning = true;         
        }

        public void Clean() {
            if(objEffect != null){
                GameObject.DestroyImmediate(objEffect);
            }
            objEffect = null;
            isInWarning = false;
            target = null;
        }
    }
}
