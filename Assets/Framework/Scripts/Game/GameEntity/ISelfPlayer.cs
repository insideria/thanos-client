using UnityEngine;
using System.Collections.Generic;
using GameDefine;
using System;
using System.Linq;
using Thanos.GameData;

using Thanos.GuideDate;
using Thanos.FSM;
using Thanos.GameState;
using Thanos.Resource;
using Thanos.Ctrl;
using Thanos.View;
//
namespace Thanos.GameEntity
{
    public class ISelfPlayer : IPlayer
    {
        public ISelfPlayer(UInt64 sGUID, EntityCampTypeEnum campType)
            : base(sGUID, campType)
        {
            UserGameItems = new Dictionary<int, int>();
            UserGameItemsCount = new Dictionary<int, int>();
            UserGameItemsCoolDown = new Dictionary<int, float>();
            for (int ct = 0; ct < 6; ct++)
            {
                UserGameItems.Add(ct, 0);
                UserGameItemsCount.Add(ct, 0);
                UserGameItemsCoolDown.Add(ct, 0f);
            }
        }

        public bool IsAbsobing = false;

        public bool isTargetChanged = false;

        public const int PerFuryValue = 120;

        public Dictionary<int, int> UserGameItems
        {
            private set;
            get;
        }

        public Dictionary<int, int> UserGameItemsCount
        {
            private set;
            get;
        }

        public Dictionary<int, float> UserGameItemsCoolDown
        {
            private set;
            get;
        }

        public int[] AbsorbMonsterType = new int[2];

        public GameObject objWarning
        {
            private set;
            get;
        }
        private const float duringTime = 1f;
        private const float hpRate = 0.3f;

        private const int SKILL_UPDATE_TOTAL_LEVEL = 3;

        public Dictionary<SkillTypeEnum, int> SkillIdDic = new Dictionary<SkillTypeEnum, int>();
        public Dictionary<SkillTypeEnum, int> BaseSkillIdDic = new Dictionary<SkillTypeEnum, int>();

       
        //
        public override void OnReborn()
        {
            base.OnReborn();
            if (GameMethod.GetCameraGray != null)
            {
                GameMethod.GetCameraGray.enabled = false;
            }
            VirtualStickUI.Instance.SetVirtualStickUsable(true);
            Thanos.AudioManager.Instance.PlayEffectAudio(RebornClip);
            UInt64 myself_guid = PlayerManager.Instance.LocalPlayer.GameObjGUID;
            MsgInfoManager.Instance.SetAudioPlay(myself_guid, MsgInfoManager.AudioPlayType.FuhuoAudio);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_Reborn);
        }

        enum SlotSkill
        {
            Slot_KILL_ONE,//吸附槽1
            Slot_KILL_TWO
        }

        public void SetAbsorbMonster(int monsterID, int slot)//slot 0 ,1
        {
            NpcConfigInfo npcInfo = ConfigReader.GetNpcInfo((int)monsterID);
            int skillId = 0;
            bool effect = true;
            string spriteName = "";
            if (AbsorbMonsterType[slot] != monsterID && npcInfo != null)
            {
                skillId = (slot == 0) ? npcInfo.NpcSkillType3 : npcInfo.NpcSkillType4;
                effect = false;
                spriteName = npcInfo.HeadPhoto.ToString();
            }
            else
            {
                if (AbsorbMonsterType[slot] == monsterID && monsterID != 0)
                {
                    return;
                }
            }

            SetBaseSkillType((SkillTypeEnum.SKILL_TYPEABSORB1 + slot), skillId);//set absorb skill
            SetAbsorbEffect(slot, effect);//set absorb btn effect          
            AbsorbMonsterType[slot] = monsterID;        //set absorb    monster id
            //广播吸收结果   创建灵魂特效  设置吸附按钮
            EventCenter.Broadcast<int,string,bool>((Int32)GameEventEnum.GameEvent_AbsorbResult,slot,spriteName,effect);
        }

        /// <summary>
        /// 设置锁定对象
        /// </summary>
        /// <param name="entity"></param>
        /// 锁定对象实例
        public override void SetSyncLockTarget(IEntity entity)
        {
            if (SyncLockTarget == entity) return; //判断是否是新目标，如果不是，直接返回  
            if (SyncLockTarget != null && SyncLockTarget.realObject != null)
            {
                if (SyncLockTarget.BloodBar != null)
                {
                    SyncLockTarget.BloodBar.IsBloodBarCpVib(false);//禁用原目标的血条，cp
                }
            }

            this.DeltLockTargetXuetiao(entity); //如果是建筑 ，删除原目标的血条，创建新目标的血条
            this.isTargetChanged = true;//目标是否转变   是
            this.SyncLockTarget = entity;//新目标的设定
            OnFocusTargetChanged();//创建光圈并设置

            UInt64 objId = (entity == null) ? 0 : entity.GameObjGUID;//获取目标id
            HolyGameLogic.Instance.EmsgToss_AskLockTarget(objId);//请求锁定目标
            if (entity != null && entity is IPlayer) //如果目标是玩家
            {
                AddOrDelEnemy((IPlayer)entity, true);//在敌人窗口显示目标
            }
            else
            {
                AddOrDelEnemy(null, true);
            }
            //广播锁定目标的消息，来更新定头像（非敌方玩家，而是小兵野怪等） GamePlayWindow   677
            EventCenter.Broadcast<IEntity>((Int32)GameEventEnum.GameEvent_LockTarget, entity);
        }

        public override void OnEntityPrepareAttack()
        {
        }

        public override void OnUpdate()
        {
            UpdateCoolDownGameItems();

            base.OnUpdate();
            if (Time.frameCount % 5 != 0)
            {
                return;
            }
            BuildingAtkWarningManager.Instance.OnUpdate();
       
            IGuideMidMatchTip.Instance.OnUpdate();
          
            if (this.FSM != null && this.FSM.State == FsmStateEnum.DEAD)
            {
                return;
            }
            if (SyncLockTarget != null && SyncLockTarget.realObject != null)
            {
                if (Vector3.Distance(this.objTransform.position, SyncLockTarget.objTransform.position) > GameConstDefine.PlayerLockTargetDis)
                {
                    if (!PlayerManager.Instance.IsLocalSameType(SyncLockTarget) && SyncLockTarget.entityType == EntityTypeEnum.Monster || SyncLockTarget.entityType == EntityTypeEnum.Soldier)
                    {
                        if (SyncLockTarget.BloodBar != null)
                        {
                            SyncLockTarget.BloodBar.IsBloodBarCpVib(false);
                        }
                    }
                    this.SetSyncLockTarget(null);
                }
                return;
            }
            if (AutoLockTarget)
            {
                IEnumerable<IEntity> ItemList = GameMethod.GetEnemyItemListByRadius(PlayerManager.Instance.LocalPlayer, GameConstDefine.PlayerLockTargetDis);
                IEnumerator<IEntity> Item = ItemList.GetEnumerator();
                Item.MoveNext();
                if (ItemList == null || Item.Current == null)
                {
                    return;
                }
                this.SetSyncLockTarget(Item.Current);
            }
        }

        public bool AutoLockTarget = true;

        /// <summary>
        /// 对象移动检测
        /// </summary>
        protected override void EntityMoveCheck()
        {
            base.EntityMoveCheck();
            //this.CheckEnterAltar();
        }

        public override void OnEntitySkill()
        {
            if (SyncLockTarget == null || SyncLockTarget.RealEntity == null)
            {
                return;
            }
        }
        public override void OnEntityExitPrepareSkill()
        {
            //ProgressBarInterface.stopBacktoCity();
        }

      
        private bool bPlaySkill = false;
       
        public void RemoveAreaCircle()
        {
            if (CircleArea != null)
            {
                UnityEngine.GameObject.DestroyImmediate(CircleArea);
            }
        }

        //临时在这里管理光圈
        private GameObject CircleArea = null;
        private GameObject CircleGreen = null;
        private GameObject CircleYellow = null;
        private GameObject CircleRed = null;
        //
        public void initAreaCircle()
        {
            //if (SceneGuideTaskManager.Instance().IsNewsGuide() == SceneGuideTaskManager.SceneGuideType.NoGuide)
            //    return;
            if (RealEntity == null)
                return;

            if (CircleArea == null)
            {
                string path = GameConstDefine.LoadGameOtherEffectPath;
                //GameObject obj = Resources.Load(path + "guangquan_fanwei") as GameObject;
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(path + "guangquan_fanwei", ResourceType.PREFAB);
                GameObject obj = objUnit.Asset as GameObject;

                if (obj == null)
                {
                    GameMethod.DebugError("CircleArea obj null");
                    return;
                }

                //root = GameObject.Instantiate (obj) as GameObject;
                CircleArea = GameObject.Instantiate(obj) as GameObject;
                CircleArea.transform.parent = RealEntity.GetTransform();
                CircleArea.transform.position = RealEntity.GetTransform().position + new Vector3(0.0f, 0.2f, 0.0f);
                EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerRange,UpdataRange);
                CircleArea.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
            }
            CapsuleCollider capsule = RealEntity.GetComponent<CapsuleCollider>();
            if (capsule)
            {
                capsule.enabled = false;
            }
        }

        private void UpdataRange()
        {
            if (CircleArea == null)
                return;
            if (RealEntity == null)
                return;
            CircleArea.transform.localScale = new Vector3(this.AtkDis * 2.0f / RealEntity.GetTransform().localScale.x, 1.0f, this.AtkDis * 2.0f / RealEntity.GetTransform().localScale.z);
        }
        //
        public void OnFocusTargetChanged()
        {
            checkCircleRes();//创建光圈
            if (SyncLockTarget == null)
            {
                setCircleEnable(CircleGreen, false);
                setCircleEnable(CircleYellow, false);
                setCircleEnable(CircleRed, false);
            }
            else if (SyncLockTarget.EntityCamp == EntityCamp)//友军  绿色光圈设置
            {
                setCircleEnable(CircleGreen, true);
                setCircleEnable(CircleYellow, false);
                setCircleEnable(CircleRed, false);
            }
            else if (SyncLockTarget.EntityCamp == EntityCampTypeEnum.Bad)//全敌对  黄色的
            {
                setCircleEnable(CircleGreen, false);
                setCircleEnable(CircleYellow, true);
                setCircleEnable(CircleRed, false);
            }
            else  //敌军  红色
            {
                setCircleEnable(CircleGreen, false);
                setCircleEnable(CircleYellow, false);
                setCircleEnable(CircleRed, true);
            }
        }

        private void checkCircleRes()
        {
            string path = GameConstDefine.LoadGameOtherEffectPath;
            if (CircleRed == null)
            {
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(path + "guangquan_red", ResourceType.PREFAB);
                CircleRed = GameObject.Instantiate(objUnit.Asset) as GameObject;
            }
            if (CircleGreen == null)
            {
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(path + "guangquan_green", ResourceType.PREFAB);
                CircleGreen = GameObject.Instantiate(objUnit.Asset) as GameObject;
            }
            if (CircleYellow == null)
            {
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(path + "guangquan_yellow", ResourceType.PREFAB);
                CircleYellow = GameObject.Instantiate(objUnit.Asset) as GameObject;
            }
        }

        private void setCircleEnable(GameObject obj, bool bEnabled)
        {

            if (bEnabled == true)
            {
                if (SyncLockTarget != null && SyncLockTarget.realObject != null)
                {
                    obj.transform.parent = SyncLockTarget.realObject.transform;
                    obj.transform.position = SyncLockTarget.realObject.transform.position + new Vector3(0.0f, 0.2f, 0.0f);
                    obj.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                    obj.gameObject.SetActive(true);
                    float radius = 1.0f;
                    if (GameUtils.IfTypeHero((ObjectTypeEnum)SyncLockTarget.NpcGUIDType))
                    {
                        radius = ConfigReader.HeroXmlInfoDict[SyncLockTarget.NpcGUIDType].n32LockRadious;
                        obj.transform.localScale = new Vector3(radius, 1.0f, radius);
                    }
                    else
                    {
                        radius = ConfigReader.NpcXmlInfoDict[SyncLockTarget.NpcGUIDType].n32LockRadius;
                        obj.transform.localScale = new Vector3(radius, 1.0f, radius);
                    }
                }
            }
            else
            {
                obj.transform.parent = null;
                obj.gameObject.SetActive(false);
            }
        }
        //
        public override void OnSkillAnimationEnd()
        {
            if (bPlaySkill == false)
            {
                OnFSMStateChange(EntityFreeFSM.Instance);
            }
        }
        //
        public override void OnAttackAnimationEnd()
        {
            OnFSMStateChange(EntityFreeFSM.Instance);
        }
        //请求吸附怪物
        const int absBigLevel = 7;
        const int absSmallLevel = 3;

        public void SendAbsortMonster()
        {
            if (SyncLockTarget == null)
            {
                //显示消息  “您还没有目标”
                MsgInfoManager.Instance.ShowMsg((int)ErrorTypeEnum.NeedLockTarget);
                return;
            }
            if (SyncLockTarget.entityType == EntityTypeEnum.Building || SyncLockTarget.entityType == EntityTypeEnum.Player || SyncLockTarget.entityType == EntityTypeEnum.Soldier)
            {
              //  显示消息  “目标不能被吸附”
                MsgInfoManager.Instance.ShowMsg(-130956);
                return;
            }

            NpcConfigInfo info = ConfigReader.GetNpcInfo(SyncLockTarget.NpcGUIDType);
            if (info == null) return;

            //目标是否可以被吸附   0：不可以   1：可以  
            int isLock = info.NpcCanControl;

            if (IsAbsobing == true) return;//如果正在吸附，返回。

            // 有两个附身对象  等级在7级以上，目标与第一个和第二个类型不同，目标允许吸附
            if (AbsorbMonsterType[0] != 0 && AbsorbMonsterType[1] != 0 && Level >= absBigLevel && (AbsorbMonsterType[1] != SyncLockTarget.NpcGUIDType && AbsorbMonsterType[0] != SyncLockTarget.NpcGUIDType) && isLock != 0)
            {
                CreateSoleSoldier();//创建灵魂交换面板
                return;
            }
            //  有一个附身对象  英雄等级在3到7级之间  目标允许吸附
            if (AbsorbMonsterType[0] != 0 && Level >= absSmallLevel && AbsorbMonsterType[0] != SyncLockTarget.NpcGUIDType && isLock != 0 && AbsorbMonsterType[1] == 0 && AbsorbMonsterType[1] != SyncLockTarget.NpcGUIDType && Level < absBigLevel)
            {
                CreateSoleSoldier();//创建灵魂交换面板
                return;
            }
            int npcType = -1;
            HolyGameLogic.Instance.EmsgToss_AskAbsorb(npcType);//通知服务器请求吸附
        }

        void CreateSoleSoldier()
        {
            SoleSoldierCtrl.Instance.Enter();
      
        }

        /// <summary>
        /// 准备释放技能
        /// </summary>
        /// <param name="skType">Sk type.</param>
        /// 技能类型
        public void SendPreparePlaySkill(SkillTypeEnum skType)
        {
            int skillID = GetSkillIdBySkillType(skType);
            //沉默 小谷
            //  if (HolyTech.Skill.BuffManager.Instance.isSelfHaveBuffType(1017)) return;
           
            if (skillID == 0)
            {
                MsgInfoManager.Instance.ShowMsg((int)ErrorTypeEnum.AbsentSkillNULL);
                return;
            }
            HolyGameLogic.Instance.EmsgToss_AskUseSkill((uint)skillID);
        }

        public override int GetSkillIdBySkillType(SkillTypeEnum type)
        {
            return GetSkillId(type);
        }

        public override void OnAttackEnd()
        {
            RealEntity.PlayerFreeAnimation();
        }


        // /////////////////////////////////////////////////////////////////////////////////////////////////


        //离开技能
        public override void OnEntityLeadSkill()
        {
            base.OnEntityLeadSkill();//执行Ientity  Iplayer中为空
            ProgressBarInterface.startProgressBar(ProgressBarInterface.BarType.BarSkill, EntitySkillID);
        }

        //释放技能
        public override void OnEntityReleaseSkill()
        {
            //小谷注释

            //if (EntitySkillID == 250101)//回城技能的id不是这个  
            //{
            //    Debug.Log("回城技能");
            //    ProgressBarInterface.startProgressBar(ProgressBarInterface.BarType.BarSkill);
            //}
            //else
            {
                bPlaySkill = true;
                base.OnEntityReleaseSkill();

            }

        }
        // 进入Free状态
        public override void OnEnterFree()
        {
            base.OnEnterFree();
            Thanos.AudioManager.Instance.StopHeroAudio();//停止音效
        }

        //进入先行状态
        public override void OnEnterEntityAdMove()
        {
            base.OnEnterEntityAdMove();//空
        }

        //执行先行状态
        public override void OnExecuteEntityAdMove()
        {
            base.OnExecuteEntityAdMove();//父级没有内容
            Quaternion DestQuaternion = Quaternion.LookRotation(EntityFSMDirection);
            Quaternion sMidQuater = Quaternion.Lerp(RealEntity.GetTransform().rotation, DestQuaternion, 10 * Time.deltaTime);
            RealEntity.GetTransform().rotation = sMidQuater;

            this.RealEntity.PlayerRunAnimation();//播放walk的动画

        }

        // Run状态进入时调用
        public override void OnEnterMove()
        {
            Thanos.AudioManager.Instance.PlayHeroAudio(WalkAudioClip);//播放行走音效
            base.OnEnterMove();
        }
    
        // 执行强制移动
        public override void OnExecuteForceMove()
        {
            //base.OnExecuteForceMove();
            //方向控制
            Quaternion DestQuaternion = Quaternion.LookRotation(EntityFSMDirection);
            Quaternion sMidQuater = Quaternion.Lerp(RealEntity.GetTransform().rotation, DestQuaternion, 10 * Time.deltaTime);
            RealEntity.GetTransform().rotation = sMidQuater;

            float fTimeSpan = Time.realtimeSinceStartup - m_pcGOSSI.fBeginTime;
            float fMoveDist = fTimeSpan * m_pcGOSSI.fForceMoveSpeed;

            m_pcGOSSI.sServerSyncPos = m_pcGOSSI.sServerBeginPos + m_pcGOSSI.sServerDir * fMoveDist;
            SkillMoveConfig skillInfo = ConfigReader.GetSkillMoveConfig(EntitySkillModelId);

            if (skillInfo != null)
            {
                this.RealEntity.PlayerAnimation(skillInfo.action);
            }

            //2D位置同步
            Vector3 serverPos2D = new Vector3(m_pcGOSSI.sServerSyncPos.x, 60, m_pcGOSSI.sServerSyncPos.z);
            Vector3 realPos2D = new Vector3(RealEntity.GetTransform().position.x, 60, RealEntity.GetTransform().position.z);

            Vector3 sSyncDir = serverPos2D - realPos2D;
            sSyncDir.y = 0;
            sSyncDir.Normalize();
            float fDistToServerPos = Vector3.Distance(serverPos2D, realPos2D);
            float fThisMoveSpeed = EntityFSMMoveSpeed;

            if (fDistToServerPos > 10)
            {
                RealEntity.GetTransform().position = m_pcGOSSI.sServerBeginPos;
                GameMethod.GetMainCamera.FixedUpdatePosition();
                return;
            }
            Vector3 pos = realPos2D + EntityFSMMoveSpeed * Time.deltaTime * sSyncDir;

            //this.realObject.transform.position = pos;
            this.RealEntity.Controller.Move(sSyncDir * m_pcGOSSI.fForceMoveSpeed * Time.deltaTime);
            GameMethod.GetMainCamera.FixedUpdatePosition();
        }

        //进入施法吟诵状态
        public override void OnEnterSing()
        {
            base.OnEnterSing();//Ientity 中
            SkillManagerConfig skillconfig = ConfigReader.GetSkillManagerCfg(EntitySkillID);
            ProgressBarInterface.startProgressBar(skillconfig.yTime);//进度条
        }

        //退出施法吟诵状态
        public override void OnExitSing()
        {
            base.OnExitSing();//基类中主要就是销毁特效，停止声音
            ProgressBarInterface.hideProgressBar();//隐藏进度条
        }

        //进入死亡状态
        public override void OnEnterDead()
        {
            VirtualStickUI.Instance.SetVirtualStickUsable(false); //禁用虚拟摇杆

            if (GameMethod.GetCameraGray != null)
            {
                GameMethod.GetCameraGray.enabled = true;//摄像机灰度处理
                this.StrategyTick = Time.time;//开始计时
            }
            this.SetSyncLockTarget(null);//锁定的目标清空
            RealEntity.PlayerDeadAnimation();//播放死亡动画

            Thanos.AudioManager.Instance.PlayEffectAudio(DeadAudioClip);//播放死亡音效
             //当英雄死了之后，就停止播放该英雄相关的所有声音
            Thanos.AudioManager.Instance.StopHeroAudio();
            //移除警告
            RemoveRuinWarning();
          
            this.DoEntityDead(); //血条隐藏，角色控制器禁用
        }

        //执行死亡状态
        public override void OnExecuteDead()
        {
            if (GameMethod.GetCameraGray != null && GameMethod.GetCameraGray.enabled)
            {
                float time = Time.time - this.StrategyTick;//得到的是一个间隔，StrategyTick在OnEnterDead（）进行赋值   

            }
        }

// ////////////////////////////////////////////////////////////////////////////////////

        public override void SetHp(float curHp)
        {
            base.SetHp(curHp);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerHpChange);
            //if(UIAvatarInfo.Instance != null){
            //    UIAvatarInfo.Instance.UpdateHpSlider();
            //}
            AddRuinWarning();
        }

        public override void SetHpMax(float curHpMax)
        {
            base.SetHpMax(curHpMax);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerHpChange);
            //if(UIAvatarInfo.Instance != null){
            //    UIAvatarInfo.Instance.UpdateHpSlider();
            //}
            AddRuinWarning();
        }

        public override void SetMp(float curMp)
        {
            base.SetMp(curMp);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerMpChange);

        }

        public override void SetMpMax(float curMpMax)
        {
            base.SetMpMax(curMpMax);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerMpChange);

        }

        public override void SetLevel(int curLv)
        {
            base.SetLevel(curLv);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerLevelChange, curLv);
            if (curLv != 1)
            {
                SetUpdateSkillId(curLv);

                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SkillDescribleUpdate);
            }
        }

        //初始化技能升级列表
        private void InitSkillDic(Dictionary<SkillTypeEnum, int> skillDic)
        {
            int id = (int)ObjTypeID;
            HeroConfigInfo heroInfo = ConfigReader.GetHeroInfo(id);
            skillDic.Add(SkillTypeEnum.SKILL_TYPE1, heroInfo.HeroSkillType1);
            skillDic.Add(SkillTypeEnum.SKILL_TYPE2, heroInfo.HeroSkillType2);
            skillDic.Add(SkillTypeEnum.SKILL_TYPE3, heroInfo.HeroSkillType3);
            skillDic.Add(SkillTypeEnum.SKILL_TYPE4, heroInfo.HeroSkillType4);
            skillDic.Add(SkillTypeEnum.SKILL_TYPEABSORB1, 0);
            skillDic.Add(SkillTypeEnum.SKILL_TYPEABSORB2, 0);
        }

        private void CleanSkillDic(Dictionary<SkillTypeEnum, int> skillDic)
        {
            skillDic.Clear();
        }

        //技能升级时获得对应技能的Id
        private void SetUpdateSkillId(int lv)
        {
            SetSkillUpdate(SkillTypeEnum.SKILL_TYPE1, lv);
            SetSkillUpdate(SkillTypeEnum.SKILL_TYPE2, lv);
            SetSkillUpdate(SkillTypeEnum.SKILL_TYPE3, lv);
            SetSkillUpdate(SkillTypeEnum.SKILL_TYPE4, lv);
            //暂时屏蔽随从技能升级
            //SetSkillUpdate(SkillType.SKILL_TYPEABSORB1, lv);
            //SetSkillUpdate(SkillType.SKILL_TYPEABSORB2, lv);            
        }

        //更具技能类型，换算出满足指定技能的准确id
        private void SetSkillUpdate(SkillTypeEnum skillType, int lv)
        {
            int baseId = 0;
            if (!BaseSkillIdDic.TryGetValue(skillType, out baseId)) return;//技能id不存在
            SkillManagerConfig info = ConfigReader.GetSkillManagerCfg(baseId);
            if (baseId == 0 || info == null)
            {
                return;//不存在技能信息 
            }

            for (int i = baseId + SKILL_UPDATE_TOTAL_LEVEL - 1; i >= 0; i--)
            {
                SkillManagerConfig infoNew = ConfigReader.GetSkillManagerCfg(i);
                if (i == 0 || infoNew == null || infoNew.n32UpgradeLevel > lv)
                    continue;
                SkillIdDic[skillType] = i;
                break;
            }
        }

        //当更改基础技能时，比如随从技能改变
        private void SetBaseSkillType(SkillTypeEnum skillType, int skillId)
        {

            BaseSkillIdDic[skillType] = skillId;

            //暂时屏蔽随从技能升级
            if (skillType == SkillTypeEnum.SKILL_TYPEABSORB1 || skillType == SkillTypeEnum.SKILL_TYPEABSORB2)
            {
                SkillIdDic[skillType] = skillId;
                return;
            }
            SkillManagerConfig info = ConfigReader.GetSkillManagerCfg(skillId);
            if (skillId != 0 && info != null)
            {
                SetSkillUpdate(skillType, Level);
            }
            else if (skillId == 0)
            {
                SkillIdDic[skillType] = skillId;
            }
        }

        //得到换算后的技能id
        private int GetSkillId(SkillTypeEnum type)
        {
            int baseId = 0;
            if (SkillIdDic.TryGetValue(type, out baseId))
            {
                return baseId;
            }
            return 0;
        }

        public override void OnSkillInfoChange(int skillID, float time, float maxTime, int slot)
        {
            SkillTypeEnum skillType = SkillTypeEnum.SKILL_NULL;
            if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPE1])
            {
                skillType = SkillTypeEnum.SKILL_TYPE1;
            }
            else if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPE3])
            {
                skillType = SkillTypeEnum.SKILL_TYPE3;
            }
            else if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPE2])
            {
                skillType = SkillTypeEnum.SKILL_TYPE2;
            }
            else if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPE4])
            {
                skillType = SkillTypeEnum.SKILL_TYPE4;
            }
            else if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPEABSORB1])
            {
                skillType = SkillTypeEnum.SKILL_TYPEABSORB1;
            }
            else if (skillID == SkillIdDic[SkillTypeEnum.SKILL_TYPEABSORB2])
            {
                skillType = SkillTypeEnum.SKILL_TYPEABSORB2;
            }
            else
            {
                return;
            }
            SetBaseSkillType(skillType, skillID);
            if (time > 0)
            {
                bPlaySkill = false;
            }

            EventCenter.Broadcast<SkillTypeEnum, float, float>((Int32)GameEventEnum.GameEvent_LocalPlayerSkillCD, skillType, maxTime, time);
        }
        //获取对应技能的冷却时间
        public float getSkillCoolDownTime(int skillID)
        {
            if (ConfigReader.GetSkillManagerCfg(skillID) == null)
            {
                return 10.0f;
            }
            return ConfigReader.GetSkillManagerCfg(skillID).coolDown / 1000.0f;
        }

        public override void SetExp(float curExp)
        {
            base.SetExp(curExp);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerExpChange);   
        }
        

        public override void InitWhenCreateModel()
        {
            // base.InitWhenCreateModel();
            if (PlayerPrefs.HasKey(GameMethod.RangeVoc))
            {
                int sKey = PlayerPrefs.GetInt(GameMethod.RangeVoc);
                bool state = (sKey == 1) ? true : false;
                GameMethod.SetRange(state);
            }

            bool effectState = false;
            if (PlayerPrefs.HasKey(GameMethod.EffectVoc))
            {
                int sKey = PlayerPrefs.GetInt(GameMethod.EffectVoc);
                effectState = (sKey == 1) ? true : false;
            }
            GameMethod.SetEffect(effectState);

            InitSkillDic(SkillIdDic);
            InitSkillDic(BaseSkillIdDic);

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerInit);

            ResourceItem WalkAudioClipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_HERO_WALKSOUND + ConfigReader.GetHeroInfo(this.NpcGUIDType).un32WalkSound, ResourceType.ASSET);
            WalkAudioClip = WalkAudioClipUnit.Asset as AudioClip;

            ResourceItem DeadAudioClipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_HERO_DEADSOUND + ConfigReader.GetHeroInfo(this.NpcGUIDType).HeroDeathSould, ResourceType.ASSET);
            DeadAudioClip = DeadAudioClipUnit.Asset as AudioClip;

            ResourceItem EnterAltarClipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_ENTERALTAR_SOUND, ResourceType.ASSET);
            EnterAltarClip = EnterAltarClipUnit.Asset as AudioClip;

            ResourceItem RebornClipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_REBORN_SOUND, ResourceType.ASSET);
            RebornClip = RebornClipUnit.Asset as AudioClip;

            base.InitWhenCreateModel();
            EventCenter.AddListener<UInt64>((Int32)GameEventEnum.GameOver, OnGameOver);

            OnCameraUpdatePosition();
        }

        private void OnGameOver(UInt64 BsGuid)
        {
            RemoveRuinWarning();
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerRange, UpdataRange);
            EventCenter.RemoveListener<UInt64>((Int32)GameEventEnum.GameOver, OnGameOver);
        }

        private AudioClip DeadAudioClip;
        private AudioClip WalkAudioClip;
        private AudioClip EnterAltarClip;
        private AudioClip RebornClip;

        public AudioClip GetAltarClip()
        {
            return EnterAltarClip;
        }
        protected override void UpdateFuryEffect()
        {
            base.UpdateFuryEffect();
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryEffect);
        }

        protected override void UpdateFuryValue()
        {
            base.UpdateFuryValue();
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryValue);
        }

        public override void SetFuryState(EFuryState state)
        {
            base.SetFuryState(state);
            if (state == EFuryState.eFuryNullState)
            {
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryValue);
            }
            EventCenter.Broadcast<EFuryState>((Int32)GameEventEnum.GameEvent_FuryStateInfo, state);
        }



        public void AddUserGameItems(int index, int id, int count, float last)
        {
            UserGameItems[index] = id;
            UserGameItemsCount[index] = count;
            UserGameItemsCoolDown[index] = last;
        }

        void UpdateCoolDownGameItems()
        {
            if (UserGameItemsCoolDown == null || UserGameItemsCoolDown.Count == 0)
                return;
            for (int i = 0; i < UserGameItemsCoolDown.Count; i++)
            {
                if (UserGameItemsCoolDown.ElementAt(i).Value > Time.deltaTime)
                {
                    float time = (float)UserGameItemsCoolDown.ElementAt(i).Value;
                    time -= Time.deltaTime;
                    UserGameItemsCoolDown[UserGameItemsCoolDown.ElementAt(i).Key] = time;
                }
                else
                {
                    UserGameItemsCoolDown[UserGameItemsCoolDown.ElementAt(i).Key] = 0f;
                }
            }
        }

        public void AddRuinWarning()
        {
            if (GameStateManager.Instance.GetCurState().GetStateType() != GameStateTypeEnum.Play)
                return;
            if (HpMax == 0)
            {
                return;
            }
            float rate = Hp / HpMax;
            if (rate >= hpRate || rate <= 0)
            {
                RemoveRuinWarning();
                return;
            }

            SendHpLessWarning(true);

            if (objWarning != null)
            {
                return;
            }
           
            //objWarning = GameObject.Instantiate(Resources.Load(GameDefine.GameConstDefine.WarningRuin)) as GameObject;
            ResourceItem objWarningUnit = ResourcesManager.Instance.loadImmediate(GameDefine.GameConstDefine.WarningRuin, ResourceType.PREFAB);
            objWarning = GameObject.Instantiate(objWarningUnit.Asset) as GameObject;

            objWarning.transform.parent = WindowManager.Instance.GetWindow(EWindowType.EWT_GamePlayWindow).GetRoot();
            objWarning.transform.localPosition = Vector3.zero;
            objWarning.transform.localScale = Vector3.one;
            UITweener tween = TweenAlpha.Begin(objWarning, duringTime, 0f);
            tween.method = UITweener.Method.BounceIn;
            tween.style = UITweener.Style.PingPong;
        }

        private void SendHpLessWarning(bool tag)
        {
            FEvent eve = new FEvent((Int32)(Int32)GameEventEnum.GameEvent_NotifyHpLessWarning);
            eve.AddParam("Tag", tag);
            EventCenter.SendEvent(eve);
        }

        public void RemoveRuinWarning()
        {
            if (objWarning == null)
            {
                return;
            }
            SendHpLessWarning(false);
            GameObject.DestroyImmediate(objWarning);
            objWarning = null;
        }

        public override void CleanWhenGameOver()
        {
            UserGameItems.Clear();
            UserGameItemsCount.Clear();
            UserGameItemsCoolDown.Clear();
            CleanSkillDic(SkillIdDic);
            CleanSkillDic(BaseSkillIdDic);
            SetFuryValue(0);
            SetFuryState(EFuryState.eFuryNullState);
            if (AbsorbMonsterType != null)
            {
                AbsorbMonsterType[0] = 0;
                AbsorbMonsterType[1] = 0;
            }

            BattleingData.AllBlueHeroBattle.Clear();
            BattleingData.AllRedHeroBattle.Clear();
        }

     
       
    }
}

