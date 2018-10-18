
using System;
using UnityEngine;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Thanos.Network
{
    public class  NetworkManager : Singleton<NetworkManager>
    {
        public enum ServerType
        {
            GateServer = 0, //专门用来连接客户端 网关服务器
            BalanceServer,  //平衡服务器。
            LoginServer     //登录服务器
        }
        private TcpClient m_Client = null;
        private TcpClient m_Connecting = null;
        private string m_IP = "127.0.0.1";
        private Int32 m_Port = 40001;
        private Int32 m_n32ConnectTimes = 0;

        private ServerType serverType = ServerType.BalanceServer;

        private float m_CanConnectTime = 0f;//链接的时间
        private float m_RecvOverTime = 0f;//超时时间？
        private float mRecvOverDelayTime = 6000f;//

        private Int32 m_ConnectOverCount = 0;//
        private float m_ConnectOverTime = 0f;
        private Int32 m_RecvOverCount = 0;

        public bool canReconnect = false;//是否可以链接 
        public byte[] m_RecvBuffer = new byte[2 * 1024 * 1024];
        public Int32 m_RecvPos = 0;
        public bool mPause = false;


        IAsyncResult mRecvResult;
        IAsyncResult mConnectResult;

        private bool m_bCustomHandleMessage;
        
        //发送数据stream
        public System.IO.MemoryStream mSendStream = new System.IO.MemoryStream();
        //接收数据stream
        public List<int> mReceiveMsgIDs = new List<int>();

        public List<System.IO.MemoryStream> mReceiveStreams = new List<System.IO.MemoryStream>();

        public List<System.IO.MemoryStream> mReceiveStreamsPool = new List<System.IO.MemoryStream>();

        public NetworkManager()
        {
            m_bCustomHandleMessage =  false;
            for (int i=0;i<50;++i)
            {
                mReceiveStreamsPool.Add(new System.IO.MemoryStream());
            }
        
            //预先创建消息RuntimeTypeModel运行时类型模型    
            //A
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.AbsorbBegin>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.AbsorbRes>();
            //B
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.BroadcastBuildingDestory>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.BuffEffect>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.BroadcastBattleHeroInfo>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.BattleStateChange>();
            //C
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.CurCP>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.CurDeadTimes>();
            //D
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.DestroyEmitEffect>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.DisappearInfo>();
            //E
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.EmitSkill>();
            //F
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.FreeState>();
            //G
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.GOAppear>();
            //H
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.HitTar>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.HeroKills>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.HPChange>();
            //L
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.LastingSkillState>();
            //M
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.MpChange>();
            //N
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyOBAppear>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillModelEmitTurn>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyPassitiveSkillLoad>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyPassitiveSkillUnLoad>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyPassitiveSkillRelease>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillModelStartForceMoveTeleport>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillModelStartForceMoveStop>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillModelStartForceMove>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyBornObj>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySummonLifeTime>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillModelLeading>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyAltarBSIco>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifySkillInfo>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.NotifyOtherItemInfo>();
            //P
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.PrepareSkillState>();
            //S
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.SummonEffect>();
            //R
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.RangeEffectEnd>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.RebornSuccess>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.RebornTimes>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.ReleasingSkillState>();
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.RunningState>();
            //U
            ProtoBuf.Serializer.PrepareSerializer<GSToGC.UsingSkillState>();
        }


        public void Pause()
        {
            mPause = true;
        }

        public void Resume()
        {
            mPause = false;
        }


        ~NetworkManager()
        {
            //接收stream
            foreach (System.IO.MemoryStream one in mReceiveStreams)
            {
                one.Close();
            }
            foreach (System.IO.MemoryStream one in mReceiveStreamsPool)
            {
                one.Close();
            }

            //发送stream
            if (mSendStream != null)
            {
                mSendStream.Close();
            }

            if (m_Client != null)
            {
                m_Client.Client.Shutdown(SocketShutdown.Both);
                m_Client.GetStream().Close();
                m_Client.Close();
                m_Client = null;
            }                            
        }

      
        // 初始化NetWorkMananger 获取地址与端口
        public void Init(string host, Int32 port, ServerType type, bool customHandleMessage=false)
        {
            m_bCustomHandleMessage = customHandleMessage;
            Debugger.Log("set network ip:" + host + " port:" + port + " type:" + type);
            //获取服务器IP  如果不是本地服务器
            m_IP = host;
            m_Port = port;
            serverType = type;
            m_n32ConnectTimes = 0;
            canReconnect = true;
            m_RecvPos = 0;

#if UNITY_EDITOR
            mRecvOverDelayTime = 20000f;
#endif
        }

        public void UnInit()
        {
            canReconnect = false;
        }

        //链接服务端  在Update中调
        public void Connect()
        {
            if (!canReconnect) return;
            if (m_CanConnectTime > Time.time) return;
            if (m_Client != null)
                throw new Exception(" The socket is connecting, cannot connect again!");
            if (m_Connecting != null)
                throw new Exception(" The socket is connecting, cannot connect again!");       
            try
            {
                m_Connecting = new TcpClient();
                //m_Connecting.Client.Blocking = false;
                mConnectResult = m_Connecting.BeginConnect(m_IP, m_Port, null, m_Connecting);
                m_ConnectOverCount = 0;
                m_ConnectOverTime = Time.time + 2;
            }
            catch (Exception exc)
            {
                Debugger.LogError(exc.ToString());
                m_Client = m_Connecting;
                m_Connecting = null;
                mConnectResult = null;
                OnConnectError(m_Client, null);
            }
        }

        public void Close()
        {
            if (m_Client != null)
            {
                OnClosed(m_Client, null);
            }
        }
      
        public void Update(float fDeltaTime)
        {
            //如果已经有客户端连接
            if (m_Client != null)  
            {
                if (mPause)
                    return;
                //处理mReceiveStreams中缓存的消息
                DealWithMsg(); 
                if (mRecvResult != null)
                {
                    //接收数据超过200此并且超时  关闭连接
                    if (m_RecvOverCount > 200 && Time.time > m_RecvOverTime)
                    {
                        Debugger.LogError("recv data over 200, so close network.");
                        Close();
                        return;
                    }
                    ++m_RecvOverCount;//记录接收次数
                    if (mRecvResult.IsCompleted)//读取数据结束
                    {
                        try
                        {
                            Int32 n32BytesRead = m_Client.GetStream().EndRead(mRecvResult);
                            m_RecvPos += n32BytesRead;

                            if (n32BytesRead == 0)
                            {
                                Debugger.LogError("can't recv data now, so close network 2.");
                                Close();//关闭连接的客户端
                                return;
                            }
                        }
                        catch (Exception exc)
                        {
                            Debugger.LogError(exc.ToString());
                            Close();//关闭连接的客户端
                            return;
                        }
                        OnDataReceived(null, null);//接收数据处理  往stream填充网络数据
                        if (m_Client != null)
                        {
                            try
                            {
                                mRecvResult = m_Client.GetStream().BeginRead(m_RecvBuffer, m_RecvPos, m_RecvBuffer.Length - m_RecvPos, null, null);//开始读取
                                m_RecvOverTime = Time.time + mRecvOverDelayTime;
                                m_RecvOverCount = 0;
                            }
                            catch (Exception exc)
                            {
                                Debugger.LogError(exc.ToString());
                                Close();
                                return;
                            }
                        }
                    }
                }
                if (m_Client != null && m_Client.Connected == false)
                {
                    Debugger.LogError("client is close by system, so close it now.");
                    Close();
                    return;
                }
            }
            //如果客户端正在连接
            else if (m_Connecting != null)
            {
                //如果连接次数超过200次并且连接超时    连接失败，断开连接
                if (m_ConnectOverCount > 200 && Time.time > m_ConnectOverTime)
                {
                    Debugger.LogError("can't connect, so close network.");
                    m_Client = m_Connecting;
                    m_Connecting = null;
                    mConnectResult = null;
                    OnConnectError(m_Client, null);
                    return;
                }
                ++m_ConnectOverCount;//记录连接次数

                if (mConnectResult.IsCompleted)//连接参数设置
                {
                    m_Client = m_Connecting;
                    m_Connecting = null;
                    mConnectResult = null;
                    if (m_Client.Connected)
                    {
                        try
                        {
                            m_Client.NoDelay = true;
                            m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
                            m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 2000);
                            m_Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                            mRecvResult = m_Client.GetStream().BeginRead(m_RecvBuffer, 0, m_RecvBuffer.Length, null, null);//从网络缓冲区异步读取数据
                            m_RecvOverTime = Time.time + mRecvOverDelayTime;
                            m_RecvOverCount = 0;
                            OnConnected(m_Client, null);//连接时服务器的选择
                        }
                        catch (Exception exc)
                        {
                            Debugger.LogError(exc.ToString());
                            Close();
                            return;
                        }
                    }
                    else
                    {
                        OnConnectError(m_Client, null);
                    }
                }
            }
            //如果没有连接
            else
            {
                Connect();
            }
        }

        //发送消息向服务器  两个参数 第一个消息体  第二个消息id
        public void SendMsg(ProtoBuf.IExtensible pMsg, Int32 n32MsgID)
        {
            if (m_Client != null)
            {
                //清除stream
                mSendStream.SetLength(0);
                mSendStream.Position = 0;
                //序列到stream
                ProtoBuf.Serializer.Serialize(mSendStream, pMsg);
              
                Message pcMsg = new Message((int)mSendStream.Length);
                pcMsg.SetProtocalID(n32MsgID);
                pcMsg.Add(mSendStream.ToArray(), 0, (int)mSendStream.Length);

#if UNITY_EDITOR
#else
                try
                {
#endif

#if LOG_FILE && UNITY_EDITOR
              
                if (n32MsgID != 8192 && n32MsgID != 16385)
                {
                    string msgName = "";
                    if (Enum.IsDefined(typeof(GCToBS.MsgNum), n32MsgID))
                    {
                        msgName = ((GCToBS.MsgNum)n32MsgID).ToString();
                    }
                    else if (Enum.IsDefined(typeof(GCToCS.MsgNum), n32MsgID))
                    {
                        msgName = ((GCToCS.MsgNum)n32MsgID).ToString();
                    }
                    else if (Enum.IsDefined(typeof(GCToLS.MsgID), n32MsgID))
                    {
                        msgName = ((GCToLS.MsgID)n32MsgID).ToString();
                    }
                    else if (Enum.IsDefined(typeof(GCToSS.MsgNum), n32MsgID))
                    {
                        msgName = ((GCToSS.MsgNum)n32MsgID).ToString();
                    }

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"F:\Log.txt", true))
                    {
                        sw.WriteLine(Time.time + " 发送消息：\t" + n32MsgID + "\t" + msgName);
                    }
                }
#endif
                //将消息下入缓冲区
                m_Client.GetStream().Write(pcMsg.GetMsgBuffer(), 0, (int)pcMsg.GetMsgSize());

#if UNITY_EDITOR
#else
                }
                catch (Exception exc)
                {
                    Debugger.LogError(exc.ToString());
                    Close();
                }
#endif
            }
        }

        //连接时服务器的选择
        public void OnConnected(object sender, EventArgs e)
        {
            switch (serverType)
            {
                case ServerType.BalanceServer://2
                    {
                        HolyGameLogic.Instance.BsOneClinetLogin();
                    }
                    break;
                case ServerType.GateServer://3
                    {
                        ++m_n32ConnectTimes;
                        if (m_n32ConnectTimes > 1)
                        {
                            HolyGameLogic.Instance.EmsgTocsAskReconnect();//重新连接
                        }
                        else
                        {
                          //  CGLCtrl_GameLogic.Instance.GameLogin();//登录账号与密码 小谷注释
                           //发送登陆
                            GameStart gameStart = new GameStart();
                            gameStart.AskLogin();

                        }
                        //广播发送服务器成功，
                        EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerSuccess); //登录成功，发送Ping值
                    }
                    break;
                case ServerType.LoginServer://1
                    {
                        HolyGameLogic.Instance.EmsgToLs_AskLogin();//如果登录服务器则请求登录
                    }
                    break;
            }
        }

        public void OnConnectError(object sender, ErrorEventArgs e)
        {
            Debugger.Log("OnConnectError begin");      
            try
            {
                m_Client.Client.Shutdown(SocketShutdown.Both);
                m_Client.GetStream().Close();          
                m_Client.Close();
                m_Client = null;
            }
            catch (Exception exc)
            {
                Debugger.Log(exc.ToString());
            }
            mRecvResult = null;
            m_Client = null;
            m_RecvPos = 0;
            m_RecvOverCount = 0;
            m_ConnectOverCount = 0;

            EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerFail);
            Debugger.Log("OnConnectError end");
        }

        public void OnClosed(object sender, EventArgs e)
        {
            EventCenter.Broadcast((Int32)GameEventEnum.ConnectServerFail);
            
            try
            {
                m_Client.Client.Shutdown(SocketShutdown.Both);
                m_Client.GetStream().Close();
                m_Client.Close();
                m_Client = null;
            }
            catch (Exception exc)
            {
                Debugger.Log(exc.ToString());
            }

            mRecvResult = null;
            m_Client = null;
            m_RecvPos = 0;
            m_RecvOverCount = 0;
            m_ConnectOverCount = 0;
            mReceiveStreams.Clear();
            mReceiveMsgIDs.Clear();
        }

        //处理mReceiveStreams中的消息
        public void DealWithMsg()
        {
            while (mReceiveMsgIDs.Count>0 && mReceiveStreams.Count>0)
            {
                int type = mReceiveMsgIDs[0];
                System.IO.MemoryStream iostream = mReceiveStreams[0];
               
                mReceiveMsgIDs.RemoveAt(0);
                mReceiveStreams.RemoveAt(0);

                try
                {
#if LOG_FILE && UNITY_EDITOR
                if (type != 1)
                {
                    string msgName = "";
                    if (Enum.IsDefined(typeof(BSToGC.MsgID), type))
                    {
                        msgName = ((BSToGC.MsgID)type).ToString();
                    }
                    else if (Enum.IsDefined(typeof(LSToGC.MsgID), type))
                    {
                        msgName = ((LSToGC.MsgID)type).ToString();
                    }
                    else if (Enum.IsDefined(typeof(GSToGC.MsgID), type))
                    {
                        msgName = ((GSToGC.MsgID)type).ToString();
                    }

                   using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"F:\Log.txt", true))
                   {
                       sw.WriteLine(Time.time + "  收到消息：\t" + type + "\t" + msgName);
                    }
                }
    #endif
                    if (m_bCustomHandleMessage)
                    {
                        EventCenter.Broadcast<System.IO.Stream, int>((Int32)GameEventEnum.GameEvent_NotifyNetMessage, iostream, type);
                    } else {                        
                        HolyGameLogic.Instance.HandleNetMsg(iostream, type);
                    }

                    //通知收到了网络消息

                    if (mReceiveStreamsPool.Count<100)
                    {
                        mReceiveStreamsPool.Add(iostream);
                    }
                    else
                    {
                        iostream = null;
                    }
                }
                catch (Exception ec)
                {
                    Debugger.LogError("Handle Error msgid: " + type);
                }
            }
        }

        //接收数据处理
        public void OnDataReceived(object sender, DataEventArgs e)
        {
            int m_CurPos = 0;
            while (m_RecvPos - m_CurPos >= 8)
            {
                int len = BitConverter.ToInt32(m_RecvBuffer, m_CurPos);
                int type = BitConverter.ToInt32(m_RecvBuffer, m_CurPos + 4);
                if (len > m_RecvBuffer.Length)
                {
                    Debugger.LogError("can't pause message" + "type=" + type + "len=" + len);
                    break;
                }
                if (len > m_RecvPos - m_CurPos) 
                {
                    break;//wait net recv more buffer to parse.
                }
                //获取stream
                System.IO.MemoryStream tempStream = null;
                if (mReceiveStreamsPool.Count>0)
                {
                    tempStream = mReceiveStreamsPool[0];
                    tempStream.SetLength(0);
                    tempStream.Position = 0;
                    mReceiveStreamsPool.RemoveAt(0);
                }
                else
                {
                    tempStream = new System.IO.MemoryStream();
                }
                //往stream填充网络数据
                tempStream.Write(m_RecvBuffer, m_CurPos + 8, len - 8);
                tempStream.Position = 0;
                m_CurPos += len;
                mReceiveMsgIDs.Add(type);
                mReceiveStreams.Add(tempStream);
            }
            if (m_CurPos > 0)
            {
                m_RecvPos = m_RecvPos - m_CurPos;

                if (m_RecvPos < 0)
                {
                    Debug.LogError("m_RecvPos < 0");
                }

                if (m_RecvPos > 0)
                {
                    Buffer.BlockCopy(m_RecvBuffer, m_CurPos, m_RecvBuffer, 0, m_RecvPos);                    
                }
            }
        }
    }
}