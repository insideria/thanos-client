using UnityEngine;
using GameDefine;
using Thanos.Ctrl;
using System;
using Thanos.GameEntity;
using Thanos.Resource;
using System.Collections.Generic;
using Thanos.GameData;

namespace Thanos.View
{
    public class GamePlayWindow : BaseWindow
    {
        public GamePlayWindow()
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.LoadGameMainUI;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化

        public override void Init()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Exit);
        }

        void Exit() {
            mResident = false;
            Hide();
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayEnter, Show);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Exit);
        }

         //窗口控件初始化
        protected override void InitWidget()
        {
            //游戏记录区
            GamePlayCtrl.Instance.showaudiotimeold = System.DateTime.Now;
            CPLabel = mRoot.Find("GameRecord/Panel/CP/CpLabel").GetComponent<UILabel>();//金币
            TimeLabel = mRoot.Find("GameRecord/Panel/Time/TimeLabel").GetComponent<UILabel>();//时间

            DeadLabel = mRoot.Find("GameRecord/Panel/Dead/DeadLabel").GetComponent<UILabel>();//击杀数
            KillLabel = mRoot.Find("GameRecord/Panel/Kill/KillLabel").GetComponent<UILabel>();//死亡数
            AssistLabel = mRoot.Find("GameRecord/Panel/Assists/AssistsLabel").GetComponent<UILabel>();//助攻数
            AssistLabel.text = "0";
            DeadLabel.text = "0";
            KillLabel.text = "0";
            spriteHomeBaseA = mRoot.Find("GameRecord/Panel/ElfLifeBar/xts").GetComponent<UISprite>();//基地A的显示条（蓝）
            spriteHomeBaseB = mRoot.Find("GameRecord/Panel/UndeadLifeBar/xts").GetComponent<UISprite>();//基地b的显示条（红）
            
            //战斗信息
            BattleInfo = mRoot.Find("ExtraButton/Adjust/InfoBtn").gameObject;//玩家及战友战况
            ShopInfo = mRoot.Find("ExtraButton/Adjust/ShopBtn").gameObject;  //商店按钮
            BackToCity = mRoot.Find("BackToCity/Adjust").gameObject;//回城按钮
            UIGuideCtrl.Instance.AddUiGuideEventBtn(ShopInfo);
            UIEventListener.Get(BattleInfo).onClick += BattlePlay;
            UIEventListener.Get(ShopInfo).onClick += ShopBtn;      
            UIEventListener.Get(BackToCity).onClick += BackToCityBtn;


            //LockHead 
            mLockRoot = mRoot.Find("LockPhoto/Adjust/Head");
            spriteLock = mRoot.Find("LockPhoto/Adjust/Head").GetComponent<UISprite>();
            spriteLockBg1 = spriteLock.transform.Find("BG1").GetComponent<UISprite>();
            spriteLockBg2 = spriteLock.transform.Find("BG2").GetComponent<UISprite>();
            spriteLockHp = mRoot.Find("LockPhoto/Adjust/Head/HP").GetComponent<UISprite>();
            spriteLockMp = mRoot.Find("LockPhoto/Adjust/Head/MP").GetComponent<UISprite>();
            mLockPos = mLockRoot.localPosition;
            mLockRoot.localPosition = new Vector3(5000, 0, 0);
            UIGuideCtrl.Instance.AddUiGuideEventBtn(spriteLock.gameObject);
            UIEventListener.Get(spriteLock.gameObject).onClick += AbsorbLockHead;
          
            //英雄Info
            sliderHp = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/bar red/Foreground").GetComponent<UISprite>();//HP背景
            sliderMp = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/bar blue/Foreground").GetComponent<UISprite>();//MP背景
            sliderExp = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/bar yellow/Foreground").GetComponent<UISprite>();//经验值背景

            labelHp = sliderHp.transform.parent.Find("Label").GetComponent<UILabel>();//HP 文本
            labelMp = sliderMp.transform.parent.Find("Label").GetComponent<UILabel>();//MP文本
            labelExp = sliderExp.transform.parent.Find("Label").GetComponent<UILabel>();//经验值文本
            labelLevel = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/level_Label").GetComponent<UILabel>();//英雄等级文本

            spriteHead = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/touxiang_Sprite").GetComponent<UISprite>();//头像
            spritePing = new UISprite[3];

            spritePing[(int)PingEnum.GreenTag] = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/PingIcon/GreenIcon").GetComponent<UISprite>();//链接状态图标
            spritePing[(int)PingEnum.YellowTag] = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/PingIcon/YellowIcon").GetComponent<UISprite>();//链接状态图标
            spritePing[(int)PingEnum.RedTag] = mRoot.Find("UI_AvatarInfos/Adjust/Avatar/PingIcon/RedIcon").GetComponent<UISprite>();//链接状态图标
            for (int i = 0; i < spritePing.Length; i++)
            {
                spritePing[i].gameObject.SetActive(false);
            }
            isPress = false;
            sliderExp.fillAmount = 0f;
            mPassiveSkill = mRoot.Find("UI_AvatarInfos/Adjust/Button_7/CutBar_7").GetComponent<UISprite>();//被动技能
            mSpritePassiveSkill = mRoot.Find("UI_AvatarInfos/Adjust/Button_7/Foreground").GetComponent<UISprite>();
            mSpritePassiveInfo = mRoot.Find("UI_AvatarInfos/Adjust/Button_7/Overlay").GetComponent<UILabel>();
            UIEventListener.Get(mPassiveSkill.transform.parent.gameObject).onPress += mPassivePress;

            mAltarSoldierHeadPoint = mRoot.Find("AltarManager");

            sliderFury = mRoot.Find("UI_AvatarInfos/Adjust/EnergyPool/FurySlider").GetComponent<UISprite>();//狂暴条（在玩家头像下方，弯月形）
            btnFury = sliderFury.transform.parent.gameObject;
            UIEventListener.Get(btnFury).onClick += OnFuryClick;
            UIGuideCtrl.Instance.AddUiGuideEventBtn(btnFury.gameObject); 
        }
        /// //////////////////////////////////////////UI响应//////////////////////////////////////////////

        private void OnFuryClick(GameObject go)
        {
            if (PlayerManager.Instance.LocalPlayer.FuryState == EFuryState.eFuryNullState)
            {
                MsgInfoManager.Instance.ShowMsg((int)ErrorCodeEnum.FuryNotEnough);
                return;
            }

            if (PlayerManager.Instance.LocalPlayer.FuryState == EFuryState.eFuryRunState)
            {
                MsgInfoManager.Instance.ShowMsg((int)ErrorCodeEnum.FuryRuning);
                return;
            }

            if (PlayerManager.Instance.LocalPlayer.FSM == null ||
               PlayerManager.Instance.LocalPlayer.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
            {
                return;
            }
            HolyGameLogic.Instance.AskGasExplosion();//请求爆气  ? ?
        }

        DateTime statePress;
        private void mPassivePress(GameObject go, bool state)
        {
            statePress = DateTime.Now;
            if (state)
            {
                isPress = true;
            }
            else
            {
                isPress = false;
                UnityEngine.GameObject.DestroyImmediate(desObj);
                desObj = null;
            }
        }

        //打开商店
        private void ShopBtn(GameObject go)
        {
            GamePlayCtrl.Instance.OpenShop();
        }


        //回城
        private void BackToCityBtn(GameObject go)
        {
            //获取技能
            int backSk = ConfigReader.HeroXmlInfoDict[PlayerManager.Instance.LocalPlayer.NpcGUIDType].HeroSkillType6;
            //向服务器请求使用技能
            GamePlayCtrl.Instance.AskUseSkill((UInt32)backSk); 
        }

        private void AbsorbLockHead(GameObject go)
        {
            //是否打开吸附界面
            PlayerManager.Instance.LocalPlayer.SendAbsortMonster();
        }

        //战斗信息
        private void BattlePlay(GameObject go)
        {
            //向服务器请求战斗信息
            GamePlayCtrl.Instance.AskBattleInfo();
        }



        //  ///////////////////////////////////////////////////////UI响应结束////////////////////////////

        GameObject desObj = null;
        //显示技能描述面板        
        void ShowDes()
        {
            if (statePress != DateTime.Now)
            {
                if (desObj == null)
                {
                    //根据英雄获取被动技能信息
                    IPlayer player = PlayerManager.Instance.LocalPlayer;
                    if(player == null)
                        return;

                    int skillId = GetPassSkill(0, player);
                    SkillPassiveConfigInfo skillInfo = ConfigReader.GetSkillPassiveConfig((uint)skillId);
                    if (skillInfo == null)
                    {
                        Debug.LogError("skillPassive is null");
                        return;
                    }
                    //面板的生成与设置
                    ResourceItem desObjUnit = ResourcesManager.Instance.loadImmediate(GameDefine.GameConstDefine.SkillDestribe, ResourceType.PREFAB);
                    desObj = GameObject.Instantiate(desObjUnit.Asset) as GameObject;
                    desObj.transform.parent = mPassiveSkill.transform;
                    desObj.transform.localScale = Vector3.one;
                    desObj.transform.localPosition = new Vector3(550, -450, 0);
                    UnityEngine.GameObject.DestroyImmediate(desObj.GetComponent<UIAnchor>());
                    //技能信息的获取与显示
                    UILabel skillCd = desObj.transform.Find("Skill_Cooldown").GetComponent<UILabel>();
                    UILabel skillDes = desObj.transform.Find("Skill_Describe").GetComponent<UILabel>();
                    UILabel skillLv = desObj.transform.Find("Skill_Level").GetComponent<UILabel>();
                    UILabel skillName = desObj.transform.Find("Skill_Name").GetComponent<UILabel>();
                    UILabel skillMpCost = desObj.transform.Find("Skill_HP").GetComponent<UILabel>();

                    if (skillInfo.isShowCoolTime)
                        skillCd.text = (skillInfo.coolTime / 1000f).ToString();
                    else
                        skillCd.text = "0";
                    skillDes.text = DestribeWithAttribue(skillInfo.info, player);
                    int bet = (int)skillInfo.id % 10;
                    if (bet == 0)
                        bet = 1;
                    skillLv.text = bet.ToString();
                    skillName.text = skillInfo.name;
                    if (skillInfo.Mp != 0)
                        skillMpCost.text = skillInfo.Mp.ToString();
                    skillMpCost.transform.Find("Label").GetComponent<UILabel>().gameObject.SetActive(skillInfo.Mp != 0);
                }
            }
        }

        string DestribeWithAttribue(string str, IPlayer player)
        {
            string tempStr = "";
            tempStr = str;
            if (!(str.Contains("mag") || str.Contains("phy")))
                return str;
            for (int i = 0; i < tempStr.Length; i++)
            {
                if (tempStr[i] != ']')
                {
                    continue;
                }
                int index = tempStr.LastIndexOf('[', i);
                string addStr = tempStr.Substring(index, i - index + 1);
                string[] strArray;
                if (addStr.Contains("mag") || addStr.Contains("phy"))
                {
                    strArray = addStr.Split(',');
                    strArray[1] = strArray[1].Remove(strArray[1].Length - 1, 1);
                    float attr = float.Parse(strArray[1]);
                    if (player == null)
                    {
                        return null;
                    }
                    float phyAttr = player.PhyAtk;
                    float magAttr = player.MagAtk;
                    attr = addStr.Contains("mag") ? (attr * magAttr) : (attr * phyAttr);
                    string attrAdd = attr >= 0 ? ("+" + attr.ToString()) : ("-" + attr.ToString());
                    attrAdd = "[00FF00]" + attrAdd + "[-]";
                    tempStr = tempStr.Replace(addStr, attrAdd);
                }

            }
            return tempStr;
        }


        //窗口控件释放
        protected override void RealseWidget()
        {

        }
        public override void Update(float deltaTime)
        {
            UpdateHomeHp();//更新AB阵营的HP
            CheckHomeBaseEffectEnd();
            if (isPress)
            {
                ShowDes();//显示被动技能描述面板
            }
            deltaTime += Time.deltaTime;
            if (runSlowDown && sliderFury != null && sliderFury.fillAmount != 0)
            {
                float cut = 1f / slowDownLimit;
                if (Time.time - startTime >= 1f)
                {
                    if (sliderFury.fillAmount - cut >= 0f)
                        sliderFury.fillAmount -= cut;
                    else
                        sliderFury.fillAmount = 0f;
                    startTime = Time.time;
                }
            }
            else
            {
                runSlowDown = false;
            }
            if (GameTimeData.Instance.GetStartTag())
            {
                //获取到游戏进行的时间（时 分 秒）
                TimeSpan span = GameTimeData.Instance.GetGameTime();
                int sHour = span.Hours;
                int sMin = span.Minutes;
                int sSec = span.Seconds;
                TimeLabel.text = GameMethod.GetCurrSystemTime(sHour, sMin, sSec);//规范时间格式  00：00
                deltaTime = 0.0f;
            }
            if (isDownTime)
            {
                LastTime = DateTime.Now;
                TimeSpan mSpan = LastTime - StartTime;
                int currTime = 0;
                if (mSpan.Hours != 0)
                {
                    currTime += mSpan.Hours * 3600;
                }
                if (mSpan.Minutes != 0)
                {
                    currTime += mSpan.Minutes * 60;
                }
                currTime += mSpan.Seconds;

                if (currTime <= skillEndTime)
                {
                    mSpritePassiveSkill.fillAmount = 1 - (float)currTime/(float)skillEndTime;
                }
                else
                {
                    isDownTime = false;
                }
            }
        }

        private void CheckHomeBaseEffectEnd()
        {
            if (objHomeBase == null)
                return;
            if (Time.time - timeCheck >= homeCheckTime)
            {
                GameObject.DestroyObject(objHomeBase);
            }
        }

        //更新基地HP
        private void UpdateHomeHp()
        {
            //更新A阵营
            IEntity entity = EntityManager.GetHomeBase(EntityCampTypeEnum.A);
            if (entity != null)
            {
                spriteHomeBaseA.fillAmount = entity.Hp / entity.HpMax;
            }

            //更新B阵营
            entity = EntityManager.GetHomeBase(EntityCampTypeEnum.B);
            if (entity != null)
            {
                spriteHomeBaseB.fillAmount = entity.Hp / entity.HpMax;
            }
        }

        public void ShowHomeBaseBeAtkEffect()
        {
            timeCheck = Time.time;
            if (objHomeBase != null)
                return;

            //objHomeBase = GameObject.Instantiate(Resources.Load(GameConstDefine.HomeBaseBeAtkEffect)) as GameObject;
            ResourceItem objHomeBaseUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.HomeBaseBeAtkEffect, ResourceType.PREFAB);
            objHomeBase = GameObject.Instantiate(objHomeBaseUnit.Asset) as GameObject;

            objHomeBase.transform.parent = mRoot.Find("GameRecord/Panel/Bk/Frame");
            objHomeBase.transform.localPosition = Vector3.zero;
            objHomeBase.transform.localScale = Vector3.one;
        }

        public void PlayAudioShow()
        {
            DateTime nowtime = System.DateTime.Now;
            TimeSpan timecha = nowtime - GamePlayCtrl.Instance.showaudiotimeold;
            if (timecha.Seconds > 20)
            {
                IPlayer player = PlayerManager.Instance.LocalPlayer;
                if (player == null)
                {
                    Debug.LogError("error player is null");
                    return;
                }
                UInt64 myself_guid = player.GameObjGUID;
                MsgInfoManager.Instance.SetAudioPlay(myself_guid, MsgInfoManager.AudioPlayType.TwentySconde);
            }
        }

        //设置金币
        void SetCp()
        {
            IPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player == null)
                return;
            Dictionary<UInt64, HeroBattleInfo> CpDic = BattleingData.Instance.GetCamp(player.EntityCamp);
            if (CpDic.ContainsKey(player.GameObjGUID))
            {
                CPLabel.text = CpDic[player.GameObjGUID].Cp.ToString();
            }
        }
        //设置死亡数
        void SetDead()
        {
            IPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player == null)
                return;
            Dictionary<UInt64, HeroBattleInfo> DeathsDic = BattleingData.Instance.GetCamp(player.EntityCamp);

            if (DeathsDic.ContainsKey(player.GameObjGUID))
            {
                DeadLabel.text = DeathsDic[player.GameObjGUID].Deaths.ToString();
            }
        }

        //设置击杀数
        void SetKill()
        {
            IPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player == null)
                return;
            Dictionary<UInt64, HeroBattleInfo> KillsDic = BattleingData.Instance.GetCamp(player.EntityCamp);
            if (KillsDic.ContainsKey(player.GameObjGUID))
            {
                KillLabel.text = KillsDic[player.GameObjGUID].Kills.ToString();
            }
        }

        //设置助攻
        void SetAssist()
        {
            IPlayer palyer = PlayerManager.Instance.LocalPlayer;
            if (palyer == null)
                return;
            Dictionary<UInt64, HeroBattleInfo> AssistDic = BattleingData.Instance.GetCamp(palyer.EntityCamp);
            if (AssistDic.ContainsKey(palyer.GameObjGUID))
            {
                AssistLabel.text = AssistDic[palyer.GameObjGUID].Assist.ToString();
            }
        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_NotifyChangeKills, SetKill);//注册击杀数改变事件
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerCp, SetCp);//注册金币改变事件
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_NotifyChangeDeaths, SetDead);//注册死亡数改变事件
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerAssist, SetAssist);//注册助攻数改变事件

            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_LockTarget, UpdateLockHead);//更新锁定头像
            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_EntityHpChange, UpdateHpSlider);//更新实体血条（左上）
            EventCenter.AddListener<IEntity>((Int32)GameEventEnum.GameEvent_EntityMpChange, UpdateMpSlider);//更新实体魔法条
            EventCenter.AddListener<FEvent>((Int32)GameEventEnum.GameEvent_SSPingInfo, OnEvent);  //游戏状态

            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerHpChange, UpdateHpSlider);//更新本地玩家血条
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerMpChange, UpdateMpSlider);//更新本地玩家魔法条
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerExpChange, UpdateExp);//更新经验值

            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerLevelChange, UpdateLevel);//更新本地玩家等级

            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerInit, InitHead);//初始化头像
            EventCenter.AddListener<int, int>((Int32)GameEventEnum.GameEvent_LocalPlayerPassiveSkillsUpLv, UpdatePassiveSkills);//更新被动技能
            EventCenter.AddListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerPassiveSkills, SetPassSkill);//设置被动技能
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryValue, UpdateFuryValue);//更新狂暴值
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryEffect, UpdateFuryEffect);//更新狂暴特效
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_NotifyChangeKills, SetKill);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerCp, SetCp);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_NotifyChangeDeaths, SetDead);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerAssist, SetAssist);

            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_LockTarget, UpdateLockHead);
            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_EntityHpChange, UpdateHpSlider);
            EventCenter.RemoveListener<IEntity>((Int32)GameEventEnum.GameEvent_EntityMpChange, UpdateMpSlider);
            EventCenter.RemoveListener<FEvent>((Int32)GameEventEnum.GameEvent_SSPingInfo, OnEvent);

            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerHpChange, UpdateHpSlider);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerMpChange, UpdateMpSlider);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerExpChange, UpdateExp);
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerLevelChange, UpdateLevel);

            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerInit, InitHead);
            EventCenter.RemoveListener<int,int>((Int32)GameEventEnum.GameEvent_LocalPlayerPassiveSkillsUpLv, UpdatePassiveSkills);
            EventCenter.RemoveListener<int>((Int32)GameEventEnum.GameEvent_LocalPlayerPassiveSkills, SetPassSkill);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryValue, UpdateFuryValue);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LocalPlayerUpdateFuryEffect, UpdateFuryEffect);
        }

        //更新被动技能
        private void UpdatePassiveSkills(int skillID,int time)
        {
            IPlayer player = PlayerManager.Instance.LocalPlayer;
            if (player == null)
                return;
            int skillId = GetPassSkill(0, player);
            if (skillId == skillID)
            {
                isDownTime = true;
                StartTime = System.DateTime.Now;
                skillEndTime = time / 1000f;
                if (!ShowPassSkill(skillId))
                    return;
                mSpritePassiveSkill.gameObject.SetActive(true);
            }
        }

        bool ShowPassSkill(int skillId)
        {
            SkillPassiveConfigInfo skillInfo = ConfigReader.GetSkillPassiveConfig((uint)skillId);
            if (skillInfo == null)
            {
                Debug.LogError(" ID = " + skillId);
                return false;
            }
            mPassiveSkill.spriteName = skillInfo.icon;
            return true;
        }

        void SetPassSkill(int skillId)
        {
            IPlayer player  = PlayerManager.Instance.LocalPlayer;
            if(player == null)
                return;
            int id = GetPassSkill(skillId, player);
            if (!ShowPassSkill(id))
                return;
        }

        private int GetPassSkill(int skillId,IPlayer player)
        {
            HeroConfigInfo heroInfo = null;
            int id = (int)player.ObjTypeID;
            if (skillId == 0)
            {
                heroInfo = ConfigReader.GetHeroInfo(id);
                PassiveTemp = heroInfo.HeroSkillTypeP;
                if (SkillLevel3 <= player.Level)
                    return PassiveTemp + 2;
                else if (SkillLevel2 <= player.Level)
                    return PassiveTemp + 1;
                else if (player.Level < SkillLevel2)
                    return PassiveTemp;
            }
            heroInfo = ConfigReader.GetHeroInfo(skillId);
            return heroInfo.HeroSkillTypeP;
        }
        
        void ShowPingIcon(float ping)
        {
            PingEnum tag = PingEnum.RedTag;
            if (ping >= 0f && ping < 150f)
            {
                tag = PingEnum.GreenTag;
            }
            else if (ping < 300f)
            {
                tag = PingEnum.YellowTag;
            }
            for (int i = 0; i < spritePing.Length; i++)
            {
                if ((PingEnum)i == tag)
                {
                    spritePing[i].gameObject.SetActive(true);
                }
                else
                {
                    spritePing[i].gameObject.SetActive(false);
                }
            }
        }
        //更新HP滑动条
        public void UpdateHpSlider()
        {
            //背景显示
            sliderHp.fillAmount = PlayerManager.Instance.LocalPlayer.Hp / PlayerManager.Instance.LocalPlayer.HpMax;
            int hp = (int)PlayerManager.Instance.LocalPlayer.Hp;
            int hpMax = (int)PlayerManager.Instance.LocalPlayer.HpMax;
            //文本显示
            labelHp.text = hp.ToString() + "/" + hpMax.ToString();
        }
        //更新MP滑动条
        public void UpdateMpSlider()
        {
            sliderMp.fillAmount = PlayerManager.Instance.LocalPlayer.Mp / PlayerManager.Instance.LocalPlayer.MpMax;
            int mp = (int)PlayerManager.Instance.LocalPlayer.Mp;
            int mpMax = (int)PlayerManager.Instance.LocalPlayer.MpMax;
            labelMp.text = mp.ToString() + "/" + mpMax.ToString();
        }

        public void UpdateLevel(int level)
        {
            labelLevel.text = level.ToString();
            if (level == SkillLevel3)
            {
                UpdatePassiveSkills(PassiveTemp + 2, 0);
            }
            else if (level == SkillLevel2)
            {
                UpdatePassiveSkills(PassiveTemp + 1, 0);
            }
            UpdateExp();
        }

        const int SkillLevel3 = 11;
        const int SkillLevel2 = 6;
        public void UpdateExp()
        {
            HeroConfigInfo info = ConfigReader.GetHeroInfo(PlayerManager.Instance.LocalPlayer.NpcGUIDType);
            if (PlayerManager.Instance.LocalPlayer.Level >= CommonDefine.MAX_LEVEL)
            {
                sliderExp.fillAmount = 1.0f;
                labelExp.text = "";
                return;
            }
            int exp = (int)PlayerManager.Instance.LocalPlayer.Exp;
            float expMax = info.HeroBaseExp + (PlayerManager.Instance.LocalPlayer.Level - 1) * info.HeroExpGrowth;

            sliderExp.fillAmount = PlayerManager.Instance.LocalPlayer.Exp / expMax;
            labelExp.text = exp.ToString() + "/" + Convert.ToInt32(expMax).ToString();
        }


        void OnEvent(FEvent eve)
        {
            switch ((GameEventEnum)eve.GetEventId())
            {
                case GameEventEnum.GameEvent_SSPingInfo:
                    float ping = (float)eve.GetParam("ping");
                    ShowPingIcon(ping);
                    break;
            }
        }
        private GameObject lockEffect = null;
        private void ShowLockEffect(bool show)
        {
            if (lockEffect == null)
            {
                LoadLockEffect();
            }

            lockEffect.SetActive(show);

        }
        public void InitHead()
        {
            HeroSelectConfigInfo info = ConfigReader.GetHeroSelectInfo(PlayerManager.Instance.LocalPlayer.NpcGUIDType);

            string head = info.HeroSelectHead.ToString();

            spriteHead.spriteName = head;
        }	
        private void LoadLockEffect()
        {
            if (null == lockEffect)
            {
                lockEffect = LoadUiResource.LoadRes(spriteLock.transform.parent, GameDefine.GameConstDefine.LockEffect);
                lockEffect.transform.localPosition = spriteLock.transform.localPosition;
            }
        }
        private void UpdateLockHead(IEntity entity)
        {
            bool mCanAbsorb = false;
            if (entity == null)//如果实体销毁为空了
            {
                CurrLockEntity = null;
                mLockRoot.localPosition = new Vector3(5000, 0, 0);//显示的头像位置重新设定
                ShowLockEffect(false);//隐藏锁定特效
                EventCenter.Broadcast<string>((Int32)GameEventEnum.GameEvent_ResetLockHead, null);
            }
            else
            {
                string head = "";
                CurrLockEntity = entity;
                spriteLockBg2.gameObject.SetActive(false);

                if (entity.entityType == EntityTypeEnum.Player)//如果锁定的敌方英雄
                {
                    HeroSelectConfigInfo info = ConfigReader.GetHeroSelectInfo(entity.NpcGUIDType);
                    head = info.HeroSelectHead.ToString();
                }
                else //野怪，士兵，箭塔士兵
                {
                    NpcConfigInfo info = ConfigReader.GetNpcInfo(entity.NpcGUIDType);
                    head = info.HeadPhoto.ToString();
                    if (entity.entityType == EntityTypeEnum.Monster || entity.entityType == EntityTypeEnum.Soldier || entity.entityType == EntityTypeEnum.AltarSoldier && !PlayerManager.Instance.IsLocalSameType(entity))
                    {
                        if (entity.BloodBar != null)
                        {
                            entity.BloodBar.IsBloodBarCpVib(true);//显示血条，cp()
                        }
                        ISelfPlayer player = PlayerManager.Instance.LocalPlayer;
                        //此目标与玩家吸收的野怪类型都不同，那么就允许吸收
                        if (player != null && player.AbsorbMonsterType[0] != entity.NpcGUIDType && player.AbsorbMonsterType[1] != entity.NpcGUIDType)
                        {
                            //目标是小野怪或者大野怪或者践踏士兵  并且和玩家不是一个阵营的，
                            if (entity.NPCCateChild == NPCCateChildEnum.SmallMonster || entity.NPCCateChild == NPCCateChildEnum.HugeMonster
                                || entity.entityType == EntityTypeEnum.AltarSoldier && !PlayerManager.Instance.IsLocalSameType(entity))
                            {
                                spriteLockBg2.gameObject.SetActive(true);//显示锁定头像
                                mCanAbsorb = true;//设置可以吸收
                            }
                        } 
                    }
                }
                spriteLockHp.fillAmount = entity.Hp / entity.HpMax;//锁定头像的HP（左）
                spriteLockMp.fillAmount = entity.Mp / entity.MpMax;//锁定头像的MP（左）
                EventCenter.Broadcast<string>((Int32)GameEventEnum.GameEvent_ResetLockHead, head);
                spriteLock.spriteName = head;//锁定的头像（左）
                mLockRoot.localPosition = mLockPos;//根节点的本地位置
                ShowLockEffect(true);//显示锁定特效
                EventCenter.Broadcast<GameObject>((mCanAbsorb) ? (Int32)GameEventEnum.GameEvent_GuideLockTargetCanAbsorb : (Int32)GameEventEnum.GameEvent_GuideLockTargetCanNotAbsorb, spriteLockBg2.gameObject);
            }

        }
        //显示
        public override void OnEnable()
        {
            GameTimeData.Instance.StartCountTime();
            PlayAudioShow();
            UpdateFuryValue();
        }

        //隐藏
        public override void OnDisable()
        {
            
        }

        public void UpdateHpSlider(IEntity entity)
        {
            if (CurrLockEntity != null && entity != null && entity == CurrLockEntity)
                spriteLockHp.fillAmount = entity.Hp / entity.HpMax;
        }

        public void UpdateMpSlider(IEntity entity)
        {
            if (CurrLockEntity != null && entity != null && entity == CurrLockEntity)
                spriteLockMp.fillAmount = entity.Mp / entity.MpMax;
        }

        public UIAltarInHead CreateAltarUIPrefab(AltarInHead target, int type)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load(GameConstDefine.pathAltarHead)) as GameObject;
            obj.gameObject.transform.parent = mAltarSoldierHeadPoint;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            UIAltarInHead altar = obj.GetComponent<UIAltarInHead>();
            altar.SetCurrHeadIcon(type, target.transform);
            return altar;
        }

        /// <summary>
        /// 爆气
        /// </summary>

        public void UpdateFuryValue()
        {
            if (PlayerManager.Instance == null || PlayerManager.Instance.LocalPlayer == null)
                return;
            int furyValue = PlayerManager.Instance.LocalPlayer.FuryValue;
            if (PlayerManager.Instance.LocalPlayer.FuryState != EFuryState.eFuryRunState)
            {
                int fury = furyValue % ISelfPlayer.PerFuryValue;
                if (furyValue != ISelfPlayer.PerFuryValue)
                    sliderFury.fillAmount = (float)fury / (float)ISelfPlayer.PerFuryValue;
                else
                    sliderFury.fillAmount = 1f;
            }

        }

        public void UpdateFuryEffect()
        {
            if (PlayerManager.Instance.LocalPlayer == null)
            {
                return;
            }

            EFuryState state = PlayerManager.Instance.LocalPlayer.FuryState;
            switch (state)
            {
                case EFuryState.eFuryRunState:
                    LoadRunFuryEffect();
                    DestroyFullFuryEffect();
                    break;
                case EFuryState.eFuryNullState:
                    DestroyFullFuryEffect();
                    DestroyRunFuryEffect();
                    break;
                case EFuryState.eFuryFullState:
                    LoadFullFuryEffect();
                    DestroyRunFuryEffect();
                    break;
            }
        }


        private void LoadRunFuryEffect()
        {
            if (null == furyRunEffect)
            {
                furyRunEffect = LoadUiResource.LoadRes(btnFury.transform, GameDefine.GameConstDefine.FuryHeadInfoEffect);
                furyRunEffect.transform.localPosition = new Vector3(sliderFury.transform.localPosition.x, sliderFury.transform.localPosition.y, sliderFury.transform.localPosition.z - 10f);
                runSlowDown = true;
                startTime = Time.time;
            }
        }

        private void DestroyRunFuryEffect()
        {
            if (null != furyRunEffect)
            {
                LoadUiResource.DestroyLoad(GameDefine.GameConstDefine.FuryHeadInfoEffect);
                furyRunEffect = null;
            }
        }

        private void LoadFullFuryEffect()
        {
            if (null == furyFullEffect)
            {
                furyFullEffect = LoadUiResource.LoadRes(btnFury.transform, GameDefine.GameConstDefine.FuryFullFuryInfoEffect);
                furyFullEffect.transform.localPosition = new Vector3(sliderFury.transform.localPosition.x, sliderFury.transform.localPosition.y, sliderFury.transform.localPosition.z - 10f);
                startTime = Time.time;
            }
        }

        private void DestroyFullFuryEffect()
        {
            if (null != furyFullEffect)
            {
                LoadUiResource.DestroyLoad(GameDefine.GameConstDefine.FuryFullFuryInfoEffect);
                furyFullEffect = null;
            }
        }

        UILabel mSpritePassiveInfo = null;
        DateTime LastTime;
        bool isDownTime = false;
        DateTime StartTime;//触发被动技能开始时间
        float skillEndTime = 0;
        UISprite mPassiveSkill = null;
        UISprite mSpritePassiveSkill = null;
        int PassiveTemp = 0;

        IEntity CurrLockEntity = null;

        GameObject BattleInfo = null;
        GameObject ShopInfo = null;
        //实时显示
        float timeCheck = 0f;
        const float homeCheckTime = 3f;
        UILabel CPLabel;
        UILabel TimeLabel;
        UILabel DeadLabel;
        UILabel KillLabel;
        UILabel AssistLabel;
        UISprite spriteHomeBaseA;
        UISprite spriteHomeBaseB;
        GameObject objHomeBase = null;

        GameObject BackToCity;

        private Transform mLockRoot;
        private UISprite spriteLock;
        private UISprite spriteLockBg1;
        private UISprite spriteLockBg2;
        private UISprite spriteLockHp;
        private UISprite spriteLockMp;

        //英雄信息
        private UISprite spriteHead;

        private UILabel labelLevel;

        private UISprite sliderHp;
        private UILabel labelHp;

        private UISprite sliderMp;
        private UILabel labelMp;

        private UISprite sliderExp;
        private UILabel labelExp;

        private UISprite[] spritePing;
        private bool isPress;

        private Transform mAltarSoldierHeadPoint;

        private UISprite sliderFury = null;

        private GameObject furyRunEffect = null;

        private GameObject furyFullEffect = null;

        private GameObject btnFury = null;

        private bool runSlowDown = false;

        private float startTime;

        private const float slowDownLimit = 18f;

        private Vector3 mLockPos;

        private enum PingEnum
        {
            GreenTag = 0,
            YellowTag,
            RedTag,
        }
    }
}
