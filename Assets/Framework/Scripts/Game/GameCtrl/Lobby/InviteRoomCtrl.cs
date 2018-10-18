using System;

namespace Thanos.Ctrl
{
    public class InviteRoomCtrl : Singleton<InviteRoomCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_InviteRoomEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_InviteRoomExit);
        }

        public void AcceptAddFriend(string roomID, string password)
        {
            HolyGameLogic.Instance.AddRoom(roomID, password);
        }

    }
}

