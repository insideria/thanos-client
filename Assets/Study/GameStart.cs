using Thanos.Tools;
using Thanos;
using Thanos.Ctrl;
using Thanos.GameData;
using Thanos.Network;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GSToGC;
using BSToGC;
using LSToGC;
using System;
using UnityEngine.SceneManagement;

public class GameStart : HolyTechGameBase {

    public UIGrid mAreaGrid;
    public UIGrid mServerGrid;
    public UILabel mSelectArea;
    public UILabel mMatchNum;
    public UILabel mTimeLabel;
    public UILabel mSelectTime;
    
    public GameObject mAreaItem;
    public GameObject mPlaySubmitBtn;
    public GameObject mChangeSever;
    public GameObject mRootLogin;
    public GameObject mRootSever;
    public GameObject mBackLogin;
    public GameObject mSelectHero;
    public GameObject mLoadingUI;
    public GameObject mCenterButtonWindow;
    public UIToggle mAgreement;
    public GameObject mServerItem;
    public UILabel mSelectServer;
    public UILabel mUserName;
    public UISprite mServerState;
  //public UISprite mThumbnail;
    public GameObject mMidShow;
    public GameObject mHeroItem;
    public GameObject teamSelectInfo;
    public GameObject MyHero;
    public GameObject EnmyHero;
    private bool mHandleMsg;
    private Dictionary<string, List<SelectServerData.ServerInfo>> mServerDict;
    private List<GameObject> mListServerItems = new List<GameObject>();
 
    List<HeroSelectConfigInfo> heroInfoList = new List<HeroSelectConfigInfo>();
    Dictionary<int, GameObject> heroModelTable = new Dictionary<int, GameObject>();

    public GameObject TeamMatch;
    public GameObject MatchSearching;
    public GameObject CancelBtn;
    public GameObject BeginGameBG;
    public UILabel MyLoadintTime;
    public UILabel EnmyLoadintTime;

    GameObject mCurHeroModel;
    float mStartTime;
    int mPercent = 0;
    int mEnemyPercent = 0;
    bool mIsDownTime=false;
    bool mIsSelectHero = false;
    bool mIsLoading = false;
    public static int heroid = 0; 

    string mGateServer="192.168.1.27";
    string mLoginServer="192.168.1.27";
    string mBalanceServer="192.168.1.27";
    int port = 49996;

    private void Awake()
    {
        mGateServer = LoginServerAdress;
        mLoginServer = LoginServerAdress;
        mBalanceServer = LoginServerAdress;
        port = LoginServerPort;
        mCurHeroModel = null;
        //网络过来的消息处理
        EventCenter.AddListener<Stream, int>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, HandleNetMsg);

        EventCenter.AddListener<bool>((Int32)GameEventEnum.UserEvent_NotifyMatchTeamBaseInfo, onNotifyMatchTeamBaseInfo);
        EventCenter.AddListener((Int32)GameEventEnum.UserEvent_NotifyServerAddr, onNotifyServerAddr);
        EventCenter.AddListener<AskGateAddressRet>((Int32)GameEventEnum.UserEvent_NotifyGateServerInfo, onNotifyGateServerInfo);
        EventCenter.AddListener<bool>((Int32)GameEventEnum.UserEvent_NotifyMatchTeamSwitch, onNotifyMatchTeamSwitch);
        EventCenter.AddListener<BattleMatcherCount>((Int32)GameEventEnum.UserEvent_NotifyBattleMatherCount, onNotifyBattleMatherCount);
        EventCenter.AddListener<BattleSeatPosInfo>((Int32)GameEventEnum.UserEvent_NotifyBattleSeatPosInfo, onNotifyBattleSeatPosInfo);
        EventCenter.AddListener<HeroList>((Int32)GameEventEnum.UserEvent_NotifyHeroList, onNotifyHeroList);
        EventCenter.AddListener<TryToChooseHero>((Int32)GameEventEnum.UserEvent_NotifyTryChooseHero, onNotifyTryChooseHero);
        EventCenter.AddListener<HeroInfo>((Int32)GameEventEnum.UserEvent_NotifyEnsureHero, onNotifyEnsureHero);
        EventCenter.AddListener<BattleStateChange>((Int32)GameEventEnum.UserEvent_NotifyBattleStateChange, onNotifyBattleStateChange);
       
        mServerDict = new Dictionary<string, List<SelectServerData.ServerInfo>>();
        var areaItemOld = mAreaItem;
        mAreaItem = Instantiate(mAreaItem);
        mAreaItem.SetActive(false);
        Destroy(areaItemOld);

        var serverItemOld = mServerItem;
        mServerItem = Instantiate(mServerItem);
        mServerItem.SetActive(false);
        Destroy(serverItemOld);

        mHandleMsg = true;
    }

    void Start () {
        NetworkManager.Instance.Init(mLoginServer, port, NetworkManager.ServerType.LoginServer, true);
    }

    void Update()
    {
        if (this.mHandleMsg)
        {
            NetworkManager.Instance.Update(Time.deltaTime);
        }
        if (mIsDownTime)
        {
            mStartTime += Time.deltaTime;
            mTimeLabel.text = GameUtils.ShowCount((int)mStartTime);
        }
        if (mIsSelectHero)
        {
            mStartTime -= Time.deltaTime;
            if (mStartTime <= 0) return;
            mSelectTime.text = GameUtils.ShowCount((int)mStartTime);
        }
        if (mIsLoading)
        {
//<<<<<<< HEAD

//            mLocalPercent += (int)UnityEngine.Random.Range(0.5f, 1.2f);
//            mTargetPercent += (int)UnityEngine.Random.Range(0.8f, 1.2f);
//            if (mLocalPercent >= 100)
//            {
//                mLocalPercent = 100;
//            }
//            if (mTargetPercent >= 100)
//            {
//                mTargetPercent = 100;
//            }
//            EnmyLoadintTime.GetComponent<UILabel>().text = mLocalPercent.ToString() + "%";
//            MyLoadintTime.GetComponent<UILabel>().text = mTargetPercent.ToString() + "%";
//=======
            if (mPercent < 100)
            {
                var datatemp = new System.Random().Next(0, 2);
                mPercent += datatemp;
                MyLoadintTime.GetComponent<UILabel>().text = mPercent.ToString() + "%";
            }
            if (mEnemyPercent<100) {
                var datatemp = new System.Random().Next(0, 2);
                mEnemyPercent += datatemp;
                EnmyLoadintTime.GetComponent<UILabel>().text = mEnemyPercent.ToString() + "%";
            }
//>>>>>>> master
        }
    }

    void OnApplicationQuit()
    {
        NetworkManager.Instance.Close();
    }
   
    /////////////////////消息接收//////////////////////////
    private void HandleNetMsg(Stream stream, int n32ProtocalID)
    {
        Debug.Log("n32ProtocalID =  " + (GSToGC.MsgID)n32ProtocalID);
      
        switch (n32ProtocalID)
        {
            case (int)LSToGC.MsgID.eMsgToGCFromLS_NotifyServerBSAddr://513  选择服务器返回
                MessageHandler.Instance.OnNotifyServerAddr(ProtobufMsg.MessageDecode<ServerBSAddr>(stream));
                break;
            case (int)BSToGC.MsgID.eMsgToGCFromBS_AskGateAddressRet://203 开始游戏后返回
                MessageHandler.Instance.OnNotifyGateServerInfo(ProtobufMsg.MessageDecode<AskGateAddressRet>(stream));
                break;
            case (int)BSToGC.MsgID.eMsgToGCFromBS_OneClinetLoginCheckRet://204  开始游戏先返回
                MessageHandler.Instance.OnOneClinetLoginCheckRet(ProtobufMsg.MessageDecode<ClinetLoginCheckRet>(stream));
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserBaseInfo:
                MessageHandler.Instance.OnNotifyUserBaseInfo(ProtobufMsg.MessageDecode<UserBaseInfo>(stream));     //用户基本信息
                break; 
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_RequestMatchTeamList:
                MessageHandler.Instance.OnRequestMatchTeamList();
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamBaseInfo: 
                MessageHandler.Instance.OnNotifyMatchTeamBaseInfo(ProtobufMsg.MessageDecode<NotifyMatchTeamBaseInfo>(stream)); //通知匹配队伍基本信息 显示匹配界面
                break;      
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamPlayerInfo:
                MessageHandler.Instance.OnNotifyMatchTeamPlayerInfo(ProtobufMsg.MessageDecode<NotifyMatchTeamPlayerInfo>(stream)); //匹配队伍玩家信息
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamSwitch:
                MessageHandler.Instance.OnNotifyMatchTeamSwitch(ProtobufMsg.MessageDecode<NotifyMatchTeamSwitch>(stream));  //匹配界面显示 
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleMatherCount:   
                MessageHandler.Instance.OnNotifyBattleMatherCount(ProtobufMsg.MessageDecode<BattleMatcherCount>(stream));
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleSeatPosInfo:
                MessageHandler.Instance.OnNotifyBattleSeatPosInfo(ProtobufMsg.MessageDecode<BattleSeatPosInfo>(stream));  // 座位设置 这里显示的英雄选择窗口
                break;
           
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyTryToChooseHero:
                MessageHandler.Instance.OnNotifyToChooseHero(ProtobufMsg.MessageDecode<TryToChooseHero>(stream));  //   试图选择英雄
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleHeroInfo:     
                MessageHandler.Instance.OnNotifyBattleHeroInfo(ProtobufMsg.MessageDecode<HeroInfo>(stream));   //   确认战斗英雄信息
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroList://英雄选择列表
                MessageHandler.Instance.OnNotifyHeroList(ProtobufMsg.MessageDecode<HeroList>(stream));
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleBaseInfo:
                 MessageHandler.Instance.OnNotifyBattleBaseInfo(ProtobufMsg.MessageDecode<BattleBaseInfo>(stream)); // 战斗基本信息  设置地图id 
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleStateChange:
                MessageHandler.Instance.OnNotifyBattleStateChange(ProtobufMsg.MessageDecode<BattleStateChange>(stream)); //战斗状态改变
               break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_BroadcastBattleHeroInfo:
               MessageHandler.Instance.OnBroadcastBattleHeroInfo(ProtobufMsg.MessageDecode<BroadcastBattleHeroInfo>(stream));  //战斗英雄信息  
               break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroInfo:
               MessageHandler.Instance.OnNotifyHeroInfo(ProtobufMsg.MessageDecode<NotifyHeroInfo>(stream)); //英雄信息
               break;
            //case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectAppear:
            //   MessageHandler.Instance.OnNotifyGameObjectAppear(ProtobufMsg.MessageDecode<GOAppear>(stream));  //通知客户端显示游戏对象
            
            //   break;


        }
    }
   
    int heroSelectNum;
    TryToChooseHero tryTopMsg;
    AsyncOperation async;

    void onNotifyGameObjectAppear(GOAppear pMsg)
    {
        foreach (GSToGC.GOAppear.AppearInfo info in pMsg.info)
        {
            string path = "Monsters" + "/" + ConfigReader.HeroSelectXmlInfoDict[(int)info.objguid].HeroSelectName;
           // LoadModel((int)info, path);          
        }
    }

    void onNotifyBattleStateChange(BattleStateChange pMsg)
    {
        if (pMsg.state== 2)
        {
            mHandleMsg = false;
            HolyGameLogic.Instance.AskLoadComplete();
            EventCenter.RemoveListener<Stream, int>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, HandleNetMsg);
            NetworkManager.Instance.Pause();
            Debug.Log("加载场景");
            async=  SceneManager.LoadSceneAsync("pvp_001");
        }
    }

    void onNotifyTryChooseHero(TryToChooseHero pMsg)
    {
        tryTopMsg = pMsg;
        var spriteName = ConfigReader.HeroSelectXmlInfoDict[(int)pMsg.heroid].HeroSelectHead.ToString();
        heroSelectNum =ConfigReader.HeroSelectXmlInfoDict[(int)pMsg.heroid].HeroSelectNum;

        Transform Thumbnail = null;
        Transform name=null;
        GameObject model=null;
        int heroid = pMsg.heroid;
        
        if (pMsg.pos==1)
        {
            if (mCurHeroModel != null)
            {
                mCurHeroModel.SetActive(false);
            }

            Thumbnail = teamSelectInfo.transform.Find("HeroIcon");
            name = teamSelectInfo.transform.Find("HeroName");
            Thumbnail.GetComponent<UISprite>().spriteName = spriteName;
            name.GetComponent<UILabel>().text = ConfigReader.HeroSelectXmlInfoDict[(int)pMsg.heroid].HeroSelectNameCh.ToString();
            foreach (var key in heroModelTable.Keys)
            {
                model = heroModelTable[key];
                if (key == heroid)
                {
                    model.transform.localPosition = new Vector3(-0.2f, -0.7f, -0.5f);
                    model.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    model.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    mCurHeroModel = model;
                    model.SetActive(true);
                    return;
                }
            }
        }  
    }

    void onNotifyHeroList(HeroList pMsg)
    {
        foreach (var id in pMsg.heroid)
        {
            var heroInfo = ConfigReader.HeroSelectXmlInfoDict[(int)id];
            heroInfoList.Add(heroInfo);
           
            if (heroInfo != null)
            {
                LoadSelectHero(heroInfo.HeroSelectHead,heroInfo.HeroSelectNum,heroInfo.HeroSelectNameCh);
            }    
            string path = "Monsters" + "/" + ConfigReader.HeroSelectXmlInfoDict[(int)id].HeroSelectName;
            LoadModel((int)id, path);          
        }     
    }

    void onNotifyEnsureHero(HeroInfo pMsg)
    {   
        Transform Thumbnail = null;
        Transform heroName = null;
        Transform enmyName = null;
        var spriteName = ConfigReader.HeroSelectXmlInfoDict[(int)tryTopMsg.heroid].HeroSelectHead.ToString();
      
        if (tryTopMsg.pos == 1)
        {
            Thumbnail = MyHero.transform.Find("MyHeroIcon");
            heroName = MyHero.transform.Find("MyHeroIcon/MyHeroName");
            heroName.GetComponent<UILabel>().text = ConfigReader.HeroSelectXmlInfoDict[(int)tryTopMsg.heroid].HeroSelectNameCh.ToString();
            heroid = pMsg.heroposinfo.heroid;
        }
        else//指定加载界面中敌方的英雄信息
        {
            Thumbnail = EnmyHero.transform.Find("EnmyyHeroIcon");
            enmyName = EnmyHero.transform.Find("EnmyyHeroIcon/EnmyHeroName");
            enmyName.GetComponent<UILabel>().text = ConfigReader.HeroSelectXmlInfoDict[(int)tryTopMsg.heroid].HeroSelectNameCh.ToString();
        }
        Thumbnail.GetComponent<UISprite>().spriteName = spriteName;
    }

    void onNotifyBattleSeatPosInfo(BattleSeatPosInfo pMsg) {

        TeamMatch.SetActive(false);
        MatchSearching.SetActive(false);
        BeginGameBG.SetActive(false);     
        mSelectHero.SetActive(true);
        mIsSelectHero = true;
        mStartTime = 11f;
    } 

    public void LoadSelectHero(int heroSelectHead, int heroSelectNum,string name)
    {
        mMidShow.SetActive(true);

        Transform gridTrans = mMidShow.transform.Find("HeroBox").Find("Grid");

        UIGrid grid = gridTrans.GetComponent<UIGrid>();
        GameObject HeroItem = GameObject.Instantiate(mHeroItem);    
        HeroItem.transform.parent = mMidShow.transform.Find("HeroBox").Find("Grid");
        HeroItem.transform.localScale = Vector3.one;
        HeroItem.transform.localPosition = Vector3.zero;
        HeroItem.name = "HeroBox" + (heroSelectHead + 1).ToString();
       
        Transform head = HeroItem.transform.Find("frame");
        head.GetComponent<UISprite>().spriteName = heroSelectHead.ToString();
        HeroItem.GetComponentInChildren<UILabel>().text =name.ToString() ;
        HeroItem.SetActive(true);
       
        grid.Reposition();
        UIEventListener.Get(HeroItem.gameObject).onClick = (GameObject go)=>
        {
            HolyGameLogic.Instance.EmsgTocs_TryToSelectHero((uint)heroSelectNum);
        };
    }

    void onNotifyBattleMatherCount(BattleMatcherCount pMsg) {
        Debug.Log(pMsg.count);
        //  更新玩家匹配数量
        mMatchNum.text = "(" + pMsg.count + "/" + pMsg.maxcount + ")";
    }

    void onNotifyMatchTeamSwitch(bool state) {
        if (state)//开始匹配时返回的值为true
        {
            MatchSearching.SetActive(true);
            mIsDownTime = true;
            this.mStartTime = 0;
        }
        else //取消匹配时返回的值为false
        {
            MatchSearching.SetActive(false);
            mIsDownTime = false;
            this.mStartTime = 0;
        }        
    }

    void onNotifyGateServerInfo(AskGateAddressRet pMsg) {
        NetworkManager.Instance.canReconnect = false;
        NetworkManager.Instance.Close();
        NetworkManager.Instance.Init(mGateServer ,port, NetworkManager.ServerType.GateServer, true);        
    }

    void onNotifyServerAddr() {
        foreach (var item in SelectServerData.Instance.GetServerDicInfo())
        {
            var areaNo = System.Convert.ToInt32(item.Value.area.Replace("区", ""));
            int nTemp = areaNo / 5;
            int nLeft = areaNo % 5;
            if (nLeft == 0)
            {
                nTemp -= 1;
            }
            nTemp *= 5;
            string strName = (nTemp + 1).ToString() + "区-" + (nTemp + 5).ToString() + "区";
            if (!mServerDict.ContainsKey(strName))
            {
                var svrList = new List<SelectServerData.ServerInfo>();
                svrList.Add(item.Value);
                mServerDict.Add(strName, svrList);
            } else {
                List<SelectServerData.ServerInfo> serverList;
                bool hasList = mServerDict.TryGetValue(strName, out serverList);
                if (hasList) {
                    serverList.Add(item.Value);
                }
            }
        }
        mAreaGrid.Reposition();        
    }

    void onNotifyMatchTeamBaseInfo(bool state) {
        TeamMatch.SetActive(state);//显示组对界
    }

    public void onAgreementValueChange() {
        this.mAgreement.Set(!this.mAgreement.value);
    }

    public void onSelectArea(GameObject btnObject) {
        var text = btnObject.GetComponentInChildren<UILabel>().text;
        if (mSelectArea.text==text) {
            return;
        }

        mSelectArea.text = text;

        foreach (var item in mServerGrid.GetChildList())
        {
            Destroy(item.gameObject);
            mServerGrid.Reposition();
        }
        foreach(var item in this.mServerDict[text]) {
            GameObject obj = Instantiate(mServerItem);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(mServerGrid.transform);
            obj.transform.localScale = Vector3.one;
            obj.GetComponentInChildren<UILabel>().text = item.area + " " + item.name;
        }
        mServerGrid.Reposition();
    }

    public void onSelectServer(GameObject btnObject) {
        mSelectServer.text = btnObject.GetComponentInChildren<UILabel>().text;
        OnBackLogin(null);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
  
    // ////////////////////UI事件响应/////////////////////
 
    public void OnPlaySubmit(GameObject go)
    {
        NetworkManager.Instance.Close();
        //设置服务器信息
        NetworkManager.Instance.Init(mBalanceServer, port, NetworkManager.ServerType.BalanceServer, true);
    }
  
    public  void OnEnsureHero(GameObject go){   
        
        HolyGameLogic.Instance.EmsgToss_AskSelectHero((uint)heroSelectNum);
        mSelectHero.SetActive(false);
        mLoadingUI.SetActive(true);
        mCurHeroModel.SetActive(false);
        mIsLoading = true;
    }
  
    public void AskLogin()
    {
        //请求登录  返回用户信息
        GCToCS.Login pMsg = new GCToCS.Login
        {
            sdk = 10,//ToReview 平台写死为10?              
            platform = 0,
            equimentid = MacAddressIosAgent.GetMacAddressByDNet(),
            name = Guid.NewGuid().ToString(),      /* SelectServerData.Instance.gateServerUin*/
            passwd = "1",    /* SelectServerData.Instance.GateServerToken*/
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    // 切换服务器选择窗口
    public void OnPlayServer(GameObject go)
    {
        //mCenterButtonWindow.SetActive(false);
        //showSever  首先显示窗体，其次显示列表
        bool showLogin = false;
        bool showServer = true;
        mRootLogin.gameObject.SetActive(showLogin);
        mRootSever.gameObject.SetActive(showServer);
        ShowSeverItem();
    }

    //返回登录窗口
    public void OnBackLogin(GameObject go)
    {
        //showLogin  打开登录窗口
        bool showLogin = true;
        bool showServer = false;
        mRootLogin.gameObject.SetActive(showLogin);
        mRootSever.gameObject.SetActive(showServer);

        //mCenterButtonWindow.SetActive(true);
    }

    //显示服务器列表
    public void ShowSeverItem()
    {
        foreach (var item in mServerGrid.GetChildList())
        {
            Destroy(item.gameObject);
            mServerGrid.Reposition();
        }
        mSelectArea.text = "";

        foreach (var transItem in this.mListServerItems)
        {
            Destroy(transItem.gameObject);
            mAreaGrid.Reposition();
        }
        mListServerItems.Clear();

        Dictionary<int, SelectServerData.ServerInfo> serverInfoDic = SelectServerData.Instance.GetServerDicInfo();
        foreach (var item in mServerDict)
        {
            GameObject obj = Instantiate(mAreaItem);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(mAreaGrid.transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.GetComponentInChildren<UILabel>().text = item.Key;
            mListServerItems.Add(obj);
        }
        mAreaGrid.Reposition();
    }

    //申请匹配
    public void OnMatch(GameObject go)//申请匹配
    {
        //申请匹配
        HolyGameLogic.Instance.AskStartTeamMatch();
    }

    //取消匹配
    public void OnCancelBtn()
    {
        TeamMatchCtrl.Instance.AskStopMatch();
    }

    ////取消组队
    public void OnQuitTeam()
    {
        TeamMatchCtrl.Instance.QuitTeam();
    }

    //当选择英雄时
    public void OnChooseHero(GameObject go)
    {
         int id =Convert.ToInt32( go.GetComponent<UISprite>().spriteName) + 10000;       
         HolyGameLogic.Instance.EmsgTocs_TryToSelectHero((uint)id);
    }

    void LoadModel(int heroid, string path)
    {
        GameObject heroModel = Resources.Load(path) as GameObject;
        GameObject model = GameObject.Instantiate(heroModel);
        model.AddComponent<HeroRotation>();
        model.SetActive(false);
        heroModelTable.Add(heroid, model);
    } 
  
}
