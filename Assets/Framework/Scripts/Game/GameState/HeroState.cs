using System;
using UnityEngine;
using GameDefine;
using Thanos.Resource;
using Thanos.Ctrl;
using Thanos.View;

namespace Thanos.GameState
{
    class HeroState : IGameState
    {
        GameStateTypeEnum stateTo;

        GameObject mScenesRoot;

        GameObject mUIRoot;

        public HeroState()
        {
        }

        public GameStateTypeEnum GetStateType()
        {
            return GameStateTypeEnum.Hero;
        }

        public void SetStateTo(GameStateTypeEnum gs)
        {
            stateTo = gs;
        }

        public void Enter()
        {
            SetStateTo(GameStateTypeEnum.Continue);

            HeroCtrl.Instance.Enter(); // 广播消息 显示选择英雄界面（SelectHero界面）

            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_UIBGSOUND, ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;
            AudioManager.Instance.PlayBgAudio(clip);
                      
            //添加事件
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent); //转换LoadingState状态
            EventCenter.AddListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);//连接服务器失败
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff); //用户断线
        }

        public void Exit()
        {
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);
            EventCenter.RemoveListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff); 

            HeroCtrl.Instance.Exit();
        }

        public void OnConnectServerFail()
        {
            EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_Reconnect);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {

        }

        public GameStateTypeEnum Update(float fDeltaTime)
        {
            return stateTo;
        }

        public void OnEvent(FEvent evt)
        {
            switch ((GameEventEnum)evt.GetEventId())
            {
                case GameEventEnum.Loading:
                    {
                        GameStateTypeEnum stateType = (GameStateTypeEnum)evt.GetParam("NextState");
                        LoadingState lState = GameStateManager.Instance.getState(GameStateTypeEnum.Loading) as LoadingState;
                        lState.SetNextState(stateType);
                        lState.SetFrontScenes(View.EScenesType.EST_Login);
                        SetStateTo(GameStateTypeEnum.Loading);
                    }
                   
                    break;
            }
        }

        private void SdkLogOff()
        {
            GameMethod.LogOutToLogin();
            SetStateTo(GameStateTypeEnum.Login);
        }
    }
}


