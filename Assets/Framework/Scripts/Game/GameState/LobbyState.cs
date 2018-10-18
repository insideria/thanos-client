using UnityEngine;
using GameDefine;
using Thanos.Resource;
using Thanos.Ctrl;
using System;
using Thanos.View;

namespace Thanos.GameState
{
    class LobbyState : IGameState
    {
        GameStateTypeEnum stateTo;
        GameObject mScenesRoot;
        GameObject mUIRoot;

        public LobbyState()
        {
        }

        public GameStateTypeEnum GetStateType()
        {
            return GameStateTypeEnum.Lobby;
        }

        public void SetStateTo(GameStateTypeEnum gs)
        {
            stateTo = gs;
        }

        public void Enter()
        {
            SetStateTo(GameStateTypeEnum.Continue);
            LobbyCtrl.Instance.Enter();//显示UI   向服务器请求队伍信息
        
            //Audio/EnvironAudio/mus_fb_login_lp  背景音乐 与之前音乐相同，但是如果不是登录状态转换过来的就没有之前的音频。所以重新获取并播放
            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_UIBGSOUND, ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;
            AudioManager.Instance.PlayBgAudio(clip);
            if (!AudioManager.Instance.LoopAudioSource.isPlaying)
            {
                AudioManager.Instance.LoopAudioSource.Play();
            }  
          
             EventCenter.AddListener<FEvent>((Int32)GameEventEnum.IntoRoom, OnEvent); // 进入房间
             EventCenter.AddListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);// 转换加载状态
             EventCenter.AddListener<FEvent>((Int32)GameEventEnum.IntoHero, OnEvent);// 转换英雄选择状态
             EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);  // 断线监听
             EventCenter.AddListener<UInt64,string>((Int32)GameEventEnum.GameEvent_InviteCreate, InviteAddFriend);//邀请朋友
             EventCenter.AddListener<string>((Int32)GameEventEnum.GameEvent_InviteAddRoom, InviteAddRoom);//邀请进入房间
             EventCenter.AddListener((Int32)GameEventEnum.ReconnectToBatttle, ReconnectToBatttle);//快速战斗  
            
        }
        private void SdkLogOff()
        {
            GameMethod.LogOutToLogin();
            SetStateTo(GameStateTypeEnum.Login);
        }

        private void InviteAddRoom(string arg1)
        {
            InviteRoomCtrl.Instance.Enter();
        }

        private void InviteAddFriend(UInt64 sGUID ,string temp)
        {
            if (InviteCtrl.Instance.AddDic(sGUID, temp) && InviteCtrl.Instance.InvatiDic.Count > 1)
            {
                InviteCtrl.Instance.ChangeInvite(sGUID, temp);
            }else
                InviteCtrl.Instance.Enter(sGUID,temp);
        }

        public void Exit()
        {
            LobbyCtrl.Instance.Exit();

            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.IntoRoom, OnEvent);
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.IntoHero, OnEvent);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);            
            EventCenter.RemoveListener<UInt64,string>((Int32)GameEventEnum.GameEvent_InviteCreate, InviteAddFriend);
            EventCenter.RemoveListener<string>((Int32)GameEventEnum.GameEvent_InviteAddRoom, InviteAddRoom);
            EventCenter.RemoveListener((Int32)GameEventEnum.ReconnectToBatttle, ReconnectToBatttle);    

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
                case GameEventEnum.IntoRoom://创建房间（自定义）
                    SetStateTo(GameStateTypeEnum.Room);
                    break;
                case GameEventEnum.IntoHero://英雄选择
                    GameStateManager.Instance.ChangeGameStateTo(GameStateTypeEnum.Hero);
                    break;
                case GameEventEnum.Loading://加载状态
                    GameStateTypeEnum stateType = (GameStateTypeEnum)evt.GetParam("NextState");
                    LoadingState lState = GameStateManager.Instance.getState(GameStateTypeEnum.Loading) as LoadingState;
                    lState.SetNextState(stateType);
                    lState.SetFrontScenes(View.EScenesType.EST_Login);
                    SetStateTo(GameStateTypeEnum.Loading);
                    break;              
            }
            
        }

        public void ReconnectToBatttle()
        {
            EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_ReEnter);
        }
    }
}


