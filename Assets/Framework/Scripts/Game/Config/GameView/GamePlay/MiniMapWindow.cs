using UnityEngine;
using System;
using System.Collections.Generic;
using Thanos.GameEntity;
using GameDefine;
using System.Linq;
using Thanos.GuideDate;
using Thanos.Model;

namespace Thanos.View
{
    public class MiniMapWindow : BaseWindow
    {
        public MiniMapWindow() 
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.LoadMiniMapUI;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
            mInited = false;
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            mMapPanel = mRoot.Find("Hold/Panel");//获取父物体
            //加载地图信息
            MapInfo map = MapLoadConfig.Instance.GetMapInfo(GameUserModel.Instance.GameMapID);

            if (map != null)
            {
                string path = "Guis/Play/" + map.mMiniMap;
                mMapGround = LoadUiResource.AddChildObject(mMapPanel, path).transform;//加载地图
                mMapGround.localPosition = new Vector3(-150, -100, 0);
                mMapGround.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        //删除Login外其他控件，例如
        public static void DestroyOtherUI()
        {
          
        }
        //窗口控件释放
        protected override void RealseWidget()
        {
            mMapElementDic.Clear();
            mMapWarningDic.Clear();
        }

        //游戏事件注册
        protected override void OnAddListener()
        { 
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_UpdateMiniMap, OnUpdateMapEvent);//更新小地图   消息管理器Update()广播 
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_InitMiniMap, InitGameMap);//初始化地图 当服务器返回消息要加载游戏对象时广播
            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_AddMiniMap, OnAddMapEvent);//添加地图元素   当服务器返回消息要加载游戏对象时广播
            EventCenter.AddListener<UInt64>((Int32)GameEventEnum.GameEvent_RemoveMiniMap, OnRemoveMapEvent);//移除地图元素   当服务器返回 游戏对象消失的消息时广播
            EventCenter.AddListener<UInt64 , uint , UInt64>((Int32)GameEventEnum.GameEvent_BroadcastBeAtk, OnWarningEvent);//
            EventCenter.AddListener<UInt64>((Int32)GameEventEnum.GameEvent_RemoveMiniWarning, RemoveMapWarning);//移除地图警告
            
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_UpdateMiniMap, OnUpdateMapEvent);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_InitMiniMap, InitGameMap);
            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_AddMiniMap, OnAddMapEvent);
            EventCenter.RemoveListener<UInt64>((Int32)GameEventEnum.GameEvent_RemoveMiniMap, OnRemoveMapEvent);
            EventCenter.RemoveListener<UInt64, uint, UInt64>((Int32)GameEventEnum.GameEvent_BroadcastBeAtk, OnWarningEvent);
            EventCenter.RemoveListener<UInt64>((Int32)GameEventEnum.GameEvent_RemoveMiniWarning, RemoveMapWarning);
        }

        //显示
        public override void OnEnable()
        {
            mInited = false;
        }

        //隐藏
        public override void OnDisable()
        {
        }

        private Transform mMapGround;

        private Transform mMapPanel;

        private Dictionary<UInt64, UIMiniMapElement> mMapElementDic = new Dictionary<UInt64, UIMiniMapElement>();//存储地图元素

        private Dictionary<UInt64, UIMiniMapInterfaceWarning> mMapWarningDic = new Dictionary<UInt64, UIMiniMapInterfaceWarning>();
    
        private float mTimeBuildingWarning = 0f;
        private const float mWarningVoice = 20f;
        private const float mWarningSound = 10f;
        private bool mIsFirstAttack = false;
        private bool hasPlayWaringVoice = false;
        private bool hasPlayWarningSound = false;
        private bool mIsFirstWarning = false;

        private bool mInited = false; //是否已经初始化

        /// <summary>
        /// 增加地图元素
        /// </summary>
        /// <param name="entity"></param>
        private void AddMapElement(UInt64 guid, EntityCampTypeEnum type, float x, float y, float z)
        {
            if (!mInited)//如果已经初始化完成
                return;

            int index = (int)type;// 获取添加元素的阵营类型
            if (index <= (int)EntityCampTypeEnum.Null) return; //判断阵营是否是空

            UIMiniMapElement element = GetMapElement(guid);//从mMapElementDic中获取地图元素

            if (element == null)
            {
                element = CreateMapElement(guid, type, x, y, z);//创建地图元素图标
                if (element == null) return;

                mMapElementDic.Add(guid, element);

                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ChangeMapState);
            }

        }

        /// <summary>
        /// 删除地图元素
        /// </summary>
        ///</param>
        private void DestroyMapElement(UInt64 guid)
        {
            //根据id得到地图元素
            UIMiniMapElement element = GetMapElement(guid);
            if (element == null) return;
            
            //将小地图类型的对象池中的所有对象做无效处理
            ObjectPool.Instance.ReleaseGO(element.resPath, element.gameObject, PoolObjectTypeEnum.MiniMap);

            //从字典中移除此元素
            mMapElementDic.Remove(guid);

            // 广播此消息  然后ChangeMapState被调用，将图片禁用
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ChangeMapState);
        }

        /// <summary>
        /// 更新地图元素
        /// </summary>
        /// <param name="entity"></param>
        private void UpdateMapElement(UInt64 guid, EntityCampTypeEnum type, float x, float y, float z)
        {
            int index = (int)type;//获取实体阵营类型
            if (index <= (int)EntityCampTypeEnum.Kind) //全敌对，全和平类型元素不更新
                return;
            UIMiniMapElement element = GetMapElement(guid);//获得Ientity所对应的地图元素  从mMapElementDic获取
            if (element != null)
            {
                element.UpdatePosDirect(x, y, z);// 将对象直接设置到目标点
            }

        }

        private void AddMapWarning(UInt64 guid)
        {
            if (!EntityManager.AllEntitys.ContainsKey(guid))
                return;
            IEntity entity = EntityManager.AllEntitys[guid];
            if (entity == null || entity.realObject == null || !entity.realObject.activeInHierarchy)
                return;
            bool isAllow = false;

            if (entity.entityType != EntityTypeEnum.Player && entity.entityType != EntityTypeEnum.Soldier && entity.entityType != EntityTypeEnum.AltarSoldier)
            {
                isAllow = false;
            }
            if (SceneGuideTaskManager.Instance().IsNewsGuide() != SceneGuideTaskManager.SceneGuideType.NoGuide && (entity.NpcGUIDType == 21017
             || entity.NpcGUIDType == 21025 || entity.NpcGUIDType == 21024))
            {
                isAllow = true;
            }
            if (!isAllow)
                return;
            if (entity.FSM != null && entity.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
                return;
            UIMiniMapInterfaceWarning element = GetMapWarning(guid);
            Vector3 pos = entity.realObject.transform.position;
            if (element == null)
            {
                element = CreateMapElementWarning(guid, pos.x, pos.y, pos.z);
                mMapWarningDic.Add(guid, element);
            }
            else
            {
                element.UpdatePosDirect(pos.x, pos.y, pos.z);
            }
        }

        private void AddBuildingWarningVoice(UInt64 sGUID)
        {
            if (!EntityManager.AllEntitys.ContainsKey(sGUID))
                return;

            IEntity entity = EntityManager.AllEntitys[sGUID];
            if (!entity.IfNPC())
                return;

            if (entity == null || entity.realObject == null || entity.entityType != EntityTypeEnum.Building)
                return;
            if (entity.FSM != null && entity.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
                return;

            if(!mIsFirstAttack)
            {
                LoadSound(GameDefine.GameConstDefine.WarningBuildingVoice);
               
                mIsFirstAttack = true;
                hasPlayWaringVoice = true;             
                return;
            }

            if (mTimeBuildingWarning == 0)
            {
                mTimeBuildingWarning = Time.time;
            }

            if (Time.time - mTimeBuildingWarning >= mWarningVoice)
            {
                LoadSound(GameDefine.GameConstDefine.WarningBuildingVoice);
                mTimeBuildingWarning = Time.time;
            
                //Debug.Log("mWarningVoice");
                hasPlayWaringVoice = true;
                hasPlayWarningSound = true;
            }


            //二十秒内受到攻击则播放警告音效
             if ((Time.time - mTimeBuildingWarning <= mWarningSound) && (hasPlayWaringVoice) && (!mIsFirstWarning))
            {

                LoadSound(GameDefine.GameConstDefine.WarningBuildingSound);
               
                mTimeBuildingWarning = Time.time;
            
                hasPlayWarningSound = true;
                mIsFirstWarning = true;
                //Debug.Log("WarningBuildSound 二十秒内受到攻击则播放警告音效");
            }

            //持续受到攻击时播放警告音效十秒之后才会再次播放
            if ((Time.time - mTimeBuildingWarning > mWarningSound)  && (hasPlayWarningSound) )
            {

                LoadSound(GameDefine.GameConstDefine.WarningBuildingSound);
                mIsFirstWarning = false;
                mTimeBuildingWarning = Time.time;
             
                //Debug.Log("持续受到攻击时播放警告音效十秒之后才会再次播放");
            }

        }

        private void LoadSound(string audio)
        {
            AudioClip clip = Resources.Load(audio) as AudioClip;
            AudioSource source = AudioManager.Instance.PlayEffectAudio(clip);
            source.volume = 1f;
        }


        /// <summary>
        /// 获得Ientity所对应的地图元素
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private UIMiniMapElement GetMapElement(UInt64 guid)
        {
            if (mMapElementDic == null || mMapElementDic.Count == 0)
                return null;
            UIMiniMapElement element = null;
            mMapElementDic.TryGetValue(guid, out element);
            return element;
        }

        private UIMiniMapInterfaceWarning GetMapWarning(UInt64 guid)
        {
            if (mMapWarningDic == null || mMapWarningDic.Count == 0)
                return null;
            UIMiniMapInterfaceWarning element = null;
            mMapWarningDic.TryGetValue(guid, out element);
            return element;
        }

        private void Clear()
        {
            for (int i = mMapElementDic.Count - 1; i >= 0; i--)
            {
                if (mMapElementDic.ElementAt(i).Value == null)
                    continue;

                ObjectPool.Instance.ReleaseGO(mMapElementDic.ElementAt(i).Value.resPath, mMapElementDic.ElementAt(i).Value.gameObject, PoolObjectTypeEnum.MiniMap);
            }
            mMapElementDic.Clear();
        }

        /// <summary>
        /// 创建地图元素图标
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private UIMiniMapElement CreateMapElement(UInt64 guid, EntityCampTypeEnum type, float x, float y, float z)
        {
            string path = null;

            bool blueTeam = true;
            if (PlayerManager.Instance.LocalPlayer!=null && type != PlayerManager.Instance.LocalPlayer.EntityCamp)
                blueTeam = false;

            IEntity entity = EntityManager.AllEntitys[guid];

            if (entity.entityType == EntityTypeEnum.Player)
            {
                path = GetPlayerElementPath(blueTeam);//获取蓝方或者红方图片路径
            }
            else if (entity.IfNPC())
            {
                path = GetNpcElementPath(entity,blueTeam);//获取NPC元素路径
            }
            if (string.IsNullOrEmpty(path))
                return null;
            return LoadElementResource(path, guid, x, y, z);//加载与元素资源并返回此元素
        }

        //获取玩家元素路径
        private string GetPlayerElementPath(bool blueTeam)
        {
            string path;
            if (blueTeam)
                path = GameDefine.GameConstDefine.PlayerMapGameObject_Green;//绿色菱形图标
            else
                path = GameDefine.GameConstDefine.PlayerMapGameObject_Red;//红色圆形图标
            return path;
        }

        //获取Npc元素路径
        private string GetNpcElementPath(IEntity entity,bool blueTeam)
        {
            string path = null;

            if (entity.entityType == EntityTypeEnum.Building || entity.entityType == EntityTypeEnum.Monster)
            {
                if (entity.NPCCateChild == NPCCateChildEnum.SmallMonster)//小怪
                {
                    path = GameDefine.GameConstDefine.NpcMinimapCreepsIconSmall;//模糊的骷髅   
                }
                else if (entity.NPCCateChild == NPCCateChildEnum.HugeMonster)// 大怪
                {
                    path = GameDefine.GameConstDefine.NpcMinimapCreepsIconHuge;//清楚的骷髅  
                }
                else if (entity.NPCCateChild == NPCCateChildEnum.BUILD_Altar)//小兵基地
                {
                    path = GameDefine.GameConstDefine.NpcMinimapBarracksIcon;//盾牌 
                }
                else if (entity.NPCCateChild == NPCCateChildEnum.BUILD_Base)//水晶塔 基地
                {
                    path = GameDefine.GameConstDefine.NpcMinimapBaseIcon;//清晰的牛角   
                }
                else if (entity.NPCCateChild == NPCCateChildEnum.BUILD_Shop)//商店图标
                {
                    path = GameDefine.GameConstDefine.NpcMinimapShopIcon; //金钱   
                }
                else if (entity.NPCCateChild == NPCCateChildEnum.BUILD_Tower)//攻击箭塔
                { 
                    path = GameDefine.GameConstDefine.NpcMinimapTowerIcon;  //模糊的牛角  
                }
            }
            else
            {
                if (blueTeam)
                    path = GameDefine.GameConstDefine.SoldierMapGameObject_Green;
                else
                    path = GameDefine.GameConstDefine.SoldierMapGameObject_Red;
            }
          
            return path;
        }

        private UIMiniMapInterfaceWarning CreateMapElementWarning(UInt64 guid, float x, float y, float z)
        {
            string path = null;
            path = GameDefine.GameConstDefine.MapWarning;
            return (UIMiniMapInterfaceWarning)LoadElementResource(path, guid, x, y, z);
        }

        //加载元素资源
        private UIMiniMapElement LoadElementResource(string path, UInt64 guid, float x, float y, float z)
        {
            GameObject obj;
            obj = ObjectPool.Instance.GetGO(path);
            obj.transform.parent = mMapGround.transform;
            obj.transform.localScale = Vector3.one;

            UIMiniMapElement element = obj.GetComponent<UIMiniMapElement>();
            element.resPath = path;
            element.CreateMiniMapElement(guid, x, y, z);

            obj.SetActive(false);
            obj.SetActive(true);
            return element;
        }


        // ///////////////////////////////////////////////// 游戏响应事件/////////////////////////////////// 
        private void OnWarningEvent(UInt64 ownerId, uint skillID, UInt64 targetID)
        {
            SkillAccountConfig skillAccConfig = ConfigReader.GetSkillAccountCfg(skillID);
            if (skillAccConfig == null || !skillAccConfig.isDmgEffect)
                return;
            if (!EntityManager.AllEntitys.ContainsKey(targetID))
                return;
            IEntity entity = EntityManager.AllEntitys[targetID];
            if (PlayerManager.Instance.LocalPlayer == null)
                return;
            if (!entity.IsSameCamp(PlayerManager.Instance.LocalPlayer.EntityCamp))
                return;
            if (entity == null || entity.realObject == null || !entity.realObject.activeInHierarchy)
                return;
            if (entity.entityType != EntityTypeEnum.Building && entity.entityType != EntityTypeEnum.Player)
            {
                if (SceneGuideTaskManager.Instance().IsNewsGuide() == SceneGuideTaskManager.SceneGuideType.NoGuide)
                {
                    return;
                }
                else
                {
                    if (entity.NpcGUIDType != 21017)//新手引导己方精灵女
                        return;
                }
            }
            AddMapWarning(targetID);
            AddBuildingWarningVoice(targetID);
        }

        private void InitGameMap()
        {
            //清除已有对象
            Clear();

            //地图初始化
            MapInfo map = MapLoadConfig.Instance.GetMapInfo(GameUserModel.Instance.GameMapID);

            if (!mInited && map != null)
            {
                IEntity player = PlayerManager.Instance.LocalPlayer;

                if (map.mCameraType == 1)
                {
                    if (player.EntityCamp == EntityCampTypeEnum.A)
                    {
                        mMapPanel.Rotate(new Vector3(0, 0, 45));
                    }
                    else
                    {
                        mMapPanel.Rotate(new Vector3(0, 0, 225));
                    }
                    mMapPanel.localPosition = new Vector3(-120, -120, 0);
                }
                else
                {
                    if (player.EntityCamp == EntityCampTypeEnum.A)
                    {
                        mMapPanel.Rotate(new Vector3(0, 0, 0));
                    }
                    else
                    {
                        mMapPanel.Rotate(new Vector3(0, 0, 180));
                    }
                    mMapPanel.localPosition = new Vector3(-130, -66, 0);
                }
            }

            mInited = true;

            //加入已经存在的对象，比如静态NPC
            foreach (IEntity entity in EntityManager.AllEntitys.Values)
            {
                OnAddMapEvent(entity);
            }

        }
        private void OnUpdateMapEvent()
        {
            for (int i = EntityManager.AllEntitys.Count - 1; i >= 0; i--)
            {
                IEntity entity = EntityManager.AllEntitys.ElementAt(i).Value;
                if (entity == null || entity.realObject == null || !entity.realObject.activeInHierarchy)
                    continue;
                if (entity.entityType != EntityTypeEnum.Player && entity.entityType != EntityTypeEnum.Soldier && entity.entityType != EntityTypeEnum.AltarSoldier)
                    continue;

                //死亡删除
                if (entity.FSM != null && entity.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
                {
                    OnRemoveMapEvent(entity.GameObjGUID);
                    continue;
                }

                //复活增加
                if (!mMapElementDic.ContainsKey(entity.GameObjGUID))
                {
                    OnAddMapEvent(entity);
                }

                Vector3 pos = entity.realObject.transform.position;
                UpdateMapElement(EntityManager.AllEntitys.ElementAt(i).Key, entity.EntityCamp, pos.x, pos.y, pos.z);

            }
        }

        private void OnAddMapEvent(IEntity entity)
        {
            if (entity == null || entity.realObject == null || !entity.realObject.activeInHierarchy)
                return;

            if (entity.entityType != EntityTypeEnum.Player && entity.entityType != EntityTypeEnum.Soldier && entity.entityType != EntityTypeEnum.AltarSoldier && entity.entityType != EntityTypeEnum.Building && entity.entityType != EntityTypeEnum.Monster)
                return;

            if (entity.FSM != null && entity.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
            {
                return;
            }

            Vector3 pos = entity.realObject.transform.position;
            AddMapElement(entity.GameObjGUID, entity.EntityCamp, pos.x, pos.y, pos.z);
        }

        private void OnRemoveMapEvent(UInt64 guid)
        {
            DestroyMapElement(guid);
        }


        /// <summary>
        /// 移出地图元素
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveMapWarning(UInt64 guid)
        {
            UIMiniMapInterfaceWarning element = GetMapWarning(guid);
            if (element == null)
            {
                return;
            }
            ObjectPool.Instance.ReleaseGO(element.resPath, element.gameObject, PoolObjectTypeEnum.MiniMap);

            mMapWarningDic.Remove(guid);

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ChangeMapState);
        }
    }
}