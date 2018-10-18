using Thanos.Tools;
using GSToGC;
using Thanos;
using Thanos.Network;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GamePlay : UnitySingleton<GamePlay> {
    public UISprite[] buttonSprite=new  UISprite[4];// 0 技能1   1技能2   2自动攻击  3 锁定
    public UIButton aotoAttack;
    public UIButton btnSkill_1;
    public UIButton btnSkill_2;
    public UIButton lockTarget;
    public System.UInt64 mMyGuid { get; set; }

    public GameObject  LocalPlayer { set; get;}
    public Dictionary<int, GameObject> mPlayerModel = new Dictionary<int, GameObject>();//模型
   
    public override void Awake()
    {
        base.Awake();
        EventCenter.AddListener<Stream, int>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, HandleNetMsg);
        EventCenter.AddListener<BroadcastBattleHeroInfo>((Int32)GameEventEnum.UserEvent_NotifyBattleHeroInfo, onNotifyBattleHeroInfor); 
        EventCenter.AddListener<GOAppear>((Int32)GameEventEnum.UserEvent_NotifyGameObjectAppear, onNotifyGameObjectAppear);
        EventCenter.AddListener<RunningState>((Int32)GameEventEnum.UserEvent_NotifyGameObjectRunState, OnNotifyGameObjectRunState);//移动状态
        EventCenter.AddListener<FreeState>((Int32)GameEventEnum.UserEvent_NotifyGameObjectFreeState, OnNotifyGameObjectFreeState);//自由状态
        EventCenter.AddListener<NotifyHPInfo>((Int32)GameEventEnum.UserEvent_NotifyHPInfo, OnNotifyHPInfo);
        EventCenter.AddListener<NotifyMPInfo>((Int32)GameEventEnum.UserEvent_NotifyMPInfo, OnNotifyMPInfo); 
    }

	void Start () {
        NetworkManager.Instance.Resume();
        RenderSettings.fog = false;
    }

    void Update () {
        NetworkManager.Instance.Update(Time.deltaTime);
	}
  
    void HandleNetMsg(Stream stream, int n32ProtocalID)
    {
        Debug.Log("n32ProtocalID =  " + (GSToGC.MsgID)n32ProtocalID);

        switch (n32ProtocalID)
        {
            case (int)GSToGC.MsgID.eMsgToGCFromGS_BroadcastBattleHeroInfo:
                MessageHandler.Instance.OnBroadcastBattleHeroInfo(ProtobufMsg.MessageDecode<BroadcastBattleHeroInfo>(stream));//战斗英雄信息 heroid  
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectAppear:
                MessageHandler.Instance.OnNotifyGameObjectAppear(ProtobufMsg.MessageDecode<GOAppear>(stream));  //通知客户端显示游戏对象
                break;

            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectRunState:
                MessageHandler.Instance.OnNotifyGameObjectRunState(ProtobufMsg.MessageDecode<RunningState>(stream));//runState
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelStartForceMoveTeleport:
                MessageHandler.Instance.OnNotifySkillModelStartForceMoveTeleport(ProtobufMsg.MessageDecode<NotifySkillModelStartForceMoveTeleport>(stream));//开始传输位置
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectFreeState:
                MessageHandler.Instance.OnNotifySGameObjectFreeState(ProtobufMsg.MessageDecode<FreeState>(stream));//进入自由状态
                break;

            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillInfo://技能信息 包含技能id 冷却时间  英雄id guid
                 MessageHandler.Instance.OnNotifySkillInfo(ProtobufMsg.MessageDecode<NotifySkillInfo>(stream));//进入自由状态
                break;

            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectReleaseSkillState://通知客户端释放技能 
                 MessageHandler.Instance.OnNotifyGameObjectReleaseSkillState(ProtobufMsg.MessageDecode<ReleasingSkillState>(stream));//释放技能
                break;

            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelEmit:
                 MessageHandler.Instance.OnNotifySkillModelEmit(ProtobufMsg.MessageDecode<EmitSkill>(stream));//产生特效
                break;           
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelBufEffect://buff效果
                MessageHandler.Instance.OnNotifySkillModelBufEffect(ProtobufMsg.MessageDecode<BuffEffect>(stream));
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelHitTarget://新技能受击
                MessageHandler.Instance.OnNotifySkillModelHitTarget(ProtobufMsg.MessageDecode<HitTar>(stream));
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelRange://范围技能
                MessageHandler.Instance.OnNotifySkillModelRange(ProtobufMsg.MessageDecode<RangeEffect>(stream));//范围技能
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelEmitDestroy://新飞行物体销毁   
                MessageHandler.Instance.OnNotifySkillModelEmitDestroy(ProtobufMsg.MessageDecode<DestroyEmitEffect>(stream));//销毁特效     
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHPInfo://HP信息
               MessageHandler.Instance.OnNotifyHPInfo(ProtobufMsg.MessageDecode<NotifyHPInfo>(stream));           
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMPInfo:
                MessageHandler.Instance.OnNotifyMPInfo(ProtobufMsg.MessageDecode<NotifyMPInfo>(stream));           
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHPChange://hp改变
                MessageHandler.Instance.OnNotifyHPChange(ProtobufMsg.MessageDecode<HPChange>(stream));           
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMPChange://通知魔法值改变
                MessageHandler.Instance.OnNotifyMPChange(ProtobufMsg.MessageDecode<MpChange>(stream));           
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectDeadState:
                MessageHandler.Instance.OnNotifyGameObjectDeadState(ProtobufMsg.MessageDecode<DeadState>(stream));  //通知游戏对象进入死亡状态       
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectReliveState:
                 MessageHandler.Instance.OnNotifyGameObjectReliveState(ProtobufMsg.MessageDecode<NotifyGameObjectReliveState>(stream));//重生状态
                break;

        }
    }

    void OnNotifyHPInfo(NotifyHPInfo pMsg)
    {
        Debug.Log("OnNotifyHPInfo");
    }
    void OnNotifyMPInfo(NotifyMPInfo pMsg)
    {
        Debug.Log("OnNotifyMPInfo");
    }
    void OnNotifyGameObjectFreeState(FreeState pMsg)
    {
        UInt64 sGUID;

        sGUID = pMsg.objguid;
        Vector3 mvPos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 mvDir = this.ConvertDirToVector3(pMsg.dir);
        Player entity = null;
        if (PlayersManager.Instance.PlayerDic.TryGetValue(sGUID, out entity))
        {
            Vector3 sLastSyncPos = entity.GOSSI.sServerSyncPos;
            mvPos.y = entity.RealEntity.transform.position.y;
            entity.GOSSI.sServerBeginPos = mvPos;
            entity.GOSSI.sServerSyncPos = mvPos;
            entity.GOSSI.sServerDir = mvDir;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;
            entity.isRuning = false;
            entity.EntityChangedata(mvPos, mvDir);
            //调用子类执行状态
            entity.OnFreeState();
        }
    }
    void OnNotifyGameObjectRunState(RunningState pMsg )
    {
        UInt64 sGUID;
        sGUID = pMsg.objguid;
        Vector3 mvPos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 mvDir = this.ConvertDirToVector3(pMsg.dir);
        float mvSp = pMsg.movespeed / 100.0f;
        Player entity = null;
       
        entity =PlayersManager.Instance.PlayerDic[sGUID];
       
        if (entity!=null)
	    {
		    mvPos.y = entity.RealEntity.transform.position.y;
            entity.GOSSI.sServerBeginPos = mvPos;
            entity.GOSSI.sServerSyncPos = mvPos;
            entity.GOSSI.sServerDir = mvDir;
            entity.GOSSI.fServerSpeed = mvSp;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;
               //数据改变
            entity.EntityChangedata(mvPos, mvDir);
            entity.isRuning = true;
            //调用子类执行状态
            //entity.OnRuntate();
	    }
    }

    void onNotifyGameObjectAppear(GOAppear pMsg)
    {
        //目的：创建并显示实体 设置实体信息
        foreach (GSToGC.GOAppear.AppearInfo info in pMsg.info)
        {
            bool hasExist = false;
            foreach (var playerItem in PlayersManager.Instance.PlayerDic)
            {
                if (playerItem.Key == info.objguid)
                {
                    hasExist = true;
                    break;
                }
            }
            if (hasExist) {
                continue;
            }
            UInt64 sMasterGUID = info.masterguid;
            UInt64 sObjGUID = info.objguid;
            UInt64 sObjID = info.obj_type_id;
           
            Int32 IntCamp = info.camp;//实体阵营 NPC可以确定  但是并不能确定对战的阵营
            Vector3 mvPos = this.ConvertPosToVector3(info.pos);
            Vector3 mvDir = this.ConvertDirToVector3(info.dir);
            //为模型添加组件
            GameObject model = mPlayerModel[(int)info.obj_type_id];
            Player playerComponent = null;
            if (mMyGuid == sMasterGUID)
            {
                playerComponent = model.AddComponent<MyPlayer>();             
                PlayersManager.Instance.LocalPlayer = playerComponent;
                LocalPlayer = model;
                OnLocalPlayerInit((int)sObjID);//设置技能图标
            }
            else
            {
                playerComponent = model.AddComponent<Player>();
                PlayersManager.Instance.targetPlayer = playerComponent;
            
            }

          
            playerComponent.RealEntity = model;
            PlayersManager.Instance.AddDic(info.objguid, playerComponent);
            playerComponent.GameObjGUID = sObjGUID;
            playerComponent.ObjTypeID = sObjID;    
            playerComponent.InitSkillDic();//初始化技能列表    
           
            playerComponent.objTransform = model.transform;
            playerComponent.showHeroLifePlate(info);      //显示血条    

            playerComponent.EntityFSMPosition = mvPos;
            model.transform.position = mvPos;
            model.transform.rotation = Quaternion.LookRotation(mvDir);
            model.SetActive(true);
        }      
    }

    void onNotifyBattleHeroInfor(BroadcastBattleHeroInfo pMsg)
    {
         foreach (GSToGC.BroadcastBattleHeroInfo.HeroInfo info in pMsg.heroinfo)
        {
            string path = "Monsters" + "/" + ConfigReader.HeroSelectXmlInfoDict[(int)info.heroid].HeroSelectName;
            GameObject model = LoadModel((int)info.heroid, path);
            //model.transform.SetParent(GameObject.Find("GameObjects").transform);
            if (mPlayerModel.ContainsKey(info.heroid))
            {
                continue;
            }
            else
            {
                mPlayerModel.Add(info.heroid, model);
            }
            //指定本地玩家
            if (GameStart.heroid == info.heroid)
            {
                LocalPlayer = model;
                mMyGuid = (ulong)info.masterguid;
            }
        }
    }
    
    // //////////////////UI事件响应//////////////////////
    public void OnAutoAttack()
    {
        HolyGameLogic.Instance.GameAutoFight(); //向服务器请求自动战斗
    }

    public void OnReleaseSkill1()
    {
        Player player = PlayersManager.Instance.LocalPlayer;
        if (!player) return;

        SkillTypeEnum type = GetSkillType((int)ShortCutBarBtnEnum.BTN_SKILL_1);

        if (type == SkillTypeEnum.SKILL_NULL) return;

        int skillID = 0;
        PlayersManager.Instance.LocalPlayer.skillDic.TryGetValue(type, out skillID);
        if (skillID==0)
        {
            return;  
        }
        HolyGameLogic.Instance.EmsgToss_AskUseSkill((uint)skillID);
        player.mSkill1Foreground.gameObject.SetActive(true);
        player.mHasSkill1used = true;

    }

    public void OnReleaseSkill2()
    {
        Player player = PlayersManager.Instance.LocalPlayer;
        if (!player) return;

        SkillTypeEnum type = GetSkillType((int)ShortCutBarBtnEnum.BTN_SKILL_2);

        if (type == SkillTypeEnum.SKILL_NULL) return;

        int skillID = 0;
        PlayersManager.Instance.LocalPlayer.skillDic.TryGetValue(type, out skillID);
        if (skillID == 0)
        {
            return;
        }
        HolyGameLogic.Instance.EmsgToss_AskUseSkill((uint)skillID);
        player.mSkill2Foreground.gameObject.SetActive(true);
        player.mHasSkill2used = true;
    }

    public void OnLocalPlayerInit(int sObjID)
    {
        //更新技能图片   
        HeroConfigInfo heroInfo = ConfigReader.GetHeroInfo((int)sObjID);
        if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType2) != null)
        {
            buttonSprite[0].spriteName = ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType2).skillIcon;//技能1
        }
        if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType3) != null)
        {
            buttonSprite[1].spriteName = ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType3).skillIcon;//技能2
        }

    }

    GameObject LoadModel(int heroid, string path)
    {
        GameObject heroModel = Resources.Load(path) as GameObject;
        GameObject model = GameObject.Instantiate(heroModel);
        model.SetActive(true);
        return model;
    }

    public Vector3 ConvertPosToVector3(GSToGC.Pos pos)
    {
        if (pos != null)
            return new Vector3((float)pos.x / 100.0f, HolyGameLogic.Instance.GetGlobalHeight(), (float)pos.z / 100.0f);
        else
            return Vector3.zero;
    }

    public Vector3 ConvertDirToVector3(GSToGC.Dir dir)
    {
        float angle = (float)(dir.angle) / 10000;
        return new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle));
    }

    SkillTypeEnum GetSkillType(int ie)
    {
        SkillTypeEnum type = SkillTypeEnum.SKILL_NULL;
        switch ((ShortCutBarBtnEnum)ie)
        {
            case ShortCutBarBtnEnum.BTN_SKILL_1:
                type = SkillTypeEnum.SKILL_TYPE1;
                break;
            case ShortCutBarBtnEnum.BTN_SKILL_2:
                type = SkillTypeEnum.SKILL_TYPE2;
                break;
        }
        return type;
    }
}

public enum SkillTypeEnum
{
    SKILL_NULL = -1,
    SKILL_TYPE1,
    SKILL_TYPE2,
    SKILL_TYPE3,
    SKILL_TYPE4,
    SKILL_TYPE5,
    SKILL_TYPEABSORB1,
    SKILL_TYPEABSORB2,
    FixDistanceArea,
}

public enum ShortCutBarBtnEnum
{
    BTN_SKILL_1 = 0,    //0 技能1
    BTN_SKILL_2,        //1 技能2  
    BTN_AUTOFIGHT,      //6 自动攻击
    BTN_CHANGELOCK,     //7 改变锁定
}