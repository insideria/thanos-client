using System;
using UnityEngine;
using GameDefine;
using Thanos.Resource;
using Thanos.Ctrl;
using Thanos.View;
using Thanos.GameEntity;
using Thanos.Model;

namespace Thanos.GameState
{
    class OverState : IGameState
    {
        GameStateTypeEnum stateTo;

        GameObject mScenesRoot;

        GameObject mUIRoot;

        float mTime;
        bool mNeedUpdate;
        bool mNeedScore;

        public GameStateTypeEnum GetStateType()
        {
            return GameStateTypeEnum.Over;
        }

        public void SetStateTo(GameStateTypeEnum gs)
        {
            stateTo = gs;
        }

        public void Enter()
        {
            SetStateTo(GameStateTypeEnum.Continue);

            mTime = 0;

            mNeedUpdate = true;
            mNeedScore = true;

            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.GetMapBgAudio((MAPTYPE)GameUserModel.Instance.GameMapID), ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;

            AudioManager.Instance.PlayBgAudio(clip);

            AdvancedGuideCtrl.Instance.Exit();

            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.IntoRoom, OnEvent);
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.IntoLobby, OnEvent);
            EventCenter.AddListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);

        }

        public void Exit()
        {
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.IntoRoom, OnEvent);
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.IntoLobby, OnEvent);
            EventCenter.RemoveListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);

            //GameMethod.DisableAllUI();

            GameMethod.RemoveUI("superwan(Clone)");
            PlayerManager.Instance.LocalPlayer.CleanWhenGameOver();
            HolyTechGameBase.Instance.OpenConnectUI();
            ObjectPool.Instance.Clear();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
        }

        public GameStateTypeEnum Update(float fDeltaTime)
        {
            if (!mNeedUpdate)
                return stateTo;

            mTime += fDeltaTime;
            if (UIGuideModel.Instance.GuideType == GCToCS.AskCSCreateGuideBattle.guidetype.first)
            {
                if (mTime > 6)
                {
                    HolyGameLogic.Instance.EmsgToss_AskReEnterRoom();
                    mNeedUpdate = false;
                    UIGuideCtrl.Instance.GuideBattleType(GCToCS.AskCSCreateGuideBattle.guidetype.other);
                }
            }
            else
            {
                if (mTime > 18)
                {
                    ScoreCtrl.Instance.Exit();
                    HolyGameLogic.Instance.EmsgToss_AskReEnterRoom();
                    mNeedUpdate = false;
                }
                else if (mNeedScore && mTime > 6)
                {
                    ScoreCtrl.Instance.Enter();
                    mNeedScore = false;
                }
            }
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
                        lState.SetFrontScenes(View.EScenesType.EST_Play);
                        SetStateTo(GameStateTypeEnum.Loading);
                    }
                    break;
                case GameEventEnum.IntoRoom:
                    {
                        LoadingState lState = GameStateManager.Instance.getState(GameStateTypeEnum.Loading) as LoadingState;
                        lState.SetNextState(GameStateTypeEnum.Room);
                        lState.SetFrontScenes(View.EScenesType.EST_Play);
                        SetStateTo(GameStateTypeEnum.Loading);
                    }
                    break;
                case GameEventEnum.IntoLobby:
                    {
                        LoadingState lState = GameStateManager.Instance.getState(GameStateTypeEnum.Loading) as LoadingState;
                        lState.SetNextState(GameStateTypeEnum.Lobby);
                        lState.SetFrontScenes(View.EScenesType.EST_Play);
                        SetStateTo(GameStateTypeEnum.Loading);
                    }
                    break;
            }
        }

        private void OnConnectServerFail()
        {
            EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_Disconnect);
        }
    }
}


