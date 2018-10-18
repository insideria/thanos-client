using System;

namespace Thanos.Ctrl
{
    public class LobbyCtrl : Singleton<LobbyCtrl>
    {
        public void Enter()
        {
            //广播进入游戏大厅  显示UI
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LobbyEnter);
            //向服务器请求队伍信息 返回的消息分别为
            /* notifyGoodsBuy
             * notifyUserCLDays
             * notifyCSHeroList
             * notifyRunesList
             * notifyOtherItemInfo
             * notifyUserSNSList
             */
            HolyGameLogic.Instance.EmsgToss_RequestMatchTeamList();
            //初始化引导的信息 小谷注释 此方法空
            //InitLobbyGuideUIInfo();
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LobbyExit);
        }

        public void AskNewGMCmd(string cmd)
        {
            HolyGameLogic.Instance.EmsgTocs_AddNewGMCmd(cmd);
        }

        /// <summary>
        /// 进入大厅的时候初始化引导的信息
        /// </summary>
        private void InitLobbyGuideUIInfo()
        {
            UIGuideCtrl.Instance.InitLobbyGuideInfo();
        }

        internal void AskPersonInfo()
        {
            HolyGameLogic.Instance.PersonInfo();
        }


        internal void InviteInfo(ulong sGUID, string nickName)
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_InviteCreate, sGUID, nickName);
        }
    }
}
