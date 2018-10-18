using UnityEngine;
using System.Collections;
using GameDefine;
using Thanos.GameData;
using Thanos.GameEntity;
using Thanos.GuideDate;
using Thanos.Resource;
using Thanos.Model;
using Thanos.Ctrl;
using Thanos.View;
using System;
using System.Linq;

namespace Thanos.GameState
{
    class PlayState : IGameState
    {
        GameStateTypeEnum stateTo;

        GameObject mScenesRoot;
        GameObject mUIRoot;

        //地图相机类型
        public int mCameraType = 1;
        public int ShopID = 0;

        public PlayState()
        {
        }

        public GameStateTypeEnum GetStateType()
        {
            return GameStateTypeEnum.Play;
        } 

        public void SetStateTo(GameStateTypeEnum gs)
        {
            stateTo = gs;
        }

        public void Enter()
        {
            SetStateTo(GameStateTypeEnum.Continue);
          
            IGuideMidMatchTip.Instance.RegisterListerner();

            //LocalAccount是Iplayer类型的
            switch (PlayerManager.Instance.LocalAccount.ObjType)
            {
                case ObPlayerOrPlayer.PlayerType:
                    GamePlayCtrl.Instance.Enter(); // 发送消息 ，消息类型GameEventEnum_GamePlayEnter  事件 Show（五大窗口）
                    break;
                case ObPlayerOrPlayer.PlayerObType:// 加载UIviewer 
                    //mUIRoot = LoadUiResource.LoadRes(GameMethod.GetUiCamera.transform, GameConstDefine.LoadGameViewer);
                    break;
                  
            }
            //游戏开始  游戏Bulidings创建
            HolyTechGameBase.Instance.PlayStart(); 
            //向服务器发送消息   消息类型 LoadComplete
            HolyGameLogic.Instance.AskLoadComplete();
           
            //音频资源
            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.GetMapBgAudio((MAPTYPE)GameUserModel.Instance.GameMapID), ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;
            AudioManager.Instance.PlayBgAudio(clip);
            
            //正常流程
            EventCenter.AddListener<UInt64>((Int32)GameEventEnum.GameOver, OnGameOver);//状态转换到GS_Over
            //新手引导流程
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);
            EventCenter.AddListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);
            EventCenter.AddListener((Int32)GameEventEnum.ReconnectToBatttle, OnReconnectToBatttle);       
            //获取地图信息
            GetMapType();
        }

        public void GetMapType()
        {
            //引导类型
            if (UIGuideModel.Instance.GuideType == GCToCS.AskCSCreateGuideBattle.guidetype.first)
            {
                UIGuideCtrl.Instance.Enter();
                GamePlayGuideModel.Instance.NowTaskId = 90004;
                EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayTaskModelFinish);
            }
            else if (UIGuideModel.Instance.GuideType == GCToCS.AskCSCreateGuideBattle.guidetype.second)
            {
                AdvancedGuideCtrl.Instance.Enter();
                UIGuideCtrl.Instance.GuideBattleType(GCToCS.AskCSCreateGuideBattle.guidetype.other);
            }
            //获取当前游戏地图数据
            string levelName = Application.loadedLevelName;
            MapInfo mapInfo  = MapLoadConfig.Instance.GetMapInfo(levelName);

            if (mapInfo == null)
                Debug.LogError("can not find levelName in MapLoadCfg.xml!");

            mCameraType = mapInfo.mCameraType;

            GamePlayCtrl.Instance.SetShopID(mapInfo.mShopID);

        }

        public void Exit()
        {
            EventCenter.RemoveListener<UInt64>((Int32)GameEventEnum.GameOver, OnGameOver);

            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.Loading, OnEvent);

            EventCenter.RemoveListener((Int32)GameEventEnum.ConnectServerFail, OnConnectServerFail);

            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);

            EventCenter.RemoveListener((Int32)GameEventEnum.ReconnectToBatttle, OnReconnectToBatttle);     

            LoadUiResource.DestroyLoad(mUIRoot);

            GamePlayCtrl.Instance.Exit();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {

        }

        public GameStateTypeEnum Update(float fDeltaTime)
        {
            return stateTo;
        }

        private void OnGameOver(UInt64 BaseGuid)
        {
            PlayFinishVedio(BaseGuid);

            GameStateManager.Instance.ChangeGameStateTo(GameStateTypeEnum.Over);

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
            }
        }

        private void OnConnectServerFail()
        {
            EventCenter.Broadcast<EMessageType>((Int32)GameEventEnum.GameEvent_ShowMessage, EMessageType.EMT_Reconnect);
        }

        private void SdkLogOff()
        {
            GameMethod.LogOutToLogin();

            LoadingState lState = GameStateManager.Instance.getState(GameStateTypeEnum.Loading) as LoadingState;
            lState.SetNextState(GameStateTypeEnum.Login);
            lState.SetFrontScenes(View.EScenesType.EST_Play);
            SetStateTo(GameStateTypeEnum.Loading);
        }

        private void OnReconnectToBatttle()
        {
            HolyGameLogic.Instance.EmsgToss_AskEnterBattle(GameUserModel.Instance.GameBattleID);
        }

        //播放特效
        void PlayFinishVedio(UInt64 ObjID)
        {
            Thanos.AudioManager.Instance.StopHeroAudio();
            GameTimeData.Instance.EndCountTime();
            ProgressBarInterface.hideProgressBar();
           
			DeathCtrl.Instance.Exit();
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player != null)
            {
                player.RemoveRuinWarning();
            }

            GameMethod.CreateWindow(GameConstDefine.GameOverBoxPath, Vector3.zero, GameMethod.GetUiCamera.transform);
            if (Camera.main != null && Camera.main.gameObject != null)
            {
                Camera.main.gameObject.AddComponent<BaseDaBomb>();
            }
            GameMethod.GetMainCamera.target = null;
            GameMethod.GetMainCamera.enabled = false;

            //ToReview wincamp没用上
            UInt64 sGUID;
            sGUID = ObjID;
            IEntity entity = EntityManager.Instance.GetEntity(sGUID);

            for (int i = EntityManager.AllEntitys.Count - 1; i >= 0; i--)
            {
                var item = EntityManager.AllEntitys.ElementAt(i);
                if (item.Value.RealEntity != null)
                {
                    item.Value.RealEntity.PlayerFreeAnimation();
                    item.Value.RealEntity.SyncEntity = null;
					item.Value.RealEntity.enabled = false;
                }
                if (item.Value.entityType == EntityTypeEnum.Player || item.Value.entityType == EntityTypeEnum.Building)
                    continue;
                EntityManager.AllEntitys.Remove(item.Key);
            }

            if (entity != null)
            {
                BaseDaBomb.Instance.SetBaseBomb(true, entity, GameUserModel.Instance.GameMapID);    //ToReview int->uint
            }

            AltarData.Instance.DelAllAltar();
            BattleingData.Instance.ClearAllGoods();
            BattleingData.Instance.ClearAllBattleData();
        }
    }
}


