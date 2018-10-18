using UnityEngine;
using System;
using System.Collections.Generic;
using GameDefine;
using Thanos.GameData;
using Thanos.GameEntity;
using Thanos.GuideDate;
using Thanos.GameState;
using Thanos.Network;
using Thanos.Effect;
using Thanos;
using Thanos.Model;


public class HolyTechGameBase : MonoBehaviour {

	public BattleStateEnum Battle_State {private set;get;}

    public static HolyTechGameBase Instance{set;get;}

	//private bool IsCutLine = false;

    public bool IsInitialize = false;

    public bool IsQuickBattle = false;//是否可快速战斗

    public List<string> ipList = new List<string>();

    public string LoginServerAdress =null /*"127.0.0.1"*/;

    public int LoginServerPort = 49996;

    public AudioManager AudioPlay{get;private set;}

    public bool SkipNewsGuide = false;

	void Awake(){

		if (Instance != null) {
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad (this.gameObject);
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ////加载所有Mian类型的窗口并初始化   窗口初始化（添加监听器）     参数：front场景类型  EScenesType.EST_None
      //  WindowManager.Instance.ChangeScenseToMain(EScenesType.EST_None);
    }

	void Start () {

        //玩家管理器
		new PlayerManager ();
        //Npc管理器
		new NpcManager(); 
        //关闭客户端连接
        NetworkManager.Instance.Close();    
        // 进入初始化状态  然后在Update中    loginState
        GameStateManager.Instance.EnterDefaultState();
        //初始化逻辑对象
        HolyGameLogic logini = HolyGameLogic.Instance;
        //预加载，减少进入游戏资源加载卡顿（创建所有配置文件）
        ConfigReader.Init();
        //读取（敏感词汇）配置文件
        //GameMethod.FileRead();       
        //预加载特效信息
        ReadPreLoadConfig.Instance.Init();
        //需要释放的资源信息
        ReadReleaseResourceConfig.Instance.Init(); 
	}

    void Update()
    {
        //更新buff
        Thanos.Skill.BuffManager.Instance.Update();
        //更新特效
        EffectManager.Instance.UpdateSelf();
        //更新提示消失
        MsgInfoManager.Instance.Update();
        //场景声音更新
        SceneSoundManager.Instance.Update();
        //声音更新
        Thanos.AudioManager.Instance.OnUpdate();
        //更新游戏状态机   
        GameStateManager.Instance.Update(Time.deltaTime);
        //更新网络模块
        NetworkManager.Instance.Update(Time.deltaTime);
        //更新界面引导
        IGuideTaskManager.Instance().OnUpdate();
        //小地图更新
        MiniMapManager.Instance.Update();
        //UI更新 
       // WindowManager.Instance.Update(Time.deltaTime);
        //特效后删除机制 
        EffectManager.Instance.HandleDelete();
        //GameObjectPool更新
        ObjectPool.Instance.OnUpdate();
        //游戏时间设置
        GameTimeData.Instance.OnUpdate();
    }

	void OnEnable()
	{
        //在NetWorkManager中连接成功后广播消息  ，连接成功后发送Ping值
        //EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ConnectServerSuccess, GameConnectServerSuccess);
        //打开链接UI
        EventCenter.AddListener((Int32)GameEventEnum.ConnectServerFail, OpenConnectUI);
        //打开链接UI
        EventCenter.AddListener((Int32)GameEventEnum.ReconnectToBatttle, OpenConnectUI);
        //连接等待事件  
        EventCenter.AddListener((Int32)GameEventEnum.GameEvent_BeginWaiting, OpenWaitingUI); 

        if (PlayerPrefs.HasKey(UIGameSetting.voiceKey))
        {
            int vKey = PlayerPrefs.GetInt(UIGameSetting.voiceKey);
            bool state = (vKey == 1) ? true : false;
            AudioManager.Instance.EnableVoice(state);
        }
        if (PlayerPrefs.HasKey(UIGameSetting.soundKey)) {
            int sKey = PlayerPrefs.GetInt(UIGameSetting.soundKey);
            bool state = (sKey == 1) ? true : false;
            AudioManager.Instance.EnableSound(state);
        }       
	}

    //移除监听器   链接成功  连接失败 重新连接战斗  开始等待
    void OnDisable()
	{
        //EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ConnectServerSuccess, GameConnectServerSuccess);
        EventCenter.RemoveListener((Int32)GameEventEnum.ConnectServerFail, OpenConnectUI);
        EventCenter.RemoveListener((Int32)GameEventEnum.ReconnectToBatttle, OpenConnectUI);
        EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_BeginWaiting, OpenWaitingUI);   
	}
    //游戏退出前执行（玩家强行关闭游戏）
    void OnApplicationQuit()
    {
       // Debug.Log("游戏退出前执行了OnAppliactionQuit");     
        NetworkManager.Instance.Close();
    }
    
    //////////////////////////////////////////////////////游戏事件响应/////////////////////////////////////////
    //打开链接UI
    public void OpenConnectUI()
    {
        //游戏结束时清除 AccountDic,AccountDic存储的是玩家entity
       // PlayerManager.Instance.CleanPlayerWhenGameOver();//
        //销毁所有的实体
       // EntityManager.Instance.DestoryAllEntity();
        //销毁所有的特效
        EffectManager.Instance.DestroyAllEffect();
        //允许初始化
        IsInitialize = true;
	}
   
    //打开等待UI
    private void OpenWaitingUI()
    {
        if (WaitingInterface.Instance == null)
        {
            //在UI摄像机下打开UI （正在链接的UI）
            HolyTechUI.Instance.OnOpenUIPathCamera(GameDefine.GameConstDefine.WaitingUI);
        }
    }
    /// <summary>
    /// 连接服务器成功，每30秒发送一个ping值
    /// </summary>
    //private void GameConnectServerSuccess()
    //{
    //    StopCoroutine("PingToServer");

    //    StartCoroutine("PingToServer");
    //}

    ////定时发送Ping值  只有链接的服务器是GateServer
    //private IEnumerator PingToServer()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(120);
          
    //        HolyGameLogic.Instance.EmsgToss_AskPing();
    //    }
    //}

    //////////////////////////////////////////////////////游戏事件响应/////////////////////////////////////////



    //游戏结束  清除所有的实体，玩家。
    public void PlayEnd()
    {
        EntityManager.AllEntitys.Clear();//清除所有实体
        if (PlayerManager.Instance.LocalPlayer != null)
        {
            PlayerManager.Instance.AccountDic.Clear();
            PlayerManager.Instance.LocalPlayer.AbsorbMonsterType = null;
        }
        
        Thanos.AudioManager.Instance.StopHeroAudio();
    }

    //游戏开始  游戏Bulidings创建
	public  void PlayStart()
    { 
        //if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerObType)
        //{
        //    state = 1;
        //}
        //场景中的对象  箭塔 防御塔， 售货员 产生小兵的基地
        GameMapObjs GameBuilding = FindObjectOfType(typeof(GameMapObjs)) as GameMapObjs; //根据场景不同，个数不同     
        //清除基地
        EntityManager.ClearHomeBase();
        if (GameBuilding != null)
        {

            //获取配置文件并生成初始化
            for (int id = 0; id < GameBuilding.transform.childCount; id++)
            {
                Transform child = GameBuilding.transform.GetChild(id);
                int objId = 0;
                try
                {
                    objId = Convert.ToInt32(child.name);//将对象名称转化为整型
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    continue;
                }

                int infoId = GetMapObjIndex(objId);//获取地图对象索引
                if (ConfigReader.MapObjXmlInfoDict.ContainsKey(infoId)) //获取对象配置文件并生成对象
                {
                    //获取配置文件
                    MapObjConfigInfo configInfp = ConfigReader.MapObjXmlInfoDict[infoId];

                    int type = configInfp.eObjectTypeID;
                    int index = configInfp.un32ObjIdx;
                    int camp = configInfp.n32Camp;
                    UInt64 sGUID = (UInt64)index;

                    //生成并初始化
                    EntityManager.HandleDelectEntity(sGUID);
                    IEntity item = NpcManager.Instance.HandleCreateEntity(sGUID, (EntityCampTypeEnum)camp);
                    item.MapObgId = objId;
                    item.realObject = child.gameObject;
                    item.objTransform = child.gameObject.transform;
                    item.GameObjGUID = sGUID;
                    item.NpcGUIDType = type;
                    item.ObjTypeID = (uint)type;
                    item.entityType = (EntityTypeEnum)ConfigReader.GetNpcInfo(type).NpcType;
                    item.SetHp(1);
                    item.SetHpMax(1);
                    EntityManager.Instance.SetCommonProperty(item, type);
                    item.RealEntity = EntityManager.AddBuildEntityComponent(item);
                    NpcManager.Instance.AddEntity(sGUID, item);
                    EntityManager.AddHomeBase(item);
                    GuideBuildingTips.Instance.AddBuildingTips(item);
                }
            }
        }

        //加载基地销毁特效，并设置为禁用
        LoadBaseDate.Instance().LoadBase();
	}

    //获取地图对象索引
    private int GetMapObjIndex(int objId){
        foreach (var item in ConfigReader.MapObjXmlInfoDict.Values) {
            if (item.un32ObjIdx == objId && (int)GameUserModel.Instance.GameMapID == item.un32MapID)
            {
                return item.un32Id;
            }
        }
        return -1;
    }
}
