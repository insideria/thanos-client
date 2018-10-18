using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Thanos.Tools;
using Thanos.GameData;
using Thanos.GameData;
using GameDefine;
using Thanos.GameEntity;
using Thanos.FSM;
using Thanos.GuideDate;
using Thanos.Effect;
using Thanos;
using Thanos.GameState;
using Thanos.Ctrl;

using System.IO;
using Thanos.Network;
using Thanos.Resource;
using Thanos.Model;
using Thanos.View;

public partial class HolyGameLogic : UnitySingleton<HolyGameLogic>
{
    private Int32 mProtoId = 0;
    const int PROTO_DESERIALIZE_ERROR = -1;
    //消息处理中心
    public void HandleNetMsg(System.IO.Stream stream, int n32ProtocalID)
    {
       
        Debug.Log("n32ProtocalID =  " + (GSToGC.MsgID)n32ProtocalID);
        // Debug.Log("n32ProtocalID =  " + n32ProtocalID);
        mProtoId = n32ProtocalID;
        switch (n32ProtocalID)
        {
             
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_GCAskPingRet:
                OnNetMsg_NotifyPing(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserBaseInfo:
                OnNetMsg_NotifyUserBaseInfo(stream);     //用户基本信息
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_GCAskRet:
                OnNetMsg_NotifyReturn(stream);//心跳
                break;
             case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyNetClash:
                OnNetMsg_NotifyNetClash(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleBaseInfo:
                OnNetMsg_NotifyBattleBaseInfo(stream);     // 战斗基本信息  设置地图id  战斗id 是否是重新连接
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleSeatPosInfo:
                OnNetMsg_NotifyBattleSeatPosInfo(stream);  // 座位设置？
                break; 
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleStateChange:
                OnNetMsg_NotifyBattleStateChange(stream);  // 5战斗状态转换
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyCurBattleChange:
                OnNetMsg_NotifyCurBattleChange(stream);  
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectAppear:
                OnNetMsg_NotifyGameObjectAppear(stream);//通知客户端显示游戏对象
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectDisappear:
                OnNetMsg_NotifyGameObjectDisAppear(stream);//通知客户端隐藏对象
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectFreeState:
                OnNetMsg_NotifyGameObjectFreeState(stream);//进入自由状态
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectRunState:
                OnNetMsg_NotifyGameObjectRunState(stream);//通知游戏对象移动
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleLoadingState:
                OnNetMsg_NotifyBattleStart(stream);//名称不对应，且函数体本身就被注释掉了 没有做
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroList:
                OnNetMsg_NotifyHeroList(stream);  //英雄列表（将服务器传过来的可以选择的英雄的ID添加到英雄列表中）
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyCSHeroList:
                OnNetMsg_NotifyCSHeroList(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectPrepareSkillState:
                OnNetMsg_NotifyPrepareSkill(stream);//通知对象准备技能状态
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectReleaseSkillState:
                OnNetMsg_NotifyReleaseSkill(stream);//通知客户端释放技能 前遥
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectUsingSkillState:
                OnNetMsg_NotifyLeadingSkill(stream);//
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectLastingSkillState:
                OnNetMsg_NotifyLastingSkill(stream);//后摇
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectDeadState:
                OnNetMsg_NotifyGameObjectDeadState(stream);//通知游戏对象进入死亡状态
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGoodsInf:
                OnNetMsg_NotifyGoodsInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_ChatInRoom:
                OnNetMsg_NotifyRoomChat(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyRoomBaseInfo:
                OnNetMsg_NotifyRoomBaseInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyTryToChooseHero:
                OnNetMsg_NotifyToChooseHero(stream);  //   试图选择英雄
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleHeroInfo:
                OnNetMsg_NotifyBattleHeroInfo(stream);//   战斗英雄信息
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHPChange:
                OnNetMsg_NotifyHPChange(stream);//hp改变
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMPChange:
                OnNetMsg_NotifyMPChange(stream);//通知魔法值改变
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillEntityInfo:
                OnNetMsg_NotifySkillUnitInfo(stream);//函数体为空，这部分内容没有做
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyAFPData:
                OnNetMsg_NotifyFightPropertyInfo(stream);//战斗属性改变
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHPInfo:
                OnNetMsg_NotifyHpInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMPInfo:
                OnNetMsg_NotifyMpInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroInfo:
                OnNetMsg_NotifyHeroInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillInfo:
                OnNetMsg_NotifySkillInfo(stream);//技能信息
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleFinish:
                OnNetMsg_NotifyBattleFinish(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyExpInfo:
                OnNetMsg_NotifyExpInfo(stream);//设置主角经验
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroLevelInfo:
                OnNetMsg_NotifyLvInfo(stream);//设置主角等级
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserGameInfo:
                OnNetMst_NotifyUserGameInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyAbsorbBegin:
                OnNetMsg_NotifyAbsorbBegin(stream);//吸附开始
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyAbsorbMonsterResult:
                OnNetMsg_NotifyAbsorbMonsterResult(stream);//吸附野怪结果
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyFuryValue:
                OnNetMsg_NotifyFuryValue(stream);//设置狂暴值
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyFuryState:
                OnNetMsg_NotifyFuryState(stream);//狂暴状态
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroKills:
                OnNetMsg_NotifyHeroKills(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyCurDeadTimes:
                OnNetMsg_NotifyCurDeadTimes(stream);//死亡数
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyCurCP:
                OnNetMsg_NotifyCurCP(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroRebornTimes:
                OnNetMsg_NotifyHeroRebornTimes(stream);//重生数
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBuyRebornSuccess:
                OnNetMsg_NotifyBuyRebornSuccess(stream);//购买重生成功
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_BroadcastBattleHeroInfo:
                OnNetMsg_BroadcastBattleHeroInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyPersonalCPChange:
                OnNetMsg_NotifyCPChange(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_BroadCurBattleResult:
                OnNetMsg_BroadCurBattleResult(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_BroadBattlePersonalResult:
                OnNetMsg_BroadCurBattlePersonalResult(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_BroadBuildingDestroyByWho:
                OnNetMst_BroadBuildingDestroyByWho(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBornSolder:
               // OnNotifyBornSoldier(stream); //生成士兵
                OnNotifyBornSoldierCoroutine(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillEnd:
                //OnNetMsg_NotifySkillEnd(stream);//本来就注释的
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyAltarBSOk:
                OnNetMsg_NotifyAltarBSIco(stream);//通知箭塔显示头像
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectReleaseSkill://新技能释放,吟唱
                //OnNetMsg_NotifyGameOjectReleaseSkill(stream);//本来就注释的
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectSkillCD://新的技能冷却控制
                OnNetMsg_NotifyGameObjectSkillCD(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelEmit://新飞行物体
                OnNetMsg_NotifySkillModelEmit(stream); 
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelEmitDestroy://新飞行物体销毁   
                OnNetMsg_NotifySkillModelEmitDestroy(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelHitTarget://新技能受击
                OnNetMsg_NotifySkillModelHitTarget(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelRange://范围技能
                OnNetMsg_NotifySkillModelRange(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelRangeEnd://范围技能结束
                OnNetMsg_NotifySkillModelRangeEnd(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelLeading://技能引导
                OnNetMsg_NotifySkillModelLeading(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelSummonEffect://召唤效果
                OnNetMsg_NotifySkillModelSummonEffect(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelBufEffect://buff效果
                OnNetMsg_NotifySkillModelBuf(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySummonLifeTime://召唤物时间协议
                OnNetMsg_NotifySummonLifeTime(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelStartForceMove:
                OnNetMsg_NotifySkillModelStartForceMove(stream); //强制位移
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelStartForceMoveStop:
                OnNetMsg_NotifySkillModelStartForceMoveStop(stream);    //强制移动结束
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyLeaveBattleSuccess:
                OnNetMsg_NotifyLeaveBattleSuccess();
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleManagerChange:
                OnNetMsg__NotifyBattleManagerChange();
                break;
            case (Int32)GSToGC.MsgID.eMsgToSSFromCS_NotifyBornObj:
                OnNetMsg_NotifyBornObj(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelStartForceMoveTeleport:
                OnNetMsg_NotifySkillModelStartForceMoveTeleport(stream);  //开始传输位置
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_WarningToSelectHero:
                OnNetMsg_WarningToSelectHero();
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyPassitiveSkillRelease:
                OnNetMsg_NotifySkillPassitive(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyOBAppear:
                OnNetMsg_NotifyOBAppear(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyPassitiveSkillLoad:
                OnNetMsg_NotifySkillPassitiveLoad(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyPassitiveSkillUnload:
                OnNetMsg_NotifySkillPassitiveUnLoad(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillModelEmitTurn:
                OnNetMsg_NotifySkillModelEmitTurn(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGameObjectReliveState:
                OnNetMsg_NotifyGameObjectReliveState(stream);//重生状态
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyKillNPC:
                OnNetMsg__NotifyKillNPC(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBlastHurt:
                OnNetMsg_NotifyBlastHurt(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGetNewCommodity:
                OnNetMsg_NotifyGetNewCommodity(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGoodsCfgInfo:
                OnNetMsg_NotifyGoodsCfgInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifySkillUpLv:
                OnNetMsg_NotifySkillUpLv(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromCS_NotifyGuideLastComStep:
                OnNetMsg_NotifyNewsGuideReConnect(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromCS_NotifyReconnectInfo:
                OnNetMsg_NotifyReconnectInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyOBReturnBattleRoom:
                OnNetMsg_NotifyOBReturnBattleRoom();
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserSNSList:
                OnNetMsg_NotifyUserFriendsList(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserSNSListChange:
                OnNetMsg_NotifyUserRemoveList(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMsgFromUser:
                OnNetMsg_NotifyMsgFromUser(stream);
                break;
          
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_UserBeInvitedToBattle:
                OnNetMsg_UserBeInvitedToBattle(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBeAddFriendMsg:
                OnNetMsg_NotifyBeAddFriendMsg(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyQueryNickNameRet:
                OnNetMsg_NotifyQueryNickNameRet(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserGuideSetups:
                OnNetMsg_NotifyUserGuideSetUps(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroReborn:
                OnNetMsg_NotifyHeroReborn(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGuideTips:
                OnNetMsg_NotifySendSoldierTip(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGs_NotifySecondaryGuideTask:
                OnNetMsg_NotifySecondaryGuide(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyNotice:
                OnNetMsg_NotifyNotice(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserCLDays:
                OnNetMsg_NotifyCLdays(stream);
                break;
            case (Int32)LSToGC.MsgID.eMsgToGCFromLS_NotifyLoginResult:
                OnNetMsg_NotifySdkLoginResult(stream);
                break;
            case (Int32)LSToGC.MsgID.eMsgToGCFromLS_NotifyServerBSAddr:
                OnNetMsg_NotifyServerAddr(stream);
                break;
            case (Int32)BSToGC.MsgID.eMsgToGCFromBS_OneClinetLoginCheckRet://204
                OnNet_OneClinetLoginCheckRet(stream);
                break;
            case (Int32)BSToGC.MsgID.eMsgToGCFromBS_AskGateAddressRet://203
                OnNet_NotifyGateServerInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGs_NotifyCurGold:
                OnNet_NotifyCurGold(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGs_NotifyCurDiamond:
                OnNet_NotifyCurDiamond(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGetloginReward_Hero:
                OnNetMsg_NotifyGetRewardHero(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGetloginReward_Skin:
                OnNetMsg_NotifyGetRewardSkin(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyGetloginReward_Rune:
                OnNetMsg_NotifyGetRewardRune(stream);
                break;
            //账号升级
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserBaseUpLv:
                OnNetMsg_NotifyUserBaseUpLv(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleSpanTime:
                OnNetMsg_NotifyBattleSpanTime(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleMatherCount:
                OnNetMsg_NotifyMatchNumber(stream);//通知匹配数量
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroAssist:
                OnNetMsg_NotifyHeroAssist(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyHeroAttributes:
                OnNetMsg_NotifyHeroAttributes(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromBS_NotifyCanInviteFriends:
                OnNetMsg_NotifyCanInviteFriends(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_AskRoomListRet:
                OnNetMsg_NotifyRoomList(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromBS_NotifyCurLastHitNum:
                OnNetMsg_NotifyCurLastHitNum(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserMail:
                OnNetMsg_NotifyUserMail(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyMailInfo:
                Debug.LogWarning("eMsgToGCFromGS_NotifyMailInfo:" + GSToGC.MsgID.eMsgToGCFromGS_NotifyMailInfo);
                OnNetMsg_NotifyMailInfo(stream);
                break;
            case (Int32)GSToGC.MsgID.eMsgToGCFromGS_NotifyIfMailDelAndSort:
                OnNetMsg_NotifyDelAndSortMail(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromBS_NotifyHerosInfo:
                OnNetMsg_NotifyHerosInfo(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamBaseInfo:
                OnNetMsg_NotifyMatchTeamBaseInfo(stream); //通知匹配队伍基本信息
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamPlayerInfo:
                OnNetMsg_NotifyMatchTeamPlayerInfo(stream); //匹配队伍玩家信息
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchTeamSwitch:
                OnNetMsg_NotifyMatchTeamSwitch(stream);  //匹配界面显示 
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMatchInviteJoin:
                OnNetMsg_NotifyMatchInviteJoin(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyNewNickname:
                OnNetMsg_NotifyNewNickname(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyOneMatchNeedOne:
                OnNotifyOneMatchNeedOne(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyNewHeaderid:
                OnNetMsg_NotifyNewHeadID(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyRunesList:
                OnNetMsg_NotifyRunesList(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_UnloadRune:
                OnNetMsg_UnloadRune(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyBattleDelayTime:
                OnNetMsg_NotifyBattleDelayTime(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyRemoveCommodity:
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_GuideResp:
              // OnNetMsg_NotifyGuideResp(stream);//任务
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyOtherItemInfo:
                OnNetMsg_NotifyOtherItemInfo(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyUserLvInfo:
                NotifyUserLvInfo(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromCS_GuideKillsInfo:
                OnNetMsg_NotifyGuideKillsInfo(stream);
                break;
                //mail
            case (int)GSToGC.MsgID.eMsgToGCFromGS_NotifyMailRet:
                OnNetMsg_NotifyMailRet(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_UpdateAllTask:
               // OnNetMsg_UpdateAllTask(stream);//更新所有任务
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_UpdateAllDailyTask:
                OnNetMsg_UpdateAllDailyTask(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_AddOneTask:
                OnNetMsg_AddOneTask(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_UpdateOneTask:
                OnNetMsg_UpdateOneTask(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_DelOneTask:
                OnNetMsg_DelOneTask(stream);
                break;
            case (int)GSToGC.MsgID.eMsgToGCFromGS_RewardsOneTask:
                OnNetMsg_RewardsOneTask(stream);
               break;

        }
    }

    Int32 OnNetMsg_UpdateAllTask(Stream stream)
    {
        GSToGC.NotifyUpdateAllTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        DailyBonusCtrl.Instance.mDailyTaskDic.Clear();
        DailyBonusCtrl.Instance.mInfiniteTaskDic.Clear();
        foreach (GSToGC.TaskData one in pMsg.taskList)
        {
            STaskConfig taskConfig = null;
            taskConfig = ConfigReader.GetDailyTaskInfo(one.task_id);
            if (taskConfig != null)
            {
                CTask oneTask = new CTask();
                oneTask.mGuid = one.task_guid;
                oneTask.mCurCount = one.task_curCount;
                oneTask.mConfig = taskConfig;
                if (oneTask.mCurCount == taskConfig.taskMaxCount) DailyBonusCtrl.Instance.mIsHadTaskFinished = true;
                DailyBonusCtrl.Instance.mDailyTaskDic.Add(oneTask.mGuid,oneTask);
                continue;
            }
            taskConfig = ConfigReader.GetInfiniteTaskInfo(one.task_id);
            if (taskConfig != null)
            {
                CTask oneTask = new CTask();
                oneTask.mGuid = one.task_guid;
                oneTask.mCurCount = one.task_curCount;
                oneTask.mConfig = taskConfig;
                DailyBonusCtrl.Instance.mInfiniteTaskDic.Add(oneTask.mGuid, oneTask);
                continue;
            }
        }
        DailyBonusCtrl.Instance.mIsHadNewDailyTask = pMsg.IsHadNewDailyTask;
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyAllTaskUpdate);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_UpdateAllDailyTask(Stream stream)
    {
        GSToGC.NotifyUpdateAllDailyTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        DailyBonusCtrl.Instance.mDailyTaskDic.Clear();
        foreach (GSToGC.TaskData one in pMsg.taskList)
        {
            STaskConfig taskConfig = null;
            taskConfig = ConfigReader.GetDailyTaskInfo(one.task_id);
            if (taskConfig != null)
            {
                CTask oneTask = new CTask();
                oneTask.mGuid = one.task_guid;
                oneTask.mCurCount = one.task_curCount;
                oneTask.mConfig = taskConfig;
                if (oneTask.mCurCount == taskConfig.taskMaxCount) DailyBonusCtrl.Instance.mIsHadTaskFinished = true;
                DailyBonusCtrl.Instance.mDailyTaskDic.Add(oneTask.mGuid, oneTask);
                continue;
            }
        }
        DailyBonusCtrl.Instance.mIsHadNewDailyTask = pMsg.IsHadNewDailyTask;
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyDailyTaskUpdate);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_AddOneTask(Stream stream)
    {
        GSToGC.NotifyAddOneTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        STaskConfig taskConfig = ConfigReader.GetDailyTaskInfo(pMsg.oneTask.task_id);
        if (taskConfig != null)
        {
            CTask oneTask = new CTask();
            oneTask.mGuid = pMsg.oneTask.task_guid;
            oneTask.mCurCount = pMsg.oneTask.task_curCount;
            oneTask.mConfig = taskConfig;
            DailyBonusCtrl.Instance.mDailyTaskDic.Add(oneTask.mGuid, oneTask);
            EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskAdd, oneTask);
        }
        taskConfig = ConfigReader.GetInfiniteTaskInfo(pMsg.oneTask.task_id);
        if (taskConfig != null)
        {
            CTask oneTask = new CTask();
            oneTask.mGuid = pMsg.oneTask.task_guid;
            oneTask.mCurCount = pMsg.oneTask.task_curCount;
            oneTask.mConfig = taskConfig;
            DailyBonusCtrl.Instance.mInfiniteTaskDic.Add(oneTask.mGuid, oneTask);
            EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskAdd, oneTask);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_UpdateOneTask(Stream stream)
    {
        GSToGC.NotifyUpdateOneTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        STaskConfig taskConfig = ConfigReader.GetDailyTaskInfo(pMsg.oneTask.task_id);
        if (taskConfig != null)
        {
            foreach (KeyValuePair<uint,CTask> oneTask in DailyBonusCtrl.Instance.mDailyTaskDic)
            {
                if (oneTask.Value.mGuid == pMsg.oneTask.task_guid)
                {
                    oneTask.Value.mCurCount = pMsg.oneTask.task_curCount;
                    if (oneTask.Value.mCurCount == oneTask.Value.mConfig.taskMaxCount) DailyBonusCtrl.Instance.mIsHadTaskFinished = true;
                    EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskUpdate, oneTask.Value);
                    break;
                }
            }
        }
        taskConfig = ConfigReader.GetInfiniteTaskInfo(pMsg.oneTask.task_id);
        if (taskConfig != null)
        {
            foreach (KeyValuePair<uint, CTask> oneTask in DailyBonusCtrl.Instance.mInfiniteTaskDic)
            {
                if (oneTask.Value.mGuid == pMsg.oneTask.task_guid)
                {
                    oneTask.Value.mCurCount = pMsg.oneTask.task_curCount;
                    EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskUpdate, oneTask.Value);
                    break;
                }
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_DelOneTask(Stream stream)
    {
        GSToGC.NotifyDelOneTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (KeyValuePair<uint, CTask> oneTask in DailyBonusCtrl.Instance.mDailyTaskDic)
        {
            if (oneTask.Value.mGuid == pMsg.task_guid)
            {
                EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskDel, oneTask.Value);
                DailyBonusCtrl.Instance.mDailyTaskDic.Remove(pMsg.task_guid);
                return (Int32)ErrorCodeEnum.Normal;
            }
        }
        foreach (KeyValuePair<uint, CTask> oneTask in DailyBonusCtrl.Instance.mInfiniteTaskDic)
        {
            if (oneTask.Value.mGuid == pMsg.task_guid)
            {
                EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskDel, oneTask.Value);
                DailyBonusCtrl.Instance.mInfiniteTaskDic.Remove(pMsg.task_guid);
                return (Int32)ErrorCodeEnum.Normal;
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_RewardsOneTask(Stream stream)
    {
        GSToGC.NotifyRewardsOneTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (KeyValuePair<uint, CTask> oneTask in DailyBonusCtrl.Instance.mDailyTaskDic)
        {
            if (oneTask.Value.mGuid == pMsg.task_guid)
            {
                EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskRewards, oneTask.Value);
                return (Int32)ErrorCodeEnum.Normal;
            }
        }
        foreach (KeyValuePair<uint, CTask> oneTask in DailyBonusCtrl.Instance.mInfiniteTaskDic)
        {
            if (oneTask.Value.mGuid == pMsg.task_guid)
            {
                EventCenter.Broadcast<CTask>((Int32)GameEventEnum.GameEvent_NotifyOneTaskRewards, oneTask.Value);
                return (Int32)ErrorCodeEnum.Normal;
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //
    //邮件操作返回值
    ///
    Int32  OnNetMsg_NotifyMailRet(Stream stream)
    {
        GSToGC.NotifyMailRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        int mailId = pMsg.mailid;
        Int32 errocode = pMsg.errcode;
        //给出提示
        MsgInfoManager.Instance.ShowMsg(errocode);
        //删除该id邮件
        Debug.Log("-----error mailid--------" + mailId); 
        MailCtrl.Instance.DelOrSortMailList(mailId, true, true);
        return (Int32)ErrorCodeEnum.Normal;
    }
    /// <summary>
    /// 引导击杀信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyGuideKillsInfo(Stream stream)
    {
        GSToGC.GuideKillsInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID = (ulong)pMsg.guid;// (ulong)pMsg.msgid;
        Int32 npcType = pMsg.npctype;
        EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_GuideKillTask, npcType);
        Debug.Log("-----sGUID:" + sGUID + ";npcType:" + npcType); 
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyOtherItemInfo(Stream stream)
    {
        GSToGC.NotifyOtherItemInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        GoodsModel.Instance.AddOrChangeRuneBaptze(pMsg.item);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 NotifyUserLvInfo(Stream stream)
    {
        GSToGC.NotifyUserLvInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        short level = (short)pMsg.curlv;
        GameUserModel.Instance.SetGameUserLv(level);
        GameUserModel.Instance.SetGameUserExp((int)pMsg.curexp);
        return 0;
    }

    /// <summary>
    /// 新手引导完成信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyGuideResp(Stream stream)
    {
        GSToGC.GuideCSStepInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        bool comp = pMsg.allcomp;
        UIGuideModel.Instance.mIsGuideComp = comp;
        if (!comp)
        {
            UIGuideCtrl.Instance.GuideRespStep(pMsg);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyRemoveCommodity(Stream stream)
    {
        GSToGC.NotifyRemoveCommodity pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        Debug.Log("收到删除商品事件！");

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyBattleDelayTime(Stream stream)
    {
        GSToGC.BattleDelayTime pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        GamePlayCtrl.Instance.BattleDelayTimeBegin(pMsg.delayTime / 1000);

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_UnloadRune(Stream stream)
    {
        GSToGC.UnloadRune pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        RuneEquipCtrl.Instance.UnloadRune(pMsg.page, pMsg.pos);

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyRunesList(Stream stream)
    {
        GSToGC.NotifyRunesList pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        for (int i = 0; i < pMsg.runes_slot_info.Count; ++i)
        {
            GSToGC.RunesSlot sSlotInfo = pMsg.runes_slot_info[i];
            RuneEquipModel.Instance.UpdateRuneSlotInfo(sSlotInfo.runeid, sSlotInfo.page, sSlotInfo.slotpos);
        }

        for (int i = 0; i < pMsg.runesbaginfo.Count; ++i)
        {
            GSToGC.RunesBagInfo sBagInfo = pMsg.runesbaginfo[i];
            MarketRuneListCtrl.Instance.UpdateRuneBagInfo(sBagInfo.runeid, sBagInfo.num, sBagInfo.gottime);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyNewHeadID(Stream stream)
    {
        GSToGC.NotifyNewHeaderid pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        PresonInfoCtrl.Instance.SetHeadID(pMsg.guid, pMsg.newheaderid);
        PresonInfoCtrl.Instance.ChangeHeadID();
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyNewNickname(Stream stream)
    {
        GSToGC.NotifyNewNickname pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        PresonInfoCtrl.Instance.SetNickName(pMsg.guid, pMsg.newnickname);
        PresonInfoCtrl.Instance.ChangeNickName();
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNotifyOneMatchNeedOne(Stream stream)
    {
        GSToGC.NotifyOneMatchNeedOne pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        TeamMatchCtrl.Instance.ShowServerInvitation(pMsg.mapid, pMsg.fightid);

        return (Int32)ErrorCodeEnum.Normal;
    }



    /// <summary>
    /// 邮件
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyDelAndSortMail(Stream stream)
    {
        GSToGC.DelAndSortMail pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //todo:
        Debug.Log("-----GSToGC.DelAndSortMail pMsg--------");
        MailCtrl.Instance.DelOrSortMailList(pMsg);
        return (Int32)ErrorCodeEnum.Normal;
    }
    /// 单封邮件内容：标题,内容，赠送
    Int32 OnNetMsg_NotifyMailInfo(Stream stream)
    {
        GSToGC.MailInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //todo:
        MailCtrl.Instance.UpdateMailInfo(pMsg);
        return (Int32)ErrorCodeEnum.Normal;
    }
    //邮件列表//
    Int32 OnNetMsg_NotifyUserMail(Stream stream)
    {
        GSToGC.NotifyMailList pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //todo:
        MailCtrl.Instance.AddMail(pMsg);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyHerosInfo(Stream stream)
    {
        GSToGC.NotifyHerosInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        foreach (var item in pMsg.info)
        {
            UInt64 sGUID;
            sGUID = (ulong)item.guid;
            BattleingData.Instance.AddInitPlayer(sGUID, item.nickname, item.killnum, item.deadtimes, item.asstimes, item.herolv, item.lasthit, (EntityCampTypeEnum)item.camgpid, (int)item.heroid);
            foreach (var goods in item.goods)
            {
                BattleingData.Instance.AddPlayer(sGUID, 0, BattleDataType.Goods, goods.grid, goods.goodid);
            }
        }
        BattleInfoCtrl.Instance.Enter();
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyRoomList(Stream stream)
    {
        GSToGC.AskRoomListRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return (Int32)ErrorCodeEnum.Normal;
        }

        BattleCtrl.Instance.UpdateRoomList(pMsg.roomlist);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换NetClash（未复查）
    //eMsgToGCFromGS_NotifyNetClash
    Int32 OnNetMsg_NotifyNetClash(Stream stream)
    {
        EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_KickOut);

        NetworkManager.Instance.canReconnect = false;
        NetworkManager.Instance.Close();

        return (Int32)ErrorCodeEnum.Normal;
    }


    Int32 OnNetMsg_NotifyCurLastHitNum(Stream stream)
    {
        GSToGC.LastHitNum pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        UInt64 sguid;
        sguid = pMsg.guid;
        BattleingData.Instance.AddPlayer(sguid, pMsg.lhnum, BattleDataType.LastHit);
        //EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AllPlayerLastHit);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyCanInviteFriends(Stream stream)
    {
        GSToGC.CanInviteFriends pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        if (pMsg.friends.Count == 0)
        {
            MsgInfoManager.Instance.ShowMsg(10033);
            return (Int32)ErrorCodeEnum.Normal;
        }
        foreach (var item in pMsg.friends)
        {
            FriendManager.Instance.AddInvite(item.guididx, item.HeaderId, item.nickname);
        }
        RoomCtrl.Instance.OpenInviteList();
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyHeroAttributes(Stream stream)
    {
        GSToGC.HeroAttributes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.guid;
        BattleingData.Instance.SetAttributes(pMsg.PlayerSpeed, pMsg.AttackInterval, pMsg.AttackRange, pMsg.ResurgenceTime, pMsg.PhysicAttack, pMsg.SpellsAttack, pMsg.PhysicDef, pMsg.SpellsDef);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroAttributesInfo);
        return (Int32)ErrorCodeEnum.Normal;
    }
   

    //通知匹配数量
    Int32 OnNetMsg_NotifyMatchNumber(Stream stream)
    {
        GSToGC.BattleMatcherCount pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //广播消息  更新玩家匹配数量
        EventCenter.Broadcast<int, int>((Int32)GameEventEnum.GameEvent_MatchNumber, pMsg.count, pMsg.maxcount);
        return (Int32)ErrorCodeEnum.Normal;

    }

    Int32 OnNetMsg_NotifyBattleSpanTime(Stream stream)
    {
        GSToGC.BattleSpanTime pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        GameTimeData.Instance.UpdatePlayTime(pMsg.spanTime);
        EventCenter.Broadcast<long>((Int32)GameEventEnum.GameEvent_GameStartTime, pMsg.spanTime);
        return (Int32)ErrorCodeEnum.Normal;
    }
    Int32 OnNetMsg_NotifyUserBaseUpLv(Stream stream)
    {
        GSToGC.UserBaseUpLv pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        Thanos.FEvent eve = new Thanos.FEvent((Int32)(Int32)GameEventEnum.GameEvent_UpLv);
        eve.AddParam("lv", pMsg.lv);
        EventCenter.SendEvent(eve);

        return (Int32)ErrorCodeEnum.Normal;
    }
    Int32 OnNetMsg_NotifyGetRewardRune(Stream stream)
    {
        GSToGC.GetloginReward_Rune pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RemoveDailyBonus);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyGetRewardSkin(Stream stream)
    {
        GSToGC.GetloginReward_Skin pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RemoveDailyBonus);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyGetRewardHero(Stream stream)
    {
        GSToGC.GetloginReward_Hero pMsg;
        if (!ProtobufMsg.MessageDecode<GSToGC.GetloginReward_Hero>(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RemoveDailyBonus);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 玩家当前钻石
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNet_NotifyCurDiamond(Stream stream)
    {
        GSToGC.NotifyCurDiamond pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //GameUserDataCtrl.Instance.
        GameUserCtrl.Instance.GameUserCurDiamond(pMsg.Diamond);
        Debug.Log("OnNet_NotifyCurDiamond  " + pMsg.Diamond);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 玩家当前金币
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNet_NotifyCurGold(Stream stream)
    {
        GSToGC.NotifyCurGold pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        GameUserCtrl.Instance.GameUserCurGold(pMsg.gold);
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNet_NotifyGateServerInfo(Stream stream)
    {
        LoginCtrl.Instance.RecvGateServerInfo(stream);

        return (Int32)ErrorCodeEnum.Normal;
    }


    Int32 OnNet_OneClinetLoginCheckRet(Stream stream)
    {
        BSToGC.ClinetLoginCheckRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt32 loginSuccess = pMsg.login_success;
        if (loginSuccess != 1)//fail
        {
            LoginCtrl.Instance.LoginFail(); //广播登录失败消息  显示失败界面，重新添加监听器
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyServerAddr(Stream stream)
    {
        LoginCtrl.Instance.UpdateServerAddr(stream);

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifySdkLoginResult(Stream stream)
    {
        LSToGC.LoginResult pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        LoginCtrl.Instance.LoginFail();
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyCLdays(Stream stream)
    {
        GSToGC.NotifyUserCLDays pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        DailyBonusCtrl.Instance.SetDayAwards(pMsg.month, pMsg.today, pMsg.totalCldays, pMsg.cldays, pMsg.isTodayCan);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyNotice(Stream stream)
    {
        GSToGC.GameNotice pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (pMsg.notice.Count == 0)
            return (Int32)ErrorCodeEnum.Normal;
        SystemNoticeData.Instance.Clear();
        foreach (var item in pMsg.notice)
        {
            if (string.IsNullOrEmpty(item.notice))
            return (Int32)ErrorCodeEnum.Normal;
            SystemNoticeData.Instance.SetSystemNotList(item.title, (NoticeIdentify)item.flag, (NoticeState)item.status, (int)item.priority, item.notice);
        }
        if (UIGuideModel.Instance.mIsGuideComp)
        {
            SystemNoticeCtrl.Instance.Enter();
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyHeroReborn(Stream stream)//隐藏死亡窗口
    {
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroReborn);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyQueryNickNameRet(Stream stream)
    {
        GSToGC.NotifyQueryNickNameRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_FindClear);
        if (pMsg.info.Count == 0)
        {
            MsgInfoManager.Instance.ShowMsg(40029);
            return (Int32)ErrorCodeEnum.Normal;
        }
        foreach (GSToGC.NotifyQueryNickNameRet.QueryInfo info in pMsg.info)
        {
            string headID = info.headid.ToString();
            string head = null;
            if (!FriendManager.Instance.AllSearch.TryGetValue(info.nickname, out head))
            {
                FriendManager.Instance.AllSearch.Add(info.nickname, headID);
            }
            EventCenter.Broadcast<string, string>((Int32)GameEventEnum.GameEvent_FindFriendInfo, info.nickname, headID);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyBeAddFriendMsg(Stream stream)
    {
        GSToGC.NotifyBeAddFriendMs pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }


        LobbyCtrl.Instance.InviteInfo(pMsg.sdnder_guididx, pMsg.sendnickname);

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_UserBeInvitedToBattle(Stream stream)
    {
        GSToGC.UserBeInvitedToBattle pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        InviteOtherPlayer.Instance.SetInviteion((ulong)pMsg.battleid, pMsg.pwd.ToString(), pMsg.pwd.Length, pMsg.Invitor);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NewInviteRoom);
        EventCenter.Broadcast<string>((Int32)GameEventEnum.GameEvent_InviteAddRoom, pMsg.Invitor);
        return (Int32)ErrorCodeEnum.Normal;
    }
    /// <summary>
    /// 聊天消息
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换
    Int32 OnNetMsg_NotifyMsgFromUser(Stream stream)
    {
        GSToGC.NotifyMsgFromUser pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //if (!IGuideTaskManager.Instance().IsLineTaskFinish())
        //{
        //    return (Int32)EErrorCode.eNormal;
        //}
        Chat chat = null;
        if (!FriendManager.Instance.AllTalkDic.ContainsKey(pMsg.guididx))
        {
            chat = new Chat();
            chat.SetMsgInfo(pMsg.guididx, pMsg.nickname, pMsg.chatstr, MsgTalkEnum.UnReadMsg, pMsg.headid, false);
            FriendManager.Instance.AllTalkDic.Add(pMsg.guididx, chat);
        }
        else
        {
            chat = FriendManager.Instance.AllTalkDic[pMsg.guididx];
            chat.SetMsgInfo(pMsg.guididx, pMsg.nickname.Trim(), pMsg.chatstr, MsgTalkEnum.UnReadMsg, pMsg.headid, false);
        }
        EventCenter.Broadcast<bool>((Int32)GameEventEnum.GameEvent_ReceiveLobbyMsg, true);
        EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameEvent_ReceiveNewMsg, pMsg.guididx);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    //消息名不一致eMsgToGCFromGS_NotifyUserRemovesList->eMsgToGCFromGS_NotifyUserSNSListChange
    Int32 OnNetMsg_NotifyUserRemoveList(Stream stream)
    {
        //ToReview 比原来多了循环
        GSToGC.NotifyUserSNSListChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        FRIENDTYPE type = (FRIENDTYPE)pMsg.type;
        EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameEvent_RemoveFriend, pMsg.guididx);

        if (type == FRIENDTYPE.FRIENDLIST)
        {
            FriendManager.Instance.DelFriend(pMsg.guididx);
        }
        else if (type == FRIENDTYPE.BLACKLIST)
        {
            FriendManager.Instance.DelBlackList(pMsg.guididx);
        }
        EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameEvent_RemoveFriendEnd, pMsg.guididx);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    //协议名不一致eMsgToGCFromGS_NotifyUserFriendsList->eMsgToGCFromGS_NotifyUserSNSList
    Int32 OnNetMsg_NotifyUserFriendsList(Stream stream)
    {
        GSToGC.NotifyUserSNSList pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (GSToGC.SNSInfo info in pMsg.info)
        {
            int headID = (int)info.headid;
            byte isOnLine = (byte)info.status;
            Friend friend = null;
            FRIENDTYPE type = (FRIENDTYPE)info.type;
            bool isFriend = true;
            if (type == FRIENDTYPE.FRIENDLIST)
            {
                isFriend = true;
                if (!FriendManager.Instance.AllFriends.TryGetValue(info.guididx, out friend))
                {
                    friend = new Friend();
                    friend.SetFriendInfo(info.guididx, info.nickname, headID, isOnLine == 1, info.viplv);
                    FriendManager.Instance.AddFriend(info.guididx, friend);
                    EventCenter.Broadcast<Friend>((Int32)GameEventEnum.GameEvent_CreateFriendPrefab, friend);
                }
                else
                {
                    friend.SetFriendInfo(info.guididx, info.nickname, headID, isOnLine == 1, info.viplv);
                    EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_FriendChangeInfo, info.guididx);
                }
            }
            else if (type == FRIENDTYPE.BLACKLIST)
            {
                isFriend = false;
                if (!FriendManager.Instance.AllBlackDic.TryGetValue(info.guididx, out friend))
                {
                    friend = new Friend();
                    friend.SetFriendInfo(info.guididx, info.nickname, headID, false);
                    FriendManager.Instance.AddOnLineBlackList(info.guididx, friend);
                    EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_CreateBlackPrefab);
                }//暂时先注掉。黑名单暂时无在线状态
                //else
                //{
                //    friend.SetFriendInfo(info.guididx, info.nickname, headID, false);
                //    EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_FriendChangeInfo, info.guididx);
                //}
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换（不需要参数）
    Int32 OnNetMsg_NotifyOBReturnBattleRoom()
    {
        Thanos.FEvent evt = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
        evt.AddParam("NextState", GameStateTypeEnum.Room);
        EventCenter.SendEvent(evt);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //祭坛头像是否成功
    Int32 OnNetMsg_NotifyAltarHeadIcon(Message pcMsg)
    {
        UInt32 m_AltarId = pcMsg.GetUInt32();
        Int32 altrId = pcMsg.GetInt32();
        Int32 soderType = pcMsg.GetInt32();
        Debug.LogError("ji tan");
        //AltarManager.Instance.ShowAltarHead(altrId, soderType);
        //if (UIAltarSelect.Instance != null)
        //    UIAltarSelect.Instance.Destoys();

        return (Int32)ErrorCodeEnum.Normal;
    }

    //GS通知OB 对象技能升级
    //已换
    Int32 OnNetMsg_NotifySkillUpLv(Stream stream)
    {
        GSToGC.NotifySkillUpLv pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGuid;
        sGuid = pMsg.guid;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGuid, out entity))
            if (entity is IPlayer)
            {
                PlayerSkillData.Instance.SetPlayerSkillInfo((IPlayer)entity, (int)pMsg.skillpos, (int)pMsg.skillid); //ToReview uint->int
                EventCenter.Broadcast<IPlayer>((Int32)GameEventEnum.GameEvent_SkillUpLvChange, (IPlayer)entity);
            }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //补兵
    //已换
    //协议名不一致eMsgToSSFromCS_NotifyKillNPC->eMsgToGCFromGS_NotifyKillNPC
    Int32 OnNetMsg__NotifyKillNPC(Stream stream)
    {
        //UInt64 sMasterGUID = pcMsg.GetGUID();
        //UInt32 DI = pcMsg.GetUInt32();
        //Iplayer player = PlayerManager.Instance.LocalPlayer;
        //if (UIViewerBattleInfo.Instance != null)
        //{
        //    UIViewerBattleInfo.Instance.SetFarmInfo(sMasterGUID, (int)DI);
        //}
        //return (Int32)EErrorCode.eNormal;

        GSToGC.NotifyKillNPC pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sMasterGUID;
        sMasterGUID = pMsg.guid;
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        if (UIViewerBattleInfo.Instance != null)
        {
            UIViewerBattleInfo.Instance.SetFarmInfo(sMasterGUID, (int)pMsg.killnum);    //ToReview uint->int
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    //协议名不一致eMsgToSSFromCS_NotifyBlastHurt->eMsgToGCFromGS_NotifyBlastHurt
    Int32 OnNetMsg_NotifyBlastHurt(Stream stream)
    {
        GSToGC.NotifyBlastHurt pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 id;
        id = pMsg.guid;
        IEntity entity;
        Vector3 posInWorld = Vector3.zero;
        if (EntityManager.AllEntitys.TryGetValue(id, out entity))
            CrticalStrikeManager.Instance.CreateCrticalStrike(pMsg.blasthp, entity);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 帐号获得新的物品
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyGetNewCommodity(Stream stream)
    {
        GSToGC.NotifyGetNewCommodity pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        GameUserCtrl.Instance.GameUserGetNewCommodity((int)pMsg.Commodityid);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 获得商城所要出售的商品的信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyGoodsCfgInfo(Stream stream)
    {
        GSToGC.GoodsBuyCfgInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (GSToGC.GoodsCfgInfo cfgInfo in pMsg.info)
        {
            int gsId = cfgInfo.goodid;
            if (GameDefine.GameConstDefine.IfHeroBuyCfg(gsId))
            {
                if (ConfigReader.HeroBuyXmlInfoDict.ContainsKey(gsId))
                {
                    MarketHeroListModel.Instance.HeroListInfo(cfgInfo);
                }
            }
            else if (GameDefine.GameConstDefine.IfRuneBuyCfg(gsId))
            {
                MarketRuneListModel.Instance.AddRuneCfgListInfo(cfgInfo);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyGameObjectReliveState(Stream stream)
    {
        GSToGC.NotifyGameObjectReliveState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 ford = this.ConvertDirToVector3(pMsg.dir);
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            pos.y = entity.RealEntity.transform.position.y;
            entity.GOSSI.sServerBeginPos = pos;
            entity.GOSSI.sServerSyncPos = pos;
            entity.GOSSI.sServerDir = ford;
            entity.GOSSI.fServerSpeed = entity.EntityFSMMoveSpeed;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;
            entity.EntityFSMChangedata(pos, ford, entity.EntityFSMMoveSpeed);
            entity.OnFSMStateChange(EntityReliveFSM.Instance);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// Notify观察者英雄信息
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    Int32 OnNetMsg_NotifyDefaultHeros(Message pcMsg)
    {
        Int32 m_total = pcMsg.GetInt32();
        for (int to = 0; to < m_total; to++)
        {
            UInt64 sGUID = pcMsg.GetGUID();
            int m_camp = pcMsg.GetInt32();
            UInt32 m_un32HeaderID = pcMsg.GetUInt32();
            UInt32 m_un32Lv = pcMsg.GetUInt32();
            UInt32 m_un32HP = pcMsg.GetUInt32();
            UInt32 m_un32MP = pcMsg.GetUInt32();
            IEntity entity;
            EntityManager.AllEntitys.TryGetValue(sGUID, out entity);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroInfoChange, (IPlayer)entity);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    //协议名不一致eMsgToSSFromCS_NotifyOBAppear->eMsgToGCFromGS_NotifyOBAppear
    Int32 OnNetMsg_NotifyOBAppear(Stream stream)
    {
        GSToGC.NotifyOBAppear pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (LoadScene.Instance != null)
        {
            LoadScene.Instance.CloseLoading();
        }
        //ToReview obid没用上
        if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerObType)
        {
            PlayerManager.Instance.LocalAccount.EntityCamp = GameMethod.GetEntityCamp(pMsg.camp);
        }
        if (UIViewer.Instance != null)
        {
            UIViewer.Instance.gameObject.AddComponent<UIDragObCamera>();
        }
        if (UIDragObCamera.Instance != null)
        {
            UIDragObCamera.Instance.SetUsable(true);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换
   // 特效转向
    Int32 OnNetMsg_NotifySkillModelEmitTurn(Stream stream)
    {
        GSToGC.NotifySkillModelEmitTurn pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 master;
        master = pMsg.guid;
        //ToReview effectid,pos,dir,targuid,tarpos没用上
        IEffect effect = EffectManager.Instance.GetEffect(pMsg.progectid);
        if (effect != null)
        {
            FlyEffect fEffect = effect as FlyEffect;
            if (fEffect != null)
            {
                fEffect.isTurn = true;
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifySkillPassitiveLoad(Stream stream)
    {
        GSToGC.NotifyPassitiveSkillLoad pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 masterKey;
        masterKey = pMsg.guid;
        uint skillid = (uint)pMsg.skillid;  //ToReview int->uint
        SkillPassiveConfigInfo skillInfo = ConfigReader.GetSkillPassiveConfig(skillid);
        IEntity entity = null;
        EntityManager.AllEntitys.TryGetValue(masterKey, out entity);
        if (skillInfo != null && entity != null)
        {
            EffectManager.Instance.CreatePassitiveEffect(entity, skillid, (uint)pMsg.uniqueid); //ToReview int->uint
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifySkillPassitiveUnLoad(Stream stream)
    {
        GSToGC.NotifyPassitiveSkillUnLoad pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 masterKey;
        masterKey = pMsg.guid;
        //ToReview 原协议skillid没用上，原协议projectId在新协议中叫skillid?
        EffectManager.Instance.DestroyEffect(pMsg.uniqueid);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    //协议名不一致eMsgToGCFromGS_NotifyPassitiveSkill->eMsgToGCFromGS_NotifyPassitiveSkillRelease
    Int32 OnNetMsg_NotifySkillPassitive(Stream stream)//触发，可以多次
    {
        GSToGC.NotifyPassitiveSkillRelease pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 masterKey;
        masterKey = pMsg.guid;
        SkillPassiveConfigInfo skillInfo = ConfigReader.GetSkillPassiveConfig((uint)pMsg.skillid);      //ToReview int->uint
        IEntity entity = null;
        EntityManager.AllEntitys.TryGetValue(masterKey, out entity);
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        if (player != null && entity is IPlayer && player == entity)
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerPassiveSkillsUpLv, pMsg.skillid, pMsg.timeLeft);
        }
        if (skillInfo != null && entity != null)
        {
            entity.RealEntity.PlayerAnimation(skillInfo.action);
            IEffect effect = EffectManager.Instance.CreateNormalEffect(GameConstDefine.LoadGameSkillEffectPath + "release/" + skillInfo.effect, entity.RealEntity.objPoint.gameObject);
            string soundPath = "";
            soundPath = GameConstDefine.LoadGameSoundPath + skillInfo.sound;

            ResourceItem unit = ResourcesManager.Instance.loadImmediate(soundPath, ResourceType.ASSET);
            if (unit.Asset != null && effect != null)
            {
                AudioClip clip = unit.Asset as AudioClip;
                if (clip != null)
                {
                    AudioSource Audio = AudioManager.Instance.PlayEffectAudio(clip);
                    SceneSoundManager.Instance.addSound(Audio, effect.obj);
                }
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifySkillModelStartForceMoveTeleport(Stream stream)
    {
        GSToGC.NotifySkillModelStartForceMoveTeleport pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 masterKey;
        masterKey = pMsg.guid;
        Vector3 beginPos = this.ConvertPosToVector3(pMsg.beginpos);//此时实体的位置
        Vector3 beginDir = this.ConvertDirToVector3(pMsg.begindir);//此时实体的方向
        Vector3 targetPos = this.ConvertPosToVector3(pMsg.tarpos);//目标位置
        IEntity entity = null;
        EntityManager.AllEntitys.TryGetValue(masterKey, out entity);//获取实体
        SkillMoveConfig skillInfo = ConfigReader.GetSkillMoveConfig(pMsg.effectid);//获取技能配置文件
        if (entity != null)
        {
            entity.RealEntity.HideTrail();//隐藏Buff
            entity.RealEntity.gameObject.transform.position = targetPos;//位置设置
            entity.RealEntity.gameObject.transform.rotation = Quaternion.LookRotation(beginDir);//朝向设置
            entity.RealEntity.ShowTrail();//延迟显示buff
        }
        if (skillInfo != null)
        {
            string startPath = GameConstDefine.LoadGameSkillEffectPath + "release/" + skillInfo.effectStart;//如果是回城，那么effectStart为0
            string endPath = GameConstDefine.LoadGameSkillEffectPath + "release/" + skillInfo.effectEnd;//如果是回城，那么effectEnd为0
            EffectManager.Instance.CreateNormalEffect(startPath, beginPos, beginDir);//创建特效时如果路径末尾是0，直接返回
            EffectManager.Instance.CreateNormalEffect(endPath, targetPos, beginDir);
            if (entity != null)
            {
                entity.RealEntity.PlayerAnimation(skillInfo.action); //如果回城，那么就播放free动画
            }
        }
        if (UIViewerPersonInfo.Instance != null)
        {
            IPlayer player = UIViewerPersonInfo.Instance.SetCurrClickPlayer;
            if (player == entity)
            {
                UIViewerPersonInfo.Instance.SetCurrCamearLockHead();
            }
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroBackTown, (IPlayer)entity);//回城后重置祭坛
        return (Int32)ErrorCodeEnum.Normal;
    }
    /// <summary>
    /// 强制位移
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换NotifySkillModelStartForceMoveStop
    //已换OnNetMsg_NotifySkillModelStartForceMoveStop
    Int32 OnNetMsg_NotifySkillModelStartForceMoveStop(Stream stream)
    {
        GSToGC.NotifySkillModelStartForceMoveStop pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            Vector3 pos = this.ConvertPosToVector3(pMsg.pos);//服务器传过来的位置（已到达目标位置）
            Vector3 dir = entity.EntityFSMDirection;//方向（方向随实体的方向，并非初始化的方向）
            entity.EntityFSMChangedata(pos, dir);
            entity.GOSSI.sServerBeginPos = pos;
            entity.GOSSI.sServerSyncPos = pos;
            entity.GOSSI.sLocalSyncDir = dir;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;
            //对比真实位置与服务器位置差值
            if (Vector3.Distance(pos, entity.realObject.transform.position) >= 1f)
            {
                entity.realObject.transform.position = pos;//按服务器的位置
            }
            SkillMoveConfig skillInfo = ConfigReader.GetSkillMoveConfig(pMsg.effectid);
            if (skillInfo != null)
            {
                EffectManager.Instance.CreateNormalEffect(GameConstDefine.LoadGameSkillEffectPath + "release/" + skillInfo.effectEnd, entity.RealEntity.objPoint.gameObject);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }


    /// <summary>
    /// 强制位移结束
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换NotifySkillModelStartForceMove
    //eMsgToGCFromGS_NotifySkillModelStartForceMove
    Int32 OnNetMsg_NotifySkillModelStartForceMove(Stream stream)
    {
        GSToGC.NotifySkillModelStartForceMove pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        float speed = (float)pMsg.speed / 100;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            Vector3 pos = this.ConvertPosToVector3(pMsg.pod);
            Vector3 dir = this.ConvertDirToVector3(pMsg.dir);
            entity.EntityFSMChangedata(pos, dir, speed);
            entity.GOSSI.sServerBeginPos = pos;
            entity.GOSSI.sServerSyncPos = pos;
            entity.GOSSI.sLocalSyncDir = dir;
            entity.GOSSI.sServerDir = dir;
            entity.GOSSI.fServerSpeed = speed;
            entity.GOSSI.fForceMoveSpeed = speed;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.EntityFSMChangeDataOnForceMove(pMsg.effectid);

            entity.OnFSMStateChange(EntityForceMoveFSM.Instance);

            //创建强制移动特效
            SkillMoveConfig skillInfo = ConfigReader.GetSkillMoveConfig(pMsg.effectid);
            if (skillInfo != null)
            {
                EffectManager.Instance.CreateNormalEffect(GameConstDefine.LoadGameSkillEffectPath + "release/" + skillInfo.effectStart, entity.RealEntity.objPoint.gameObject);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyBornObj(Stream stream)
    {
        GSToGC.NotifyBornObj pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        NpcConfigInfo info = ConfigReader.GetNpcInfo(pMsg.type);       //ToReview npcId是否就是type
        if (info != null)
        {
            List<float> probabilityList = GameMethod.ResolveToFloatList(info.n32Script1Rate);//获得概率集合
            for (int i = 0; i < probabilityList.Count; i++)
            {
                probabilityList[i] = (float)(probabilityList[i] - 90000) / 100f;
            }
            int index = GameMethod.RandProbablityIndex(probabilityList);//获得概率下标
            List<string> voiceList = GameMethod.ResolveToStrList(info.un32Script1);
            string name = voiceList[index];//获得概率下标对应的声音
            string path = AudioDefine.PATH_JUNGLE_MONSTER_BE_ATK_SOUND + name;
            //AudioClip clip = Resources.Load(path) as AudioClip;
            ResourceItem unit = ResourcesManager.Instance.loadImmediate(path, ResourceType.ASSET);
            AudioClip clip = unit.Asset as AudioClip;
            AudioManager.Instance.PlayLongVoiceAudio(clip);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg__NotifyBattleManagerChange()
    {
        if (GameStateManager.Instance.GetCurState().GetStateType() == GameStateTypeEnum.Room)
        {
            MsgInfoManager.Instance.ShowMsg((int)ErrorTypeEnum.ManagerChange);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换LeaveBattleSuccess
    //eMsgToGCFromGS_NotifyLeaveBattleSuccess
    Int32 OnNetMsg_NotifyLeaveBattleSuccess()
    {
        PlayerManager.Instance.CleanAccount();

        EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.RoomBack));

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换NotifySummonLifeTime
    //eMsgToGCFromGS_NotifySummonLifeTime
    Int32 OnNetMsg_NotifySummonLifeTime(Stream stream)
    {
        GSToGC.NotifySummonLifeTime pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 objGuid;
        objGuid = pMsg.guid;
        float totalTime = pMsg.lifetime / 1000.0f;
        float remainTime = pMsg.resttime / 1000.0f;
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);
        IEntity entity;
        EntityManager.AllEntitys.TryGetValue(objGuid, out entity);
        if (entity != null)
        {
            entity.RealEntity.gameObject.transform.position = pos;
            entity.RealEntity.gameObject.transform.rotation = Quaternion.LookRotation(dir);
            entity.EntityOverplusRemainTime = remainTime;
            entity.EntityOverplusTotalTime = totalTime;
        }
        //设置进度条
        //原本为空
        return (Int32)ErrorCodeEnum.Normal;
    }

    /************************************************************************/
    /*OnNetMsg_NotifySkillModelSummonEffect                                 */
    /************************************************************************/
    Int32 OnNetMsg_NotifySkillModelSummonEffect(Stream stream)
    {
        StartCoroutine(OnNetMsg_NotifySkillModelSummonEffectCoroutine(stream));

        return (Int32)ErrorCodeEnum.Normal;
    }

    public IEnumerator OnNetMsg_NotifySkillModelSummonEffectCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.SummonEffect pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        //创建特效
        yield return 1;

        UInt64 skillowner;
        skillowner = pMsg.guid;
        UInt64 npcid;
        npcid = pMsg.guid;
        IEntity entitynpc = null;
        EntityManager.AllEntitys.TryGetValue(npcid, out entitynpc);
        SkillSummonConfig skillconfig = ConfigReader.GetSkillSummonConfig((int)pMsg.effectid);  //ToReview uint->int
        if (entitynpc != null && skillconfig != null)
        {
            Thanos.Effect.IEffect effect = Thanos.Effect.EffectManager.Instance.CreateNormalEffect(GameConstDefine.LoadGameSkillEffectPath + "release/" + skillconfig.effect, entitynpc.RealEntity.objPoint.gameObject);


            //创建声音
            yield return 1;

            string soundPath = "";
            soundPath = GameConstDefine.LoadGameSoundPath + skillconfig.sound;

            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(soundPath, ResourceType.ASSET);

            if (objUnit.Asset != null)
            {
                AudioClip clip = objUnit.Asset as AudioClip;
                if (clip != null)
                {
                    AudioSource Audio = AudioManager.Instance.PlayEffectAudio(clip);
                    if (effect != null)
                    {
                        SceneSoundManager.Instance.addSound(Audio, effect.obj);
                    }
                }
            }
        }

    }
    /************************************************************************/
    /* OnNetMsg_NotifySkillModelBuf                                         */
    /************************************************************************/
    Int32 OnNetMsg_NotifySkillModelBuf(Stream stream)
    {
        StartCoroutine(OnNetMsg_NotifySkillModelBufCoroutine(stream));
        return (Int32)ErrorCodeEnum.Normal;
    }

    public IEnumerator OnNetMsg_NotifySkillModelBufCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.BuffEffect pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        yield return 1;

        //创建特效
        UInt64 skillowner;
        skillowner = pMsg.guid;
        UInt64 skilltarget;
        skilltarget = pMsg.targuid;
        float rTime = pMsg.time / 1000.0f;
        IEntity target = null;
        EntityManager.AllEntitys.TryGetValue(skilltarget, out target);
        if (0 == pMsg.state)
        {
            Thanos.Skill.BuffManager.Instance.AddBuff(pMsg.uniqueid, pMsg.effectid, rTime, target);
            IEntity entity = null;
            EntityManager.AllEntitys.TryGetValue(skilltarget, out entity);
          //  HolyTech.Effect.EffectManager.Instance.CreateBuffEffect(entity, pMsg.effectid, pMsg.uniqueid);    //ToReview uniqueid是否就是instid
        }
        else if (1 == pMsg.state)
        {
            Thanos.Skill.BuffManager.Instance.RemoveBuff(pMsg.uniqueid);
            Thanos.Effect.EffectManager.Instance.DestroyEffect(pMsg.uniqueid);
        }
    }


    /************************************************************************/
    /*OnNetMsg_NotifySkillModelLeading                                      */
    /************************************************************************/
    public Int32 OnNetMsg_NotifySkillModelLeading(Stream stream)
    {
        StartCoroutine(SkillModelLeadingCoroutine(stream));
        return (Int32)ErrorCodeEnum.Normal;
    }

    public IEnumerator SkillModelLeadingCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.NotifySkillModelLeading pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        //创建特效
        yield return 1;

        UInt64 owner = pMsg.guid;
        UInt64 target = pMsg.targuid;
        IEntity ownerEntity;
        EntityManager.AllEntitys.TryGetValue(owner, out ownerEntity);
        SkillLeadingonfig skillconfig = ConfigReader.GetSkillLeadingConfig(pMsg.effectid);
        if (skillconfig != null && ownerEntity != null)
        {
            IPlayer player = PlayerManager.Instance.LocalPlayer;
            if (0 == pMsg.state)    //0:开始，1：结束，2：失败
            {
                if (false == Thanos.Effect.EffectManager.Instance.IsValid(pMsg.uniqueid))
                {
                    Thanos.Effect.EffectManager.Instance.CreateLeadingEffect(owner, (uint)pMsg.effectid, (uint)pMsg.uniqueid);
                }
                if (player != null && owner == player.GameObjGUID)
                {
                    ProgressBarInterface.startProgressBar(skillconfig.time);
                    //Debug.Log("pMsg.effectid" + pMsg.effectid);
                }
            }
            else
            {
                Thanos.Effect.EffectManager.Instance.DestroyEffect(pMsg.uniqueid);
                if (player != null && owner == player.GameObjGUID)
                {
                    ProgressBarInterface.hideProgressBar();
                }
            }
        }
    }


    //已换RangeEffectEnd
    //eMsgToGCFromGS_NotifySkillModelRangeEnd
    Int32 OnNetMsg_NotifySkillModelRangeEnd(Stream stream)
    {
        GSToGC.RangeEffectEnd pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //ToReview guid没用上
        Thanos.Effect.EffectManager.Instance.DestroyEffect(pMsg.uniqueid);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /************************************************************************/
    /*OnNetMsg_NotifySkillModelRange                                         */
    /************************************************************************/
    Int32 OnNetMsg_NotifySkillModelRange(Stream stream)
    {
        StartCoroutine(OnNetMsg_NotifySkillModelRangeCoroutine(stream));
        return (Int32)ErrorCodeEnum.Normal;
    }
    
    public IEnumerator OnNetMsg_NotifySkillModelRangeCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.RangeEffect pMsg = null;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }
        if (pMsg != null)
        {
            UInt64 owner = pMsg.guid;
            Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
            Vector3 dir = this.ConvertDirToVector3(pMsg.dir);

            //创建特效
            yield return 1;
           // 创建技能范围特效
            Thanos.Effect.EffectManager.Instance.CreateAreaEffect(owner, pMsg.effectid, pMsg.uniqueid, dir, pos);
        }
        else
        {
            Debug.LogError("msg is null in OnNetMsg_NotifySkillModelRangeCoroutine");
        }
    }


    /************************************************************************/
    /*OnNetMsg_NotifySkillModelEmit                                         */
    /************************************************************************/
    Int32 OnNetMsg_NotifySkillModelEmit(Stream stream)
    {
        StartCoroutine(OnNetMsg_NotifySkillModelEmitCoroutine(stream));       
        return (Int32)ErrorCodeEnum.Normal;  
    }

    public IEnumerator OnNetMsg_NotifySkillModelEmitCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.EmitSkill pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        UInt64 skillPlayerID = pMsg.guid;
        UInt64 skillTargetID = pMsg.targuid;
        Vector3 pos = this.ConvertPosToVector3(pMsg.tarpos);
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);

        //普通追踪特效
        yield return 1;
        FlyEffect effect = Thanos.Effect.EffectManager.Instance.CreateFlyEffect(skillPlayerID, skillTargetID, pMsg.effectid, (uint)pMsg.uniqueid, pos, dir, pMsg.ifAbsorbSkill);
    }

    //特效销毁
    Int32 OnNetMsg_NotifySkillModelEmitDestroy(Stream stream)
    {
        GSToGC.DestroyEmitEffect pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        IEffect effect = EffectManager.Instance.GetEffect(pMsg.uniqueid);
        if (effect != null)
        {
            if (effect.mType == IEffect.ESkillEffectType.eET_FlyEffect)
            {
                FlyEffect flyEffect = effect as FlyEffect;

                //拖拽类型
                if (flyEffect.emitType == 8)
                {
                    flyEffect.DragRibbonBack();
                }
                else
                    EffectManager.Instance.DestroyEffect(flyEffect);
            }
            else
            {
                EffectManager.Instance.DestroyEffect(effect);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    /************************************************************************/
    /* OnNetMsg_NotifySkillModelHitTarget                                   */
    /************************************************************************/
    Int32 OnNetMsg_NotifySkillModelHitTarget(Stream stream)
    {
        StartCoroutine(OnNetMsg_NotifySkillModelHitTargetCoroutine(stream));
        return (Int32)ErrorCodeEnum.Normal;
    }

    public IEnumerator OnNetMsg_NotifySkillModelHitTargetCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.HitTar pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        //创建特效
        UInt64 ownerID;
        ownerID = pMsg.guid;
        UInt64 targetID;
        targetID = pMsg.targuid;

        EventCenter.Broadcast<UInt64, uint, UInt64>((Int32)GameEventEnum.GameEvent_BroadcastBeAtk, ownerID, pMsg.effectid, targetID);//添加警告  光圈
        yield return 1;
        Thanos.Effect.EffectManager.Instance.CreateBeAttackEffect(ownerID, targetID, pMsg.effectid);//创建受击特效
    }

    //广播祭坛头像
    public enum eAltarBorn
    {
        eAltarBornNPCType_Add,
        eAltarBornNPCType_Del,
    }
    Int32 OnNetMsg_NotifyAltarBSIco(Stream stream)
    {
        GSToGC.NotifyAltarBSIco pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        // Int64 altrGuid = (Int64)pMsg.guid;
        Int32 altrId = (int)pMsg.altarid;
        Int32 soderType = (int)pMsg.objtype;
        Int32 optype = (int)pMsg.optype;
        if ((eAltarBorn)optype == eAltarBorn.eAltarBornNPCType_Add)
        {
            AltarData.Instance.AddAltarDic(altrId, soderType);
            if (AltarManager.Instance != null)
                AltarManager.Instance.ShowAltarHead(altrId, soderType);
        }
        else
        {
            AltarData.Instance.DelAltarDic(altrId);
            if (AltarManager.Instance != null)
                AltarManager.Instance.DelAltarHead(altrId, soderType);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }
    Int32 OnNotifyHeroDisplacementInfo(Message pcMsg)
    {
        UInt64 sGUID = pcMsg.GetGUID();
        float dirX = pcMsg.GetFloat() / 100;
        float dirY = pcMsg.GetFloat() / 100;
        float dirZ = pcMsg.GetFloat() / 100;
        float posX = pcMsg.GetFloat() / 100;
        float posY = pcMsg.GetFloat() / 100;
        float posZ = pcMsg.GetFloat() / 100;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            if (entity.realObject != null)
            {
                entity.realObject.transform.position = new Vector3(posX, posY, posZ);
                entity.realObject.transform.rotation = Quaternion.LookRotation(new Vector3(dirX, dirY, dirZ));
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    #region Skill

    //已换
    Int32 OnNetMsg_BroadCurBattleResult(Stream stream)
    {
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        if (player == null)
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        GSToGC.BroadcastBatteleRes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        ScoreCtrl.Instance.GetSettlementList().Clear();
        foreach (GSToGC.BroadcastBatteleRes.ResInfo info in pMsg.resinfo)
        {
            UInt64 objGUID;
            objGUID = info.objguid;
            ScoreCtrl.Instance.SetSettlementInfo(objGUID, info.heroid, info.nickname, info.killtimes,
                info.deadtimes, info.asstimes, info.curlevel, info.totalcp / 1000, info.lasthit, (EntityCampTypeEnum)info.camgpid);
        }
        bool isWinCamp = pMsg.ifwin;
        if (isWinCamp)
        {
            PlayerManager.Instance.LocalPlayer.StateSituation = SITUATION.WIN;
        }
        else
        {
            PlayerManager.Instance.LocalPlayer.StateSituation = SITUATION.LOSE;
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_BroadCurBattlePersonalResult(Stream stream)
    {
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        if (player == null)
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        GSToGC.BroadcastBattelePersonalRes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        ScoreCtrl.Instance.SetScoreInfo(pMsg.got_gold, pMsg.old_exp, pMsg.got_exp, (short)pMsg.old_lv, (short)pMsg.cur_lv, pMsg.cur_exp);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //战斗英雄信息
    Int32 OnNetMsg_BroadcastBattleHeroInfo(Stream stream)
    {
        GSToGC.BroadcastBattleHeroInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (GSToGC.BroadcastBattleHeroInfo.HeroInfo info in pMsg.heroinfo)
        {
            UInt64 MsGUID = info.masterguid;
            UInt64 sGUID = info.guid;
            string nickName = info.nickname;
            int level = info.level;
            int heroKills = info.kills;
            int deadTimes = info.deadtimes;
            Int32 campID = info.campid;
            Int32 headID = info.headid;
            Int32 hp = info.hp;
            Int32 mp = info.mp;
            EntityCampTypeEnum Type = GameMethod.GetEntityCamp(campID);
            BattleingData.Instance.AddInitPlayer(sGUID, nickName, heroKills, deadTimes, info.assist, level, 0, Type, info.heroid);
            foreach (GSToGC.BroadcastBattleHeroInfo.BaseGoodsInfo goods in pMsg.goodsinfo)
            {
                BattleingData.Instance.AddPlayer(sGUID, 0, BattleDataType.Goods, goods.index, goods.id);
            }
            IPlayer player;
            if (!PlayerManager.Instance.AccountDic.TryGetValue(MsGUID, out player))
            {
                player = (IPlayer)PlayerManager.Instance.HandleCreateEntity(MsGUID, EntityCampTypeEnum.Null);
                player.BattleData.Level = (uint)level;
                player.BattleData.HeadId = (uint)headID;
                player.BattleData.Hp = (uint)hp;
                player.BattleData.Mp = (uint)mp;
            }
            if (UIGameBattleInfo.Instance != null && PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
            {
                UIGameBattleInfo.Instance.SetInfoInit(sGUID, nickName, level, heroKills, deadTimes, Type);
            }
            else
            {
                if (UIViewerBattleInfo.Instance != null)
                {
                    UIViewerBattleInfo.Instance.SetInfoInit(sGUID, nickName, level, heroKills, deadTimes, Type, campID);
                }

            }
            PlayerDataManager.Instance.SetPersonInfo(player, campID);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PersonInitInfo, player);//广播初始化任务基本信息
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SkillInfo, player);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerCp);//广播设置金币
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyChangeDeaths);//广播设置死亡数据
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyChangeKills);//广播设置击杀数据
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroInfoChange, player);
        }
        if (UIViewerPersonInfo.Instance != null)
            UIViewerPersonInfo.Instance.SetInitCamearHead();
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换AbsorbBegin
    Int32 OnNetMsg_NotifyAbsorbBegin(Stream stream)
    {
        GSToGC.AbsorbBegin pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        UInt64 sceneGuid;
        sceneGuid = pMsg.guid;
        UInt64 targetGuid;
        targetGuid = pMsg.monsterguid;
        IEntity entity;
        IEntity target;
        EntityManager.AllEntitys.TryGetValue(sceneGuid, out entity);
        EntityManager.AllEntitys.TryGetValue(targetGuid, out target);
        IPlayer player = entity as IPlayer;
        if (player != null && target != null)
        {
            //创建吸附特效
            AbsorbEffect.createAbsorbEffect(player, target.RealEntity.objAttackPoint.gameObject, player.RealEntity.objAttackPoint.gameObject);
            //吸附音频设置
            if (player.mAbsorbSound != null)
            {
                player.mAbsorbSound.Stop();
                player.mAbsorbSound = null;
            }
            string soundPath = "";
            soundPath = GameConstDefine.LoadGameSoundPath + "Txifu";
            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(soundPath, ResourceType.ASSET);
            if (objUnit.Asset != null)
            {
                AudioClip clip = objUnit.Asset as AudioClip;
                if (clip != null)
                {
                    AudioSource Audio = AudioManager.Instance.PlayLongVoiceAudio(clip);
                    SceneSoundManager.Instance.addSound(Audio, player.RealEntity.gameObject);
                    player.mAbsorbSound = Audio;
                    AudioManager.Instance.ChangeAudioVolume(Audio, 1.0f);
                }
            }
            //如果是本地玩家，设置进度条
            if (player == PlayerManager.Instance.LocalPlayer)
            {
                ProgressBarInterface.startProgressBar(ProgressBarInterface.BarType.BarAbsorb);
                PlayerManager.Instance.LocalPlayer.IsAbsobing = true;
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    enum EAbsorbResult
    {
        eAbsorbResult_Failed,
        eAbsorbResult_Success
    }

    //已换AbsorbRes
    //eMsgToGCFromGS_NotifyAbsorbMonsterResult
    Int32 OnNetMsg_NotifyAbsorbMonsterResult(Stream stream)
    {
        GSToGC.AbsorbRes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sceneGuid;
        sceneGuid = pMsg.guid;
        UInt32 newMonsterID1 = (UInt32)pMsg.absorb1; //吸收的野怪1
        UInt32 newMonsterID2 = (UInt32)pMsg.absorb2; //吸收的野怪2
        IEntity abosorbPlayer; //主动吸收的玩家
        EntityManager.AllEntitys.TryGetValue(sceneGuid, out abosorbPlayer);
        if (null == abosorbPlayer) return 0;
        //停止播放声音
        if (null != abosorbPlayer.mAbsorbSound)
        {
            abosorbPlayer.mAbsorbSound.Stop();
            SceneSoundManager.Instance.remove(abosorbPlayer.mAbsorbSound);
            abosorbPlayer.mAbsorbSound = null;
        }

        //设置玩家吸收技能信息
        PlayerSkillData.Instance.SetPlayerAbsSkillInfo((IPlayer)abosorbPlayer, 4, (int)newMonsterID1, 5, (int)newMonsterID2);

        ISelfPlayer curPlayer = PlayerManager.Instance.LocalPlayer;//是不是本地玩家

        if (null == curPlayer)//本地玩家不为空
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AbsSkillInfo, (IPlayer)abosorbPlayer);//广播添加英雄吸附技能
            return 0;
        }
        if (curPlayer.GameObjGUID == sceneGuid)//如果主动吸收的玩家是本地玩家
        {
            curPlayer.IsAbsobing = false;
            if (pMsg.res)
            {
                UInt32 un32OldMonsterID1 = (UInt32)curPlayer.AbsorbMonsterType[0];
                if (un32OldMonsterID1 != newMonsterID1)
                {
                    curPlayer.SetAbsorbMonster((int)newMonsterID1, 0); //设置吸收野怪灵魂，第一个按钮
                    EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, (int)newMonsterID1);//广播引导完成
                }
                UInt32 un32OldMonsterID2 = (UInt32)curPlayer.AbsorbMonsterType[1];
                if (un32OldMonsterID2 != newMonsterID2)
                {
                    curPlayer.SetAbsorbMonster((int)newMonsterID2, 1); //设置吸收野怪灵魂，第二个按钮
                    EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_GuideAbsorbTask, (int)newMonsterID2);//广播引导完成
                }
                //广播设置更新兵营头像
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_ResetAbsHead, (int)newMonsterID1, (int)newMonsterID2);
            }
            //销毁进度条
            if (ProgressBarInterface.barType == ProgressBarInterface.BarType.BarAbsorb)
            {
                ProgressBarInterface.hideProgressBar();
            }
            //销毁进度特效
            if (curPlayer.AbsorbProgressEffect != null)
            {
                DestroyImmediate(curPlayer.AbsorbProgressEffect);
                curPlayer.AbsorbProgressEffect = null;
            }
        }
        else //其他玩家吸附结果   
        {
            IPlayer otherplayer = abosorbPlayer as IPlayer;
            if (otherplayer != null)
            {
                if (otherplayer.AbsorbProgressEffect != null)
                {
                    DestroyImmediate(otherplayer.AbsorbProgressEffect);//销毁吸收特效
                    otherplayer.AbsorbProgressEffect = null;
                }
                if (pMsg.res)
                {
                    bool isRemove = (newMonsterID1 == 0) ? true : false;
                    otherplayer.SetAbsorbEffect(0, isRemove);//创建灵魂特效

                    isRemove = (newMonsterID2 == 0) ? true : false;
                    otherplayer.SetAbsorbEffect(1, isRemove);//创建灵魂特效
                }
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyBuyRebornSuccess(Stream stream)
    {
        GSToGC.RebornSuccess pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.guid;
        DeathCtrl.Instance.Exit();
        IEntity entity = null;
        EntityManager.AllEntitys.TryGetValue(sguid, out entity);
        PlayerDataManager.Instance.SetDeathTime((IPlayer)entity, 0);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroDeathTime, (IPlayer)entity);
        return (Int32)ErrorCodeEnum.Normal;
    }

    /// <summary>
    /// 复活事件
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换
    Int32 OnNetMsg_NotifyHeroRebornTimes(Stream stream)
    {
        GSToGC.RebornTimes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.masterguid;
        UInt64 BeforTime = (ulong)pMsg.sendtimes;   //ToReview sendTimes是否就是Beforetime，强制类型转换long->ulong             
        Int32 RebornTime = pMsg.reborn_time;
        IEntity entity = null;
        if (EntityManager.AllEntitys.TryGetValue(sguid, out entity))
        {

        }
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        if (player != null)
        {
            DeathCtrl.Instance.SetTime((float)GameUtils.GetClientUTCMillisec() - BeforTime, (float)pMsg.remain_times / 1000f, pMsg.gold, RebornTime, true);//Num是否就是剩余次数remain_times    
            DeathCtrl.Instance.Enter();
        }
        PlayerDataManager.Instance.SetDeathTime((IPlayer)entity, (int)(pMsg.remain_times / 1000f));
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroDeathTime, (IPlayer)entity);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //击杀数
    //已换HeroKills
    //eMsgToGCFromGS_NotifyHeroKills
    Int32 OnNetMsg_NotifyHeroKills(Stream stream)
    {
        GSToGC.HeroKills pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.guid;

        BattleingData.Instance.AddPlayer(sguid, pMsg.kills, BattleDataType.Kills);
        if (UIGameBattleInfo.Instance != null)
        {
            UIGameBattleInfo.Instance.SetKillInfo(sguid, pMsg.kills);
        }
        if (UIViewerBattleInfo.Instance != null)
        {
            UIViewerBattleInfo.Instance.SetKillInfo(sguid, pMsg.kills);
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyChangeKills);
      
        return (Int32)ErrorCodeEnum.Normal;
    }

    //死亡数 
    //已换CurDeadTimes
    //eMsgToGCFromGS_NotifyCurDeadTimes
    Int32 OnNetMsg_NotifyCurDeadTimes(Stream stream)
    {
        GSToGC.CurDeadTimes pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        if (GameStateManager.Instance.GetCurState().GetStateType() != GameStateTypeEnum.Play)
            return (Int32)ErrorCodeEnum.Normal;

        UInt64 targetID;
        targetID = pMsg.objguid;
        UInt64 killID;
        killID = pMsg.reasonheroguid;

        BattleingData.Instance.AddPlayer(targetID, pMsg.deadtimes, BattleDataType.Deaths);
        MsgInfoManager.Instance.SetKillDeathPlayer((byte)pMsg.herostate, pMsg.killer_camp, targetID, killID, pMsg.ifAced, (EHeroKillTitle)pMsg.herotitle);       //ToReview int->byte
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotifyChangeDeaths);//广播设置死亡数 本地玩家
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AllPlayerDeaths);//广播设置死亡数   游戏战绩
  
        return (Int32)ErrorCodeEnum.Normal;
    }

    //设置助攻数据
    Int32 OnNetMsg_NotifyHeroAssist(Stream stream)
    {
        GSToGC.HeroAssist pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.guid;
        BattleingData.Instance.AddPlayer(sguid, pMsg.assist, BattleDataType.Assist);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerAssist);
        return (Int32)ErrorCodeEnum.Normal;

    }

    
    //已换CurCP   当前金币
    //eMsgToGCFromGS_NotifyCurCP
    Int32 OnNetMsg_NotifyCurCP(Stream stream)
    {
        GSToGC.CurCP pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 targetID;
        targetID = pMsg.targetguid;
        if (PlayerManager.Instance.LocalPlayer != null && PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
        {
            Thanos.FEvent eve = new Thanos.FEvent((Int32)(Int32)GameEventEnum.GameEvent_NotifyChangeCp);
            eve.AddParam("Cp", pMsg.person_cp / 1000);
            eve.AddParam("TeamCp", pMsg.team_cp / 1000);
            EventCenter.SendEvent(eve);

            PlayerManager.Instance.LocalPlayer.SetCp(pMsg.person_cp / 1000);//这里设置的金币并非在游戏界面显示的金币，而是在商店
            PlayerManager.Instance.LocalPlayer.SetTeamCp(pMsg.team_cp / 1000);
            
        }
        else
        {
            if (UIViewerBattleInfo.Instance != null)
            {
                UIViewerBattleInfo.Instance.SetCpInfo(targetID, pMsg.person_cp / 1000);
            }
        }
        BattleingData.Instance.AddPlayer(targetID, pMsg.person_cp / 1000, BattleDataType.Cp);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerCp);//广播设置金币
        return (Int32)ErrorCodeEnum.Normal;
    }

    //通知金币改变
    Int32 OnNetMsg_NotifyCPChange(Stream stream)
    {
        GSToGC.PersonCPChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 targetID;
        targetID = pMsg.guid;
        if (CPChanggeManager.Instance != null && PlayerManager.Instance.LocalPlayer!=null)
        {
            CPChanggeManager.Instance.CreateCPAdd(pMsg.cp / 1000);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayerGetCp);
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerCp);//广播设置金币
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMst_BroadBuildingDestroyByWho(Stream stream)
    {
        GSToGC.BroadcastBuildingDestory pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        if (GameStateManager.Instance.GetCurState().GetStateType() != GameStateTypeEnum.Play)
            return (Int32)ErrorCodeEnum.Normal;

        UInt64 targetID = pMsg.buildingguid;
        if (!EntityManager.AllEntitys.ContainsKey(targetID))
        {
            Debug.LogError("null entity!");
            return 0;
        }

        IEntity targetEntity = EntityManager.AllEntitys[targetID];

        UInt64 ps_kill = pMsg.killer_guid;
        if (!EntityManager.AllEntitys.ContainsKey(ps_kill))
        {
            Debug.LogError("null entity!");
            return 0;
        }

        IEntity psKillEntity = EntityManager.AllEntitys[ps_kill];

        Int32 killID = pMsg.killer_camp;
        EntityCampTypeEnum Type = (EntityCampTypeEnum)killID;
        string npcname = "";
        string readXml = "";
        string killname = "";
        if (killID > 0)
        {
            if (killID % 2 == 0)
            {
                Type = EntityCampTypeEnum.B;
            }
            else
            {
                Type = EntityCampTypeEnum.A;
            }
        }
        IPlayer player = PlayerManager.Instance.LocalPlayer;
        killname = MsgInfoManager.Instance.GetNameGame(ps_kill);
        if (killname == null)
        {
            NpcConfigInfo info;
            ConfigReader.NpcXmlInfoDict.TryGetValue(killID, out info);
            if (info != null)
            {
                killname = info.NpcName;
            }
            else
            {
                NpcConfigInfo npcinfo = ConfigReader.GetNpcInfo((int)psKillEntity.ObjTypeID);
                if (null != npcinfo && killname == null)
                {
                    if (Type == EntityCampTypeEnum.A)
                        killname = "精灵势力";
                    else
                        killname = "亡灵势力";
                }
            }
        }
        if (targetEntity.IfNPC())
        {
            int id = (int)targetEntity.ObjTypeID;
            NpcConfigInfo npcinfo = ConfigReader.GetNpcInfo(id);
            if (null == npcinfo)
            {
                return (Int32)ErrorCodeEnum.NullPointer;
            }
            if (npcinfo.ENPCCateChild == (int)NPCCateChildEnum.BUILD_Tower)
                npcname = "箭塔";
            else if (npcinfo.ENPCCateChild == (int)NPCCateChildEnum.BUILD_Altar)
                npcname = "祭坛";
            else if (npcinfo.ENPCCateChild == (int)NPCCateChildEnum.BUILD_Base)
                npcname = "主基地";
            if (player != null && Type == player.EntityCamp)
            {
                readXml = ConfigReader.GetMsgInfo(10005).content;
            }
            else
            {
                readXml = ConfigReader.GetMsgInfo(10006).content;
            }
        }

        if (killname != "")
        {
            MsgInfoManager.Instance.SetKills(MsgInfoManager.eKillMsgType.eKillBuild, false, killname, npcname, readXml);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMst_NotifyUserGameInfo(Stream stream)
    {
        GSToGC.UserGameInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //senlin
        //UILobby.Instance.SetGameLobbyBtn(OptionLobby.PERSONALINFO);
        PresonInfoCtrl.Instance.SetCurrentDate((int)pMsg.headid, pMsg.nickname, pMsg.level, pMsg.upgradeexp, pMsg.curexp, pMsg.totalgameinns, pMsg.totalwintimes, pMsg.herokills,
                pMsg.destorybuildings, pMsg.deadtimes, pMsg.total_achnum, pMsg.achnum, pMsg.assistnum, pMsg.vipscore, (ulong)pMsg.exp_adtime, (ulong)pMsg.gold_addtime);
        PresonInfoCtrl.Instance.Enter();
        //EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PersonInitInfo);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //NotifyBuffRemove已取消
    Int32 OnNetMsg_NotifyBuffRemove(Message pcMsg)
    {
        UInt64 targetID = pcMsg.GetGUID();
        UInt32 buffID = pcMsg.GetUInt32();
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(targetID, out entity))
        {
            Thanos.Effect.EffectManager.Instance.DestroyEffect(buffID);
            if (entity.GameObjGUID == PlayerManager.Instance.LocalPlayer.GameObjGUID)
            {
                Thanos.Skill.BuffManager.Instance.RemoveBuff(buffID);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }


    //已换NotifySkillInfo
    //eMsgToGCFromGS_NotifySkillInfo
    Int32 OnNetMsg_NotifySkillInfo(Stream stream)
    {
        GSToGC.NotifySkillInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 targetID;
        targetID = pMsg.guid;
        float timeInSecond = pMsg.time / 1000.0f;   //状态持续的时间毫秒数
        float timeMax = pMsg.cooldown / 1000.0f;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(targetID, out entity))
        {
            if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
            {
                entity.OnSkillInfoChange(pMsg.skillid, timeInSecond, timeMax, pMsg.skillslot); //0,空闲 1，准备 2施放 3冷却
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换GOSkillCD
    //eMsgToGCFromGS_NotifyGameObjectSkillCD
    Int32 OnNetMsg_NotifyGameObjectSkillCD(Stream stream)
    {
        GSToGC.GOSkillCD pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sguid;
        sguid = pMsg.guid;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sguid, out entity))
        {
            PlayerSkillData.Instance.SetPlayerCdTime((IPlayer)entity, pMsg.skillid, pMsg.time / 1000.0f);
        }
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SkillCDInfo, (IPlayer)entity);
        return (Int32)ErrorCodeEnum.Normal;
    }
    /// <summary>
    /// Notify 物品信息
    /// </summary>
    /// <param name="pcMsg"></param>
    /// <returns></returns>
    //已换NotifyGoodsInfo
    //eMsgToGCFromGS_NotifyGoodsInf
    Int32 OnNetMsg_NotifyGoodsInfo(Stream stream)
    {
        HolyTechGameBase.Instance.IsInitialize = false;

        GSToGC.NotifyGoodsInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 targetID;
        targetID = pMsg.guid;
        foreach (GSToGC.NotifyGoodsInfo.GoodsInfo info in pMsg.info)
        {
            int n32RemainCDTime = info.statetime;
            if (n32RemainCDTime < 0)
            {
                n32RemainCDTime = 0;
            }
            float totTime = 0.0f;
            if (true == ConfigReader.ItemXmlInfoDict.ContainsKey(info.tyepid))
            {
                totTime = ConfigReader.ItemXmlInfoDict[info.tyepid].un32CdTime;
            }
            float second = (float)n32RemainCDTime / 1000.0f;
            if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
            {
                PlayerManager.Instance.LocalPlayer.AddUserGameItems(info.pos, info.tyepid, info.num, second);
            }
            IEntity entity;
            if (EntityManager.AllEntitys.TryGetValue(targetID, out entity))
            {
                PackageDataManager.Instance.SetPackageData((IPlayer)entity, info.pos, info.tyepid, info.num, totTime, second);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PackageBuyInfo, (IPlayer)entity);
            }
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateUserGameItems);
            if (info.ifComposed)
            {
                ResourceItem unit = ResourcesManager.Instance.loadImmediate(AudioDefine.ITEMCOMBINESOUNDEFFECT, ResourceType.ASSET);
                AudioClip clip = unit.Asset as AudioClip;
                AudioSource source = AudioManager.Instance.PlayEffectAudio(clip);
            }
            BattleingData.Instance.AddPlayer(targetID, info.num, BattleDataType.Goods, info.pos, info.tyepid);
            //EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AllPlayerGoods);
            /*
              //talkingdata
              #region  talkingdata
              CEvent eve = new CEvent((Int32)(Int32)GameEventEnum.GameEvent_TalkgameAction);
              eve.AddParam("type", EActionType.eA_Battle_Hero_BuyGoods);
              eve.AddParam("p1", info.tyepid);
              eve.AddParam("p2", info.num);
              eve.AddParam("p3", System.DateTime.Now.ToString("yyyyMMddHHmmss"));
              EventCenter.SendEvent(eve);
              #endregion  
              */
        }
      
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyGameObjectDeadState(Stream stream)
    {
        GSToGC.DeadState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        UInt64 deadID;
        deadID = pMsg.objguid;
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);//位置
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);//方向
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(deadID, out entity))
        {
            pos.y = entity.realObject.transform.position.y;//
            entity.deadSpot = pMsg.spot;    //死亡现场(用于是否播放死亡特效)
            entity.EntityFSMChangeDataOnDead(pos, dir);

            //主基地延时处理
            if (entity.NPCCateChild != NPCCateChildEnum.BUILD_Base)
            {
                entity.OnFSMStateChange(EntityDeadFSM.Instance);//切换到死亡状态
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换SkillEntityInfo
    //eMsgToGCFromGS_NotifySkillUnitInfo->eMsgToGCFromGS_NotifySkillEntityInfo
    Int32 OnNetMsg_NotifySkillUnitInfo(Stream stream)
    {
        //ToReview 函数体是空的?
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换SkillHitTar
    //eMsgToGCFromGS_NotifySkillHitTarget
    Int32 OnNetMsg_NotifySkillHitTarget(Stream Stream)
    {
        //ToReview 函数体是空的？
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换HPChange
    //eMsgToGCFromGS_NotifyHPChange
    Int32 OnNetMsg_NotifyHPChange(Stream stream)
    {
        GSToGC.HPChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        int crticalHp = pMsg.hp;//当前血量

        IEntity entity;
        Vector3 posInWorld = Vector3.zero;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            posInWorld = entity.RealEntity.objAttackPoint.transform.position + new Vector3(0.0f, 2.0f, 0.0f);

            if (pMsg.reason != GSToGC.HPMPChangeReason.SkillHurt)//如果不是被技能伤害的
            {
                entity.SetJungleMonsterBeAtkVoice();//设置野怪攻击音频
            }
            //先计算伤血数值再更新  更新实体的血条
            entity.UpdateHpChange((byte)pMsg.reason, (float)crticalHp);

            if (entity is IPlayer) 
            {
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroHpChange, (IPlayer)entity);//涉及到UIViewerPersonInfo
            }
            GamePlayCtrl.Instance.HpChange(entity);//更新头像血条
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyMPChange(Stream stream)
    {
        GSToGC.MpChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 id;
        id = pMsg.guid;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(id, out entity))
        {
            if (pMsg.reason != 0)           //ToReview enum->byte
            {
                //以下两行以前就是被注释掉的
                //HolyTech.Effect.EffectManager.Instance.CreateLabelEffect("" + (entity.Mp - CurMP), HolyTech.Effect.LabelJumpEffect.LabelJumpType.LJT_MP, posInWorld);
                //HolyTech.Effect.EffectManager.Instance.CreateLabelEffect("" + (CurMP - entity.Mp), HolyTech.Effect.LabelJumpEffect.LabelJumpType.LJT_MP, posInWorld);
            }
            entity.SetMp((float)pMsg.mp);//设置Mp值

            if (entity is IPlayer)
            {
                BloodBarPlayer BloodBarPlayer = (BloodBarPlayer)entity.BloodBar;
                BloodBarPlayer.UpdateMp();//更新蓝条 指的是模型头顶上的
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroMpChange, (IPlayer)entity);// 此事件更新的是叫Player+i对象的MP？？？
            }
            GamePlayCtrl.Instance.MpChange(entity); //更新实体Mp(被锁定的的头像)
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyPrepareSkill(Stream stream)
    {
        GSToGC.PrepareSkillState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos); //工具函数GSToGC.Pos转Vector3（会将厘米转成米）
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir); //工具函数GSToGC.Dir转Vector3
        dir.y = 0.0f;

        UInt64 sGUID = pMsg.objguid;       
        UInt64 targetID = pMsg.targuid;

        IEntity targetEntity;
        EntityManager.AllEntitys.TryGetValue(targetID, out targetEntity);
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            pos.y = entity.realObject.transform.position.y;
            entity.EntityFSMChangeDataOnPrepareSkill(pos, dir, pMsg.skillid, targetEntity);
            entity.OnFSMStateChange(EntitySingFSM.Instance);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyReleaseSkill(Stream stream)
    {
        GSToGC.ReleasingSkillState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);
        dir.y = 0.0f;

        UInt64 targetID = pMsg.targuid;//目标id；
        UInt64 sGUID = pMsg.objguid;//主动方id


        IEntity target;
        EntityManager.AllEntitys.TryGetValue(targetID, out target);//获取目标对象
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))//获取主动对象
        {
            pos.y = entity.realObject.transform.position.y;
            entity.EntityFSMChangeDataOnPrepareSkill(pos, dir, pMsg.skillid, target); //实体状态机改变（当准备技能时）
            entity.OnFSMStateChange(EntityReleaseSkillFSM.Instance);//进入释放技能状态
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyLastingSkill(Stream stream)
    {
        GSToGC.LastingSkillState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);
        dir.y = 0.0f;
        UInt64 targetID;
        targetID = pMsg.targuid;
        UInt64 sGUID;
        sGUID = pMsg.objguid;
        IEntity target;
        EntityManager.AllEntitys.TryGetValue(targetID, out target);
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            pos.y = entity.realObject.transform.position.y;
            entity.EntityFSMChangeDataOnPrepareSkill(pos, dir, pMsg.skillid, target);
            entity.OnFSMStateChange(EntityLastingFSM.Instance);//进入EntityLastingFSM
        }
        return (Int32)ErrorCodeEnum.Normal;
    }


    Int32 OnNetMsg_NotifyLeadingSkill(Stream stream)
    {
        GSToGC.UsingSkillState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Vector3 pos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 dir = this.ConvertDirToVector3(pMsg.dir);
        dir.y = 0.0f;
        UInt64 targetID;
        targetID = pMsg.targuid;
        UInt64 sGUID;
        sGUID = pMsg.objguid;
        IEntity target;
        EntityManager.AllEntitys.TryGetValue(targetID, out target);
        IEntity entity;

        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            pos.y = entity.realObject.transform.position.y;//????啥意思为什么反过来？
            entity.EntityFSMChangeDataOnPrepareSkill(pos, dir, pMsg.skillid, target);
            entity.OnFSMStateChange(EntityLeadingFSM.Instance);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }
    #endregion




    #region Entity State Change
    //已换BattleLoadingState
    //eMsgToGCFromGS_NotifyBattleLoadingState
    //ToReview：名称不对应，且函数体本身就被注释掉了
    Int32 OnNetMsg_NotifyBattleStart(Stream stream)
    {

        return (Int32)ErrorCodeEnum.Normal;
    }
    //通知客户端销毁游戏对象
    //已换DisappearInfo
    //已换OnNetMsg_NotifyGameObjectDisAppear
    Int32 OnNetMsg_NotifyGameObjectDisAppear(Stream stream)
    {
        GSToGC.DisappearInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (ulong guid in pMsg.guid)
        {
            UInt64 sGUID = guid;
            if (!EntityManager.AllEntitys.ContainsKey(sGUID))
            {
                continue;
            }

            IEntity sEntity = EntityManager.AllEntitys[sGUID];
            EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameEvent_RemoveMiniMap, sGUID);
            if (EntityManager.AllEntitys.ContainsKey(sGUID) && GameUtils.IfTypeHero((ObjectTypeEnum)sEntity.ObjTypeID))
            {
                PlayerManager.Instance.HideEntity(sGUID);
                continue;
            }

            EntityManager.HandleDelectEntity(sGUID);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //通知客户端显示游戏对象
    //eMsgToGCFromGS_NotifyGameObjectAppear  
    Int32 OnNetMsg_NotifyGameObjectAppear(System.IO.Stream stream)
    {
        OnNetMsg_NotifyGameObjectAppearCoroutine(stream);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //加载游戏对象
    public void OnNetMsg_NotifyGameObjectAppearCoroutine(System.IO.Stream stream)
    {
        //如果现在的状态是游戏状态
        if (GameStateManager.Instance.GetCurState().GetStateType() == GameStateTypeEnum.Play)
        {
            //反序列化 
            GSToGC.GOAppear pMsg;
            if (!ProtobufMsg.MessageDecode(out pMsg, stream))
            {
                return;
            }

            foreach (GSToGC.GOAppear.AppearInfo info in pMsg.info)
            {
                //获取实体信息
                UInt64 sMasterGUID = info.masterguid;
                UInt64 sObjGUID = info.objguid;

                if (sObjGUID < 1)
                {
                    Debug.LogError("objguid:" + sObjGUID);
                }

                Int32 IntCamp = info.camp;//实体阵营 NPC可以确定  但是并不能确定对战的阵营
                Vector3 mvPos = this.ConvertPosToVector3(info.pos);
                Vector3 mvDir = this.ConvertDirToVector3(info.dir);
                mvDir.y = 0.0f;
                // 如果是对战的阵营  确定到底是A还是B 如果都不是。这句话仅仅是类型转换的作用 
                EntityCampTypeEnum Type = GameMethod.GetEntityCamp(IntCamp);//大于0的情况 不是A就是B  

                //实体类型  
                GSToGC.ObjType objType = info.obj_type;
               
                
                //先判断是否已经创建了此实体，如果已经创建，则直接显示，否则就跳过去创建
                if (EntityManager.AllEntitys.ContainsKey(sObjGUID))
                {
                    if (objType == GSToGC.ObjType.ObjType_Hero)
                    {
                        PlayerManager.Instance.ShowEntity(sObjGUID, mvPos, mvDir);

                        EventCenter.Broadcast<IEntity>((Int32)GameEventEnum.GameEvent_AddMiniMap, EntityManager.AllEntitys[sObjGUID]);
                    }
                    continue;
                }
                //创建实体
                IEntity entity = null;
                if (GameUtils.IfTypeNPC((ObjectTypeEnum)info.obj_type_id))
                {
                    entity = NpcManager.Instance.HandleCreateEntity(sObjGUID, Type); //创建实体类
                    entity.EntityCamp = Type;
                    entity.ObjTypeID = info.obj_type_id; 
                    NpcManager.Instance.CreateEntityModel(entity, sObjGUID, mvDir, mvPos);//创建实体模型
                }
                else if (GameUtils.IfTypeHero((ObjectTypeEnum)info.obj_type_id))
                {
                    IPlayer player = null;
                    if (!PlayerManager.Instance.AccountDic.TryGetValue(sMasterGUID, out player))
                    {
                        player = (IPlayer)PlayerManager.Instance.HandleCreateEntity(sMasterGUID, Type);//创建玩家类
                        player.ObjTypeID = info.obj_type_id;
                        PlayerManager.Instance.AddAccount(sMasterGUID, player); //将Player添加到AccountDic中
                    }
                    player.EntityCamp = Type;
                    entity = player;//玩家组件
                    entity.ObjTypeID = info.obj_type_id;
                  
                    PlayerManager.Instance.CreateEntityModel(entity, sObjGUID, mvDir, mvPos); //创建玩家模型 ，并给模型添加组件。
                   
                    
                    GameMethod.CreateCharacterController(player);//创建角色控制器
                    ReadPreLoadConfig.Instance.AddPreLoadRoleEffect((int)entity.ObjTypeID);
                }
                //创建实体后的实体初始化
                if (entity != null)
                {
                    entity.GameObjGUID = sObjGUID;
                    EntityManager.Instance.AddEntity(sObjGUID, entity);
                    if (GameUserModel.Instance.IsLocalPlayer(sMasterGUID))
                    {
                        Debug.Log("Set local player guid!" + sMasterGUID);
                        PlayerManager.Instance.LocalPlayer = (ISelfPlayer)entity;//给本地玩家赋值
                        GameMethod.GetMainCamera.target = entity.realObject.transform;//摄像机指定对象
                        GamePlayCtrl.Instance.UpdateSkillPriv((int)info.obj_type_id);

                        CPChanggeManager.Instance.ClearList();//金币初始化
                        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_InitMiniMap); //初始化地图
                    }

                    entity.InitWhenCreateModel();//设置攻击速度
                    entity.OnCreateShadow();//创建脚下阴影并初始化
                    entity.GOSSI.sServerBeginPos = mvPos;//位置初始化
                    entity.GOSSI.sServerSyncPos = mvPos;

                    entity.EntityFSMChangedata(mvPos, mvDir);//实体状态机数据改变（位置和方向）
                    entity.OnFSMStateChange(EntityFreeFSM.Instance);//实体状态进入自由状态 Enter（）调用。

                    EventCenter.Broadcast<IEntity>((Int32)GameEventEnum.GameEvent_AddMiniMap, entity);//添加地图元素  
                }
            }
        }

    }

    Int32 OnNetMsg_NotifyGameObjectFreeState(Stream stream)
    {
        GSToGC.FreeState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (null == pMsg.dir || null == pMsg.pos)
            return 0;
        UInt64 sGUID;
        sGUID = pMsg.objguid;
        Vector3 mvPos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 mvDir = this.ConvertDirToVector3(pMsg.dir);
        IEntity entity = null;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {

            Vector3 sLastSyncPos = entity.GOSSI.sServerSyncPos;
            mvPos.y = entity.RealEntity.transform.position.y;
            entity.GOSSI.sServerBeginPos = mvPos;
            entity.GOSSI.sServerSyncPos = mvPos;
            entity.GOSSI.sServerDir = mvDir;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;
            entity.EntityFSMChangedata(mvPos, mvDir);

            entity.OnFSMStateChange(EntityFreeFSM.Instance);
           
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换RunningState
    //eMsgToGCFromGS_NotifyGameObjectRunState
    Int32 OnNetMsg_NotifyGameObjectRunState(Stream stream)
    {
        GSToGC.RunningState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (null == pMsg.dir || null == pMsg.pos)
            return 0;
        UInt64 sGUID;
        sGUID = pMsg.objguid;
        Vector3 mvPos = this.ConvertPosToVector3(pMsg.pos);
        Vector3 mvDir = this.ConvertDirToVector3(pMsg.dir);
        float mvSp = pMsg.movespeed / 100.0f;
        IEntity entity = null;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            mvPos.y = entity.RealEntity.transform.position.y;
            entity.GOSSI.sServerBeginPos = mvPos;
            entity.GOSSI.sServerSyncPos = mvPos;
            entity.GOSSI.sServerDir = mvDir;
            entity.GOSSI.fServerSpeed = mvSp;
            entity.GOSSI.fBeginTime = Time.realtimeSinceStartup;
            entity.GOSSI.fLastSyncSecond = Time.realtimeSinceStartup;

            entity.EntityFSMChangedata(mvPos, mvDir, mvSp);
            entity.OnFSMStateChange(EntityRunFSM.Instance);//进入Run状态
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    #endregion
    Int32 OnNetMsg_NotifyBattleStateChange(System.IO.Stream stream)
    {
        GSToGC.BattleStateChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (GameUserModel.Instance.IsReconnect)
        {
            GameUserModel.Instance.IsReconnect = false;

            GameStateTypeEnum curState = GameStateManager.Instance.GetCurState().GetStateType();
            switch ((BattleStateEnum)pMsg.state)
            {
                case BattleStateEnum.eBS_SelectHero://选择英雄
                    //确定英雄选择状态    InitSelect   EnterSelect   RandomSelect   OutSeclect
                    HeroCtrl.Instance.SetSelectState(HeroCtrl.HeroSelectState.EnterSelect);

                    EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.IntoHero));  //进入GC_Hero状态，Enter()方法立即调用
                    
                    EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroFirstTime, pMsg.statetimeleft);

                    break;
                case BattleStateEnum.eBS_SelectRune://选择符文

                    HeroCtrl.Instance.SetSelectState(HeroCtrl.HeroSelectState.EnterSelect); //清除临时选择的英雄列表

                    EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.IntoHero)); // 进入GC_Hero状态，Enter()方法立即调用

                    EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroSecondTime, pMsg.statetimeleft);

                    HeroCtrl.Instance.SetSelectState(HeroCtrl.HeroSelectState.RandomSelect);//广播进入随机选择消息，进入选择皮肤阶段 倒计时10秒开始

                    break;
                case BattleStateEnum.eBS_Loading://加载
                    if (curState < GameStateTypeEnum.Loading)
                    {
                        Thanos.FEvent evt = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
                        evt.AddParam("NextState", GameStateTypeEnum.Play);
                        EventCenter.SendEvent(evt);
                    }
                    else if (curState > GameStateTypeEnum.Loading)
                    {
                        HolyGameLogic.Instance.AskLoadComplete();//向服务端发送消息
                    }

                    break;
                case BattleStateEnum.eBS_Playing://游戏
                    if (curState < GameStateTypeEnum.Loading)
                    {
                        Thanos.FEvent evt2 = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
                        evt2.AddParam("NextState", GameStateTypeEnum.Play);
                        EventCenter.SendEvent(evt2);
                        Debug.LogWarning("battle play ");
                    }
                    else
                    {
                        HolyGameLogic.Instance.AskLoadComplete();
                        Debug.LogWarning("battle play but send loadcomplete!");
                    }
                    break;
                case BattleStateEnum.eBS_Finished://游戏结束
                    {
                        EventCenter.Broadcast((Int32)GameEventEnum.BatttleFinished);
                    }
                    break;
            }
        }
        else
        {
            GameStateTypeEnum curState = GameStateManager.Instance.GetCurState().GetStateType();

            switch ((BattleStateEnum)pMsg.state)
            {
                case BattleStateEnum.eBS_SelectHero://选择英雄状态
                    {         
                        if (HolyTechGameBase.Instance.IsQuickBattle)//如果是快速战斗模式  显示英雄选择界面
                        {
                            //有LobbyState进入HeroState状态，显示英雄选择界面
                            EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.IntoHero));
                            HolyTechGameBase.Instance.IsQuickBattle = false;
                        }
                        else  //否则
                        {
                            EventCenter.Broadcast((Int32)GameEventEnum.RoomEnd);//进入房间
                        }

                        EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroFirstTime, pMsg.statetimeleft);
                    }
                    break;
                case BattleStateEnum.eBS_SelectRune://选择符文状态
                    {
                        //设置第二次计时   SetSecondTime 为状态剩余时间，给mSecondCountTime赋值==10
                        EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroSecondTime, pMsg.statetimeleft);
                        //进入最后随机阶段
                        HeroCtrl.Instance.SetSelectState(HeroCtrl.HeroSelectState.RandomSelect); 
                    }
                    break;
                case BattleStateEnum.eBS_Loading://加载状态
                    {
                        //状态要转换了  进入GS_Play
                        Thanos.FEvent evt = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
                        evt.AddParam("NextState", GameStateTypeEnum.Play);
                        EventCenter.SendEvent(evt);
                    }
                    break;
                case BattleStateEnum.eBS_Playing://战斗状态
                    {
                        //判断此时的状态是否是play。如果不是，那么就要转换状态了
                            if (curState < GameStateTypeEnum.Play)
                        {
                            Thanos.FEvent evt = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
                            evt.AddParam("NextState", GameStateTypeEnum.Play);
                            EventCenter.SendEvent(evt);
                        }
                        else
                        {
                            //进入游戏
                            if (LoadScene.Instance != null)
                            {
                                //进入游戏前需要强制清除界面贴图资源
                                LoadScene.Instance.ReleaseResource();
                                LoadScene.Instance.CloseLoading();
                            }
                        }
                    }
                    break;
                case BattleStateEnum.eBS_Finished://完成状态
                    {
                        if (curState < GameStateTypeEnum.Play)
                        {
                            if (LoadScene.Instance != null)
                            {
                                LoadScene.Instance.CloseLoading();
                            }
                            HolyGameLogic.Instance.EmsgToss_AskReEnterRoom();
                        }
                    }
                    break;
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换CurBattleChange（未复查） 
    //eMsgToGCFromGS_NotifyCurBattleChange
    Int32 OnNetMsg_NotifyCurBattleChange(System.IO.Stream stream)
    {
        GSToGC.CurBattleChange pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream)) 
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (pMsg.battleid != 0)
        {
            GameUserModel.Instance.GameBattleID = pMsg.battleid;
        }
        return (Int32)ErrorCodeEnum.Normal;
    }
     //战斗基本信息
    Int32 OnNetMsg_NotifyBattleBaseInfo(System.IO.Stream stream)
    {
        GSToGC.BattleBaseInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        GameUserModel.Instance.GameBattleID = pMsg.battleid;
        GameUserModel.Instance.GameMapID = pMsg.mapid;
        GameUserModel.Instance.IsReconnect = pMsg.ifReconnect;

        if (pMsg.ifReconnect)
        {
            GameUserModel.Instance.GameBattleID = pMsg.battleid;
            GameUserModel.Instance.GameMapID = (UInt32)pMsg.mapid;
            EventCenter.Broadcast((Int32)GameEventEnum.ReconnectToBatttle);
        }
        else
        {
            //向服务器发送消息请求战斗
            HolyGameLogic.Instance.EmsgToss_AskEnterBattle(pMsg.battleid);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }


    Int32 OnNetMsg_NotifyRoomBaseInfo(System.IO.Stream stream)
    {
        GSToGC.RoomBaseInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        RoomCtrl.Instance.UpdateRoomBaseInfo(pMsg.roomid, pMsg.mapid);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换BattleSeatPosInfo（未复查）
    //eMsgToGCFromGS_NotifyBattleSeatPosInfo
    Int32 OnNetMsg_NotifyBattleSeatPosInfo(System.IO.Stream stream)
    {
        GSToGC.BattleSeatPosInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //此消息中包含了所有战斗的信息并存储起来
        foreach (GSToGC.BattleSeatPosInfo.PosInfo posinfo in pMsg.posinfo)
        {
            UInt64 sGUID = posinfo.guid;
            IPlayer entity = null;
            UInt64 id = sGUID;
            if (id == 0)
            {
                PlayerManager.Instance.RemoveAccountBySeat((uint)posinfo.pos);
            }
            else
            {
                if (!PlayerManager.Instance.AccountDic.TryGetValue(sGUID, out entity))
                {
                    entity = (IPlayer)PlayerManager.Instance.HandleCreateEntity(sGUID, EntityCampTypeEnum.Null);
                    PlayerManager.Instance.AddAccount(sGUID, entity);
                    if (GameUserModel.Instance.IsLocalPlayer(sGUID))
                    {
                        PlayerManager.Instance.LocalAccount = entity;
                    }
                }
            }
            if (entity != null)
            {
                //设置实体的信息（玩家而非英雄）例如：   1          false              true        正暗守卫者        
                entity.SetSeatPosInfo((uint)posinfo.pos, posinfo.ifmaster, posinfo.ifready, posinfo.nickname, posinfo.headid, (int)posinfo.gold);
            }
        }
        //此广播只有在创建房间中进入选择英雄状态中广播时才会起作用
        //由匹配模式进入英雄昂选择状态时广播没用，因为此时在在RoomState注册。

        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SeatPosUpdate); 
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换NetClash（未复查）
    //eMsgToGCFromGS_NotifyNetClash
    Int32 OnNetMsg_NotifyNetClah(Stream stEnuream)
    {
        EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_KickOut);

        NetworkManager.Instance.canReconnect = false;
        NetworkManager.Instance.Close();

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换PingRet
    //eMsgToGCFromGS_GCAskPingRet  
    Int32 OnNetMsg_NotifyPing(Stream stream)
    {
        GSToGC.PingRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Int64 n64NowTime = HolyGameLogic.Instance.GetNowTime();

        float ping = HolyGameLogic.Instance.GetDuration(n64NowTime,pMsg.time);
        if (pMsg.flag == 0)
        {
            ShowFPS.Instance.cSPing = ping;
        }
        else
        {
            // ShowFPS.Instance.sSPing = ping;///////////////////////////////////////////////////////////////////////////////
            Thanos.FEvent eve = new Thanos.FEvent((Int32)(Int32)GameEventEnum.GameEvent_SSPingInfo);
            eve.AddParam("ping", ping);
            EventCenter.SendEvent(eve);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyReturn(Stream stream)
    {
        GSToGC.AskRet pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        var askID = pMsg.askid;
        switch (askID)
        {
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_AskBuyGoods:
                Debug.LogError("购买失败!");
                break;
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_EuipRunes:
                Debug.LogError("装备符文失败！");
                break;
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_UnloadRunes:
                Debug.LogError("卸载符文失败！");
                break;
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_ComposeRunes:
                Debug.LogError("合成符文失败！");
                break;
            case (int)GCToCS.MsgNum.eMsgToGSToCSFromGC_AskRecoinRune:
                Debug.LogError("洗练符文失败！");
                break;
            //case (int)16406:
            //    Debug.LogError("自动攻击失败");
            //    break;
        }

        Int32 m_n32ErrorId = pMsg.errorcode;

        switch ((ErrorCodeEnum)m_n32ErrorId)
        {
           
            case ErrorCodeEnum.TheBattleUserFull:
            case ErrorCodeEnum.BattlePDWNotMatch:
            case ErrorCodeEnum.InvalidMapID:
            case ErrorCodeEnum.JustInBattle:
            case ErrorCodeEnum.AddBattleFailForLackOfGold:
            case ErrorCodeEnum.BattleIsPlaying:
            case ErrorCodeEnum.UserInBlackList:
            case ErrorCodeEnum.AddBattleFailForUserFull:
            case ErrorCodeEnum.AddBattleFailForAllFull:
            case ErrorCodeEnum.CounterpartFriendListFull:
            case ErrorCodeEnum.BlackListFull:
                EventCenter.Broadcast<ErrorCodeEnum>((Int32)GameEventEnum.GameEvent_AskFriendEorr, (ErrorCodeEnum)m_n32ErrorId);
                break;
            case ErrorCodeEnum.JustNotInBattle:
            case ErrorCodeEnum.YouAreNotBattleManager:
            case ErrorCodeEnum.NotAllUserReady:
            case ErrorCodeEnum.CampNotBalance:
            case ErrorCodeEnum.InvalidBattlePos:
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AskBeginBattleError, (ErrorCodeEnum)m_n32ErrorId);
                break;
            case ErrorCodeEnum.NullGateServer:
            case ErrorCodeEnum.InvalidUserName:
            case ErrorCodeEnum.InvalidUserPwd:
            case ErrorCodeEnum.UserInfoUnComplete:
                LoginCtrl.Instance.LoginError(m_n32ErrorId);
                break;
            case ErrorCodeEnum.NullUser:
                LoginCtrl.Instance.LoginError(m_n32ErrorId);
                EventCenter.Broadcast((Int32)GameEventEnum.AskAddInBattle, (ErrorCodeEnum)m_n32ErrorId);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotEnoughGold, (ErrorCodeEnum)m_n32ErrorId);
                break;

            case ErrorCodeEnum.HeroNotDead:
            case ErrorCodeEnum.NoRebornTimes:
            case ErrorCodeEnum.NotEnoughGold:
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotEnoughGold, (ErrorCodeEnum)m_n32ErrorId);
                break;
            case ErrorCodeEnum.NullBattle:
                EventCenter.Broadcast((Int32)GameEventEnum.AskAddInBattle, (ErrorCodeEnum)m_n32ErrorId);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_NotEnoughGold, (ErrorCodeEnum)m_n32ErrorId);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleUpdateRoomList);
                break;
            case ErrorCodeEnum.Normal:
                break;
            case ErrorCodeEnum.BattleClosing:
                EventCenter.Broadcast((Int32)GameEventEnum.ReConnectFail);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BeginWaiting);
                MsgInfoManager.Instance.ShowMsg(m_n32ErrorId);
                break;
            case ErrorCodeEnum.InvalidBattleID:
                PlayerManager.Instance.CleanAccount();

                Thanos.FEvent evt = new Thanos.FEvent((Int32)(Int32)GameEventEnum.Loading);
                evt.AddParam("NextState", GameStateTypeEnum.Lobby);
                EventCenter.SendEvent(evt);
                break;
            case ErrorCodeEnum.ReEnterRoomFail:
                EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.IntoLobby));
                break;
                break;            
            default:
                MsgInfoManager.Instance.ShowMsg(m_n32ErrorId);
                break;
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyUserBaseInfo(System.IO.Stream stream)
    {
        GSToGC.UserBaseInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        UInt64 sGUID = pMsg.guid;
        //如果用户有昵称
        if (pMsg.nickname.Count() > 1)
        {
            //设置游戏基本信息
            GameUserModel.Instance.SetGameBaseInfo(pMsg);
            //广播消息  进入游戏大厅
            EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.IntoLobby));
        }
        else if (sGUID > 0)
        {
            //没有昵称，进入补充玩家信息界面 此事件在LoginState中注册
            EventCenter.SendEvent(new Thanos.FEvent((Int32)(Int32)GameEventEnum.InputUserData));
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换HeroList
    //eMsgToGCFromGS_NotifyHeroList
    Int32 OnNetMsg_NotifyHeroList(Stream stream)
    {
        GSToGC.HeroList pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        
        foreach (int heroId in pMsg.heroid)
        {
            GameUserModel.Instance.CanChooseHeroList.Add(heroId);
        }

        GameUserModel.Instance.STCTimeDiff = pMsg.timeDiff;  //ss 和 client 时差
        
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyCSHeroList(Stream stream)
    {
        GSToGC.NotifyCSHeroList pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        EventCenter.Broadcast<ProtoBuf.IExtensible>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, pMsg);
        foreach (GSToGC.NotifyCSHeroList.HeroListCfg hero in pMsg.herocfg)
        {
            CommodityInfos info = new CommodityInfos();
            info.Expired_time = hero.expired_time;
            info.goodsId = (int)hero.heroid;
            info.If_free = hero.if_free;
            info.GoodsType = MarketGoodsType.GoodsTypeHero;
            if (ConfigReader.HeroBuyXmlInfoDict.ContainsKey(info.goodsId))
            {
                GameUserCtrl.Instance.DeltGetHeroInfoData(info);
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyRoomChat(Stream stream)
    {
        GSToGC.ChatInRoom pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        Int32 m_un8Seat = pMsg.pos;
        string str = pMsg.chat;

        RoomCtrl.Instance.RecvTalkMessage((uint)m_un8Seat, str);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换NotifyChooseHeroTimeEnd
    //eMsgToGCFromGS_NotifyChooseHeroTimeEnd
    Int32 OnNetMsg_NotifyToChooseHero(Stream stream)
    {
        GSToGC.TryToChooseHero pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        uint m_un8Seat = (uint)pMsg.pos;//
        int heroId = pMsg.heroid;//英雄id
        HeroCtrl.Instance.AddPreSelectHero(m_un8Seat, heroId);  
        return (Int32)ErrorCodeEnum.Normal;

    }

    //当点击确定按钮时服务端返回消息时调用
    //eMsgToGCFromGS_NotifyBattleHeroInfo
    Int32 OnNetMsg_NotifyBattleHeroInfo(Stream stream)
    {
        GSToGC.HeroInfo pMsg = ProtoBuf.Serializer.Deserialize<GSToGC.HeroInfo>(stream);
        if (null == pMsg)
        {
            Debug.LogError("TODO Replace");
            return PROTO_DESERIALIZE_ERROR;
        }
        //玩家确定英雄
        HeroCtrl.Instance.AddRealSelectHero((uint)pMsg.heroposinfo.pos, pMsg.heroposinfo.heroid);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换FPInfo
    //eMsgToGCFromGS_NotifyFightPropertyInfo
    Int32 OnNetMsg_NotifyFightPropertyInfo(Stream stream)
    {

        GSToGC.NotifyAFPData pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        IEntity entity;
        UInt64 sGUID;
        sGUID = pMsg.guid;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            foreach (GSToGC.NotifyAFPData.FPInfo fpInfo in pMsg.info)
            {
                switch ((GSToGC.NotifyAFPData.EnumFpType)fpInfo.type)
                {
                    case GSToGC.NotifyAFPData.EnumFpType.PhyAttack:
                        {
                            entity.PhyAtk = fpInfo.value;//物理攻击
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MagicAttack:
                        {
                            entity.MagAtk = fpInfo.value;//魔法攻击
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.PhyDefense:
                        {
                            entity.PhyDef = fpInfo.value;//物理防御
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MagicDefense:
                        {
                            entity.MagDef = fpInfo.value;//魔法防御
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MoveSpeed:
                        {
                            entity.OnEntityMoveSpeedChange(fpInfo.value);//移动速度
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.AttackSpeed:
                        {
                            entity.AtkSpeed = fpInfo.value;//攻击速度

                            if (entity.AtkSpeed != 0)
                            {
                                float spd = 1 / (entity.AtkSpeed / 1000.0f);
                                entity.RealEntity.SetAttackAnimationSpd(spd);
                            }
                            else
                            {
                                entity.RealEntity.SetAttackAnimationSpd(1.0f);
                            }
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.AttackDist:
                        {
                            entity.AtkDis = fpInfo.value / 100;//攻击距离
                            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LocalPlayerRange);
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MaxHP:
                        {
                            entity.SetHpMax(fpInfo.value);//最大血量

                            if (entity.BloodBar != null)
                            {
                                entity.BloodBar.SetXueTiaoInfo();
                            }
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MaxMP:
                        {
                            entity.SetMpMax(fpInfo.value);//最大魔法量
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.HPRecoverRate:
                        {
                            entity.HpRecover = fpInfo.value;//血量恢复速度
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.MPRecoverRate:
                        {
                            entity.MpRecover = fpInfo.value;//魔法量恢复速度
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.ReliveTime:
                        {
                            entity.RebornTime = fpInfo.value;//重生时间
                        }
                        break;
                    case GSToGC.NotifyAFPData.EnumFpType.CooldownReduce:
                        {
                            entity.CooldownReduce = fpInfo.value;//冷却时间
                        }
                        break;
                    }
            }
        }
        return (Int32)ErrorCodeEnum.Normal;

    }

    //已换NotifyHPInfo
    //eMsgToGCFromGS_NotifyHPInfo
    Int32 OnNetMsg_NotifyHpInfo(Stream stream)
    {
        GSToGC.NotifyHPInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (GSToGC.NotifyHPInfo.HPInfo info in pMsg.hpinfo)
        {
            UInt64 sGUID;
            sGUID = info.guid;
            IEntity entity;
            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
            {
                entity.SetHp((float)info.curhp);
                entity.SetHpMax((float)info.maxhp);


                entity.OnUpdateHp();
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换NotifyMPInfo
    //eMsgToGCFromGS_NotifyMPInfo
    Int32 OnNetMsg_NotifyMpInfo(Stream stream)
    {
        GSToGC.NotifyMPInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        foreach (GSToGC.NotifyMPInfo.MPInfo info in pMsg.mpinfo)
        {
            UInt64 sGUID;
            sGUID = info.guid;
            IEntity entity;
            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
            {
                entity.SetMp((float)info.curmp);        //ToReview int->float（是否本身就是float，有没有丢失小数部分）
                entity.SetMpMax((float)info.maxmp);     //ToReview int->float
                if (entity is IPlayer)
                {
                    BloodBarPlayer playerXueTiao = (BloodBarPlayer)entity.BloodBar;
                    playerXueTiao.UpdateMp();//实体头顶的蓝条
                }
            }
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyHeroInfo(Stream stream)
    {
        GSToGC.NotifyHeroInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;     //Review long->ulong
        IEntity entity = null;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            entity.SetLevel(pMsg.level);
            entity.SetExp((float)pMsg.exp);
            IPlayer player = (IPlayer)entity;
            player.SetFuryState((EFuryState)pMsg.fury);
            BloodBarPlayer BloodBarPlayer = (BloodBarPlayer)entity.BloodBar;
            BloodBarPlayer.UpdateLevel();
            BloodBarPlayer.SetXueTiaoInfo();
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroInfoChange, (IPlayer)entity);
            //ToReview absorb1和absorb2没用上
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //战斗结束
    //已换BattleFinishInfo
    //eMsgToGCFromGS_NotifyBattleFinish
    Int32 OnNetMsg_NotifyBattleFinish(Stream stream)
    {
        GSToGC.BattleFinish pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        EventCenter.Broadcast<UInt64>((Int32)GameEventEnum.GameOver, pMsg.bulidguid);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //主角经验
    //已换Exp
    //eMsgToGCFromGS_NotifyExpInfo
    Int32 OnNetMsg_NotifyExpInfo(Stream stream)
    {
        if (PlayerManager.Instance.LocalPlayer == null)
        {
            return (Int32)ErrorCodeEnum.Normal;
        }
        GSToGC.Exp pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        PlayerManager.Instance.LocalPlayer.SetExp((float)pMsg.exp);
        return (Int32)ErrorCodeEnum.Normal;
    }

    //主角等级
    /************************************************************************/
    /*OnNetMsg_NotifyLvInfo                                                 */
    /************************************************************************/
    Int32 OnNetMsg_NotifyLvInfo(Stream stream)
    {
        StartCoroutine(LvInfoCoroutine(stream));
        return (Int32)ErrorCodeEnum.Normal;
    }


    public IEnumerator LvInfoCoroutine(Stream stream)
    {
        //解析消息
        GSToGC.LevelInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            yield break;
        }

        //创建特效
        yield return 1;
        UInt64 sGUID;
        sGUID = pMsg.guid;
        Int32 level = pMsg.level;
        IEntity entity;
        if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity))
        {
            if (entity.Level != level)
            {
                Thanos.Effect.EffectManager.Instance.CreateTimeBasedEffect("effect/other/shengji", 10.0f, entity);

                yield return 1;
                Vector3 pos = PlayerManager.Instance.LocalPlayer.RealEntity.transform.position;
                if (entity.GetDistanceToPos(pos) <= 30)
                {
                    ResourceItem unit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_LEVELUP_SOUND, ResourceType.ASSET);
                    AudioClip clip = unit.Asset as AudioClip;

                    AudioManager.Instance.PlayEffectAudio(clip);
                }
            }

            //更新level
            yield return 1;
            entity.SetLevel(level);
            if (entity.BloodBar is BloodBarPlayer)
            {
                BloodBarPlayer BloodBarPlayer = (BloodBarPlayer)entity.BloodBar;
                BloodBarPlayer.UpdateLevel();
            }
        }
        if (UIGameBattleInfo.Instance != null)
        {
            UIGameBattleInfo.Instance.SetLevelInfo(sGUID, (int)level);
        }
        if (UIViewerBattleInfo.Instance != null)
        {
            UIViewerBattleInfo.Instance.SetLevelInfo(sGUID, (int)level);
        }
        BattleingData.Instance.AddPlayer(sGUID, level, BattleDataType.Level);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AllPlayerLevel);
        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroLvChange, (IPlayer)entity);

    }


    Int32 OnNetMsg_NotifyFuryValue(Stream stream)
    {
        GSToGC.FuryVal pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = pMsg.guid;
        if (PlayerManager.Instance.LocalPlayer != null)
        {
            PlayerManager.Instance.LocalPlayer.SetFuryValue(pMsg.fury);//设置狂暴值
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换FuryState
    //eMsgToGCFromGS_NotifyFuryState
    Int32 OnNetMsg_NotifyFuryState(Stream stream)
    {
        GSToGC.FuryState pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 sGUID;
        sGUID = (ulong)pMsg.guid;
        IEntity entity = PlayerManager.Instance.GetEntity(sGUID);
        if (entity != null)
        {
            IPlayer player = (IPlayer)entity;
            player.SetFuryState((EFuryState)pMsg.state);
            PlayerSkillData.Instance.SetPlayerFuryState(player, (EFuryState)pMsg.state);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SkillCDInfo, (IPlayer)entity);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyVoipRoomId(Message pcMsg)
    {
        long roomId = pcMsg.GetInt64();
        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换（见OnNotifyBornSoldierCoroutine函数）
    Int32 OnNotifyBornSoldier(Stream stream)
    {
      //  OnNotifyBornSoldierCoroutine(stream);
        return (Int32)ErrorCodeEnum.Normal;
    }

    public Int32 OnNotifyBornSoldierCoroutine(Stream stream)
    {
        GSToGC.BornSoler pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return 0;
        }
        int type = pMsg.type;   // 1 altar solder ; 2 atk city solder 3 bomb soldier
        int campType = pMsg.camp;
        string[] effectPath = {
                                    "effect/building/Altar",//蓝方小兵生成特效
                                    "effect/building/Altar_red",//红方小兵生成特效
                              };
        int redIndex = (campType % 2 == 0) ? 1 : 0;

        NormalEffect effect = EffectManager.Instance.CreateNormalEffect(effectPath[redIndex], null);
        effect.obj.transform.position = ConvertPosToVector3(pMsg.pos);

        UInt64 guid;
        guid = pMsg.guid;
        MsgInfoManager.Instance.SetAudioPlay(guid, MsgInfoManager.AudioPlayType.KillAudio);

        return 0;
    }

    //已换
    void OnNetMsg_WarningToSelectHero()
    {
        if (GameStateManager.Instance.GetCurState().GetStateType() == GameStateTypeEnum.Hero)
        {
            MsgInfoManager.Instance.ShowMsg((int)ErrorTypeEnum.WarnningToSelectHero);
        }
    }

    Int32 OnNetMsg_NotifyNewsGuideReConnect(Stream stream)
    {
        GSToGC.GuideLastStep pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        int lastId = pMsg.stepid;
        SceneGuideTaskManager.Instance().InitSenseTaskData();
        SceneGuideTaskManager.Instance().SetHasFinishedAllGuide(pMsg.ifComp);
        if (pMsg.ifComp || lastId == 0)//finish all or first 
        {
            SceneGuideTaskManager.Instance().StartAsignedStep(1001);
            return (Int32)ErrorCodeEnum.Normal;
        }
        else if (lastId != 0)
        {//这个是新手引导接着上面一步            
            SceneGuideTaskManager.Instance().ReConnectTask(lastId);
        }
        return (Int32)ErrorCodeEnum.Normal;
    }

    //ToReviw 未换（消息参数不对应）
    //协议名不一致eMsgToGCFromCS_NotifyReconnectHeroGoodsInfo
    //     Int32 OnNetMsg_NotifyReconnectHeroGoods(CMsg pcMsg)
    //     {
    //         UInt64 targetID = pcMsg.GetGUID();
    //         int time = pcMsg.GetInt32();
    //         UInt32 num = pcMsg.GetUInt32();
    //         //GameMethod.DebugError("time = " + time);
    //         if (UIGameRecord.Instance != null)
    //         {
    //             UIGameRecord.Instance.SetReconnectTime(time);
    //         }
    //         for (int i = 0; i < num; i++)
    //         {
    //             byte itPos = pcMsg.GetByte();
    //             int m_32Type = (int)pcMsg.GetUInt32();
    //             int m_n32Num = pcMsg.GetInt32();
    // 
    //             int n32RemainCDTime = pcMsg.GetInt32();
    //             if (n32RemainCDTime < 0)
    //             {
    //                 n32RemainCDTime = 0;
    //             }
    //             float totTime = 0.0f;
    //             if (true == ConfigReader.ItemXmlInfoDict.ContainsKey(m_32Type))
    //             {
    //                 totTime = ConfigReader.ItemXmlInfoDict[m_32Type].un32CdTime;
    //             }
    // 
    //             float second = (float)n32RemainCDTime / 1000.0f;
    // 
    //             if (PlayerManager.Instance.LocalAccount.ObjType == ObPlayerOrPlayer.PlayerType)
    //             {
    //                 PlayerManager.Instance.LocalPlayer.AddUserGameItems((int)itPos, m_32Type, m_n32Num, second);
    //             }
    //             Ientity entity;
    //             if (EntityManager.AllEntitys.TryGetValue(targetID, out entity))
    //             {
    //                 PackageData.Instance.SetPackageData((Iplayer)entity, (int)itPos, m_32Type, m_n32Num, totTime, second);
    //                 EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PackageBuyInfo, (Iplayer)entity);
    //             }
    //             EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateUserGameItems);
    //         }
    //         EventCenter.Broadcast<Int64>((Int32)GameEventEnum.GameEvent_GameStartTime, time);
    //         return (Int32)EErrorCode.eNormal;
    //     }

    //已换
    Int32 OnNetMsg_NotifyReconnectInfo(Stream stream)
    {
        GSToGC.NotifyReconnectInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        EventCenter.Broadcast((Int32)GameEventEnum.ReConnectSuccess);
        BattleStateEnum battleState = (BattleStateEnum)pMsg.battlestate;
        Dictionary<uint, int> heroSelectDic = new Dictionary<uint, int>();
        Dictionary<uint, bool> heroIsSelectDic = new Dictionary<uint, bool>();
        foreach (GSToGC.NotifyReconnectInfo.ReconnectInfo info in pMsg.reconnectinfo)
        {
            int seat = info.pos;
            UInt64 guid;
            guid = info.guid;
            int heroId = (int)info.heroid;      //ToReview uint->int
            IPlayer player = null;
            if (!PlayerManager.Instance.AccountDic.TryGetValue(guid, out player))
            {
                player = (IPlayer)PlayerManager.Instance.HandleCreateEntity(guid, EntityCampTypeEnum.Null);
            }
            player.SetReconnectPlayerInfo((uint)seat, info.nickname);
            if (!PlayerManager.Instance.AccountDic.ContainsKey(guid))
            {
                PlayerManager.Instance.AddAccount(guid, player);
            }
            ObPlayerOrPlayer obType = (seat == 7 || seat == 8) ? ObPlayerOrPlayer.PlayerObType : ObPlayerOrPlayer.PlayerType;
            player.SetObjType(obType);
            if (GameUserModel.Instance.IsLocalPlayer(guid))
            {
                PlayerManager.Instance.LocalAccount = player;
            }
            if (heroId != 0 && obType != ObPlayerOrPlayer.PlayerObType)
            {
                heroSelectDic.Add((uint)seat, heroId);
                heroIsSelectDic.Add((uint)seat, info.ifselected);
            }
        }


        //更新英雄选择数据
        if (battleState == BattleStateEnum.eBS_SelectRune || battleState == BattleStateEnum.eBS_SelectHero)
        {
            foreach (var item in heroSelectDic.Keys)
            {
                int heroId = 0;
                bool isSelect = false;
                if (!heroSelectDic.TryGetValue(item, out heroId))
                {
                    continue;
                }
                heroIsSelectDic.TryGetValue(item, out isSelect);
                if (isSelect)
                {
                    HeroCtrl.Instance.AddRealSelectHero(item, heroId);
                }
                else
                {
                    HeroCtrl.Instance.AddPreSelectHero(item, heroId);

                    if (item == (int)PlayerManager.Instance.LocalAccount.GameUserSeat)
                    {
                        HeroCtrl.Instance.ReconnectPreSelect(heroId);
                    }
                }
            }
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    //已换
    Int32 OnNetMsg_NotifyUserGuideSetUps(Stream stream)
    {
        if (HolyTechGameBase.Instance.SkipNewsGuide)
        {
            IGuideTaskManager.Instance().SetTaskIsFinish(true, true);
            return (Int32)ErrorCodeEnum.Normal;
        }

        GSToGC.GuideSteps pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        UInt64 guide;
        guide = pMsg.guid;
        Int32 isLineGuide = pMsg.type;
        bool isGuideOver = pMsg.ifComp; // 1 over, 0 no over
        string str = pMsg.steps;
        IGuideTaskManager.Instance().SetTaskIsFinish((isLineGuide == 1 ? true : false), isGuideOver);

        if (isLineGuide == 1 && !isGuideOver)//line task
        {
            int taskId = (str.Length == 0) ? 0 : Convert.ToInt32(str);
            bool isStart = (str.Length == 0) ? true : false;
            if (isStart)
            {
                //Debug.LogError("isStart");
                taskId = IGuideTaskManager.startId;
            }
            else
            {
                //Debug.LogError(taskId);
                List<int> idList = ConfigReader.GetIGuideManagerInfo(taskId).NextTaskId;
                taskId = idList.ElementAt(0);
            }
            IGuideTaskManager.Instance().SetTaskId(taskId);
        }
        else if (isLineGuide == 2 && !isGuideOver && IGuideTaskManager.Instance().IsLineTaskFinish())//trigger task
        {
            IGuideTaskManager.Instance().SetHasTriggerTask(str);
            IGuideTaskManager.Instance().StartTriggerTask();
        }


        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifySendSoldierTip(Stream stream)
    {
        GSToGC.GameTips pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        if (SceneGuideTaskManager.Instance().IsNewsGuide() != SceneGuideTaskManager.SceneGuideType.NoGuide)
            return (Int32)ErrorCodeEnum.Normal;
        int type = pMsg.errocode;
        int camp = pMsg.campid;
        string campTip = null;
        if (type == -130825)// content   “出动超级士兵”    effect 1     type 1
        {   //超级兵
            if (camp % 2 != 0)//打爆亡灵族
            {
                campTip = "亡灵势力";
            }
            else//打爆精灵族
            {
                campTip = "精灵势力";
            }
            MsgConfigInfo msg = new MsgConfigInfo(ConfigReader.GetMsgInfo(type));
            msg.content = campTip + msg.content;  
            MsgInfoManager.Instance.ShowMsg(msg);
        }
        else
        { 
            MsgInfoManager.Instance.ShowMsg(type); //“战斗开始”
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifySecondaryGuide(Stream stream)
    {
        GSToGC.SecondGuideTask pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        if (SceneGuideTaskManager.Instance().IsNewsGuide() != SceneGuideTaskManager.SceneGuideType.NoGuide)
            return (Int32)ErrorCodeEnum.Normal;
        SecondaryGuideManager.Instance.CleanAll();
        foreach (GSToGC.SecondGuideTask.task_info info in pMsg.taskinfo)
        {
            int taskId = info.taskid;
            int matches = info.num;
            SecondaryGuideManager.Instance.InitFinishTask(taskId, matches);
        }
        SecondaryGuideManager.Instance.InitTask();
        SecondaryGuideManager.Instance.StartAllTask();
        return (Int32)ErrorCodeEnum.Normal;
    }
    //通知匹配队伍基本信息
    Int32 OnNetMsg_NotifyMatchTeamBaseInfo(Stream stream)
    {
        GSToGC.NotifyMatchTeamBaseInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        //初始化组队伍基本信息  设置队伍地图id 及匹配模式
        TeamMatchCtrl.Instance.InitTeamBaseInfo(pMsg.mapid, pMsg.matchtype);

        //teamid不等于0时
        if (pMsg.teamid != 0)
        {
            TeamMatchCtrl.Instance.Enter();//广播进入匹配组队界面，显示组队界面
        }
        else//点击退出匹配的时候返回的teamid为0
        {
            TeamMatchCtrl.Instance.Exit();//隐藏界面
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    //匹配队伍玩家信息
    Int32 OnNetMsg_NotifyMatchTeamPlayerInfo(Stream stream)
    {
        GSToGC.NotifyMatchTeamPlayerInfo pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }
        if (pMsg.isInsert)
        {
            //新增队友
            TeamMatchCtrl.Instance.AddTeammate(pMsg.postion, pMsg.nickname, pMsg.headid.ToString(), pMsg.userlevel);
        }
        else
        {
            //删除队友
            TeamMatchCtrl.Instance.DelTeammate(pMsg.nickname);
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    //匹配界面显示
    Int32 OnNetMsg_NotifyMatchTeamSwitch(Stream stream)
    {
 
        GSToGC.NotifyMatchTeamSwitch pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        if (pMsg.startflag)//取消匹配时返回的值为true
        {
            HolyTechGameBase.Instance.IsQuickBattle = true;
            TeamMatchCtrl.Instance.StartMatchSearching();//广播消息，显示匹配界面
        }
        else //取消匹配时返回的值为false
        {
            TeamMatchCtrl.Instance.StopMatchSearching();//广播消息，隐藏匹配界面
        }

        return (Int32)ErrorCodeEnum.Normal;
    }

    Int32 OnNetMsg_NotifyMatchInviteJoin(Stream stream)
    {
        if (GameStateManager.Instance.GetCurState().GetStateType() != GameStateTypeEnum.Lobby)
            return (Int32)ErrorCodeEnum.Normal;

        GSToGC.NotifyMatchInviteJoin pMsg;
        if (!ProtobufMsg.MessageDecode(out pMsg, stream))
        {
            return PROTO_DESERIALIZE_ERROR;
        }

        TeamMatchCtrl.Instance.ShowInvitation(pMsg.nickname);

        return (Int32)ErrorCodeEnum.Normal;
    }

    //工具函数GSToGC.Pos转Vector3（会将厘米转成米）
    private Vector3 ConvertPosToVector3(GSToGC.Pos pos)
    {
        if (pos != null)
            return new Vector3((float)pos.x / 100.0f, GetGlobalHeight(), (float)pos.z / 100.0f);
        else
            return Vector3.zero;
    }

    //工具函数GSToGC.Dir转Vector3
    private Vector3 ConvertDirToVector3(GSToGC.Dir dir)
    {
        float angle = (float)(dir.angle) / 10000;
        return new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle));
    }
}

