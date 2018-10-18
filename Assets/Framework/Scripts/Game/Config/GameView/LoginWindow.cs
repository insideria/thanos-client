using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thanos.GameData;
using GameDefine;
using System.Linq;
using Thanos.Ctrl;

namespace Thanos.View
{
    public class LoginWindow : BaseWindow
    {
        public LoginWindow() 
        {
            mScenesType = EScenesType.EST_Login; //窗口归属类型（Login场景中的窗口）
            mResName = GameConstDefine.LoadGameLoginUI;//资源名称  实际上资源加载路径
            mResident = false; //是否是常驻窗口
        }

        ////////////////////////////继承接口/////////////////////////
    
        //注册事件接受器   登录事件  退出事件
        public override void Init()
        {
            //登录事件zaI loginCtrl中广播（LoginState调用）  show：创建窗体并初始化  打开 isVisible      
            //显示初始化OnEnable()   游戏事件注册   在LoginCtrl.enter()中广播
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LoginEnter, Show);
            //在login.exit()中广播
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LoginExit, Hide);

        } 
        //类对象释放  移除登录监听器  移除退出监听器
        public override void Realse()
        {
            //移除监听器
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LoginEnter, Show);  
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LoginExit, Hide);
        }
         
        //窗口控件初始化   寻找对象   添加监听器  将服务器选项添加到集合中  删除其他UI控件
        protected override void InitWidget()
        {
            //窗口控件获取   
            mLoginParent = mRoot.Find("Server_Choose");
            mLoginInput = mRoot.Find("Server_Choose/Loginer");
            mLoginSubmit = mRoot.Find("Server_Choose/Button");//选择登录服务器
            mLoginAccountInput = mRoot.Find("Server_Choose/Loginer/AcountInput").GetComponent<UIInput>();//账号输入
            mLoginPassInput = mRoot.Find("Server_Choose/Loginer/PassInput").GetComponent<UIInput>();//密码输入
            //
            mPlayParent = mRoot.Find("LoginBG");//登录 父级
            mPlaySubmitBtn = mRoot.Find("LoginBG/LoginBtn");//开始游戏
            mPlayServerBtn = mRoot.Find("LoginBG/CurrentSelection");//选择战场
            mPlayNameLabel = mRoot.Find("LoginBG/CurrentSelection/Label3").GetComponent<UILabel>();//当前选择的服务器
            mPlayStateLabel = mRoot.Find("LoginBG/CurrentSelection/Label4").GetComponent<UILabel>();//当前服务器状态
            mPlayAnimate = mPlaySubmitBtn.GetComponent<Animator>();
            mChangeAccountBtn = mRoot.Find("ChangeAccount");// 切换账号按钮
            mChangeAccountName = mRoot.Find("ChangeAccount/Position/Label1").GetComponent<UILabel>();
            mServerParent = mRoot.Find("UIGameServer");

            mReLoginParent = mRoot.Find("LogInAgain");
            mReLoginSubmit = mRoot.Find("LogInAgain/Status1/Button");//重新登录，账号异常时
            mVersionLable = mRoot.Find("Label").GetComponent<UILabel>();
            mWaitingParent = mRoot.Find("Connecting");

            //为指定对象添加监听器（登录过程中的按钮）
            UIEventListener.Get(mPlaySubmitBtn.gameObject).onClick += OnPlaySubmit;//监听 （开始游戏）按钮
            UIEventListener.Get(mPlayServerBtn.gameObject).onClick += OnPlayServer;//监听（选择战场）按钮
            UIEventListener.Get(mChangeAccountBtn.gameObject).onClick += OnChangeAccount;//监听（切换账号）按钮
            UIEventListener.Get(mReLoginSubmit.gameObject).onClick += OnReLoginSubmit;//监听（重新登录）  LoginAgain界面
            UIEventListener.Get(mLoginSubmit.gameObject).onClick += OnLoginSubmit;//服务器选择按钮监听

            mServerList.Clear();
            //将所有服务器的Toggle添加到mServerList中
            for (int i = 0; i < 4; i++)
            {
                UIToggle toggle = mLoginParent.Find("Server" + (i + 1).ToString()).GetComponent<UIToggle>();
                mServerList.Add(toggle);
            }
            //添加事件  选择登录服务器
            for (int i = 0; i < mServerList.Count; i++)
            {
                EventDelegate.Add(mServerList.ElementAt(i).onChange, OnSelectIp);
            }
            DestroyOtherUI(); //删除Login外其他控件
        }

        //删除Login外其他控件，例如
        public static void DestroyOtherUI()
        {
            Camera camera = GameMethod.GetUiCamera;
            for (int i = 0; i < camera.transform.childCount; i++)
            {
                if (camera.transform.GetChild(i) != null && camera.transform.GetChild(i).gameObject != null)
                {

                    GameObject obj = camera.transform.GetChild(i).gameObject;
                    if (obj.name != "UIGameLogin(Clone)")
                    {
                        GameObject.DestroyImmediate(obj);
                    }                    
                }
            }
        }

        //窗口控件释放 没有内容，仅仅是实现基类的方法
        protected override void RealseWidget()
        {
        }

        //游戏事件注册 在创建窗口时调用，baseWindow
        protected override void OnAddListener()
        {
            EventCenter.AddListener<ErrorCodeEnum>((Int32)GameEventEnum.GameEvent_LoginError, LoginFail);//登录失败反馈
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LoginSuccess, LoginSuceess);//登录成功反馈
            EventCenter.AddListener<string, string>((Int32)GameEventEnum.GameEvent_SdkRegisterSuccess, SdkRegisterSuccess);//sdk 注册成功，当513的消息返回时启动
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkServerCheckSuccess, SdkServerCheckSuccess);//sdk 检查成功
            EventCenter.AddListener((Int32)GameEventEnum.SelectServer, SelectServer);//选择服务器
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_LoginFail, ShowLoginFail);//显示登录界面失败
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff); //SDK退出
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener<ErrorCodeEnum>((Int32)GameEventEnum.GameEvent_LoginError, LoginFail);
            EventCenter.RemoveListener<string,string>((Int32)GameEventEnum.GameEvent_SdkRegisterSuccess, SdkRegisterSuccess);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkServerCheckSuccess, SdkServerCheckSuccess);
            EventCenter.RemoveListener((Int32)GameEventEnum.SelectServer, SelectServer);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LoginFail, ShowLoginFail);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_LoginSuccess, LoginSuceess);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SdkLogOff, SdkLogOff);
        }

        //显示 当被设置为active true时调用，注意这个类没有继承Mono，在Show方法中激活Active
        public override void OnEnable()
        {
            mVersionLable.text = "1.0.0";
            mPlayAnimate.enabled = true;
            ShowServer(LOGINUI.None);
            mLoginInput.gameObject.SetActive(true);
        }

        //隐藏  当被设置为active false时调用，注意这个类没有继承Mono
        public override void OnDisable()
        {

        }

        ////////////////////////////////UI事件响应////////////////////////////////////
       
        //当点击选择服务器的时候 响应事件
        void OnPlaySubmit(GameObject go)
        {
            //显示等待连接图标
            mWaitingParent.gameObject.SetActive(true);
            UIEventListener.Get(mPlaySubmitBtn.gameObject).onClick -= OnPlaySubmit;//移除事件
            LoginCtrl.Instance.GamePlay();//开始游戏
        }

        //监听选择战场事件
        void OnPlayServer(GameObject go)
        {
            ShowServer(LOGINUI.SelectServer);
        }

        void OnChangeAccount(GameObject go)
        {
        }

        void OnReLoginSubmit(GameObject go)
        {
            mReLoginParent.gameObject.SetActive(false);
        }
    
        //服务器选择按钮监听事件响应
        void OnLoginSubmit(GameObject go)
        {
//#if UNITY_STANDALONE_WIN
            if (string.IsNullOrEmpty(mLoginAccountInput.value))
                return;
            if (string.IsNullOrEmpty(mLoginPassInput.value))
                return;

//#else
           if (string.IsNullOrEmpty(mLoginAccountInput.value) || string.IsNullOrEmpty(mLoginPassInput.value))
                return;
//#endif
            mWaitingParent.gameObject.SetActive(true);
            //登陆  初始化网络模块,逻辑部分都在Login
            LoginCtrl.Instance.Login(mLoginAccountInput.value, mLoginPassInput.value);
        }

        void OnSelectIp()
        {
            if (UIToggle.current == null || !UIToggle.current.value)
                return;
            for (int i = 0; i < mServerList.Count; i++)
            {
                if (mServerList.ElementAt(i) == UIToggle.current)
                {
                    LoginCtrl.Instance.SelectLoginServer(i);
                    break;
                }
            }
        }


        ////////////////////////////////游戏事件响应////////////////////////////////////

        //登录失败
        void LoginFail(ErrorCodeEnum errorCode)
        {
            mPlayAnimate.enabled = true;

            mPlaySubmitBtn.gameObject.SetActive(true);
            GameObject.DestroyImmediate(mPlayEffect.gameObject);
        }

        //登陆失败反馈   
        void ShowLoginFail()
        {
            mReLoginParent.gameObject.SetActive(true);
            mWaitingParent.gameObject.SetActive(false);
            UIEventListener.Get(mPlaySubmitBtn.gameObject).onClick += OnPlaySubmit;
        }

        //登陆成功 移除监听事件
        void LoginSuceess()
        {
            UIEventListener.Get(mPlaySubmitBtn.gameObject).onClick -= OnPlaySubmit;
        }

        //选择了服务器
        void SelectServer()
        {
            ShowSelectServerInfo();
            ShowServer(LOGINUI.Login);
        }

        //显示服务器信息或者显示登录信息
        void ShowServer(LOGINUI state)
        {
            bool showLogin = false;
            bool showServer = false;
            bool showSelectServer = false;
            switch (state)
            {
                case LOGINUI.Login: //登录时
                    ShowSelectServerInfo();
                    showLogin = true;
                    showServer = false;
                    showSelectServer = false;
                    break;
                case LOGINUI.SelectServer:
                    showLogin = false;
                    showServer = true;
                    showSelectServer = false;
                    break;
                case LOGINUI.None:
                    showLogin = false;
//                    showServer = false;
//#if UNITY_STANDALONE_WIN || UNITY_EDITOR || SKIP_SDK
                    showSelectServer = true;
//#endif
                    break;
            }
            mPlayParent.gameObject.SetActive(showLogin);
            mServerParent.gameObject.SetActive(showServer);
            mLoginParent.gameObject.SetActive(showSelectServer);

            if (showLogin)
            {
                mChangeAccountName.text = mLoginAccountInput.value;
            }

            mChangeAccountBtn.gameObject.SetActive(showLogin);
        }

        //显示选中的server信息 
        void ShowSelectServerInfo()
        {
            SelectServerData.ServerInfo info = SelectServerData.Instance.curSelectServer;
            mPlayNameLabel.text = info.name;
            mPlayStateLabel.text = "(" + SelectServerData.Instance.StateString[(int)info.state] + ")"; //服务器状态       
            SelectServerData.Instance.SetLabelColor(mPlayStateLabel, info.state);
        }

        //SDK注册成功
        void SdkRegisterSuccess(string uid, string sessionId)
        {
            LoginCtrl.Instance.SdkRegisterSuccess(uid, sessionId);

            mWaitingParent.gameObject.SetActive(true);
        }

        //SDK检查成功
        void SdkServerCheckSuccess()
        {
            ShowServer(LOGINUI.Login);//显示服务器信息或者显示登录信息
            mWaitingParent.gameObject.SetActive(false);
        }

        //SDK退出
        void SdkLogOff()
        {
            
            ShowServer(LOGINUI.None);
            mLoginPassInput.value = "";
            mLoginAccountInput.value = "";

        }

        IEnumerator ShakeLabel()
        {
            mPlayEffect = GameMethod.CreateWindow(GameConstDefine.LoadGameLoginEffectPath, new Vector3(-5, -270, 0), mRoot.transform);
            mPlaySubmitBtn.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.4f);
        }
        
        enum LOGINUI
        {
            None = -1,
            Login,
            SelectServer,
        }

        //开始
        Transform mPlayParent;
        Transform mPlaySubmitBtn;
        Transform mPlayServerBtn;
        UILabel mPlayNameLabel;
        UILabel mPlayStateLabel;
        Animator mPlayAnimate;
        GameObject mPlayEffect;

        //登录
        Transform mLoginParent;
        Transform mLoginInput;
        Transform mLoginSubmit;
        UIInput mLoginPassInput;
        UIInput mLoginAccountInput;

        //改变帐号
        Transform mChangeAccountBtn;
        UILabel mChangeAccountName;

        //选服
        Transform mServerParent;

        //重新登录选择
        Transform mReLoginParent;
        Transform mReLoginSubmit;

        //等待中
        Transform mWaitingParent;

        //版本号   
        UILabel mVersionLable;

        //服务器列表
        private List<UIToggle> mServerList = new List<UIToggle>();

    }

}
