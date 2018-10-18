using System;
using UnityEngine;

using GameDefine;
using Thanos.GameEntity;
using System.Collections.Generic;
using System.Linq;
using Thanos.GuideDate;
using Thanos.Resource;
using Thanos.Ctrl;
using Thanos.Model;

namespace Thanos.View
{
    public class SkillWindow : BaseWindow
    {
        public SkillWindow()
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.LoadSkillUI;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            //技能按钮初始化
            Transform Adjust = mRoot.Find("Adjust");
            mBtnArray = new ButtonOnPress[Adjust.childCount]; //容量为8
            for (int i = 0; i < mBtnArray.Length; i++)//i最大值为7
            {
                ButtonOnPress btn = mRoot.Find("Adjust/Button_" + i).GetComponent<ButtonOnPress>();
                mBtnArray[i] = btn;

                //事件注册
                switch ((ShortCutBarBtn)i)
                {
                    case ShortCutBarBtn.BTN_SKILL_1: // 0  button0     中间技能1
                        btn.AddListener(i, OnSkillBtnFunc);//0     目的是向服务器发送技能
                        btn.AddListener(i, OnSkillPress, ButtonOnPress.EventType.PressType);//0 按下状态可以显示节能描述
                        UIGuideCtrl.Instance.AddUiGuideEventBtn(btn.gameObject);//将要触发事件的新手引导的按钮放入列表
                        break;
                    case ShortCutBarBtn.BTN_SKILL_2: // 1  下方技能
                    case ShortCutBarBtn.BTN_SKILL_3: // 2  技能1曝气  狂暴状态下显示
                    case ShortCutBarBtn.BTN_SKILL_4: // 3  技能2曝气  狂暴状态下显示
                    case ShortCutBarBtn.BTN_SKILL_5: // 4   吸附技能
                    case ShortCutBarBtn.BTN_SKILL_6: // 5   吸附技能
                        btn.AddListener(i, OnSkillBtnFunc);// 目的是向服务器发送技能
                        btn.AddListener(i, OnSkillPress, ButtonOnPress.EventType.PressType); //按下状态可以显示节能描述
                        break;
                    case ShortCutBarBtn.BTN_AUTOFIGHT: //6  右下 自动攻击
                    case ShortCutBarBtn.BTN_CHANGELOCK: //7 上方技能
                        btn.AddListener(i, OnCutBarBtnFunc, ButtonOnPress.EventType.ClickType);//如果是6 向服务器请求自动攻击  如果是7 锁定敌人士兵
                        UIGuideCtrl.Instance.AddUiGuideEventBtn(btn.gameObject);//将要触发事件的新手引导的按钮放入列表
                        break;
                }

                //状态控制组件初始化
                if (i < SkillCount)  // skillCount=6
                {
                    GameObject obj = btn.transform.Find("Light").gameObject;
                    mCanPressEffect.Add((ShortCutBarBtn)i, obj);

                    UISprite sprite = mRoot.Find("Adjust/Button_" + i + "/CutBar_" + i).GetComponent<UISprite>();
                    mBtnSprite.Add((ShortCutBarBtn)i, sprite);

                    ButtonSelectPic selectPic = btn.GetComponent<ButtonSelectPic>();
                    selectPic.state = ButtonSelectPic.SelectState.DisableState;
                    mBtnSelectPic.Add((ShortCutBarBtn)i, selectPic);
                }
            }

            //暴气技能初始化
            ResourceItem effectUnit = ResourcesManager.Instance.loadImmediate(GameDefine.GameConstDefine.FurySkillBtnEffect, ResourceType.PREFAB);

            //技能1暴气
            mEffect3 = GameObject.Instantiate(effectUnit.Asset) as GameObject;
            mEffect3.transform.parent = mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_3].transform;
            mEffect3.transform.localPosition = new Vector3(0f, 0f, -10f);

            //技能2暴气
            mEffect4 = GameObject.Instantiate(effectUnit.Asset) as GameObject;
            mEffect4.transform.parent = mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_4].transform;
            mEffect4.transform.localPosition = new Vector3(0f, 0f, -10f);

            ChangeFuryState(EFuryState.eFuryNullState);

            //初始化技能按钮
            ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_1, true);
            ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_2, true);
            ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_3, true);
            ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_4, true);

            if (SceneGuideTaskManager.Instance().IsNewsGuide() == SceneGuideTaskManager.SceneGuideType.NoGuide)
            {
                mBtnSelectPic[ShortCutBarBtn.BTN_SKILL_5].gameObject.SetActive(false);
                mBtnSelectPic[ShortCutBarBtn.BTN_SKILL_6].gameObject.SetActive(false);
                ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_5, false);
                ShowValideUseSkillBtn(ShortCutBarBtn.BTN_SKILL_6, false);
            }

            //CD初始化
            for (int i = 0; i < SkillCount; i++)
            {
                CdCountDown cd = mBtnArray[i].GetComponent<CdCountDown>();
                mCdDownDic.Add((ShortCutBarBtn)i, cd);
            }
            ResetSkill();

            mIsShowDes = false;

            mTimePressStart = 0f;
            mCurSkillPress = -1;

        }

        //窗口控件释放
        protected override void RealseWidget()
        {
            mCanPressEffect.Clear();
            mBtnSprite.Clear();
            mBtnSelectPic.Clear();
            mCdEndEffect.Clear();
            mCdEndEffectTime.Clear();
            mPressBtnEffect.Clear();
            mPressBtnEffectTime.Clear();

            mCdDownDic.Clear();
            mSkill2CDList.Clear();
        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener<EFuryState>((Int32)GameEventEnum.GameEvent_FuryStateInfo, ChangeFuryState);
            EventCenter.AddListener<int, string, bool>((Int32)GameEventEnum.GameEvent_AbsorbResult, OnAbsorbResult);
            EventCenter.AddListener<SkillTypeEnum, float, float>((Int32)GameEventEnum.GameEvent_LocalPlayerSkillCD, OnSkillCountDown);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerInit, OnLocalPlayerInit);
            EventCenter.AddListener<bool>((Int32)GameEventEnum.GameEvent_LocalPlayerSilence, OnLocalPlayerSilence);
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener<EFuryState>((Int32)GameEventEnum.GameEvent_FuryStateInfo, ChangeFuryState);
            EventCenter.RemoveListener<int, string, bool>((Int32)GameEventEnum.GameEvent_AbsorbResult, OnAbsorbResult);
            EventCenter.RemoveListener<SkillTypeEnum, float, float>((Int32)GameEventEnum.GameEvent_LocalPlayerSkillCD, OnSkillCountDown);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerInit, OnLocalPlayerInit);
            EventCenter.RemoveListener<bool>((Int32)GameEventEnum.GameEvent_LocalPlayerSilence, OnLocalPlayerSilence);
        }

        //显示
        public override void OnEnable()
        {

        }

        //隐藏
        public override void OnDisable()
        {
            EventCenter.Broadcast<bool, SkillTypeEnum, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleType, false, SkillTypeEnum.SKILL_NULL, PlayerManager.Instance.LocalPlayer);
            mIsShowDes = false;
            mCurSkillPress = -1;
            mTimePressStart = 0f;
        }

        public override void Update(float deltaTime)
        {
            //清除技能描述
            CloseSkillDestribe();

            //限帧
            mElapseTime += Time.deltaTime;
            if (mElapseTime > 0.2f)
            {
                mElapseTime = 0.0f;

                CheckToDeletePressEffect();
                CheckToDeleteCdEndEffect();
                ShowAllBtnCanUseEffect();
            }

            if (mTimePressStart == 0 || mCurSkillPress == -1)
                return;

            //显示技能描述 mTimePressStart在按下按钮时赋值
            if (Time.time - mTimePressStart >= mTimeLimit)
            {
                EventCenter.Broadcast<bool, SkillTypeEnum, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleType, true, GetSkillType(mCurSkillPress), PlayerManager.Instance.LocalPlayer);

                mIsShowDes = true;
                mTimePressStart = 0f;
                mCurSkillPress = -1;
            }
        }

        //关闭技能描述
        void CloseSkillDestribe()
        {
            bool isUp = false;
            if (Application.platform == RuntimePlatform.Android
                     || Application.platform == RuntimePlatform.IPhonePlayer
                     || Application.platform == RuntimePlatform.WP8Player
                     || Application.platform == RuntimePlatform.BlackBerryPlayer)
            {
                if (Input.touchCount <= 0)
                    return;
                Touch touch = Input.GetTouch(0);
                isUp = (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended);

            }
            else
            {
                isUp = Input.GetMouseButtonUp(0);
            }
            if (isUp)
            {

                EventCenter.Broadcast<bool, SkillTypeEnum, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleType, false, SkillTypeEnum.SKILL_NULL, PlayerManager.Instance.LocalPlayer);

                mCurSkillPress = -1;
                mTimePressStart = 0f;
            }

        }

        //设计技能图片
        public void SetSkillBtnPic(ShortCutBarBtn btntype, string spriteName)
        {
            mBtnSprite[btntype].spriteName = spriteName;

            if (btntype == ShortCutBarBtn.BTN_SKILL_5 || btntype == ShortCutBarBtn.BTN_SKILL_6)
            {
                if (spriteName == "")
                {
                    mBtnSprite[btntype].enabled = false;
                    mBtnSelectPic[btntype].gameObject.SetActive(false);
                    RemoveSkillCountDown(btntype);
                }
                else
                {
                    mBtnSelectPic[btntype].gameObject.SetActive(true);
                    mBtnSprite[btntype].enabled = true;
                }
            }
        }

        public void DisableSkillBtn()
        {
            for (int i = 0; i < SkillCount; i++)
            {
                mBtnSelectPic[(ShortCutBarBtn)i].ShowSelectPic(true);
            }
        }

        public void EnableSkillBtn()
        {
            for (int i = 0; i < SkillCount; i++)
            {
                mBtnSelectPic[(ShortCutBarBtn)i].ShowSelectPic(false);
            }
        }

        public void ChangeFuryState(EFuryState state)
        {
            if (state == EFuryState.eFuryRunState)
            {
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_1].gameObject.SetActive(false);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_2].gameObject.SetActive(false);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_3].gameObject.SetActive(true);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_4].gameObject.SetActive(true);
            }
            else
            {
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_1].gameObject.SetActive(true);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_2].gameObject.SetActive(true);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_3].gameObject.SetActive(false);
                mBtnArray[(int)ShortCutBarBtn.BTN_SKILL_4].gameObject.SetActive(false);
            }
        }

        bool IsHeroSkillBtn(int index)
        {
            if (ShortCutBarBtn.BTN_SKILL_1 == (ShortCutBarBtn)index ||
                ShortCutBarBtn.BTN_SKILL_2 == (ShortCutBarBtn)index ||
                ShortCutBarBtn.BTN_SKILL_3 == (ShortCutBarBtn)index ||
                ShortCutBarBtn.BTN_SKILL_4 == (ShortCutBarBtn)index)
                return true;
            return false;
        }

        private void OnSkillPress(int ie, bool isDown)
        {
            if (isDown)
            {
                mTimePressStart = Time.time;//开始计时
                mCurSkillPress = ie;//技能索引
                mIsShowDes = false;
                return;
            }
           
            //广播技能描述消息  来显示  技能描述  (小谷注释的，感觉没用)
            //EventCenter.Broadcast<bool, SkillType, Iplayer>((Int32)GameEventEnum.GameEvent_SkillDescribleType, false, SkillType.SKILL_NULL, PlayerManager.Instance.LocalPlayer);
            
            mCurSkillPress = -1;
            mTimePressStart = 0f;//计时清零
        }

        //按技能
        private void OnSkillBtnFunc(int ie, bool isDown)//按技能
        {
            if (mIsShowDes)
            {
                mIsShowDes = false;
                return;
            }
            mIsShowDes = false;

            GamePlayCtrl.Instance.showaudiotimeold = System.DateTime.Now;

            if (PlayerManager.Instance.LocalPlayer.FSM == null ||  //状态机是否为空
                PlayerManager.Instance.LocalPlayer.FSM.State == Thanos.FSM.FsmStateEnum.DEAD || //状态是否为死亡状态
                Thanos.Skill.BuffManager.Instance.isSelfHaveBuffType(1017))  //此时是否属于沉默  
            {
                return;
            }
            //向服务器发送消息，准备释放技能
            SendSkill(ie);
        }

        //按功能键
        private void OnCutBarBtnFunc(int ie, bool isDown)
        {
            if (isDown)
            {
                return;
            }
            switch ((ShortCutBarBtn)ie)
            {
                case ShortCutBarBtn.BTN_AUTOFIGHT:
                    HolyGameLogic.Instance.GameAutoFight(); //向服务器请求自动战斗
                    break;
                case ShortCutBarBtn.BTN_CHANGELOCK:
                    OnLockEnemySoldier();                 
                    break;
            }
        }

        //设置技能状态，是普通技能还是怒气技能
        private void SetSkillState(int ie)
        {
            if (!IsHeroSkillBtn(ie))
            {
                mSkillState = SkillState.NormalSkill;
                return;
            }

            if (PlayerManager.Instance.LocalPlayer.FuryState == EFuryState.eFuryRunState)
            {
                mSkillState = SkillState.FurySkill;
            }
            else
            {
                mSkillState = SkillState.NormalSkill;
            }
        }

        //使用技能
        private void SendSkill(int btn)
        {
            //if (PlayerManager.Instance.LocalPlayer.FSM.State == HolyTech.FSM.FsmState.FSM_STATE_DEAD)
            //    return;   //小谷
            SkillTypeEnum type = GetSkillType(btn);
            if (type == SkillTypeEnum.SKILL_NULL) return;       
            //准备释放技能
            PlayerManager.Instance.LocalPlayer.SendPreparePlaySkill(type);
        }

        //快键到技能类型转换
        SkillTypeEnum GetSkillType(int ie)
        {
            SetSkillState(ie);
            SkillTypeEnum type = SkillTypeEnum.SKILL_NULL;
            switch ((ShortCutBarBtn)ie)
            {
                case ShortCutBarBtn.BTN_SKILL_1:
                    type = SkillTypeEnum.SKILL_TYPE1;
                    break;
                case ShortCutBarBtn.BTN_SKILL_2:
                    type = SkillTypeEnum.SKILL_TYPE2;
                    break;
                case ShortCutBarBtn.BTN_SKILL_3:
                    type = SkillTypeEnum.SKILL_TYPE3;
                    break;
                case ShortCutBarBtn.BTN_SKILL_4:
                    type = SkillTypeEnum.SKILL_TYPE4;
                    break;
                case ShortCutBarBtn.BTN_SKILL_5:
                    type = SkillTypeEnum.SKILL_TYPEABSORB1;
                    break;
                case ShortCutBarBtn.BTN_SKILL_6:
                    type = SkillTypeEnum.SKILL_TYPEABSORB2;
                    break;
            }
            return type;
        }

        //快键到技能类型转换
        private ShortCutBarBtn GetBtnType(SkillTypeEnum type)
        {
            ShortCutBarBtn btnType = ShortCutBarBtn.BTN_END;
            switch (type)
            {
                case SkillTypeEnum.SKILL_TYPE1:
                    btnType = ShortCutBarBtn.BTN_SKILL_1;
                    break;
                case SkillTypeEnum.SKILL_TYPE2:
                    btnType = ShortCutBarBtn.BTN_SKILL_2;
                    break;
                case SkillTypeEnum.SKILL_TYPE3:
                    btnType = ShortCutBarBtn.BTN_SKILL_3;
                    break;
                case SkillTypeEnum.SKILL_TYPE4:
                    btnType = ShortCutBarBtn.BTN_SKILL_4;
                    break;
                case SkillTypeEnum.SKILL_TYPEABSORB1:
                    btnType = ShortCutBarBtn.BTN_SKILL_5;
                    break;
                case SkillTypeEnum.SKILL_TYPEABSORB2:
                    btnType = ShortCutBarBtn.BTN_SKILL_6;
                    break;
            }
            return btnType;
        }


        /// <summary>
        /// 锁定目标
        /// </summary>
        /// <param name="ItemList"></param>
        private void OnLockEnemySoldier()
        {
            GameObject mGameObjectLock = mBtnArray[(int)ShortCutBarBtn.BTN_CHANGELOCK].gameObject;//锁定按钮
            List<IEntity> itemList = new List<IEntity>();//目标锁定集合
        
            if (GamePlayGuideModel.Instance.IsGuideTrigger(ButtonTriggerType.mTypeClick, mGameObjectLock))
            {
                //根据距离排序所有敌对阵营的野怪集合
                itemList = GameMethod.GetMonsterEnemyItemListByRadius(PlayerManager.Instance.LocalPlayer, GameConstDefine.PlayerLockTargetDis);
            }
            else
            {
                //根据距离排序所有敌对阵营的小兵集合
                itemList = GameMethod.GetSoldierEnemyItemListByRadius(PlayerManager.Instance.LocalPlayer, GameConstDefine.PlayerLockTargetDis);
            }

            if (itemList == null || itemList.Count == 0) return;
           
            if (mSyncLockSelIndex >= itemList.Count)//当索引大于集合长度时，索引从0再次开始
            {
                mSyncLockSelIndex = 0;
            }

            PlayerManager.Instance.LocalPlayer.SetSyncLockTarget(itemList[mSyncLockSelIndex]);//设置索引对象
            mSyncLockSelIndex++;//当再次点击时，目标自动寻找集合中的下一个
        }

        public void CreateEffect(Transform tran, string pathName)
        {
            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate("effect/ui_effect/" + pathName, ResourceType.PREFAB);
            GameObject obj = objUnit.Asset as GameObject;

            if (obj == null)
            {
                return;
            }

            for (int i = 0; i < tran.childCount; i++)
                GameObject.DestroyImmediate(tran.GetChild(i).gameObject);

            GameObject ShowEffect = GameObject.Instantiate(obj) as GameObject;
            ShowEffect.transform.parent = tran;
            ShowEffect.transform.localPosition = Vector3.zero;
            ShowEffect.transform.localScale = Vector3.one;
        }

        public void ShowValideUseSkillBtn(ShortCutBarBtn btnType, bool visiable)
        {
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
            if (null == player)
            {
                return;
            }
            int index = (int)btnType;
            int skillId = 0;
            SkillManagerConfig info = null;
            switch (btnType)
            {
                case ShortCutBarBtn.BTN_SKILL_1:
                case ShortCutBarBtn.BTN_SKILL_2:
                case ShortCutBarBtn.BTN_SKILL_3:
                case ShortCutBarBtn.BTN_SKILL_4:
                    break;
                case ShortCutBarBtn.BTN_SKILL_5:
                    if (player != null && player.BaseSkillIdDic.ContainsKey(SkillTypeEnum.SKILL_TYPEABSORB1))
                    {
                        skillId = player.BaseSkillIdDic[SkillTypeEnum.SKILL_TYPEABSORB1];
                    }
                    info = ConfigReader.GetSkillManagerCfg(skillId);
                    if (skillId == 0 || info == null)
                    {
                        visiable = false;
                    }
                    break;
                case ShortCutBarBtn.BTN_SKILL_6:
                    if (player != null && player.BaseSkillIdDic.ContainsKey(SkillTypeEnum.SKILL_TYPEABSORB2))
                    {
                        skillId = player.BaseSkillIdDic[SkillTypeEnum.SKILL_TYPEABSORB2];
                    }
                    info = ConfigReader.GetSkillManagerCfg(skillId);
                    if (skillId == 0 || info == null)
                    {
                        visiable = false;
                    }
                    break;
                default:
                    return;
            }
            if (!visiable)
            {
                ShowCdEndEffect(btnType, false);
            }
        }

        public void ShowPressEffect(ShortCutBarBtn btnType)
        {
            if (IsSkillInCd(btnType) || mPressBtnEffect.ContainsKey(btnType))
            {
                return;
            }

            ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.PressBtnEffect, ResourceType.PREFAB);
            GameObject obj = GameObject.Instantiate(objUnit.Asset) as GameObject;

            int index = (int)btnType;
            obj.transform.parent = mBtnArray[index].transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            mPressBtnEffect.Add(btnType, obj);
            mPressBtnEffectTime.Add(btnType, 1.5f);
        }

        private void DeletePressEffect(ShortCutBarBtn btnType)
        {
            GameObject obj = null;
            if (mPressBtnEffect.TryGetValue(btnType, out obj))
            {
                GameObject.DestroyObject(obj);
                mPressBtnEffect.Remove(btnType);
            }
        }

        private float timePressCount = 0f;
        private void CheckToDeletePressEffect()
        {
            if (mPressBtnEffectTime == null || mPressBtnEffectTime.Count == 0) return;
            if (timePressCount == 0)
                timePressCount = Time.time;
            if (Time.time - timePressCount < 0.5f)
            {
                return;
            }
            timePressCount = Time.time;
            for (int i = mPressBtnEffect.Count - 1; i >= 0; i--)
            {
                ShortCutBarBtn type = mPressBtnEffectTime.ElementAt(i).Key;
                mPressBtnEffectTime[type] = mPressBtnEffectTime[type] - 0.5f;
                if (mPressBtnEffectTime[type] <= 0)
                {
                    mPressBtnEffectTime.Remove(type);
                    DeletePressEffect(type);
                }
            }
        }

        public void ShowCdEndEffect(ShortCutBarBtn btnType, bool show)
        {
            GameObject obj = null;
            if (show)
            {
                if (mCdEndEffect.ContainsKey(btnType))
                {
                    return;
                }

                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.CdEndEffect, ResourceType.PREFAB);
                obj = GameObject.Instantiate(objUnit.Asset) as GameObject;


                int index = (int)btnType;
                obj.transform.parent = mBtnArray[index].transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                mCdEndEffect.Add(btnType, obj);
                mCdEndEffectTime.Add(btnType, 1.5f);
            }
            else
            {
                if (!mCdEndEffect.ContainsKey(btnType))
                {
                    return;
                }
                obj = null;
                if (mCdEndEffect.TryGetValue(btnType, out obj))
                {
                    GameObject.DestroyObject(obj);
                }
                mCdEndEffect.Remove(btnType);
                mCdEndEffectTime.Remove(btnType);
            }
        }

        private float timeCount = 0f;
        private void CheckToDeleteCdEndEffect()
        {
            if (mCdEndEffect == null || mCdEndEffect.Count == 0) return;
            if (timeCount == 0)
                timeCount = Time.time;
            if (Time.time - timeCount < 0.5f)
            {
                return;
            }
            timeCount = Time.time;
            for (int i = mCdEndEffect.Count - 1; i >= 0; i--)
            {
                ShortCutBarBtn type = mCdEndEffectTime.ElementAt(i).Key;
                mCdEndEffectTime[type] = mCdEndEffectTime[type] - 0.5f;
                if (mCdEndEffectTime[type] <= 0)
                {
                    ShowCdEndEffect(type, false);
                }
            }
        }

        private void ShowBtnCanUseEffect(ShortCutBarBtn type)
        {
            if (PlayerManager.Instance == null || PlayerManager.Instance.LocalPlayer == null)
                return;
            if (PlayerManager.Instance.LocalPlayer.SkillIdDic == null || PlayerManager.Instance.LocalPlayer.SkillIdDic.Count == 0)
                return;
            ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
            SkillTypeEnum skillType = GetSkillType((int)type);
            int skillId = PlayerManager.Instance.LocalPlayer.SkillIdDic[skillType];
            SkillManagerConfig info = ConfigReader.GetSkillManagerCfg(skillId);
            if (info == null)
                return;
            GameObject sprite = mCanPressEffect[type];
            bool isInCd = IsSkillInCd(type);
            if (info.mpUse > player.Mp || info.cpUse > player.Cp || info.hpUse > player.Hp || isInCd)
            {
                if (sprite.activeInHierarchy)
                {
                    sprite.SetActive(false);
                }
            }
            else
            {
                if (!sprite.activeInHierarchy)
                {
                    sprite.SetActive(true);
                }
            }
        }

        private void ShowAllBtnCanUseEffect()
        {
            for (int i = 0; i < SkillCount; i++)
            {
                ShowBtnCanUseEffect((ShortCutBarBtn)i);
            }
        }

        // 开始技能CD显示，参数：按钮槽类型，总CD时间，CD时间；返回true 成功，否则 技能正在CD中
        public bool StartSkillCountDown(ShortCutBarBtn btnType, float totalTime, float lastTime)
        {
            CdCountDown cd;
            if (IsSkillInCd(btnType))
            {
                return false;
            }
            if (mCdDownDic.TryGetValue(btnType, out cd) && lastTime > 0)
            {
                ShowValideUseSkillBtn(btnType, false);
                ShowPressEffect(btnType);
                cd.StartCdCountDown(totalTime, lastTime);
                cd.CdCountDownEvent += SkillCdEnd;
                mSkill2CDList.Add(btnType);
            }
            return true;
        }

        //清除某个技能CD
        public void RemoveSkillCountDown(ShortCutBarBtn type)
        {
            CdCountDown cd;
            if (mCdDownDic.TryGetValue(type, out cd) && IsSkillInCd(type))
            {
                cd.EndCdCountDown();
                RemoveCdList(type);
                ShowValideUseSkillBtn(type, true);
            }
        }


        //清除所有技能CD
        public void ResetSkill()
        {
            foreach (var item in mCdDownDic)
            {
                item.Value.EndCdCountDown();
                ShowValideUseSkillBtn(item.Key, true);
            }
            for (int i = mSkill2CDList.Count - 1; i >= 0; i--)
            {
                RemoveCdList(mSkill2CDList.ElementAt(i));
            }
        }

        public void ResetSkillAsignedSkill(ShortCutBarBtn btnType)
        {
            CdCountDown cdDown = null;
            if (mCdDownDic.TryGetValue(btnType, out cdDown) && IsSkillInCd(btnType))
            {
                cdDown.EndCdCountDown();
                RemoveCdList(btnType);
                ShowValideUseSkillBtn(btnType, true);
            }
        }

        // 监听某个技能CD结束
        private void SkillCdEnd(CdCountDown cd)
        {
            for (int i = mCdDownDic.Count - 1; i >= 0; i--)
            {
                if (mCdDownDic.ElementAt(i).Value == cd && IsSkillInCd(mCdDownDic.ElementAt(i).Key))
                {
                    ShowValideUseSkillBtn(mCdDownDic.ElementAt(i).Key, true);
                    RemoveCdList(mCdDownDic.ElementAt(i).Key);
                    break;
                }
            }
        }

        //判断某个技能是否在CD中
        public bool IsSkillInCd(ShortCutBarBtn type)
        {
            return mSkill2CDList.Contains(type);
        }

        private void RemoveCdList(ShortCutBarBtn type)
        {
            if (!IsSkillInCd(type))
                return;
            ShowCdEndEffect(type, true);
            mSkill2CDList.Remove(type);
        }

        private void OnAbsorbResult(int slot, string spriteName, bool remove)
        {
            if (remove == false)
            {
                if (slot == 0)
                {
                    CreateEffect(mBtnSprite[ShortCutBarBtn.BTN_SKILL_5].transform, "soul_01_burst");
                }
                else
                {
                    CreateEffect(mBtnSprite[ShortCutBarBtn.BTN_SKILL_6].transform, "soul_02_burst");
                }
            }

            SetSkillBtnPic((ShortCutBarBtn.BTN_SKILL_5 + slot), spriteName);//show absorb btn sprite
            ShowValideUseSkillBtn((ShortCutBarBtn.BTN_SKILL_5 + slot), !remove);//show absorb btn
        }

        private void OnSkillCountDown(SkillTypeEnum type, float CDTime, float time)
        {
            if (time > 0)
            {
                StartSkillCountDown(GetBtnType(type), CDTime, time);
            }
            else
            {
                ResetSkillAsignedSkill(GetBtnType(type));
            }
        }

        private void OnLocalPlayerInit()
        {
            int id = (int)PlayerManager.Instance.LocalPlayer.ObjTypeID;
            HeroConfigInfo heroInfo = ConfigReader.GetHeroInfo(id);

            if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType1) != null)
            {
                SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_1, ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType1).skillIcon);
            }
            if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType2) != null)
            {
                SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_2, ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType2).skillIcon);
            }
            if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType3) != null)
            {
                SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_3, ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType3).skillIcon);
            }
            if (ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType4) != null)
            {
                SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_4, ConfigReader.GetSkillManagerCfg(heroInfo.HeroSkillType4).skillIcon);
            }
            SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_5, "");
            SetSkillBtnPic(ShortCutBarBtn.BTN_SKILL_6, "");
        }

        private void OnLocalPlayerSilence(bool val)
        {
            if (val)
            {
                DisableSkillBtn();
            }
            else
            {
                EnableSkillBtn();
            }
        }


        //技能按钮索引
        public enum ShortCutBarBtn
        {
            BTN_SKILL_1 = 0,    //0 技能1
            BTN_SKILL_2,        //1 技能2
            BTN_SKILL_3,        //2 技能1暴气
            BTN_SKILL_4,        //3 技能2暴气
            BTN_SKILL_5,        //4 吸附技能1
            BTN_SKILL_6,        //5 吸附技能2
            BTN_AUTOFIGHT,      //6 自动攻击
            BTN_CHANGELOCK,     //7 改变锁定
            BTN_END,
        }

        private enum SkillState
        {
            NormalSkill,
            FurySkill,
        }

        private ButtonOnPress[] mBtnArray;       //技能按钮列表
        private Dictionary<ShortCutBarBtn, GameObject> mCanPressEffect = new Dictionary<ShortCutBarBtn, GameObject>();   //可点击特效列表
        private Dictionary<ShortCutBarBtn, UISprite> mBtnSprite = new Dictionary<ShortCutBarBtn, UISprite>();            //技能按钮图片列表
        private Dictionary<ShortCutBarBtn, ButtonSelectPic> mBtnSelectPic = new Dictionary<ShortCutBarBtn, ButtonSelectPic>();  //技能选择图片列表
        private Dictionary<ShortCutBarBtn, GameObject> mCdEndEffect = new Dictionary<ShortCutBarBtn, GameObject>();
        private Dictionary<ShortCutBarBtn, float> mCdEndEffectTime = new Dictionary<ShortCutBarBtn, float>();
        private Dictionary<ShortCutBarBtn, GameObject> mPressBtnEffect = new Dictionary<ShortCutBarBtn, GameObject>();
        private Dictionary<ShortCutBarBtn, float> mPressBtnEffectTime = new Dictionary<ShortCutBarBtn, float>();
        private Dictionary<ShortCutBarBtn, CdCountDown> mCdDownDic = new Dictionary<ShortCutBarBtn, CdCountDown>();
        private List<ShortCutBarBtn> mSkill2CDList = new List<ShortCutBarBtn>();

        private GameObject mEffect3;
        private GameObject mEffect4;

        private float mTimePressStart = 0f;
        private const float mTimeLimit = 0.5f; //原先是1.2
        private int mCurSkillPress = -1;
        private int mSyncLockSelIndex;
        private const int SkillCount = 6;
        private float mElapseTime = 0.0f;
        private bool mIsShowDes = false;//是否显示技能描述面板

        SkillState mSkillState = SkillState.NormalSkill;
    }
}

