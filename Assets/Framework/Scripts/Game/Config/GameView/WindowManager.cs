using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thanos;
using Thanos.GameState;

namespace Thanos.View
{
    public enum EScenesType
    {
        EST_None,
        EST_Login,
        EST_Play,
    }

    public enum EWindowType
    {
        EWT_LoginWindow,
        EWT_UserWindow,
        EWT_LobbyWindow,
        EWT_BattleWindow,
        EWT_RoomWindow,
        EWT_HeroWindow,
        EWT_BattleInfoWindow,
        EWT_MarketWindow,
        EWT_MarketHeroListWindow,
        EWT_MarketHeroInfoWindow,
        EWT_MarketRuneListWindow,
        EWT_MarketRuneInfoWindow,
        EWT_SocialWindow,
        EWT_GamePlayWindow,
        EWT_InviteWindow,
        EWT_ChatTaskWindow,
        EWT_ScoreWindow,
        EWT_InviteAddRoomWindow,
        EWT_RoomInviteWindow,
        EWT_TeamMatchWindow,
        EWT_TeamMatchInvitationWindow,
        EWT_TeamMatchSearchingWindow,
        EWT_MailWindow,
        EWT_HomePageWindow,
        EWT_PresonInfoWindow,
        EWT_ServerMatchInvitationWindow,
        EWT_SoleSoldierWindow,
        EWT_MessageWindow,
        EWT_MiniMapWindow,
        EWT_VIPPrerogativeWindow,
        EWT_RuneEquipWindow,
        EWT_DaliyBonusWindow,
        EWT_EquipmentWindow,
        EWT_SystemNoticeWindow,
        EWT_TimeDownWindow,
        EWT_RuneCombineWindow,
        EWT_HeroDatumWindow,
        EWT_RuneRefreshWindow,
        EWT_GamePlayGuideWindow,
        EMT_PurchaseSuccessWindow,
        EMT_GameSettingWindow,
        EMT_AdvancedGuideWindow,
        EMT_ExtraBonusWindow,
        EMT_EnemyWindow,
        EMT_HeroTimeLimitWindow,
        EMT_SkillWindow,
        EMT_SkillDescribleWindow,
        EMT_RuneBuyWindow,
        EMT_DeathWindow,
    }

    public class WindowManager : Singleton<WindowManager>
    {
        //存储所有的窗口
        private Dictionary<EWindowType, BaseWindow> mWidowDic;
        //初始化 生成所有的窗口并将生成的窗口添加到mWindowDic
        public WindowManager()
        {
            mWidowDic = new Dictionary<EWindowType, BaseWindow>();
        }

        public void initAllWindow() {
            mWidowDic[EWindowType.EWT_LoginWindow] = new LoginWindow();
            mWidowDic[EWindowType.EWT_UserWindow] = new UserInfoWindow();
            mWidowDic[EWindowType.EWT_LobbyWindow] = new LobbyWindow();
            mWidowDic[EWindowType.EWT_BattleWindow] = new BattleWindow();
            mWidowDic[EWindowType.EWT_RoomWindow] = new RoomWindow();
            mWidowDic[EWindowType.EWT_HeroWindow] = new HeroWindow();
            mWidowDic[EWindowType.EWT_BattleInfoWindow] = new BattleInfoWindow();
            mWidowDic[EWindowType.EWT_MarketWindow] = new MarketWindow();
            mWidowDic[EWindowType.EWT_MarketHeroListWindow] = new MarketHeroListWindow();
            mWidowDic[EWindowType.EWT_MarketHeroInfoWindow] = new MarketHeroInfoWindow();
            mWidowDic[EWindowType.EWT_SocialWindow] = new SocialWindow();
            mWidowDic[EWindowType.EWT_GamePlayWindow] = new GamePlayWindow();
            mWidowDic[EWindowType.EWT_InviteWindow] = new InviteWindow();
            mWidowDic[EWindowType.EWT_ChatTaskWindow] = new ChatTaskWindow();
            mWidowDic[EWindowType.EWT_ScoreWindow] = new ScoreWindow();
            mWidowDic[EWindowType.EWT_InviteAddRoomWindow] = new InviteAddRoomWindow();
            mWidowDic[EWindowType.EWT_RoomInviteWindow] = new RoomInviteWindow();
            mWidowDic[EWindowType.EWT_TeamMatchWindow] = new TeamMatchWindow();
            mWidowDic[EWindowType.EWT_TeamMatchInvitationWindow] = new TeamMatchInvitationWindow();
            mWidowDic[EWindowType.EWT_TeamMatchSearchingWindow] = new TeamMatchSearchingWindow();
            mWidowDic[EWindowType.EWT_MailWindow] = new MailWindow();
            mWidowDic[EWindowType.EWT_HomePageWindow] = new HomePageWindow();
            mWidowDic[EWindowType.EWT_PresonInfoWindow] = new PresonInfoWindow();
            mWidowDic[EWindowType.EWT_ServerMatchInvitationWindow] = new ServerMatchInvitationWindow();
            mWidowDic[EWindowType.EWT_SoleSoldierWindow] = new SoleSoldierWindow();
            mWidowDic[EWindowType.EWT_MessageWindow] = new MessageWindow();
            mWidowDic[EWindowType.EWT_MarketRuneListWindow] = new MarketRuneListWindow();
            mWidowDic[EWindowType.EWT_MiniMapWindow] = new MiniMapWindow();
            mWidowDic[EWindowType.EWT_MarketRuneInfoWindow] = new MarketRuneInfoWindow();
            mWidowDic[EWindowType.EWT_VIPPrerogativeWindow] = new VIPPrerogativeWindow();
            mWidowDic[EWindowType.EWT_RuneEquipWindow] = new RuneEquipWindow();
            mWidowDic[EWindowType.EWT_DaliyBonusWindow] = new DaliyBonusWindow();
            //mWidowDic[EWindowType.EWT_EquipmentWindow] = new EquipmentWindow();
            mWidowDic[EWindowType.EWT_SystemNoticeWindow] = new SystemNoticeWindow();
            mWidowDic[EWindowType.EWT_TimeDownWindow] = new TimeDownWindow();
            mWidowDic[EWindowType.EWT_RuneCombineWindow] = new RuneCombineWindow();
            mWidowDic[EWindowType.EWT_HeroDatumWindow] = new HeroDatumWindow();
            mWidowDic[EWindowType.EWT_RuneRefreshWindow] = new RuneRefreshWindow();
            mWidowDic[EWindowType.EWT_GamePlayGuideWindow] = new GamePlayGuideWindow();
            mWidowDic[EWindowType.EMT_PurchaseSuccessWindow] = new PurchaseSuccessWindow();
            mWidowDic[EWindowType.EMT_GameSettingWindow] = new GameSettingWindow();
            mWidowDic[EWindowType.EMT_AdvancedGuideWindow] = new AdvancedGuideWindow();
            mWidowDic[EWindowType.EMT_ExtraBonusWindow] = new ExtraBonusWindow();
            mWidowDic[EWindowType.EMT_EnemyWindow] = new EnemyWindow();
            mWidowDic[EWindowType.EMT_HeroTimeLimitWindow] = new HeroTimeLimitWindow();
            mWidowDic[EWindowType.EMT_SkillWindow] = new SkillWindow();
            mWidowDic[EWindowType.EMT_SkillDescribleWindow] = new SkillDescribleWindow();
            mWidowDic[EWindowType.EMT_RuneBuyWindow] = new RuneBuyWindow();
            mWidowDic[EWindowType.EMT_DeathWindow] = new DeathWindow();            
        }

        public BaseWindow GetWindow(EWindowType type)
        {
            if (mWidowDic.ContainsKey(type))
                return mWidowDic[type];

            return null;
        }

        //遍历每个窗口，调用已打开的窗口的Update更新函数。
        public void Update(float deltaTime)
        {
            //遍历所有的窗口，哪个打开那个更新
            foreach (BaseWindow pWindow in mWidowDic.Values)
            {
                if (pWindow.IsVisible())
                {
                    pWindow.Update(deltaTime);
                }
            }
        }

        //Play场景中窗口的控制  LoadingState 中Enter调用
        public void ChangeScenseToPlay(EScenesType front)
        {
            foreach (BaseWindow pWindow in mWidowDic.Values)
            {
                // 如果窗口类型是EST_Play，初始化窗口
                if (pWindow.GetScenseType() == EScenesType.EST_Play)
                {
                    pWindow.Init();
                    //如果是常驻窗口，预加载
                    if (pWindow.IsResident())
                    {
                        pWindow.PreLoad();
                    }
                }
                //否则 隐藏窗口并释放类对象 
                else if ((pWindow.GetScenseType() == EScenesType.EST_Login) && (front == EScenesType.EST_Login))
                {
                    pWindow.Hide();
                    //释放窗口对象
                    pWindow.Realse();

                    if (pWindow.IsResident())
                    {
                        pWindow.DelayDestory();
                    }
                }
            }
        }

        //Login场景中窗口的控制   1.在HolyTechGame中调用时参数传递的是 EScenesType.EST_None
        // 2.在LoadingState.enter调用
        public void ChangeScenseToMain(EScenesType front)
        {
            //遍历每一个窗口类。
            foreach (BaseWindow pWindow in mWidowDic.Values)
            {
                //如果Front的类型是EST_None并且遍历的这个窗口的类型是EST_None，
                if (front == EScenesType.EST_None && pWindow.GetScenseType() == EScenesType.EST_None)
                {
                    pWindow.Init();//添加监听器（显示窗口的监听器）
                    //如果是常驻窗口，那么就加载出来
                    if (pWindow.IsResident())
                    {
                        //创建窗体 但是没有激活,创建完成之后并初始化  creat() Initwidget()
                        pWindow.PreLoad();
                    }
                }
                //如果当前窗口的类型是EST_Login，
                if (pWindow.GetScenseType() == EScenesType.EST_Login)
                {
                    pWindow.Init();
                    if (pWindow.IsResident())
                    {
                        pWindow.PreLoad();
                    }
                }
                //否则隐藏并释放
                else if ((pWindow.GetScenseType() == EScenesType.EST_Play) && (front == EScenesType.EST_Play))
                {
                    pWindow.Hide();//游戏事件注销  隐藏或销毁窗体
                    pWindow.Realse();//移除监听器（指的是窗口显示或隐藏的监听器）
                    if (pWindow.IsResident())
                    {
                        pWindow.DelayDestory();//销毁窗体
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏该类型的所有Window
        /// </summary>
        /// <param name="front"></param>
        public void HideAllWindow(EScenesType front)
        {
            foreach (var item in mWidowDic)
            {
                if (front == item.Value.GetScenseType())
                {
                    Debug.Log(item.Key);
                    item.Value.Hide();
                    //item.Value.Realse();
                }
            }
        }

        //按类型显示窗口
        public void ShowWindowOfType(EWindowType type)
        {
            BaseWindow window;
            if (!mWidowDic.TryGetValue(type, out window))
            {
                return;
            }
            window.Show();
        }


    }
}

