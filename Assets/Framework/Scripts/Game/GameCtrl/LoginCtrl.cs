using System;
using UnityEngine;
using Thanos.GameData;
using Thanos.GameData;
using Thanos.Network;
using System.IO;
using System.Linq;
using Thanos.Tools;

namespace Thanos.Ctrl
{
    public class LoginCtrl : Singleton<LoginCtrl>
    {
        //广播登录事件
        public void Enter()
        {
            //绑定的方法是Show方法  
            //在LoginWindow.Init()添加的监听器
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LoginEnter);
        }

        //广播登录退出事件
        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LoginExit);
        }
      
        //登陆  链接LoginSever   在LoginWindow中InitWidget  服务器选择按钮监听绑定的事件
        public void Login(string account, string pass)
        {
            //设置登陆信息
            SelectServerData.Instance.SetServerInfo(10, account, pass);
            NetworkManager.Instance.canReconnect = false;
            NetworkManager.Instance.Close();
            //设置服务器地IP与端口 
            NetworkManager.Instance.Init(HolyTechGameBase.Instance.LoginServerAdress, 49996, NetworkManager.ServerType.LoginServer);


        }
        
        //登陆错误反馈
        public void LoginError(int code)
        {
            MsgInfoManager.Instance.ShowMsg(code);
            EventCenter.Broadcast<ErrorCodeEnum>((Int32)GameEventEnum.GameEvent_LoginError, (ErrorCodeEnum)code);
        }

        //接收GateServer信息    广播登录成功信息
        public void RecvGateServerInfo(Stream stream)
        {
            BSToGC.AskGateAddressRet pMsg = ProtobufMsg.MessageDecode<BSToGC.AskGateAddressRet>(stream);     
            SelectServerData.Instance.GateServerAdress = pMsg.ip;
            SelectServerData.Instance.GateServerPort = pMsg.port;
            SelectServerData.Instance.GateServerToken = pMsg.token;
            SelectServerData.Instance.SetGateServerUin(pMsg.user_name);
            NetworkManager.Instance.canReconnect = false;
            NetworkManager.Instance.Close();

            NetworkManager.Instance.Init(pMsg.ip, pMsg.port, NetworkManager.ServerType.GateServer,true);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LoginSuccess); //登陆成功事件来移除绑定事件
        }

        //登陆失败
        public void LoginFail()
        {
            NetworkManager.Instance.canReconnect = false;
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_LoginFail);
        }

        //选择LoginServer
        public void SelectLoginServer(int i)
        {
            string ip = HolyTechGameBase.Instance.ipList.ElementAt(i);
            HolyTechGameBase.Instance.LoginServerAdress = ip;//根据选择的服务器指定不同的IP地址
        }
       
        //更新服务器列表   当服务端返回eMsgToGCFromLS_NotifyServerBSAddr(513)此消息时调用
        public void UpdateServerAddr(Stream stream)
        {
            LSToGC.ServerBSAddr pMsg;
            if (!ProtobufMsg.MessageDecode<LSToGC.ServerBSAddr>(out pMsg, stream))
            {
                return;
            }

            SelectServerData.Instance.Clean();
            for (int i = 0; i < pMsg.serverinfo.Count; i++)
            {
                string addr = pMsg.serverinfo.ElementAt(i).ServerAddr;
                int state = pMsg.serverinfo.ElementAt(i).ServerState;
                string name = pMsg.serverinfo.ElementAt(i).ServerName;
                int port = pMsg.serverinfo.ElementAt(i).ServerPort;
                //添加到服务器列表中
                SelectServerData.Instance.SetServerList(i, name, (SelectServerData.ServerState)state, addr, port);
            }
            
            NetworkManager.Instance.Close();
            NetworkManager.Instance.canReconnect = false;

            SelectServerData.Instance.SetDefaultServer();
            //与之绑定的事件是SdkServerCheckSuccess
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_SdkServerCheckSuccess);
        }

        //选择服务器 1，2，3，4
        public void SelectServer(int id)
        {
            SelectServerData.Instance.SetSelectServer(id);
            EventCenter.Broadcast((Int32)GameEventEnum.SelectServer); 
        }       
        //开始游戏 断开原先的服务器，开始连接BalanceServer服务器
        public void GamePlay()
        {
            //设置服务器选择id
            int index = SelectServerData.Instance.curSelectIndex;
            //根据id获取服务器信息
            SelectServerData.ServerInfo info = SelectServerData.Instance.GetServerDicInfo().ElementAt(index).Value;
            NetworkManager.Instance.canReconnect = false;
            NetworkManager.Instance.Close();
            //设置服务器信息
            NetworkManager.Instance.Init(info.addr, info.port, NetworkManager.ServerType.BalanceServer);

            //通过serverKey来设置服务器的名字
            PlayerPrefs.SetString(SelectServerData.serverKey, info.name);
        }

        //SDK注册成功 注册成功后，断掉 链接，重新连接LoginSever服务器
        public void SdkRegisterSuccess(string uin, string sessionId)
        {
            SelectServerData.Instance.SetServerInfo(10, uin, sessionId);
            NetworkManager.Instance.canReconnect = false;
            NetworkManager.Instance.Close();
            NetworkManager.Instance.Init(HolyTechGameBase.Instance.LoginServerAdress, HolyTechGameBase.Instance.LoginServerPort, NetworkManager.ServerType.LoginServer);                      
        }
   }
}
