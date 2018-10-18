using UnityEngine;
using Thanos.GameData;
using Thanos.Effect;
using GameDefine;
using Thanos.Resource;
using Thanos.GameState;
using System;

namespace Thanos.GameEntity
{
	public class IPlayer : IEntity
	{
        //吸附特效
        private GameObject AbsorbEffect1;
        private GameObject AbsorbEffect2;

        private GameObject AbsorbEffectDel1;
        private GameObject AbsorbEffectDel2;

        public GameObject AbsorbProgressEffect{ get;set;}
        public IPlayer(UInt64 sGUID, EntityCampTypeEnum campType)
            : base(sGUID, campType)
        {
            BattleData = new PlayerBattleData();
        }
        public PlayerBattleData BattleData
        {
            private set;
            get;
        }

        public UInt64 GameUserId;

		public string GameUserNick {
			get;
			set;
		}

        public IEntity SyncLockTarget
        {
            protected set;
            get;
        }
		
		public bool GameUserIsMaster{
			get;
			private set;
		}
		
		public bool GameUserIsReady{
			get;
			private set;
		}
		
		public uint GameUserSeat{
			get;
			private set;
		}

		public int GameHeadId{
			get;
			private set;
		}

        public ObPlayerOrPlayer ObjType
        {
            get;
            private set;
        }
      
        public SITUATION StateSituation
        {
            set;
            get;
        }

        public EFuryState FuryState
        {
            private set;
            get;
        }

        public int FuryValue
        {
            private set;
            get;
        }

        public int AccountGold
        { 
            private set;
            get;
        }

		public const bool MasterTag = true;
		public const bool ReadyTag = true;
        public const int GameLookTag = 7;

        private float mDefaultHeight = HolyGameLogic.Instance.GetGlobalHeight();

        public void SetObjType(ObPlayerOrPlayer obType)
        {
            ObjType = obType;
        }

        public override void OnEntityPrepareAttack()
        {
        }

        void CreateEffect(ref GameObject Obj, string effectPath)
        {
            IEffect e = EffectManager.Instance.CreateNormalEffect(effectPath, RealEntity.objAttackPoint.gameObject);
            if (e != null)
            {
                if (Obj != null)
                {
                    GameObject.DestroyImmediate(Obj);
                    Obj = null;
                }
                Obj = e.obj;
            }
        }

        public void SetAbsorbEffect(int slot, bool isRemove)
        {
            if (isRemove == false)
            {
                if (slot == 0)
                {
                    CreateEffect(ref AbsorbEffect1,"effect/other/soul_01");
                }
                else
                {
                   CreateEffect(ref AbsorbEffect2,"effect/other/soul_02");
                   //[yaz]第二个吸魂特效位置根据第一个吸魂特效位置设置相对位置
                   if (AbsorbEffect2 && AbsorbEffect1)
                   {
                       Vector3 rot1 = AbsorbEffect1.transform.GetChild(0).transform.localEulerAngles;                       
                       AbsorbEffect2.transform.localEulerAngles = new Vector3(rot1.x, rot1.y, rot1.z);                            
                   }
                } 
            }
            else if (isRemove == true)
            {
                if (slot == 0)
                {
                    DestroyEffect(ref AbsorbEffectDel1, AbsorbEffect1, "effect/other/soul_01_delete");
                }
                else if (slot == 1)
                {
                    DestroyEffect(ref AbsorbEffectDel2, AbsorbEffect2, "effect/other/soul_02_delete");
                } 
            }
        }

        /// <summary>
        /// 销毁特效
        /// </summary>
        /// <param name="desEffect"></param>
        /// <param name="TranObj"></param>
        /// <param name="tran"></param>
        /// <param name="path"></param>
        /// <param name="delPath"></param>
        void DestroyEffect(ref GameObject desEffect,GameObject TranObj, string delPath)
        {
            if(TranObj != null){
                if (desEffect != null)
                {
                    desEffect.transform.parent = null;
                    GameObject.DestroyImmediate(desEffect);
                }
                IPlayer player = PlayerManager.Instance.LocalPlayer;
                if (player == null)
                {
                    return;
                }
                //GameObject obj = Resources.Load(delPath) as GameObject;
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(delPath, ResourceType.PREFAB);
                GameObject obj = objUnit.Asset as GameObject;
                
                if (obj == null)
                {
                    Debug.LogError("Res Not Found:" + delPath);
                }
                desEffect = GameObject.Instantiate(obj) as GameObject;
                desEffect.transform.position = TranObj.transform.position;
                TranObj.transform.parent = null;
                GameObject.DestroyImmediate(TranObj);
            }
        }



        /// <summary>
        /// 设置锁定对象
        /// </summary>
        /// <param name="entity"></param>
        /// 锁定随想实例
        public virtual void SetSyncLockTarget(IEntity entity)
        {
            if (SyncLockTarget == entity)
            {
                return;
            }
            this.DeltLockTargetXuetiao(entity);
      
            this.SyncLockTarget = entity;
        }

        public void DeltLockTargetXuetiao(IEntity target)
        {
            //锁定目标不为空且为建筑，那么就销毁血条
            //（注意，SyncLockTarget此时不是target，SyncLockTarget是上一个目标）
            if (this.SyncLockTarget != null && this.SyncLockTarget.entityType == EntityTypeEnum.Building)
            {
                this.SyncLockTarget.DestroyBloodBar();
            }
            //目标不为空，如果是建筑的话那么就创建新目标的血条
            if (target != null && target.entityType == EntityTypeEnum.Building)
            {
                target.CreateBloodBar();
            }
        }

        /// <summary>
        /// Entity移动属性变化
        /// </summary>
        public override void OnEntityMoveSpeedChange(int value)
        {
            base.OnEntityMoveSpeedChange(value);
            HeroConfigInfo heroConfig;
            if (ConfigReader.HeroXmlInfoDict.TryGetValue(NpcGUIDType, out heroConfig))
            {
                float speed = value / heroConfig.HeroMoveSpeed * heroConfig.n32BaseMoveSpeedScaling / 1000;
                if (speed == 0)
                {
                    return;
                }
                this.RealEntity.SetMoveAnimationSpd(speed);
            }
        }


        /// <summary>
        /// 转动朝向
        /// </summary>
        private void TurnAngle()
        {
            float fAngle = Vector3.Angle(RealEntity.GetTransform().forward, EntityFSMDirection);
            if (fAngle > 2)
            {
                Quaternion DestQuaternion = Quaternion.LookRotation(EntityFSMDirection);
                Quaternion sMidQuater = Quaternion.Lerp(RealEntity.GetTransform().rotation, DestQuaternion, 5 * Time.deltaTime);
                RealEntity.GetTransform().rotation = sMidQuater;
            }
        }

        //  /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 进入Idle状态
        /// </summary>
        public override void OnEnterIdle()
        {
           
            this.RealEntity.PlayerIdleAnimation();

        }

        /// <summary>
        /// 状态机执行Free消息
        /// </summary>
        public override void OnExecuteFree()  //覆盖父类的OnExecuteFree
        {
            if (RealEntity == null || this.entityType == EntityTypeEnum.Building) return;
          
            //计算平面位置
            Vector3 synPos2D = new Vector3(m_pcGOSSI.sServerSyncPos.x, 60, m_pcGOSSI.sServerSyncPos.z);//服务器传过来的位置
            Vector3 realPos2D = new Vector3(RealEntity.GetTransform().position.x, 60, RealEntity.GetTransform().position.z);//玩家真实位置

            float fDistToServerPos = Vector3.Distance(synPos2D, realPos2D);//距离差值

            if (fDistToServerPos < 0.5f)//当差值小于0.5时，
            {
                if (EntityStrategyHelper.IsTick(this, EntityStrategyHelper.IdleTimeTick))
                {
                 
                    this.OnFSMStateChange(Thanos.FSM.EntityIdleFSM.Instance);//进入Idle状态
                    return;
                }
                RealEntity.PlayerFreeAnimation();//播放free动画

            }
            else if (fDistToServerPos > 5)//当差值大于5时，
            {
                RealEntity.PlayerFreeAnimation();//播放free动画
                RealEntity.GetTransform().position = m_pcGOSSI.sServerBeginPos;

            }
            else //当差值在0.5 到5之间
            {
                Vector3 sSyncDir = synPos2D - realPos2D;
                sSyncDir.Normalize();
                Vector3 pos = sSyncDir * 2 * Time.deltaTime + realPos2D;

                RealEntity.PlayerRunAnimation();
                this.RealEntity.Controller.Move(sSyncDir * 2 * Time.deltaTime);//在距离方向上移动过去。
            }

            OnCameraUpdatePosition();//摄像机为直角度的更新
            TurnAngle();//转动朝向

        }


        /// <summary>
        /// 状态机执行移动
        /// </summary>
        public override void OnExecuteMove()//Iplayer中
        {
            if (RealEntity == null || this.entityType == EntityTypeEnum.Building){ return;}

            Quaternion DestQuaternion = Quaternion.LookRotation(EntityFSMDirection);
            Quaternion sMidQuater = Quaternion.Lerp(RealEntity.GetTransform().rotation, DestQuaternion, 10 * Time.deltaTime);
            RealEntity.GetTransform().rotation = sMidQuater;//实体方向旋转

            float fTimeSpan = Time.realtimeSinceStartup - m_pcGOSSI.fBeginTime;//时间间隔
            float fMoveDist = fTimeSpan * m_pcGOSSI.fServerSpeed;//在此间隔内移动的距离

            m_pcGOSSI.sServerSyncPos = m_pcGOSSI.sServerBeginPos + m_pcGOSSI.sServerDir * fMoveDist;//新位置
            RealEntity.PlayerRunAnimation();//播放跑的动画

            //同步2D位置处理
            Vector3 serverPos2d = new Vector3(m_pcGOSSI.sServerSyncPos.x,60,m_pcGOSSI.sServerSyncPos.z);//需要同步的位置，新的位置
            Vector3 entityPos2d = new Vector3(RealEntity.GetTransform().position.x, 60, RealEntity.GetTransform().position.z);//实体真实位置
            
            Vector3 sSyncDir = serverPos2d - entityPos2d;//方向向量
            sSyncDir.Normalize();//向量归一化 
            float fDistToServerPos = Vector3.Distance(serverPos2d, entityPos2d);//距离差值

            if (fDistToServerPos > 5)//距离差值大5 说明速度太快，直接赋值，忽略碰撞体
            {
                RealEntity.GetTransform().position = m_pcGOSSI.sServerSyncPos;//移动到新位置上
                OnCameraUpdatePosition();//摄像机更新跟随
                return;
            }
            //当距离差值小于5的时候 速度说明慢，慢速的时候不能穿过碰撞体
            Vector3 pos = entityPos2d + sSyncDir * EntityFSMMoveSpeed * Time.deltaTime;//在原位置上移动一帧后的新位置

            //移动到新位置上 使用角色空控制器的Move时动力只受限制于碰撞。它将沿着碰撞器滑动，不应用于重力
            this.RealEntity.Controller.Move(sSyncDir * EntityFSMMoveSpeed * Time.deltaTime);

            //限制Y值始终在默认的高度上
            RealEntity.GetTransform().position = new Vector3(RealEntity.GetTransform().position.x, mDefaultHeight, RealEntity.GetTransform().position.z);
          

            OnCameraUpdatePosition();//摄像机更新
           //  EntityMoveCheck();  小谷注释
        }      

        /// <summary>
        /// 对象移动检测
        /// </summary>
        protected virtual void EntityMoveCheck()
        { 

        }

        protected void OnCameraUpdatePosition()
        {         
            if (objTransform.transform == GameMethod.GetMainCamera.target && GameMethod.GetMainCamera != null && GameMethod.GetMainCamera.target != null && RealEntity != null)
            {
                //获取当前PlayState
                PlayState playState = GameStateManager.Instance.GetCurState() as PlayState;
                if (playState == null)
                    return;

                if (playState.mCameraType == 1)          //斜45度
                {
                    //根据角色阵营调整相机旋转角度
                    Vector3 euler = GameMethod.GetMainCamera.transform.eulerAngles;
                    if (RealEntity.mCampType == EntityCampTypeEnum.A)
                    {
                        GameMethod.GetMainCamera.transform.eulerAngles = new Vector3(euler.x, 45.0f, 0/*euler.z*/);
                    }
                    else if (RealEntity.mCampType == EntityCampTypeEnum.B)
                    {
                        GameMethod.GetMainCamera.transform.eulerAngles = new Vector3(euler.x, -135.0f, 0/*euler.z*/);
                    }
                    GameMethod.GetMainCamera.FixedUpdatePosition();
                }
                else
                {
                    //根据角色阵营调整相机旋转角度
                    Vector3 euler = GameMethod.GetMainCamera.transform.eulerAngles;
                    if (RealEntity.mCampType == EntityCampTypeEnum.A)
                    {
                        GameMethod.GetMainCamera.transform.eulerAngles = new Vector3(euler.x, 0.0f, 0/*euler.z*/);
                    }
                    else if (RealEntity.mCampType == EntityCampTypeEnum.B)
                    {
                        GameMethod.GetMainCamera.transform.eulerAngles = new Vector3(euler.x, -180.0f, 0/*euler.z*/);
                    }
                    GameMethod.GetMainCamera.FixedUpdatePosition();
                }                                    
            }
        }
        //空
        public override void OnEntityLeadSkill()
        {
        }

        //  ////////////////////////////////////////////////////////////////////////////////////
        public void SetSeatPosInfo(uint seat , bool ifMaster, bool ifReady, string userNick,int userHead,int gold)
		{
            // Debug.LogError("seat:" + seat + "     ifMaster:" + ifMaster + "   ifReady:" + ifReady + "   userNick :" + userNick);
			GameUserSeat = seat;
			GameUserIsReady = ifReady;
			GameUserIsMaster = ifMaster;
			GameUserNick = userNick;
			GameHeadId = userHead;
            AccountGold = gold;
		}

		/// <summary>
		/// Raises the update hp event.
		/// </summary>
		public override void OnUpdateHp ()
		{
			base.OnUpdateHp ();
		}

		public bool IsMaster(){			 
			if (GameUserIsMaster == MasterTag) {
				return true; 			
			}
			return false;
		}
        public bool isGameLookViewer()
        {
            if (GameUserSeat >= GameLookTag)
            {
                return true;
            }
            return false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            ISelfPlayer playerSelf = PlayerManager.Instance.LocalPlayer;
            if (playerSelf != null && playerSelf.realObject != null && !this.IsSameCamp(playerSelf.EntityCamp))
            {//enemy player
                float dis = Vector3.Distance(playerSelf.objTransform.position, objTransform.position);//self player distance with enemy player
                if(dis > GameConstDefine.PlayerLockTargetDis) return ;
                if ((this.FSM != null && this.FSM.State == Thanos.FSM.FsmStateEnum.DEAD) || (this.realObject.activeSelf == false))
                {
                    AddOrDelEnemy(this, false);// UIEnemyTeamMateInfo.Instance.RemovePlayerEnemy(this);
                }
                else {// if(UIEnemyTeamMateInfo.Instance != null){
                   // UIEnemyTeamMateInfo.Instance.SetPlayerEnemy(this);
                        if (this.realObject.active == true)
                            AddOrDelEnemy(this, true);
                    }
               }
            }

        public static void AddOrDelEnemy(IPlayer entity,bool add)
        {
            FEvent eve = new FEvent((Int32)(Int32)GameEventEnum.GameEvent_AddOrDelEnemy);
            eve.AddParam("Add",add);
            eve.AddParam("Target", entity);
            EventCenter.SendEvent(eve);
        }

		public bool IsReady(){
			if (ReadyTag == GameUserIsReady) {
				return true;			
			}
			return false;
		}

		public bool IsSameCamp(IPlayer player){
			if((player.GameUserSeat % 2) == (GameUserSeat % 2))
				return true;
			else
				return false;
		}

        public override void InitWhenCreateModel()
        {
          
            RealEntity.SetAttackAnimationLoop(true); 
        }
 
        /// <summary>
        /// 创建shadow
        /// </summary>
        public override void OnCreateShadow()
        {
            int id = (int)ObjTypeID;
            float radius = ConfigReader.GetHeroInfo(id).HeroCollideRadious;
          
            float range = radius / 100.0f;

           
            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(GameDefine.GameConstDefine.LoadGameShadow, ResourceType.PREFAB);
            GameObject obj = GameObject.Instantiate(objUnit.Asset) as GameObject;

            
            obj.transform.parent = this.realObject.transform;
            obj.transform.localScale = new Vector3(range, 1, range);
            obj.transform.localPosition = Vector3.zero;
        }

        protected override void DoEntityDead()
        {
            //base.DoEntityDead();
            this.BloodBar.SetVisible(false);//血条不可见
            if (this.RealEntity.Controller.enabled)
            {
                this.RealEntity.Controller.enabled = false;//角色控制器禁用
            }
        }

        /// <summary>
        /// Do 复活
        /// </summary>
        protected override void DoReborn()
        {
            base.DoReborn();
            if (!this.RealEntity.Controller.enabled)
            {
                this.RealEntity.Controller.enabled = true;
            }
            if (this == PlayerManager.Instance.LocalPlayer && FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
            {
                GameMethod.GetMainCamera.target = RealEntity.mTransform;
            }
            Thanos.Effect.EffectManager.Instance.CreateNormalEffect("effect/other/fuhuo", RealEntity.objAttackPoint.gameObject);
            ShowBloodBar(); 
        }
       
        
        public override int GetSkillIdBySkillType(SkillTypeEnum type)
        {
            switch (type)
            {
                case SkillTypeEnum.SKILL_TYPE1:
                    return ConfigReader.GetHeroInfo(NpcGUIDType).HeroSkillType1;
                case SkillTypeEnum.SKILL_TYPE2:
                    return ConfigReader.GetHeroInfo(NpcGUIDType).HeroSkillType2;
                case SkillTypeEnum.SKILL_TYPE3:
                    return ConfigReader.GetHeroInfo(NpcGUIDType).HeroSkillType3;
                case SkillTypeEnum.SKILL_TYPE4:
                    return ConfigReader.GetHeroInfo(NpcGUIDType).HeroSkillType4;
                case SkillTypeEnum.SKILL_TYPE5:
                    return ConfigReader.GetHeroInfo(NpcGUIDType).HeroSkillType5;
            }
            return -1;
        }

        protected virtual void UpdateFuryEffect() {
            if (null != this.RealEntity)
            {
                this.RealEntity.UpdateFuryEffect();
            }
        }

        protected virtual void UpdateFuryValue() { 

        }

        public virtual void SetFuryState(EFuryState state) {
            FuryState = state;
            UpdateFuryEffect(); 
        }

        public void SetFuryValue(int value) {
            FuryValue = value;
            UpdateFuryValue();
        }

        public virtual void CleanWhenGameOver() { 

        }

        public void SetReconnectPlayerInfo(uint seat, string nickName)
        {
            GameUserNick = nickName;
            GameUserSeat = seat;
        }
	}
}