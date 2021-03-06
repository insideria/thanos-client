/// <summary>
/// 消息定义（ID）
/// </summary>
public enum GameEventEnum
{
    ErrorStr = 1,
    ConnectServerFail,
    ConnectServerSuccess,
    ReConnectSuccess,
    ReConnectFail,
    InputUserData,
    SelectServer,
    IntoLobby,
    IntoRoom,
    RoomBack,
    RoomEnd,
    IntoHero,
    Loading,
    LoadingEnd,
    GameOver,  
    ReconnectToBatttle,
    BatttleFinished,
    AskReadySucces,
    AskLeaveBattleSucces,
    BattleStateChange,
    AskComplementedRegisterInfo,
    AskAddInBattle,
    TheBattleUserFull,
    NullBattle,
    BattlePDWNotMatch,
    NullUser,
    InvalidMapID,
    InvalidBattlePos,
    JustInBattle,
    GameEvent_JustNotInBattle,
    GameEvent_UserDonotInTheBattle,
    GameEvent_InvalidBattleState,
    GameEvent_YouAreNotBattleManager,
    GameEvent_NotAllUserReady,
    GameEvent_NotEnoughGold,
    GameEvent_Reborn,
    GameEvent_PlayerEnterAltar,
    GameEvent_ChangeMapState,
    GameEvent_UpdateUserGameItems,
    GameEvent_GameStartTime,
    GameEvent_OpenUIPlay,
    GameEvent_ScenseChange,
    GameEvent_SystemNoticeCreataPrefab,
    GameEvent_CretateDailyBonus,
    GameEvent_DailyBonus,
    GameEvent_RemoveDailyBonus,
    GameEvent_ChangeMoney,
    GameEvent_UpdateMiniMap,
    GameEvent_InitMiniMap,
    GameEvent_AddMiniMap,
    GameEvent_RemoveMiniMap,
    GameEvent_BroadcastBeAtk,
    GameEvent_RemoveMiniWarning,

    GameEvent_AllPlayerGoods,
    GameEvent_AllPlayerAssist,
    GameEvent_AllPlayerKills,
    GameEvent_AllPlayerDeaths,
    GameEvent_AllPlayerCp,
    GameEvent_AllPlayerLevel,
    GameEvent_AllPlayerLastHit,

    GameEvent_NotifyChangeKills,
    GameEvent_NotifyChangeDeaths,
    GameEvent_LocalPlayerCp,
    GameEvent_PlayerGetCp,
    GameEvent_LocalPlayerAssist,

    GameEvent_UpLv,


    GameEvent_BeginWaiting,
    GameEvent_EndWaiting,

    GameEvent_MatchNumber,

    GameEvent_EntityHpChange,//entityHP改变
    GameEvent_EntityMpChange,//entityMP改变
    //OB
    GameEvent_HeroInfoChange,
    GameEvent_HeroHpChange,
    GameEvent_HeroLvChange,
    GameEvent_HeroMpChange,
    GameEvent_SkillUpLvChange,
    GameEvent_SkillInfo,
    GameEvent_HeroHeadInfoChange,
    GameEvent_HeroDeathTime,
    GameEvent_HeroBackTown,
    GameEvent_AbsSkillInfo,
    GameEvent_SkillCDInfo,
    GameEvent_FuryStateInfo,
    GameEvent_PackageBuyInfo,
    GameEvent_PersonInitInfo,

    GameEvent_InvitatAddFriendInfo,
    GameEvent_RemoveFriend,
    GameEvent_RemoveFriendEnd,
    GameEvent_FriendPersonInfo,//详细信息
    GameEvent_BlackInfo,//黑名单简介
    GameEvent_ReceiveLobbyMsg,
    GameEvent_ReceiveNewMsg,
    GameEvent_RemoveChatTask,
    GameEvent_InvitationMsg,
    GameEvent_FindFriendInfo,
    GameEvent_FindClear,
    GameEvent_TalkUIDetails,
    GameEvent_AskFriendEorr,

    GameEvent_HeroReborn,//英雄复活
    GameEvent_NotifyDeathEnter,
    GameEvent_NotifyDeathExit,

    GameEvent_ChatCheck,
    
    
    GameEvent_LockTarget,//广播自己锁定目标
    GameEvent_AddOrDelEnemy,//广播增加或者删除敌方玩家
    GameEvent_SSPingInfo,//广播SSPing;
    GameEvent_NotifyChangeCp,//Cp改变
    GameEvent_NotifyHpLessWarning,//血量警告特效
    GameEvent_NotifyOpenShop,//打开商店，关闭商店
    GameEvent_NotifyBuildingDes,//Ientity死亡
    GameEvent_GuideShowBuildingTips,



    // sdk event
    GameEvent_SdkRegisterSuccess,//sdk登錄成功
    GameEvent_SdkServerCheckSuccess,//服務器向SDK的驗證成功 
    GameEvent_SdkLogOff,
    


    //Login
    GameEvent_LoginEnter, //进入登陆
    GameEvent_LoginExit, //退出登陆
    GameEvent_LoginError,  //登陆错误码
    GameEvent_LoginSuccess,  //登陆成功
    GameEvent_LoginFail,  //登陆失败

    //system notice
    GameEvent_SystemNoticeEnter,
    GameEvent_SystemNoticeExit,
    //兑换码
    GameEvent_ExtraBonusEnter,
    GameEvent_ExtraBonusExit,
    //英雄时限
    GameEvent_TimeLimitHeroEnter,
    GameEvent_TimeLimitHeroExit,

    //英雄信息

    GameEvent_HeroDatumEnter,
    GameEvent_HeroDatumExit,
    //user info
    GameEvent_UserEnter, //进入用户信息设置
    GameEvent_UserExit, //退出用户信息设置

    //帐号获得商品
    GameEvent_UserGetGoodsHero,   //获得英雄
    GameEvent_UserGetGoodsNml,    //获得普通物品
    GameEvent_UserGetMoney,   
    GameEvent_RefreshGoodHero,      //刷新商城显示物品  
    GameEvent_CsGetNewHero,      //帐号获得新的英雄 
    GameEvent_RefreshTimeLimitHero,

    //lobby
    GameEvent_LobbyEnter, //进入大厅
    GameEvent_LobbyExit, //退出大厅
    GameEvent_BattleEnter, //进入战斗
    GameEvent_BattleExit, //退出战斗
    GameEvent_MarketEnter,
    GameEvent_MarketExit,
    GameEvent_MarketHeroListEnter,
    GameEvent_MarketHeroListExit,
    GameEvent_MarketHeroInfoEnter,
    GameEvent_MarketHeroInfoExit,

     GameEvent_HeroTimeLimitEnter,
     GameEvent_HeroTimeLimitExit,

    GameEvent_MarketRuneListEnter,
    GameEvent_MarketRuneListExit,
    GameEvent_MarketRuneInfoEnter,
    GameEvent_MarketRuneInfoExit,

    GameEvent_MarketRuneBuyEnter,
    GameEvent_MarketRuneBuyExit,

    GameEvent_RuneEquipWindowEnter,
    GameEvent_RuneEquipWindowExit,

    GameEvent_RuneCombineWindowEnter,
    GameEvent_RuneCombineWindowExit,

    GameEvent_RuneRefreshCardUpdate,

    GameEvent_RuneRefeshWindowEnter,
    GameEvent_RuneRefeshWindowExit,

    GameEvent_RuneBuyWindowEnter,
    GameEvent_RuneBuyWindowExit,

    GameEvent_PurchaseSuccessWindowEnter,
    GameEvent_PurchaseSuccessWindowExit,

    GameEvent_PurchaseRuneSuccessWin,

    GameEvent_RuneQuipUpdate,
    GameEvent_RuneQuipUnload,
    GameEvent_RuneBagUpdate,

    GameEvent_GameSettingEnter,
    GameEvent_GameSettingExit,

    GameEvent_TeamMatchEnter, //进入组队匹配
    GameEvent_TeamMatchExit, //退出组队匹配
    GameEvent_TeamMatchAddTeammate,  //增加队友
    GameEvent_TeamMatchDelTeammate,  //删除队友

    GameEvent_TeamMatchInvitationEnter,  //组队邀请界面
    GameEvent_TeamMatchInvitationExit,   //组队邀请界面退出

    GameEvent_TeamMatchSearchinEnter,   //匹配中界面
    GameEvent_TeamMatchSearchinExit,    //匹配中界面

    GameEvent_ServerMatchInvitationEnter,  //服务器匹配邀请界面
    GameEvent_ServerMatchInvitationExit,   //服务器匹配邀请界面退出

    GameEvent_HomePageEnter, //进入主页
    GameEvent_HomePageExit, //退出主页

    // 个人信息
    GameEvent_PresonInfoEnter,
    GameEvent_PresonInfoExit,
    GameEvent_ChangeNickName,
    GameEvent_ChangeHeadID,
    GameEvent_ChangeUserLevel,
    //VIP
    GameEvent_VIPPrerogativeEnter,
    GameEvent_VIPPrerogativeExit,

    //mail
    GameEvent_MailEnter,
    GameEvent_MailExit,
    GameEvent_AskMailInfo,
    GameEvent_AskCloseOrGetMailGift,
    GameEvent_UpdateMailList,
    GameEvent_UpdateMailInfo, 
    GameEvent_AddNewMailReq,
    GameEvent_AddNewMailRsp,
    GameEvent_SetDefaultMailInfo,

    GameEvent_BattleUpdateRoomList, //更新房间列表

    //social
    GameEvent_SocialEnter,
    GameEvent_SocialExit,
    GameEvent_CreateFriendPrefab,
    GameEvent_CreateBlackPrefab,
    GameEvent_FriendChangeInfo,//简介
    //Invite
    GameEvent_InviteCreate,
    GameEvent_InviteEnter,
    GameEvent_InviteExit,
    GameEvent_InviteChange,

    //在大厅接收Invite
    GameEvent_InviteAddRoom,
    GameEvent_InviteRoomEnter,
    GameEvent_InviteRoomExit,
    GameEvent_NewInviteRoom,
   
    //chatTask
    GameEvent_ChatTaskEnter,
    GameEvent_ChatTaskExit,
    GameEvent_ChatTaskFriend,
    GameEvent_ReadyChatTaskEnter,
    GameEvent_ShowChatTaskFriend,

    //room
    GameEvent_RoomEnter, //进入登陆
    GameEvent_RoomExit, //退出登陆
    GameEvent_AskBeginBattleError,  //开始游戏错语反馈
    GameEvent_SeatPosUpdate,  //更新座位
    GameEvent_RecvChatMsg,   //收到房间聊天消息

    //RoomOpenInvite
    GameEvent_RoomInviteEnter,
    GameEvent_RoomInviteExit,
    GameEvent_InviteList,

    //hero
    GameEvent_HeroEnter, //进入英雄选择
    GameEvent_HeroExit, //退出英雄选择
    GameEvent_HeroPreSelect, //英雄预选择
    GameEvent_HeroRealSelect,//英雄选择确认
    GameEvent_HeroAddSelect, //增加一个英雄选择
    GameEvent_HeroEnterRandom, //进入随机选择英雄
    GameEvent_HeroFirstTime,   //第一次读秒时间
    GameEvent_HeroSecondTime,  //第二次读秒时间
    GameEvent_HeroReconnectPreSelect, //重连预选择处理


    //play
    GameEvent_BattleInfoEnter,//进入实时战斗查看信息
    GameEvent_BattleInfoExit,//退出实时战斗查看信息
    GameEvent_BattleIngEnter,//查看当前战绩
    GameEvent_BattleMineEnter,//查看自己当前的属性
    GameEvent_BattleTimeDownEnter,//进入战斗倒计时

    //LocalPlayer
    GameEvent_LocalPlayerHpChange,
    GameEvent_LocalPlayerMpChange,
    GameEvent_LocalPlayerLevelChange,
    GameEvent_LocalPlayerExpChange,
    GameEvent_LocalPlayerInit,
    GameEvent_LocalPlayerPassiveSkillsUpLv,
    GameEvent_LocalPlayerPassiveSkills,
    GameEvent_LocalPlayerSkillCD,
    GameEvent_LocalPlayerSilence,
    GameEvent_SkillDescribleType,
    GameEvent_SkillDescribleId,
    GameEvent_SkillDescribleUpdate,
    GameEvent_LocalPlayerUpdateFuryValue,
    GameEvent_LocalPlayerUpdateFuryEffect,
    GameEvent_LocalPlayerRange,

    //over
    GameEvent_ScoreEnter,  //进入胜负显示
    GameEvent_ScoreExit,   //退出胜负显示
    GameEvent_SettlementEnter,  //进入结算显示
    GameEvent_SettlementExit,   //退出结算显示

    GameEvent_GamePlayEnter,
    GameEvent_GamePlayExit,
    GameEvent_HeroAttributesInfo,

    //吸附
    GameEvent_SoleSoldierEnter,
    GameEvent_SoleSoldierExit,
    GameEvent_ResetAbsHead,
    GameEvent_ResetLockHead,
    GameEvent_AbsorbResult,



    //新手引导
    GameEvent_UIGuideEnter,
    GameEvent_UIGuideExit,
    GameEvent_UIGuideEvent,
    GameEvent_PlayGuideEnter,
    GameEvent_PlayGuideExit,
    GameEvent_PlayGuideDragEvent,
    GameEvent_PlayChildTaskFinish,
    GameEvent_PlayTaskModelFinish,

    GameEvent_GuideKillTask,
    GameEvent_GuideAbsorbTask,
    GameEvent_GuideLockTargetCanAbsorb,   //如果锁定了可以吸附的小怪
    GameEvent_GuideLockTargetCanNotAbsorb, //锁定对象为空了或者锁定对象不能吸附

    //进阶引导
    GameEvent_AdvancedGuideEnter,
    GameEvent_AdvancedGuideExit,
    GameEvent_AdvancedGuideShowTip,
    GameEvent_AdvancedGuideBeAttackByBuilding,


    //其它
    GameEvent_ShowMessage, //显示MessageBox
    GameEvent_ShowLogicMessage,

    //通知收到了网络消息
    GameEvent_NotifyNetMessage,

    //DailyBonus
    GameEvent_DailyBonusEnter,
    GameEvent_DailyBonusExit,
    GameEvent_DailyBonusUpdate,
    GameEvent_NotifyAllTaskUpdate,
    GameEvent_NotifyDailyTaskUpdate,
    GameEvent_NotifyOneTaskAdd,
    GameEvent_NotifyOneTaskUpdate,
    GameEvent_NotifyOneTaskDel,
    GameEvent_NotifyOneTaskRewards,

    GameEvent_UIGuideInputNickNameEnd = -1000,
    GameEvent_UIGuideSelectSexEnd = -1001,
    GameEvent_UIGuideSelectHeadEnd = -1002,
    GameEvent_UIGuideCommitRegisterEnd = -1003,
    GameEvent_UIGuideNewsGuideEnd = -1004,
    GameEvent_UIGuideSelfDefBtnEnd = -1005,
    GameEvent_UIGuideSelectPrimaryBtnEnd = -1006,
    GameEvent_UIGuideSelectCreateRoomBtnEnd = -1007,
    GameEvent_UIGuideBackLobbyBtnEnd = -1008,
    GameEvent_UIGuideMatchBtnEnd = -1009,
    GameEvent_UIGuideRoomBeginBtnEnd = -1010,
    GameEvent_UIGuideSelectHeroHeadEnd = -1011,
    GameEvent_UIGuideSelectHeroCommitEnd = -1012, 

    //UIGuide // register guide
    GameEvent_UIGuideTriggerRegister = -5000,
    GameEvent_UIGuideTriggerNewsGuide = -5001,
    GameEvent_UIGuideTriggerSelfDefGame = -5002,
    GameEvent_UIGuideTriggerCreateRoom = -5003,
    GameEvent_UIGuideTriggerBackLobby = -5004,
    GameEvent_UIGuideTriggerMatchGame = -5005, 
    GameEvent_UIGuideTriggerSelectHero = -5006,
    GameEvent_UIGuideTriggerRoomBeginGame = -5007,

    GameEvent_SecondaryGuideTipToBuyStart = -5008,
    GameEvent_SecondaryGuideTipToBackCityStart = -5009,
    GameEvent_SecondaryGuideTipToFurySkillStart = -5010,
    GameEvent_SecondaryGuideTipToShopCpStart = -5011,
    GameEvent_SecondaryGuideTipToBuyEnd = -5012,
    GameEvent_SecondaryGuideTipToBackCityEnd = -5013,
    GameEvent_SecondaryGuideTipToFurySkillEnd = -5014,
    GameEvent_SecondaryGuideTipToShopCpEnd = -5015, 

    GameEvent_UIGuideInputNickNameStart = -8000,
    GameEvent_UIGuideSelectSexStart = -8001,
    GameEvent_UIGuideSelectHeadStart = -8002,
    GameEvent_UIGuideCommitRegisterStart = -8003,
    GameEvent_UIGuideNewsGuideStart = -8004,
    GameEvent_UIGuideSelfDefBtnStart = -8005,
    GameEvent_UIGuideSelectPrimaryBtnStart = -8006,
    GameEvent_UIGuideCreateRoomBtnStart = -8007,
    GameEvent_UIGuideBackLobbyBtnStart = -8008,
    GameEvent_UIGuideMatchBtnStart = -8009,
    GameEvent_UIGuideRoomBeginBtnStart = -8010,
    GameEvent_UIGuideSelectHeroHeadStart = -8011,
    GameEvent_UIGuideSelectHeroCommitStart = -8012,


    UserEvent_Base = 1000000,
    UserEvent_NotifyGateServerInfo,
    UserEvent_OneClinetLoginCheckRet,
    UserEvent_NotifyBattleHeroInfo,
    UserEvent_NotifyToChooseHero,
    UserEvent_NotifyBattleBaseInfo,
    UserEvent_NotifyBattleSeatPosInfo,
    UserEvent_NotifyBattleMatherCount,
    UserEvent_RequestMatchTeamList,
    UserEvent_NotifyUserBaseInfo,
    UserEvent_NotifyMatchTeamSwitch,
    UserEvent_NotifyMatchTeamBaseInfo,
    UserEvent_NotifyMatchTeamPlayerInfo,
    UserEvent_NotifyServerAddr,
    UserEvent_NotifyBattleStateChange,
    UserEvent_NotifyHeroList,
    UserEvent_NotifyTryChooseHero,
    UserEvent_NotifyEnsureHero,
    UserEvent_NotifyGameObjectAppear,
    UserEvent_NotifySkillModelStartForceMoveTeleport,
    UserEvent_NotifyGameObjectRunState,
    UserEvent_NotifyGameObjectFreeState,
    UserEvent_NotifySkillInfo,
    UserEvent_NotifyGameObjectReleaseSkillState,
    UserEvent_NotifySkillModelEmit,
    UserEvent_NotifyHPInfo,
    UserEvent_NotifyMPInfo

}
