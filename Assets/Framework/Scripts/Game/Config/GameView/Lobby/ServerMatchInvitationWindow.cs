using System;
using GameDefine;
using Thanos.Ctrl;

namespace Thanos.View
{
    public class ServerMatchInvitationWindow : BaseWindow
    {
        public ServerMatchInvitationWindow()
        {
            mScenesType = EScenesType.EST_Login;
            mResName = GameConstDefine.LoadGameMatchInvitationUI;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener<uint,uint>((Int32)GameEventEnum.GameEvent_ServerMatchInvitationEnter, ShowUI);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ServerMatchInvitationExit, Hide);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LobbyExit, Hide);
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener<uint,uint>((Int32)GameEventEnum.GameEvent_ServerMatchInvitationEnter, ShowUI);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ServerMatchInvitationExit, Hide);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LobbyExit, Hide);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            //地图
            mTipLabel = mRoot.Find("Tips").GetComponent<UILabel>();
            mAccept = mRoot.Find("Accept").GetComponent<UIButton>();
            mCancel = mRoot.Find("Cancel").GetComponent<UIButton>();

            EventDelegate.Add(mAccept.onClick, OnAccept);
            EventDelegate.Add(mCancel.onClick, OnmCancel);
        }

        //窗口控件释放
        protected override void RealseWidget()
        {

        }

     
        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_TeamMatchEnter, Hide);
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_TeamMatchEnter, Hide);
        }

        //显示
        public override void OnEnable()
        {
           

        }

        //隐藏
        public override void OnDisable()
        {

        }
        
        public void ShowUI(uint mapid, uint fihtid)
        {
            Show();
            mTipLabel.text =  ConfigReader.GetMsgInfo(40037).content;
            mMapId = mapid;
            mFightId = fihtid;
        }

        public void OnAccept()
        {
            TeamMatchCtrl.Instance.ResponseServerInvitation(mMapId, mFightId,true);
            Hide();
        }

        public void OnmCancel()
        {
            TeamMatchCtrl.Instance.ResponseServerInvitation(mMapId, mFightId, false);
            Hide();
        }

        UILabel mTipLabel;
        UIButton mAccept;
        UIButton mCancel;

        uint mMapId;
        uint mFightId;

    }
}

