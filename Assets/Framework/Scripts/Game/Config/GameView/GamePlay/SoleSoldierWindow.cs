using System;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using System.Collections.Generic;
using Thanos.Ctrl;

namespace Thanos.View
{
    public class SoleSoldierWindow : BaseWindow
    {
        public SoleSoldierWindow()
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.UISoulsReplace;
            mResident = true;
        }

        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SoleSoldierEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SoleSoldierExit, Hide);
        }

        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SoleSoldierEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SoleSoldierExit, Hide);
        }

         //窗口控件初始化
        protected override void InitWidget()
        {
            SoldierSelectBtn = new List<GameObject>();
            AltarHeadPhoto = new List<UISprite>();
            SoldierSelectBtn.Add(mRoot.Find("Postion/HeadShow/SoldierSelect/Soldier1").gameObject);
            SoldierSelectBtn.Add(mRoot.Find("Postion/HeadShow/SoldierSelect/Soldier2").gameObject);
            AltarHeadPhoto.Add(mRoot.Find("Postion/HeadShow/LastSelect/HalfPhoto").GetComponent<UISprite>());
            AltarHeadPhoto.Add(SoldierSelectBtn[1].transform.Find("HalfPhoto").GetComponent<UISprite>());
            AltarHeadPhoto.Add(SoldierSelectBtn[0].transform.Find("HalfPhoto").GetComponent<UISprite>());
            for (int id = 0; id < SoldierSelectBtn.Count; id++)
            {
                UIEventListener.Get(SoldierSelectBtn[id]).onClick += OnSoldierSelectFunc;
            }
            GameObject BtnClose = mRoot.Find("Postion/CtrlBtn").gameObject;
            UIEventListener.Get(BtnClose).onClick += OnUiCloseFunc;
        }

        private void OnSoldierSelectFunc(GameObject go)
        {
            ISelfPlayer pl = PlayerManager.Instance.LocalPlayer;
            int npcType = -1;
            if (go == SoldierSelectBtn[0] && pl.Level >= 7)
            {
                npcType = 1;
                HolyGameLogic.Instance.EmsgToss_AskAbsorb(npcType);
            }
            if (go == SoldierSelectBtn[1])
            {
                npcType = 0;
                HolyGameLogic.Instance.EmsgToss_AskAbsorb(npcType);
            }
            DestroySole();
        }


        void ResetLockHead(string head)
        {
            //注释新的头相
            if (head != null)
            {
                AltarHeadPhoto[0].spriteName = head;
            }
            else
            {
                AltarHeadPhoto[0].spriteName = "0";
            }
            DestroySole();
        }
        void ResetAbsHead(int newMonsterID1, int newMonsterID2)
        {
            //注释更新头相
            NpcConfigInfo info = ConfigReader.GetNpcInfo(newMonsterID1);
            if (info != null)
                AltarHeadPhoto[1].spriteName = info.HeadPhoto.ToString();
            info = ConfigReader.GetNpcInfo(newMonsterID2);
            if (info != null)
                AltarHeadPhoto[2].spriteName = info.HeadPhoto.ToString();
            if (this != null)
                DestroySole();
        }
        public void DestroySole()
        {
            VirtualStickUI.Instance.SetVirtualStickUsable(true);
            SoleSoldierCtrl.Instance.Exit();
        }
        private void ShowAltarSelectPic()
        {
            ISelfPlayer pl = PlayerManager.Instance.LocalPlayer;
            if (pl == null)
                return;
            VirtualStickUI.Instance.SetVirtualStickUsable(false);
            for (int i = 0; i < 2; i++)
            {
                if (pl.AbsorbMonsterType[i] != 0)
                {
                    AltarHeadPhoto[1 + i].gameObject.SetActive(true);
                    AltarHeadPhoto[1 + i].spriteName = ConfigReader.GetNpcInfo((int)pl.AbsorbMonsterType[i]).HeadPhoto.ToString();
                }
            }

            AltarHeadPhoto[0].gameObject.SetActive(true);
            AltarHeadPhoto[0].spriteName = ConfigReader.GetNpcInfo(pl.SyncLockTarget.NpcGUIDType).HeadPhoto.ToString();
        }

        private void OnSoldierSelectFunc(int ie, bool presses)
        {
            if (presses)
            {
                return;
            }
            ISelfPlayer pl = PlayerManager.Instance.LocalPlayer;
            int npcType = -1;
            if ((AltarSelectBtn)ie == AltarSelectBtn.SelectFirst && pl.Level >= 7)
            {
                npcType = 1;
                HolyGameLogic.Instance.EmsgToss_AskAbsorb(npcType);
            }
            if ((AltarSelectBtn)ie == AltarSelectBtn.SelectSecond)
            {
                npcType = 0;
                HolyGameLogic.Instance.EmsgToss_AskAbsorb(npcType);
            }
            DestroySole();
            //HolyTechUI.Instance.OnDestroyUI(this.transform.parent.gameObject);
        }

        private void OnUiCloseFunc(GameObject go)
        {
            DestroySole();
            //HolyTechUI.Instance.OnDestroyUI(this.transform.parent.gameObject);
        }

        //窗口控件释放
        protected override void RealseWidget()
        {

        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener<int, int>((Int32)GameEventEnum.GameEvent_ResetAbsHead, ResetAbsHead);//兵营设置更新吸收头像
            EventCenter.AddListener<string>((Int32)GameEventEnum.GameEvent_ResetLockHead, ResetLockHead);
        }

         //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener<int, int>((Int32)GameEventEnum.GameEvent_ResetAbsHead, ResetAbsHead);
            EventCenter.RemoveListener<string>((Int32)GameEventEnum.GameEvent_ResetLockHead, ResetLockHead);
        }

        //显示
        public override void OnEnable()
        {
            ShowAltarSelectPic();
        }

        //隐藏
        public override void OnDisable()
        {

        }

        private List<GameObject> SoldierSelectBtn;
        private List<UISprite> AltarHeadPhoto;
        enum AltarSelectBtn
        {
            SelectFirst,
            SelectSecond,
        }
    }
}