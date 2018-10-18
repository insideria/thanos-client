using System;
using GameDefine;
using UnityEngine;
using Thanos.GameEntity;
using System.Collections.Generic;
using Thanos.Resource;

[Serializable]
public class EntityData
{
    public float Hp;//血量
    public float HpMax;//最大血量
    public float Mp;//魔法值
    public float MpMax;//最大魔法值
    public float Speed;//速度
    public float PhyAtk;//物理攻击
    public float MagAtk;//魔法攻击
    public float PhyDef;//物理防御
    public float MagDef;//魔法防御
    public float AtkSpeed;//攻击速度
    public float AtkDis;//攻击距离
    public float HpRecover;//血量恢复
    public float MpRecover;//魔法值恢复
    public float RebornTime;//重生时间

    public void Update(IEntity entity)
    {
        Hp = entity.Hp;
        HpMax = entity.HpMax;
        Mp = entity.Mp;
        MpMax = entity.MpMax;
        Speed = entity.EntityFSMMoveSpeed;

        PhyAtk = entity.PhyAtk;
        MagAtk = entity.MagAtk;
        PhyDef = entity.PhyDef;
        MagDef = entity.MagDef;

        AtkSpeed = entity.AtkSpeed;
        AtkDis = entity.AtkDis;

        HpRecover = entity.HpRecover;
        MpRecover = entity.MpRecover;
        RebornTime = entity.RebornTime;
    }
}

public class Entity : MonoBehaviour
{
	public Transform objAttackPoint{private set;get;}
    public Transform objBuffPoint{private set;get;}
    public Transform objPoint{ private set;get;}
	public IEntity SyncEntity {set;get;}	

	public CharacterController Controller {set;get;}

    public UnityEngine.AI.NavMeshAgent NavAgent{private set; get;}
    public OptimizeDynamicModel mDynModel;
    public EntityCampTypeEnum mCampType;
	public string mFSMStateName;
    public EntityTypeEnum mEntityType;
    public NPCCateChildEnum mNPCCateChild;
    public EntityData mEntityData;
    public int mGUID;
    public int mMasterGuid;
    public Transform mTransform = null;   
    private GameObject mFuryEffect = null;

    //获取Transform
    public Transform GetTransform()
    {
        if (mTransform == null)
        {
            mTransform = this.transform;
        }
        return mTransform;
    }    

    public GameObject focoEffectStart{private set;get;}

    public GameObject focoEffectSuccess
    {
        private set;
        get;
    }

    //获取寻路代理  和实例属性
    public virtual void Awake()
    {
        NavAgent = this.transform.GetComponent<UnityEngine.AI.NavMeshAgent>();

        objAttackPoint = transform.Find("hitpoint");
        objBuffPoint = transform.Find("buffpoint");
        objPoint = transform.Find("point");
        
        //在Update中进行赋值
        mEntityData = new EntityData();
    }

    //为动画添加技能事件(如果Entity是实例)
    public virtual void Start() 
    {
        //如果实例不为空并且实例类型是player
		if (SyncEntity != null && SyncEntity.entityType == EntityTypeEnum.Player) {
            if (this.GetComponent<Animation>() == null) return;
			
			List<string> aniList = ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack;
			foreach(string ani in aniList)
			{
                //为动画添加技能事件
				if (GetComponent<Animation>().GetClip(ani))
				{
					AnimationEvent skillEvent = new AnimationEvent();
					skillEvent.time = getAnimationTime(ani);
					skillEvent.functionName = "OnAttackEnd";
					GetComponent<Animation>().GetClip(ani).AddEvent(skillEvent);
				}

			}

		}
	}
	
	public void SetAttackAnimationLoop(bool b)
	{
		if (GetComponent<Animation>() == null)
		{
			return;
		}

       if (SyncEntity.entityType == EntityTypeEnum.Player)
        {
            List<string> aniList = ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack;
            foreach (string ani in aniList)
            {
                AnimationState aSt = GetComponent<Animation>()[ani];
                if (aSt != null)
                {
                    if (b == true)
                    {
                        aSt.wrapMode = WrapMode.Loop;
                    }
                    else
                    {
                        aSt.wrapMode = WrapMode.Once;
                    }
                }
            }
        }
        else
        {
            if (b == true)
            {
                AnimationState state = GetComponent<Animation>()["attack"];
                state.wrapMode = WrapMode.Loop;
            }
            else
            {
                AnimationState state = GetComponent<Animation>()["attack"];
                state.wrapMode = WrapMode.Once;
            }
        }
        
	}

	/// <summary>
	/// 设置移动速度播放缩放
	/// </summary>
	/// <param name="spd"></param>
	public void SetMoveAnimationSpd(float spd)
	{
		if (GetComponent<Animation>() == null)
		{
			return;
		}
		AnimationState aState = GetComponent<Animation>()["walk"];
		if (aState != null)
		{
			aState.speed = spd;
		}
	}

	/// <summary>
	/// 设置攻击速度
	/// </summary>
	/// <param name="spd">Spd.</param>
	public void SetAttackAnimationSpd(float spd)
	{
        if (GetComponent<Animation>() == null || SyncEntity == null)
		{
			return;
		}
 		if (SyncEntity.entityType == EntityTypeEnum.Player) {
			List<string> aniList = ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack;
			foreach(string ani in aniList)
			{
				AnimationState aSt = GetComponent<Animation>()[ani];
				if (aSt != null)
				{
					aSt.speed = spd;
				}
			}
		} else {
			AnimationState aState = GetComponent<Animation>()["attack"];
			if (aState != null)
			{
				aState.speed = spd;
			}
		}
		//spd = spd;
		
	}

	/// <summary>
	/// Raises the destroy collider event.
	/// </summary>
    public void OnDestroyCollider()
    {
        if (NavAgent != null)
        {
            DestroyImmediate(NavAgent);//销毁寻路代理
        }
        if(Controller != null)
        {
            DestroyImmediate(Controller);//销毁角色控制器
        }
    }

	// Update is called once per frame
	void Update ()
    {
		if (SyncEntity != null) 
        {
            SyncEntity.OnUpdate();//状态更新
            mEntityData.Update(SyncEntity);   //战斗属性更新         
		}
	}
    //淡入下一个动画
    public void CrossFadeSqu(string name)
    {
        GetComponent<Animation>().CrossFadeQueued(name);
    }
    //根据名称播放动画
    public void PlayerAnimation(string name)
    {
        if (name == "" || name == "0")
        {
            return;
        }
        if (this.GetComponent<Animation>() == null)
        {
            return;
        }

        GetComponent<Animation>().CrossFade(name);
    }
  
    //播放Idle动画
    public void PlayerIdleAnimation()
    {
        if (this.GetComponent<Animation>() == null)
        {
            return;
        }

        PlayerAnimation("idle");
       // Debug.Log("播放Idle动画");
        this.GetComponent<Animation>().PlayQueued("free");
    }

    //播放自由动画
	public void PlayerFreeAnimation()
    {
        if (this.GetComponent<Animation>() == null)
        {
            return;
        }

        PlayerAnimation("free");
	}

   //播放跑的动画
	public void PlayerRunAnimation(){
		if (this.GetComponent<Animation>() == null) {
			return;
		}
        PlayerAnimation("walk");
	}
    //播放攻击动画
    public void PlayeAttackAnimation()
    {
        if (this.GetComponent<Animation>() == null)
        {
            return;
        }

        string aniName = "attack";
        if (this.SyncEntity!=null && this.SyncEntity.entityType == EntityTypeEnum.Player)
        {
            int id = UnityEngine.Random.Range(0, ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack.Count);
            aniName = ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack[id];
        }

        if (mEntityType == EntityTypeEnum.Player && GetComponent<Animation>().IsPlaying(aniName))
            GetComponent<Animation>().Stop();

        PlayerAnimation(aniName);
    }
    //播放死亡动画
	public void PlayerDeadAnimation()
	{
		if (this.GetComponent<Animation>() == null) {
			return;
		}
        this.GetComponent<Animation>().cullingType = AnimationCullingType.AlwaysAnimate;
        PlayerAnimation("death");
	}
    //获取某个动画的时间
	public float getAnimationTime(string name)
	{
		AnimationClip clip = GetComponent<Animation>().GetClip(name);
		if(clip != null)
		{
			return clip.length;
		}
		return 0.0f;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (this.SyncEntity == null || !(other is SphereCollider)) return;
       

        ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
        if (this.SyncEntity.GameObjGUID != player.GameObjGUID) return;      //当前ENTITY 不是玩家
       
        Entity entity = other.gameObject.GetComponent<Entity>();
        if(entity == null || entity.SyncEntity == null) return;                //进入对象的entity为空

        if (player.AbsorbMonsterType[0] == 0 && player.AbsorbMonsterType[1] == 0) return;      //没有附身对象

        if (entity.SyncEntity.EntityCamp != player.EntityCamp || entity.SyncEntity.entityType != EntityTypeEnum.Building    //如果进入的不是友方祭坛、
            || entity.SyncEntity.NPCCateChild != NPCCateChildEnum.BUILD_Altar)
        {
            return;
        }

        Thanos.AudioManager.Instance.PlayEffectAudio(player.GetAltarClip());
        //广播事件，引导任务事件触发
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayerEnterAltar);

        if (UIAltarSelect.Instance == null)
        {
            HolyTechUI.Instance.OnOpenUI(GameConstDefine.AltarSoldierSelect);
            UIAltarSelect.Instance.OnTriggerAltar(entity.SyncEntity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(UIAltarSelect.Instance == null)
        {
            return;
        }
        if (this.SyncEntity == null || !(other is SphereCollider))
        {
            return;
        }
        ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
        if (this.SyncEntity.GameObjGUID != player.GameObjGUID)          //当前ENTITY 不是玩家
        {
            return;
        }
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity == null || entity.SyncEntity == null)                 //退出对象的entity为空
        {
            return;
        }
        if (UIAltarSelect.Instance.TrAltar == entity.SyncEntity)
        {
            Destroy(UIAltarSelect.Instance.gameObject);
        }
    }
 
    //更新狂暴特效
    public void UpdateFuryEffect() {
        if (SyncEntity == null)
            return;
        if (EntityTypeEnum.Player != SyncEntity.entityType)
        {
            return;
        }
        IPlayer player = (IPlayer)SyncEntity;
        if (EFuryState.eFuryRunState == player.FuryState) {
            LoadFuryEffect();
        }
        else {
            DestroyFuryEffect();
        }
    }
    //加载狂暴特效
    private void LoadFuryEffect() 
    {
        if (null == mFuryEffect) 
        {
            //furyEffect = GameObject.Instantiate(Resources.Load(GameDefine.GameConstDefine.FuryGasExplosionEffect)) as GameObject;
            ResourceItem furyEffectUnit = ResourcesManager.Instance.loadImmediate(GameDefine.GameConstDefine.FuryGasExplosionEffect, ResourceType.PREFAB);
            mFuryEffect = GameObject.Instantiate(furyEffectUnit.Asset) as GameObject;
           
            
            mFuryEffect.transform.parent = transform;
            mFuryEffect.transform.localScale = Vector3.one;
            mFuryEffect.transform.localPosition = Vector3.zero; 
        }
    }

    //销毁狂暴特效
    private void DestroyFuryEffect() {
        if (null != mFuryEffect) {
            GameObject.DestroyImmediate(mFuryEffect);                      
            mFuryEffect = null;
            //System.GC.Collect(); 
        }
    }

   
    private Dictionary<TrailRenderer, float> trailTime = new Dictionary<TrailRenderer, float>();

    public void HideTrail()
    {
        foreach (Transform child in objBuffPoint)
        {
            if (child.tag == "trail")
            {
                ParticleSystem[] particles = child.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem part in particles)
                {
                    part.Clear();
                    part.gameObject.SetActive(false);
                }

                TrailRenderer[] trailRenders = child.GetComponentsInChildren<TrailRenderer>();
                foreach (TrailRenderer trail in trailRenders)
                {
                    trailTime[trail] = trail.time;
                    trail.time = -10;
                }
            }
        }

       
    }

    public void ShowTrail()
    {
        Invoke("ShowTrailDelay", 1.0f);
    }

    public void ShowTrailDelay()
    {
        foreach (Transform child in objBuffPoint)
        {
            if (child.tag == "trail")
            {
                ParticleSystem[] particles = child.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem part in particles)
                {
                    part.gameObject.SetActive(true);
                }

                TrailRenderer[] trailRenders = child.GetComponentsInChildren<TrailRenderer>();
                foreach (TrailRenderer trail in trailRenders)
                {
                    trail.time = trailTime[trail];
                }
            }
        }

        trailTime.Clear();
    }
    //攻击结束
    public void OnAttackEnd()
    {
        if (this.GetComponent<Animation>() == null)
        {
            return;
        }

        if (this.SyncEntity == null || this.SyncEntity.FSM == null || this.SyncEntity.FSM.State != Thanos.FSM.FsmStateEnum.RELEASE)
        {
            return;
        }

        int id = UnityEngine.Random.Range(0, ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack.Count);
        string aniName = ConfigReader.HeroXmlInfoDict[SyncEntity.NpcGUIDType].n32RandomAttack[id];


        if (mEntityType == EntityTypeEnum.Player && GetComponent<Animation>().IsPlaying(aniName))
            GetComponent<Animation>().Stop();

        PlayerAnimation(aniName);
    }

    /// <summary>
    /// 重载手指
    /// 或者鼠标点中消息
    /// </summary>
    private void OnFingerClick()
    {
        if (this.SyncEntity != null)
        {
            this.SyncEntity.OnEntityFingerClick();
        }
    }

    //
    public void AddFocoEnergiaStart()
    {
        if (null == focoEffectStart)
        {
            focoEffectStart = LoadUiResource.LoadRes(transform, GameDefine.GameConstDefine.FuryStartLongPressEffect);
        }
    }

    public void DestroyFocoEnergiaStart()
    {
        if (null != focoEffectStart)
        {
            LoadUiResource.DestroyLoad(GameDefine.GameConstDefine.FuryStartLongPressEffect);
            focoEffectStart = null;
        }
    }
}