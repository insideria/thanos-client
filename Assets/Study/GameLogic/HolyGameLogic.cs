using UnityEngine;
using System;
using System.Collections.Generic;
using Thanos.GameData;
using Thanos.Network;
using System.Text;
using GCToCS;
using GCToBS;
using Thanos;
using Thanos.Model;

//CGLCtrl_GameLogic游戏核心功能，本身没有逻辑，所有函数用来发送消息的，负责向服务器发送请求
public partial class HolyGameLogic : UnitySingleton<HolyGameLogic>
{

#if UNITY_STANDALONE_WIN  
    private Win32.HiPerfTimer m_HiPerfTimer = new Win32.HiPerfTimer();
#endif
    private float g_MapHeight = 60.0f;

    public void SetGateIPChoosed(string ip, Int32 port)
    {
        NetworkManager.Instance.Close();
        NetworkManager.Instance.Init(ip, port, NetworkManager.ServerType.GateServer);
    }

    public float GetGlobalHeight()
    {
        return g_MapHeight;
    }

    public Int64 GetNowTime()
    {
        return GameUtils.GetClientUTCMillisec();
    }

    public float GetDuration(Int64 n64StopTime, Int64 n64Start)
    {
        return (float)(n64StopTime - n64Start);
    }

    private const float CheckConnectTime = 1.0f;
    private float ConnectTime;

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 200;

        ConnectTime = Time.time;
    }


    void OnDestroy()
    {
    }

   /// <summary>
   /// 完成基本信息
   /// </summary>
   /// <param name="usName"></param>
   /// <param name="nickName"></param>
   /// <param name="headerID"></param>
   /// <param name="sex"></param>
   public void GameCompleteBaseInfo(string usName, byte[] nickName, int headerID, byte sex)
   {
       GCToCS.CompleteInfo pMsg = new GCToCS.CompleteInfo
       {
           nickname = System.Text.Encoding.UTF8.GetString(nickName),   //ToReview 字符串重复编码解码，需写一个测试确定是否必要
           headid = headerID,
           sex = sex                                                   //ToReview byte->int
       };
       NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);  
   }

    /// <summary>
    /// 游戏登录
    /// </summary>
    /// <param name="plat"></param>
    /// 平台
    /// <param name="usName"></param>
    /// 名字
    /// <param name="ps"></param>
    public void GameLogin()
    {
        GCToCS.Login pMsg = new GCToCS.Login
        {
            sdk = 10,//ToReview 平台写死为10?
           
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR || SKIP_SDK || UNITY_EDITOR_OSX
             platform = 0,
            #elif UNITY_IOS
             platform = 1,
            #elif UNITY_ANDROID
             platform = 2,
            #endif

            equimentid = MacAddressIosAgent.GetMacAddressByDNet(),
            name =  SelectServerData.Instance.gateServerUin,
            passwd = SelectServerData.Instance.GateServerToken,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 请求GateServer的地址
    /// </summary>
    /// <param name="plat"></param>
    /// <param name="usName"></param>
    /// <param name="ps"></param>
    //  已换AskGateAddress
    //eMsgToBSFromGC_AskGateAddress
    public void BsOneClinetLogin()
    {
        GCToBS.OneClinetLogin pMsg = new GCToBS.OneClinetLogin()
        {
           uin = "1"/* SelectServerData.Instance.serverUin*/,
           plat = (uint)SelectServerData.Instance.serverPlafrom,
           nsid = 0,
           login_success = 0,
           sessionid ="1"/*SelectServerData.Instance.serverSionId*/
        };
         
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskGsGateAddr() {
        GCToBS.AskGateAddress pMsg = new AskGateAddress()
        {
            plat = (int)SelectServerData.Instance.serverPlafrom,
            token = "",
            username = ""
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }


    //已换QuickBattle
    //eMsgToGSToCSFromGC_AskQuickBattle
    //ToReview 函数名差别很大
    public void QuickBattle(int mapID)
    {
        GCToCS.AskQuickBattle pMsg = new GCToCS.AskQuickBattle()
        {
            mapid = mapID
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void ChangeRoomSeat(int changeSeat)
    {
        GCToCS.AskChangeRoomSeat pMsg = new GCToCS.AskChangeRoomSeat()
        {
            newpos = changeSeat
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void SendUnloadRune(int spage, int spos)
    {
        GCToCS.UnEuipRunes pMsg = new GCToCS.UnEuipRunes()
        {
            page = spage,
            pos = spos
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换ReadyBattle
    //eMsgToGSToCSFromGC_AskReadyBattle
    public void ReadyRoom()
    {
        GCToCS.AskReadyRoom pMsg = new AskReadyRoom();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换CancelReadyBattle
    //eMsgToGSToCSFromGC_AskCancelReadyBattle
    //ToReview 函数名错别字？
    public void CancelRoom()
    {
        GCToCS.AskCancelRoom pMsg = new AskCancelRoom();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换BeginBattle
    //eMsgToGSToCSFromGC_AskBeginBattle
    public void StartRoom()
    {
        GCToCS.AskStartRoom pMsg = new AskStartRoom();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskCombine(List<uint> listRune)
    {
        GCToCS.ComposeRunes sMsg = new ComposeRunes();
        foreach(var value in listRune)
        {
            sMsg.runesid.Add(value);
        }

        NetworkManager.Instance.SendMsg(sMsg, (int)sMsg.msgnum);
    }

    public void LeaveRoom()
    {
        GCToCS.AskLeaveRoom pMsg = new AskLeaveRoom();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    public void AskCurrNotice()
    {
        GCToCS.AskCurtNotice pMsg = new AskCurtNotice();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskSSGuideStepComp(GCToSS.AskSSGuideStepComp.edotype mtype , int id)
    {
        GCToSS.AskSSGuideStepComp.taskinfo rfo = new GCToSS.AskSSGuideStepComp.taskinfo();
        rfo.stepid = id;
        rfo.dtype = mtype;

        GCToSS.AskSSGuideStepComp pMsg = new GCToSS.AskSSGuideStepComp();
        pMsg.info.Add(rfo);
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgid);
    }

    //已换LoadComplete
    //eMsgToGSToCSFromGC_ReportLoadBattleComplete 
    //ToReview名字差距很大
    public void AskLoadComplete()
    {
        GCToSS.LoadComplete pMsg = new GCToSS.LoadComplete();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换SelectHero
    //eMsgToGSToCSFromGC_AskSelectHero
    public void EmsgToss_AskSelectHero(UInt32 heroId)
    {
        GCToSS.SelectHero pMsg = new GCToSS.SelectHero()
        {
            heroid = (int)heroId                                    
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskRefresh(uint runeid, uint id)
    {
        GCToCS.AskRecoinRune sMsg = new AskRecoinRune()
        {
            rune_id = runeid,
            cost = id
        };

        NetworkManager.Instance.SendMsg(sMsg, (int)sMsg.msgnum);
    }

    //已换CeateBattle
    //eMsgToGSToCSFromGC_AskCreateBattle
    public void CreateRoom(int mapID, string password)
    {
        GCToCS.AskCreateRoom pMsg = new GCToCS.AskCreateRoom()
        {
            mapid = mapID,
            passwd = password
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskRoomList()
    {
        GCToCS.AskRoomList pMsg = new GCToCS.AskRoomList();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AddBattle
    //eMsgToGSToCSFromGC_AskAddBattle
    public void AddRoom(string roomID, string password)
    {
        GCToCS.AskAddRoom pMsg = new GCToCS.AskAddRoom()
        {
            battleid = Convert.ToUInt64(roomID),        //ToReview string->ulong 需查看roomID来源是否可能出现bug
            passwd = password
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 请求物品信息
    /// </summary>
    public void AskGoodsCfg()
    {
        GCToCS.AskGoodscfg pMsg = new GCToCS.AskGoodscfg();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 请求创建新手引导战役
    /// </summary>
    /// <param name="mapId"></param>
    public void AskCSToCreateGuideBattle(int mapId , AskCSCreateGuideBattle.guidetype mGtype)
    {
        GCToCS.AskCSCreateGuideBattle pMsg = new GCToCS.AskCSCreateGuideBattle()
        {
            mapid = mapId,
            ntype = mGtype
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 完成引导模块
    /// </summary>
    /// <param name="compId"></param>
    /// <param name="comp"></param>
    public void GuideCSStepComplete(int compId, bool comp)
    {
        GCToCS.GuideCSStepComp pMsg = new GCToCS.GuideCSStepComp()
        {
            guidepart = compId,
            bcomp = comp,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }


    //请求战斗匹配
    public void AskMatchBattle(int id, EBattleMatchType type)
    {
        GCToCS.AskCreateMatchTeam pMsg = new AskCreateMatchTeam
        {
            mapid = (uint)id,
            matchtype = (uint)type
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
        //服务端返回的消息
        //eMsgToGCFromGS_NotifyMatchTeamBaseInfo
        //eMsgToGCFromGS_NotifyMatchTeamPlayerInfo
    }

    public void AskEquipRune(UInt32 n32Runeid, Int32 n32ToPos)
    {
        GCToCS.EuipRunes pMsg = new GCToCS.EuipRunes
        {
            runeid = n32Runeid,
            topos = n32ToPos
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    //邀请好友
    public void AskInvitationFriend(string nickName)
    {
        GCToCS.AskInviteJoinMatchTeam pMsg = new GCToCS.AskInviteJoinMatchTeam
        {
            friendsNickName = nickName
        };

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //接受邀请(组队匹配)
    public void AskAcceptInvitation(string nickName)
    {
        GCToCS.AskAddMatchTeam pMsg = new GCToCS.AskAddMatchTeam
        {
            friendsNickName = nickName
        };

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //接受邀请(服务器主动请求)
    public void AskResponseServerInvitation(uint _mapid, uint _fightid, bool _accpet)
    {
         GCToCS.NotifyOneMatchNeedOneRet pMsg = new GCToCS.NotifyOneMatchNeedOneRet
         {
             mapid = _mapid,
             fightid = _fightid,
             isAccept = _accpet,
         };
 
         NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //退出队伍
    public void AskQuitTeam()
    {
        GCToCS.AskRemoveMatchTeam pMsg = new GCToCS.AskRemoveMatchTeam();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
        

    //申请匹配
    public void AskStartTeamMatch()
    {
        GCToCS.AskStartMatch pMsg = new GCToCS.AskStartMatch();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
        //服务端返回
    }

    //停止申请匹配
    public void AskStopTeamMatch()
    {
        GCToCS.AskStopMatch pMsg = new GCToCS.AskStopMatch();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //

    //已换ReconnectToBattle
    //eMsgToGSToCSFromGC_AskReConnetToBattle
    public void EmsgTocs_AskReConnetToBattle()
    {
        EmsgToss_AskEnterBattle(GameUserModel.Instance.GameBattleID);
    }

    #region to Scene Server--------------------------------------------------------------------------------------------------------------------------------
    ////////////////#########################################################################################################////////////
    ////////////////#########################################################################################################////////////
    ////////////////#########################################################################################################////////////

    //已换AskPingSS
    //eMsgToGSToSSFromGC_AskPingSS
    public void EmsgToss_AskPing()
    {
        GCToSS.AskPingSS pMsg = new GCToSS.AskPingSS()
        {
            time = GetNowTime()
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    public void EmsgToss_AskInviteFriendsToBattle(Int32 roomID, UInt64 sGUID)
    {
        GCToCS.AskInviteFriendsToBattle pMsg = new AskInviteFriendsToBattle()
        {
            battleid = roomID,
            guididx = sGUID,
        };
         NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    public void EmsgToss_AskCanInviteFriends()
    {
        GCToCS.AskCanInviteFriends pMsg = new AskCanInviteFriends();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }


    //已换ChatInRoom
    //eMsgToGSToCSFromGC_ChatInRoom
    public void EmsgToss_AskSendRoomTalk(UInt32 length, byte[] talkMsg)
    {
        GCToCS.AskChatInRoom pMsg = new GCToCS.AskChatInRoom()
        {
            chat = Encoding.UTF8.GetString(talkMsg)     //ToReview 重复编码解码UTF8字符串，需写一个测试看必要性
        };
        Debug.Log("SendMsg " + pMsg.chat);
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskEnterBattle
    //eMsgToGSToSSFromGC_AskEnterBattle
    public void EmsgToss_AskEnterBattle(UInt64 m_un64BattleID)
    {
        Int64 n64NowTime = Instance.GetNowTime();

        GCToSS.AskEnterBattle pMsg = new GCToSS.AskEnterBattle()
        {
            battleid = m_un64BattleID,
            clientTime = n64NowTime,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换MoveDir
    //eMsgToGSToSSFromGC_AskMoveDir
    public void EmsgToss_AskMoveDir(Vector3 forward)
    {
        GCToSS.MoveDir pMsg = new GCToSS.MoveDir()
        {
            dir = this.ConvertVector3ToDir(forward)
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换StopMove
    //eMsgToGSToSSFromGC_AskStopMove
    public void EmsgToss_AskStopMove()
    {
        GCToSS.StopMove pMsg = new GCToSS.StopMove();
        //ToReview 比原协议少了pos
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换BuyGoods
    //eMsgToGSToSSFromGC_AskBuyGoods
    public void EmsgToss_AskBuyGoods(int item)
    {
        GCToSS.BuyGoods pMsg = new GCToSS.BuyGoods()
        {
            typeid = item       //ToReview typeid与item名称不一致，是否正确
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换UseGoods
    //eMsgToGSToSSFromGC_AskUseGoods
    public void EmsgToss_AskUseGoods(int itPos)
    {
        GCToSS.UseGoods pMsg = new GCToSS.UseGoods()
        {
            goodspos = itPos
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换SellGoods
    //eMsgToGSToSSFromGC_AskSellGoods
    public void EmsgToss_AskSellGoods(int itPos)
    {
        GCToSS.SellGoods pMsg = new GCToSS.SellGoods()
        {
            goodspos = itPos
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换MoveGoods
    //eMsgToGSToSSFromGC_AskMoveGoods
    public void EmsgToss_AskMoveGoods(int from, int to)
    {
        GCToSS.MoveGoods pMsg = new GCToSS.MoveGoods()
        {
            frompos = from,
            topos = to
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //add by zhoujie 2014-4-15 请求攻击和使用技能
    //请求攻击
    //未换
    //ToReview 本来就没有函数体，似乎不用更换
    public void EmsgToss_AskAttack()
    {
        return;
        //Debug.LogError("ask attack");
        //CMsg pcMyMsg = new CMsg(64);
        //pcMyMsg.SetProtocalID((Int32)EmsgToSSFromGC.eMsgToGSToSSFromGC_AskAttack);
        //pcMyMsg.Add (HolyTechGameBase.Instance.Jx_n32SSID);
        //pcMyMsg.Add(HolyTech.GameEntity.PlayerManager.Instance.LocalPlayer.SyncLockTarget.GameObjGUID);
        //NetworkManager.Instance.SendMsg(pcMyMsg);

    }
    //请求吸收的技能
    //已换Absorb
    //eMsgToGSToSSFromGC_AskAbsorbMonster      
    public void EmsgToss_AskAbsorb(Int32 abs)
    {
        GCToSS.Absorb pMsg = new GCToSS.Absorb()
        {
            removeid = abs              //ToReview 名称差距较大，最好确认一下
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //请求使用技能
    //已换UseSkill
    //eMsgToGSToSSFromGC_AskUseSkill
    public void EmsgToss_AskUseSkill(UInt32 skillID)
    {
        GCToSS.UseSkill pMsg = new GCToSS.UseSkill()
        {
            skillid = (int)skillID
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //请求锁定目标
    //已换LockTar
    //协议名不一致eMsgToGSToSSFromGC_AskLockTargt->eMsgToGSToSSFromGC_AskLockTarget
    public void EmsgToss_AskLockTarget(UInt64 targetID)
    {
        GCToSS.LockTar pMsg = new GCToSS.LockTar()
        {
            guid = (long)targetID
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换ReportAltarStrawSolder
    //eMsgToGSToSSFromGC_ReportAltarStrawSolder
    public void EmsgToss_ReportAltarStrawSolder(UInt32 tgType, Int32 atIndex)
    {
        GCToSS.ReportAltarStrawSolder pMsg = new GCToSS.ReportAltarStrawSolder()
        {
            type = (int)tgType,   //ToReview unit->int
            index = atIndex
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// GM指令内容
    /// </summary>
    /// <param name="contens"></param>
    //已换GMCmd
    //协议名不一致eMsgToGSToCSFromGC_GMCmd->eMsgToGSToSSFromGC_GMCmd
    public void EmsgToss_eMsgToGSToCSFromGC_GMCmd(string contens)
    {
        
        GCToSS.GMCmd pMsg = new GCToSS.GMCmd()
        {
            cmd = contens
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
   
     
    }


    #endregion


    //已换
    public void EmsgTocs_Notice(Int32 typeNotice, string notice)//内部系统公告
    {
        //byte[] utf8Bytes = Encoding.UTF8.GetBytes(notice);
        //CMsg pcMyMsg = new CMsg(sizeof(Int32) + utf8Bytes.Length);
        //pcMyMsg.SetProtocalID((Int32)EMsgToGSToCSFromGC.eMsgToGSToCSFromGC_Notice);
        //pcMyMsg.Add(typeNotice);
        //pcMyMsg.Add(utf8Bytes.Length);
        //pcMyMsg.Add(utf8Bytes, 0, utf8Bytes.Length);
        //NetworkManager.Instance.SendMsg(pcMyMsg);

        GCToCS.Notice pMsg = new GCToCS.Notice();
        pMsg.type = (GCToCS.notice_type)typeNotice;
        pMsg.notice = notice;
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 商城入口请求购买英雄
    /// </summary>
    /// <param name="goodsId"></param>
    /// <param name="csmTp"></param>
    public void EMsgToGSToCSFromGC_AskBuyGoods(int goodsId , int csmTp, int Num = 1)
    {
        GCToCS.AskBuyGoods pMsg = new GCToCS.AskBuyGoods()
        {
            commondityid = goodsId,
            consumetype = csmTp,
            num = (uint)Num
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskGetLoginReWard()//每天登陆领取
    {
        GCToCS.UserAskGetCLReward pcMsg = new GCToCS.UserAskGetCLReward();
        NetworkManager.Instance.SendMsg(pcMsg, (int)pcMsg.msgnum);
    }

    public void EmsgTocs_AskAddToFriendList(string nickName, Int32 friendType)
    {
        GCToCS.AskAddToSNSList pMsg = new GCToCS.AskAddToSNSList()
        {
            nickname = nickName,
            type = friendType
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    public void EmsgTocs_AskAddToSNSListByID(UInt64 guid, Int32 friendType)
    {
        GCToCS.AskAddToSNSListByID pMsg = new GCToCS.AskAddToSNSListByID()
        {
            userid = guid,
            type = friendType
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskRemoveFromSNSList
    //协议名不一致eMsgToGSToCSFromGC_AskRemoveFromFriendList->eMsgToGSToCSFromGC_AskRemoveFromSNSList
    public void EmsgTocs_AskRemoveFromFriendList(UInt64 sGUID, Int32 friendType)
    {
        GCToCS.AskRemoveFromSNSList pMsg = new GCToCS.AskRemoveFromSNSList()
        {
            guididx = sGUID,
            type = friendType
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskSendMsgToUser
    //eMsgToGSToCSFromGC_AskSendMsgToUser
    public void EmsgTocs_AskSendMsgToUser(UInt64 sGUID, uint length, byte[] talkMsg)
    {
        GCToCS.AskSendMsgToUser pMsg = new GCToCS.AskSendMsgToUser()
        {
            guididx = sGUID,
            contents = Encoding.UTF8.GetString(talkMsg)         //ToReview 重复编码解码UTF8字符串
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskInviteFriendsToBattle
    //eMsgToGSToCSFromGC_AskInviteFriendsToBattle
    public void EmsgTocs_AskInviteFriendsToBattle(string nickName, UInt64 roomId)
    {
        GCToCS.AskInviteFriendsToBattle pMsg = new GCToCS.AskInviteFriendsToBattle();
        //ToReview 比原协议少了nickName
        //ToReview 比原协议少了roomId
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换GCReplyAddFriendRequst
    //eMsgToGSToCSFromGC_GCReplyAddFriendRequst
    public void EmsgTocs__GCReplyAddFriendRequst(UInt64 sGUID, bool isAgree)
    {
        GCToCS.GCReplyAddFriendRequst pMsg = new GCToCS.GCReplyAddFriendRequst()
        {
            guididx = sGUID
        };
        //ToReview bool->int 是否可将协议直接改为bool
        if (isAgree)
        {
            pMsg.reply = 1;
        }
        else
        {
            pMsg.reply = 0;
        }
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换GCReplyInviteToBattle
    //协议名不一致eMsgToGSToCSFromGC_GCReplyDownTime->eMsgToGSToCSFromGC_GCReplyInviteToBattle
    //ToReview 名字差别较大，是否正确
    public void EmsgTocs__GCReplyDownTime(string nickName)
    {
        GCToCS.GCReplyInviteToBattle pMsg = new GCToCS.GCReplyInviteToBattle()
        {
            nickname = nickName
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void EmsgTocs__AskBlackListOnlineInfo()
    {
        GCToCS.BlackListOnlineInfo pMsg = new GCToCS.BlackListOnlineInfo();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskQueryUserByNickName
    //协议名不一致eMsgToGSToCSFromGC_AskSendFriendPersonInfo->eMsgToGSToCSFromGC_AskQueryUserByNickName
    public void EmsgTocs_FindFriendPlayer(string niceName, uint length)
    {
        GCToCS.AskQueryUserByNickName pMsg = new GCToCS.AskQueryUserByNickName()
        {
            nickname = niceName             //nickname是否等同于niceName
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //预选英雄
    //已换TryToSeleceHero
    //eMsgToGSToCSFromGC_TryToSelectHero
    public void EmsgTocs_TryToSelectHero(UInt32 heroId)
    {
        GCToSS.TrySeleceHero pMsg = new GCToSS.TrySeleceHero
        {
            heroid = heroId
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void AskHeroAttributesInfo()
    {
        GCToSS.AskHeroAttributesInfo pMsg = new GCToSS.AskHeroAttributesInfo();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgid);
    }

    //已换GasExplosion
    //协议名不一致eMsgToGSToCSFromGC_AskGasExplosion->eMsgToGSToSSFromGC_AskGasExplosion
    public void AskGasExplosion()//请求爆气
    {
        GCToSS.GasExplosion pMsg = new GCToSS.GasExplosion();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换ReconnectToGame
    //eMsgToGSToCSFromGC_AskReconnect
    public void EmsgTocsAskReconnect()
    {
        Debug.LogError("EmsgTocsAskReconnect");
        GCToCS.ReconnectToGame pMsg = new GCToCS.ReconnectToGame()
        {
            name = SelectServerData.Instance.serverUin,
            passwd = SelectServerData.Instance.GateServerToken
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换CompGuideStepId
    //协议名不一致eMsgToGSToCSFromGC_FinishUIGuideTask->eMsgToGSToCSFromGC_CompCSGuideStepId
    public void EmsgTocsAskFinishUIGuideTask(byte isLineTask, int taskId, byte isFinishAll)
    {
        //编译不过注释掉
        //GCToCS.CompGuideStepId pMsg = new GCToCS.CompGuideStepId()
        //{
        //    type = (int)isLineTask,        //ToReview type->int
        //    stepid = taskId
        //};
        ////ToReview byte->bool 何值为true?是否可改为bool
        //if (1 == isFinishAll)
        //{
        //    pMsg.ifcomplete = true;
        //}
        //else
        //{
        //    pMsg.ifcomplete = false;
        //}
        //NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    /// <summary>
    /// 请求自动战斗
    /// </summary>
    //已换AutoAtk
    //eMsgToGSToSSFromGC_AskAutoAttack
    public void GameAutoFight()
    {
        GCToSS.AutoAtk pMsg = new GCToSS.AutoAtk();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换UserGameInfo
    //eMsgToGSToCSFromGC_AskUserGameInfo
    public void PersonInfo()
    {
        GCToCS.UserGameInfo pMsg = new GCToCS.UserGameInfo();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换BuReborn
    //eMsgToGSToSSFromGC_AskBuyRebornHero
    public void Resurrection()
    {
        GCToSS.BuReborn pMsg = new GCToSS.BuReborn();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    ////与EmsgToss_GuideAskBornNpc函数重复使用了同一个消息，并且无代码引用，先屏蔽，如无异样以后删除
    //public void EmsgToss_GuideAskSendNpc(int militaryId, int step, int sendTag)
    //{
    //    //CMsg pcMyMsg = new CMsg(64);
    //    //pcMyMsg.SetProtocalID((Int32)EmsgToSSFromGC.eMsgToGsToCSFromGC_GuideSendNpc);
    //    //pcMyMsg.Add(HolyTechGameBase.Instance.Jx_n32SSID);
    //    //pcMyMsg.Add(step);
    //    //pcMyMsg.Add(militaryId);
    //    //pcMyMsg.Add(sendTag);
    //}

    //已换GuideJumpToHall
    //eMsgToGSToSSFromGC_AskGuideEndJumpToHall
    public void EmsgToss_FinishAllGuideToLobby()
    {
        GCToSS.GuideJumpToHall pMsg = new GCToSS.GuideJumpToHall();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换StepID
    //协议名不一致eMsgToGsToCSFromGC_GuideStep->eMsgToGSToSSFromGC_StepId
    public void EmsgToss_GuideFinishStep(int taskId, int taskType = 1)
    {
        GCToSS.StepID pMsg = new GCToSS.StepID()
        {
            stepid = taskId,
            type = taskType
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    //已换AskBornNPC
    //协议名不一致eMsgToGsToCSFromGC_GuideSendNpc->eMsgToGSToSSFromGC_AskBornNPC
    public void EmsgToss_GuideAskBornNpc(int taskId, int tag)
    {
        GCToSS.AskBornNPC pMsg = new GCToSS.AskBornNPC()
        {
            stepid = taskId,
            state = tag
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    public void EmsgToCs_AskGuideBattleEnd()
    {
        //GCToCS.GuideInfo pMsg = new GCToCS.GuideInfo();
        GCToSS.AskQuickBattleEnd pMsg = new GCToSS.AskQuickBattleEnd();
        {
            pMsg.other = 1;
        }
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgid);
    }

    //已换AskReconnect
    //协议名不一致eMsgToGSToSSFromGC_AskReconnectInfo->eMsgToGSToSSFromGC_AskReconnect
    //ToReview 该消息之前未定义，临时定义了一个，需检查是否完善
    public void EmsgToss_AskReconnectInfo()
    {
        Debug.LogError("EmsgToss_AskReconnectInfo");
        GCToSS.AskReconnect pMsg = new GCToSS.AskReconnect();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    /************************************************************************/
    /*                         当房间玩家位满，请求加入OB位                           */
    /************************************************************************/
    //已换AddOB
    //eMsgToGSToCSFromGC_AskAddObserver
    public void EmsgToss_AskAddObserver(string roomID, string password, int length)
    {
        //senlin
        Debug.LogError("EmsgToss_AskAddObserver");
        /*
        GCToCS.AddOB pMsg = new GCToCS.AddOB()
        {
            battleid = Convert.ToUInt64(roomID),        //ToReview string->ulong，需检查来源是否可靠
            passwd = password
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
         */
    }

    //工具函数
    private GCToSS.Dir ConvertVector3ToDir(Vector3 vec)
    {
        GCToSS.Dir dir = new GCToSS.Dir()
        {
            x = vec.x,
            z = vec.z,
            angle = (float)Math.Atan2(vec.z, vec.x)
        };
        return dir;
    }


    public void EmsgToLs_AskLogin()
    {
        GCToLS.AskLogin pMsg = new GCToLS.AskLogin();
        {
            pMsg.platform = (uint)SelectServerData.Instance.serverPlafrom;
            pMsg.uin =SelectServerData.Instance.serverUin;
            pMsg.sessionid =SelectServerData.Instance.serverSionId;
        }
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgid);
    }

    public void EmsgToss_AskReEnterRoom()
    {
        HolyTechGameBase.Instance.PlayEnd();

        GCToCS.AskReEnterRoom pMsg = new GCToCS.AskReEnterRoom();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    //向服务器请求队伍消息
    public void EmsgToss_RequestMatchTeamList()
    {
        GCToCS.RequestMatchTeamList pMsg = new GCToCS.RequestMatchTeamList();

        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    internal void EmsgToss_AskBattleInfo()
    {
        GCToSS.HerosBattleInfo pMsg = new GCToSS.HerosBattleInfo();
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgid);
    }
    public void EmsgToss_AskChangeNickName(string nickName)
    {
        GCToCS.ChangeNickName pMsg = new GCToCS.ChangeNickName()
        {
            newnickname = nickName
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void EmsgTocs_AddNewGMCmd(string cmd)
    {
        GCToCS.AddCSGMCmd pMsg = new GCToCS.AddCSGMCmd()
        {
            gmcmd = cmd
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void EmsgToss_AskChangeheaderId(uint headID)
    {
        GCToCS.AskChangeheaderId pMsg = new GCToCS.AskChangeheaderId()
        {
            newheaderid = headID,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void EmsgToss_AskChangeRunePage(int index)
    {
        //Debug.LogError("mRunePages = " + index);

        GCToSS.SelectRunePage pMsg = new GCToSS.SelectRunePage()
        {
            pageindex = index,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
    //用于记录点击的ui按钮次数
    public void EmsgTocs_ReportUIEvent()
    {
        Dictionary<GameLog.UIEventType, int> curtEvent = GameLog.Instance.GetCurtUIEvent();
        if (curtEvent == null) { return; }

        GCToCS.CurtUIEvent pMsg = new GCToCS.CurtUIEvent();
        foreach (KeyValuePair<GameLog.UIEventType, int> cv in curtEvent)
        {
            GCToCS.CurtUIEvent.UIEvent ev = new GCToCS.CurtUIEvent.UIEvent();
            ev.uiidx = (uint)cv.Key;
            ev.eventNum = (uint)cv.Value; 
            pMsg.eventlist.Add(ev); 
        }
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }

    public void EmsgTocs_CDKReq(string mTemp)
    {
        GCToCS.CDKReq pMsg = new GCToCS.CDKReq()
        {
            cdkstr = mTemp,
        };
        NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
    }
}
