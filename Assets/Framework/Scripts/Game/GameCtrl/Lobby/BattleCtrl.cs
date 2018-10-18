using System;
using System.Collections.Generic;
using Thanos.GameData;
using GCToCS;

namespace Thanos.Ctrl
{
    public class RoomItem
    {
        public UInt64 mRoomId;
        public UInt32 mMapId;
        public string mOwer;
        public UInt32 mCurNum;
        public UInt32 mMaxNum;
        public bool mIsPassWord;

        public RoomItem(UInt64 roomId,  UInt32 mapId, string ower, UInt32 curNum, UInt32 maxNum, bool isPassWord)
        {
            mRoomId = roomId;
            mMapId = mapId;
            mOwer = ower;
            mCurNum = curNum;
            mMaxNum = maxNum;
            mIsPassWord = isPassWord;
        }
    }

    public class BattleCtrl : Singleton<BattleCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleExit);
        }

        //创建房间
        public void CreateRoom(int mapId, string password)
        {
            HolyGameLogic.Instance.CreateRoom(mapId, password);
        }

        //向服务器请求刷新房间列表
        public void AskRoomList()
        {
            HolyGameLogic.Instance.AskRoomList();
        }

        //请求加入房间
        public void AskAddRoom(string roomId, string password)
        {
            if (roomId == null || roomId == "" || UInt64.Parse(roomId) <= 0)
            {
                MsgInfoManager.Instance.ShowMsg((int)ErrorCodeEnum.NullBattle);
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleUpdateRoomList);
                return;
            }

            UInt64 id = UInt64.Parse(roomId);
            if (mRoomList.ContainsKey(id))
            {
                if (mRoomList[id].mIsPassWord && (password == null || password == ""))
                {
                    MsgInfoManager.Instance.ShowMsg((int)ErrorCodeEnum.NULLPassWord);
                    return;
                }
            }

            HolyGameLogic.Instance.AddRoom(roomId, password);
        }

        //刷新服务器列表
        public void UpdateRoomList(List<GSToGC.RoomInfo> roomList)
        {
            mRoomList.Clear();

            foreach (GSToGC.RoomInfo roomInfo in roomList)
            {
                AddRoomItem((UInt32)roomInfo.roomId, (UInt32)roomInfo.mapId, roomInfo.master, (UInt32)roomInfo.curUserCount,(UInt32)roomInfo.maxUserCount, (roomInfo.ifPwd == 1));
            }

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_BattleUpdateRoomList);
        }

        //更新房间列表
        private void AddRoomItem(UInt64 roomId,UInt32 mapId,string ower,UInt32 curNum,UInt32 maxNum,bool isPassword)
        {
            RoomItem room = new RoomItem(roomId,mapId,ower,curNum,maxNum, isPassword);
            
            mRoomList.Add(roomId,room);
        }

        //取得房间列表
        public Dictionary<UInt64, RoomItem> GetRoomList()
        {
            return mRoomList;
        }

        //取得房间信息
        public RoomItem GetRoomInfo(UInt32 id)
        {
            if (mRoomList.ContainsKey(id))
            {
                return mRoomList[id];
            }

            return null;
        }

        //申请组队匹配
        public void AskMatchBattle(int mapId, EBattleMatchType mt)
        {
            mMapId = mapId;
            mMatchType = mt;

            HolyGameLogic.Instance.AskMatchBattle(mapId,mt);
        }

        //申请创建新手战役
        public void AskCreateGuideBattle(int mapId, AskCSCreateGuideBattle.guidetype mGtype)
        {
            mMapId = mapId;

            UIGuideCtrl.Instance.GuideBattleType(mGtype);

            HolyGameLogic.Instance.AskCSToCreateGuideBattle(mapId, mGtype);
        }

        public int GetMapId()
        {
            return mMapId;
        }

        public EBattleMatchType GetMatchType()
        {
            return mMatchType;
        }

        Dictionary<UInt64, RoomItem> mRoomList = new Dictionary<UInt64, RoomItem>();

        private int mMapId = 0;
        private EBattleMatchType mMatchType = EBattleMatchType.EBMT_Normal;
    }
}
