using System;
using System.Collections.Generic;

namespace Thanos.Ctrl
{
    public class Teammate
    {
        public uint mPostion;
        public string mName;
        public string mPic;
        public uint mLv;

        public Teammate(uint pos, string name, string pic, uint lv)
        {
            mPostion = pos;
            mName = name;
            mPic = pic;
            mLv = lv;
        }
    }

    public class TeamMatchCtrl : Singleton<TeamMatchCtrl>
    {

        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_TeamMatchEnter);
        }

        public void Exit()
        {
            mTeammateList.Clear();
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_TeamMatchExit);
        }

        //邀请好友组队匹配
        public void InvitationFriend(string nickName)
        {
            HolyGameLogic.Instance.AskInvitationFriend(nickName);
        }

        //新增队友
        public void AddTeammate(uint pos,string name, string pic, uint lv)
        {
            Teammate teammate = new Teammate(pos,name, pic, lv);
            mTeammateList.Add(name, teammate);

            //广播添加队友消息  与之绑定的事件为AddTeammate
            EventCenter.Broadcast<Teammate>((Int32)GameEventEnum.GameEvent_TeamMatchAddTeammate, teammate);
        }

        //删除队友
        public void DelTeammate(string nickName)
        {
            mTeammateList.Remove(nickName);

            EventCenter.Broadcast<string>((Int32)GameEventEnum.GameEvent_TeamMatchDelTeammate, nickName);
        }

        //请求开始匹配
        public void AskStartMatch()
        {
            HolyGameLogic.Instance.AskStartTeamMatch();
        }

        //请求停止匹配
        public void AskStopMatch()
        {
            HolyGameLogic.Instance.AskStopTeamMatch();
        }

        //开始匹配
        public void StartMatchSearching()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_TeamMatchSearchinEnter);
        }

        //停止匹配
        public void StopMatchSearching()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_TeamMatchSearchinExit);
        }

        //取得队友列表
        public Dictionary<string,Teammate> GetTeammateList()
        {
            return mTeammateList;
        }

        //初始化组队匹配
        public void InitTeamBaseInfo(uint mapId, uint matchType)
        {
            mMapId = mapId;
            mMatchType = (EBattleMatchType)matchType;

            mTeammateList.Clear();//清除队友列表
        }

        //打开组队邀请
        public void ShowInvitation(string name)
        {
            EventCenter.Broadcast<string>((Int32)GameEventEnum.GameEvent_TeamMatchInvitationEnter, name);
        }

       

        //隐藏组队邀请
        public void HideInvitation()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_TeamMatchInvitationExit);
        }

        //接受组队邀请
        public void AccpetInvitation(string name)
        {
            HolyGameLogic.Instance.AskAcceptInvitation(name);
        }

        //打开服务器邀请
        public void ShowServerInvitation(uint mapid, uint fightid)
        {
            EventCenter.Broadcast<uint, uint>((Int32)GameEventEnum.GameEvent_ServerMatchInvitationEnter, mapid, fightid);
        }

        //反馈服务器邀请
        public void ResponseServerInvitation(uint mapid, uint fightid, bool accpet)
        {
            HolyGameLogic.Instance.AskResponseServerInvitation(mapid, fightid, accpet);
        }      

        //退出队伍
        public void QuitTeam()
        {
            HolyGameLogic.Instance.AskQuitTeam();
        }

        public bool IsTeamFull()
        {
            MapInfo info = MapLoadConfig.Instance.GetMapInfo(mMapId);

            if (info != null && mTeammateList.Count >= (info.mPlayerNum / 2))
            {
                return true;
            }

            return false;
        }

        Dictionary<string, Teammate> mTeammateList = new Dictionary<string,Teammate>();

        public uint GetMapId()
        {
            return mMapId;
        }

        public EBattleMatchType GetMatchType()
        {
            return mMatchType;
        }

        private uint mMapId = 0;
        private EBattleMatchType mMatchType = EBattleMatchType.EBMT_Normal;
    }

}