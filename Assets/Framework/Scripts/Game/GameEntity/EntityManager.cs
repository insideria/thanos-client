using UnityEngine;
using System.Collections.Generic;
using GameDefine;
using Thanos.GameData;
using System;

namespace Thanos.GameEntity
{
    public class EntityManager
    {
        public static EntityManager Instance
        {
            private set;
            get;
        }
        //存储所有的实体
        public static Dictionary<UInt64, IEntity> AllEntitys = new Dictionary<UInt64, IEntity>();

        //战队标签
        public enum CampTag
        {
            SelfCamp = 1,
            EnemyCamp = 0,
        }

        static int[] HOME_BASE_ID = { 21006, 21007, 21020, 21021 };

        private static List<IEntity> homeBaseList = new List<IEntity>();

        public EntityManager()
        {
            Instance = this;
        }

        //显示实例
        public void ShowEntity(UInt64 sGUID, Vector3 pos, Vector3 dir)
        {
            if (!AllEntitys.ContainsKey(sGUID) || AllEntitys[sGUID].realObject == null)
            {
                return;
            }
            //显示实例
            AllEntitys[sGUID].realObject.transform.position = pos;
            AllEntitys[sGUID].realObject.transform.rotation = Quaternion.LookRotation(dir);
            AllEntitys[sGUID].realObject.SetActive(true);

            //如果实例的状态机存在而且状态不是死亡状态
            if (AllEntitys[sGUID].FSM != null && AllEntitys[sGUID].FSM.State != Thanos.FSM.FsmStateEnum.DEAD)
            {
                //显示血条
                AllEntitys[sGUID].ShowBloodBar();
            }
            else if (AllEntitys[sGUID].FSM != null && AllEntitys[sGUID].FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
            {
                //隐藏血条
                AllEntitys[sGUID].HideBloodBar();
            }

        }

        //隐藏实例  
        public void HideEntity(UInt64 sGUID)
        {
            if (!AllEntitys.ContainsKey(sGUID))
            {
                return;
            }

            IEntity entity = null;
            //如果字典中存在这个实例并且实例类型是Player
            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity) && entity.entityType == EntityTypeEnum.Player)
            {
                //
                if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
                {
                    IPlayer.AddOrDelEnemy((IPlayer)entity,false);
                }
            }
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player != null && player.SyncLockTarget == AllEntitys[sGUID])
            {
                //锁定对象实例为空
                player.SetSyncLockTarget(null);
            }
            //隐藏血条
            AllEntitys[sGUID].HideBloodBar();
            //禁用实例
            AllEntitys[sGUID].realObject.active = false;

        }

        //创建实体
        public virtual IEntity HandleCreateEntity(UInt64 sGUID, EntityCampTypeEnum campType)
        { 
            return new IEntity(sGUID, campType);
        }

        //销毁实体
        public void DestoryAllEntity()
        {
            List<UInt64> keys = new List<UInt64>();
 
            foreach (IEntity entity in AllEntitys.Values)
            {
                if (entity.entityType != EntityTypeEnum.Building)
                {
                    keys.Add(entity.GameObjGUID);
                }
            }

            foreach (UInt64 gui in keys)
            {
                HandleDelectEntity(gui);
            }
        }

        //将实例添加到字典中
        public void AddEntity(UInt64 sGUID, IEntity entity)
        {
            if (AllEntitys.ContainsKey(sGUID))
            {
                Debug.LogError("Has the same Guid: " + sGUID);
                return;
            }
            AllEntitys.Add(sGUID, entity);
        }

        //从字典中获取实例
        public virtual IEntity GetEntity(UInt64 id)
        {
            IEntity entity;
            if (AllEntitys.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }

        //创建实体模型
        public GameObject CreateEntityModel(IEntity entity, UInt64 sGUID, Vector3 dir, Vector3 pos)
        {
            if (entity != null)
            {
                int id = (int)entity.ObjTypeID;
                this.SetCommonProperty(entity, id);//设置公共属性 设置模型名称与id
                if (entity.ModelName == null || entity.ModelName == "")
                {
                    return null;
                }
               
                //获取模型路径（小兵，野怪）     LoadMonsterModels = "Monsters/";
                string path = GameDefine.GameConstDefine.LoadMonsterModels;

                //创建GameObject    
                string resPath = path + entity.ModelName;//获取模型路径
                entity.realObject = ObjectPool.Instance.GetGO(resPath);//实例化对象
                if (entity.realObject == null)
                {
                    Debug.LogError("entity realObject is null");
                }
                //填充Entity信息
                entity.resPath = resPath; //实体路径
                entity.objTransform = entity.realObject.transform; 

                entity.realObject.transform.localPosition = pos;//实体位置
                entity.realObject.transform.localRotation = Quaternion.LookRotation(dir);//实体朝向

                //如果模型是玩家
                if (entity is IPlayer)
                {       
                    entity.entityType = EntityTypeEnum.Player;   //类型指定
                    //判断是否是我方队友
                    if (entity is ISelfPlayer)
                    {
                        PlayerManager.Instance.LocalPlayer = (ISelfPlayer)entity;
                    }
                }
                else//如果不是玩家 肯定就是NPC
                {
                    entity.entityType = (EntityTypeEnum)ConfigReader.GetNpcInfo(id).NpcType;//实体类型
                    //实体类型属于野怪并且阵营属于A或者B   ,那么就一定是箭塔士兵
                    if (entity.entityType == EntityTypeEnum.Monster && (int)entity.EntityCamp >= (int)EntityCampTypeEnum.A)
                    { 
                        entity.entityType = EntityTypeEnum.AltarSoldier;
                    }
                    //设置实体的碰撞半径
                    entity.ColliderRadius = ConfigReader.GetNpcInfo(id).NpcCollideRadius / 100f;
                }
                //创建血条
                if (entity.NPCCateChild != NPCCateChildEnum.BUILD_Shop)
                {

                    entity.CreateBloodBar();
                }

                AddEntityComponent(entity);//添加entity组件
                return entity.realObject;
            }
            return null;
        }

        //设置entity基本属性、读取配置表
        public virtual void SetCommonProperty(IEntity entity, int id)
        {
            entity.ModelName = GetModeName(id);
            entity.NpcGUIDType = id;
        }

        public static int HandleDelectEntity(UInt64 sGUID)
        {
            if (!AllEntitys.ContainsKey(sGUID))
            {
                return (int)ResultEnum.DelEntityFailed;
            }
            IEntity entity = null;

            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity) && entity.entityType == EntityTypeEnum.Player)
            {
                if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
                {
                    IPlayer.AddOrDelEnemy((IPlayer)entity, false);
                }
            }
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;

            if (player != null && player.SyncLockTarget == AllEntitys[sGUID])
            {
                player.SetSyncLockTarget(null);
            }
            if (entity.entityType == EntityTypeEnum.Building)
            {
                MonoBehaviour.DestroyImmediate(AllEntitys[sGUID].realObject);
            }
            else {
                //删除GameObject 
                ObjectPool.Instance.ReleaseGO(AllEntitys[sGUID].resPath, AllEntitys[sGUID].realObject, PoolObjectTypeEnum.Entity);
            }
            AllEntitys[sGUID].DestroyBloodBar();
            AllEntitys[sGUID] = null;
            AllEntitys.Remove(sGUID);

            return (int)ResultEnum.Normal;
        }
    
        public static void DelectBombBase(IEntity entity)
        {
            MonoBehaviour.DestroyImmediate(entity.realObject);
            entity.DestroyBloodBar();
            entity = null;
        }

        //获取模型名称
        protected virtual string GetModeName(int id)
        {
            return null;
        }

        //为建筑模型添加组件
        public static Entity AddBuildEntityComponent(IEntity entity)
        {
            //添加Entity组件
            Entity syncEntity = (Entity)entity.realObject.AddComponent<Entity>();         
            entity.RealEntity = syncEntity;
            syncEntity.SyncEntity = entity;
            syncEntity.mCampType = entity.EntityCamp;
            syncEntity.mGUID = (int)entity.GameObjGUID;
            syncEntity.mEntityType = entity.entityType;
            syncEntity.mNPCCateChild = entity.NPCCateChild;
            return syncEntity;
        }

        //添加实例组件
        public static void AddEntityComponent(IEntity entity)
        {
            //没有，添加Entity组件
            if (entity.realObject.GetComponent<Entity>() == null)
            {
                Entity syncEntity = entity.realObject.AddComponent<Entity>() as Entity;
                syncEntity.SyncEntity = entity;
                syncEntity.mCampType = entity.EntityCamp;
                syncEntity.mGUID = (int)entity.GameObjGUID;
                syncEntity.mEntityType = entity.entityType;
                syncEntity.mNPCCateChild = entity.NPCCateChild;

                //添加动态模型优化组件 
                if (entity.entityType == EntityTypeEnum.Monster || entity.entityType == EntityTypeEnum.Soldier || entity.entityType == EntityTypeEnum.AltarSoldier)
                {
                    OptimizeDynamicModel optimizeDynamicModel = entity.realObject.AddComponent<OptimizeDynamicModel>() as OptimizeDynamicModel;
                    syncEntity.mDynModel = optimizeDynamicModel;
                }

                entity.RealEntity = syncEntity;
            }
            //直接取
            else
            {
                Entity syncEntity = entity.realObject.GetComponent<Entity>() as Entity;
                syncEntity.SyncEntity = entity;
                syncEntity.mCampType = entity.EntityCamp;
                syncEntity.mGUID = (int)entity.GameObjGUID;
                syncEntity.mEntityType = entity.entityType;
                syncEntity.mNPCCateChild = entity.NPCCateChild;
                entity.RealEntity = syncEntity;
            }
        }
      
        //  =================================================和基地有关的===============================================
        public static void AddHomeBase(IEntity entity)
        {
            if (entity == null || entity.entityType != EntityTypeEnum.Building)
                return;

            for (int i = 0; i < HOME_BASE_ID.Length; i++)
            {
                if (HOME_BASE_ID[i] == entity.NpcGUIDType)
                {
                    homeBaseList.Add(entity);
                    break;
                }
            }
        }

        public static void ClearHomeBase()
        {
            homeBaseList.Clear();
        }

        public static bool IsSelfHomeBase(IEntity entity)
        {
            if (entity == null)
                return false;
            if (PlayerManager.Instance.LocalPlayer != null && PlayerManager.Instance.LocalPlayer.EntityCamp != entity.EntityCamp)
                return false;
            for (int i = 0; i < HOME_BASE_ID.Length; i++)
            {
                if (HOME_BASE_ID[i] == entity.NpcGUIDType)
                {
                    return true;
                }
            }
            return false;
        }


        public static IEntity GetHomeBase(EntityCampTypeEnum type)
        {
            foreach (var item in homeBaseList)
            {
                if (item.IsSameCamp(type))
                {
                    return item;
                }
            }
            return null;
        }

        public static List<IEntity> GetHomeBaseList()
        {
            return homeBaseList;
        }

    }
}