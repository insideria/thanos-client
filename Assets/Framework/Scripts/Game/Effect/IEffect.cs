using UnityEngine;
using System;
using System.Collections.Generic;
using Thanos.Resource;

namespace Thanos.Effect
{
    public class IEffect
    {
        //特效类型
        public enum ESkillEffectType
        {
            eET_Passive,
            eET_Buff,
            eET_BeAttack,
            eET_FlyEffect,
            eET_Normal,
            eET_Area,
            eET_Link,
        }
        //基本信息
        public GameObject obj = null;           //特效GameObject物体
        public Transform mTransform = null;

        protected float currentTime = 0.0f;     //特效运行时间
        public bool isDead = false;		        //特效是否死亡
        public string resPath;                  //特效资源路径        
        public string templateName;             //特效模板名称
        public Int64 projectID = 0;             //特效id  分为服务器创建id 和本地生成id        
        public uint skillID;                    //特效对应的技能id

        public float cehuaTime = 0.0f;         //特效运动持续时间或者是特效基于外部设置的时间    策划配置      
        public float artTime = 0.0f;            //美术设置的特效时间                            美术配置

        public float lifeTime = 0;              //特效生命周期, 0为无限生命周期

        public UInt64 enOwnerKey;            //技能释放者
        public UInt64 enTargetKey;           //技能受击者        
        public AudioSource mAudioSource = null; //声音


        //运动信息
        public Vector3 fPosition;
        public Vector3 fixPosition;
        public Vector3 dir;
        public Vector3 distance;
        public ESkillEffectType mType;

        //创建时保存所有的ParticleSystem
        private List<ParticleSystem> particleSystemList = new List<ParticleSystem>();
        //创建时有TrailRender保存所有的TrailRender
        private List<TrailRenderer> trailRenderList = new List<TrailRenderer>();

        private EffectScript effectScript = null;

        public IEffect()
        {
            enOwnerKey = 0;
            enTargetKey = 0;
        }


        //获取Transform
        public Transform GetTransform()
        {
            if (mTransform == null)
            {
                mTransform = obj.transform;
            }
            return mTransform;
        }


        //特效创建接口
        public void Create()
        {
            //创建的时候检查特效projectId,服务器没有设置生成本地id
            CheckProjectId();
            //获取特效模板名称
            templateName = ResourceCommon.getResourceName(resPath);

            //使用特效缓存机制           
            obj = ObjectPool.Instance.GetGO(resPath);

            if (null == obj)
            {
                Debugger.LogError("load effect object failed in IEffect::Create" + resPath);
                return;
            }

            //创建完成,修改特效名称，便于调试
            obj.name = templateName + "_" + projectID.ToString();

            OnLoadComplete();

            //获取美术特效脚本信息                
            effectScript = obj.GetComponent<EffectScript>();
            if (effectScript == null)
            {
                Debugger.LogError("cant not find the effect script in " + resPath);
                return;
            }
            artTime = effectScript.lifeTime;

            //美术配置时间为0,使用策划时间
            if (effectScript.lifeTime == 0)
                lifeTime = cehuaTime;
            //否则使用美术时间
            else
                lifeTime = artTime;

            //特效等级不同，重新设置
            EffectLodLevel effectLevel = effectScript.lodLevel;
            EffectLodLevel curLevel = EffectManager.Instance.mLodLevel;

            if (effectLevel != curLevel)
            {
                //调整特效显示等级
                AdjustEffectLodLevel(curLevel);
            }
        }

        public void Release()
        {
            ObjectPool.Instance.ReleaseGO(resPath, obj, PoolObjectTypeEnum.Effect);

            //如果有声音，释放声音
            if (mAudioSource != null)
            {
                SceneSoundManager.Instance.remove(mAudioSource);
            }
        }

        //调整特效等级
        public void AdjustEffectLodLevel(EffectLodLevel level)
        {
            List<GameObject> childObjs = new List<GameObject>();
            FindAllChildWithTag(obj, "loweffect", ref childObjs);

            //高效果
            if (level == EffectLodLevel.High)
            {
                foreach (GameObject child in childObjs)
                {
                    child.SetActive(true);
                }
            }
            //低效果
            else
            {
                foreach (GameObject child in childObjs)
                {
                    child.SetActive(false);
                }
            }

            //保存特效等级信息
            effectScript.lodLevel = level;
        }

        //检查projectid,为0表示没有设置，生成本地id
        public void CheckProjectId()
        {
            if (projectID == 0)
                projectID = EffectManager.Instance.GetLocalId();
        }

        //特效更新接口
        public virtual void Update()
        {
            currentTime += Time.deltaTime;

            //如果是非循环特效并且当前时间大于生命周期，特效死亡
            if (lifeTime != 0 && currentTime > lifeTime)
                isDead = true;
        }

        //特效是否死亡
        public virtual bool IsDead()
        {
            return isDead;
        }

        #region 重载函数
        public virtual void OnLoadComplete()
        {
        }

        //遍历收集特效预制件中的信息
        public void RecusionParticleSystemObject(GameObject gameObject)
        {
            //获取子ParticleSystem        
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform ts = gameObject.transform.GetChild(i);
                GameObject psObj = ts.gameObject;
                if (psObj)
                {
                    //保存ParticleSystem
                    ParticleSystem psComp = psObj.GetComponent<ParticleSystem>();
                    if (psComp != null)
                    {
                        particleSystemList.Add(psComp);
                    }
                    //保存TrailRender
                    // TrailRenderer
                    // RecusionParticleSystemObject(psObj);
                }
            }
        }

        //获取所有指定Tag的子物体
        public void FindAllChildWithTag(GameObject gameObject, string tag, ref List<GameObject> childObjs)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform ts = gameObject.transform.GetChild(i);
                GameObject psObj = ts.gameObject;
                if (psObj)
                {
                    if (psObj.tag == tag)
                    {
                        childObjs.Add(psObj);
                        continue;
                    }

                    FindAllChildWithTag(psObj, tag, ref childObjs);
                }
            }
        }

        #endregion
    }
}

