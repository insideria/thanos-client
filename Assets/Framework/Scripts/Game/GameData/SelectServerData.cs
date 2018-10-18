using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace Thanos.GameData
{
    public class SelectServerData : Singleton<SelectServerData>
    {
        public enum ServerState
        { 
            Fluent = 0,
            Busy,
            HouseFull,
        }

        public class ServerInfo
        {
            public int index;
            public string name;
            public ServerState state;
            public string addr;
            public int port;
            public string area;
        }

        public string GateServerAdress
        {
            set;
            get;
        }

        public int GateServerPort
        {
            set;
            get;
        }

        public string GateServerToken
        {
            set;
            get;
        }

        Dictionary<int, ServerInfo> serverInfoDic = new Dictionary<int, ServerInfo>(); 
        


        public int serverPlafrom
        {
            private set;
            get;
        }

        public string serverUin
        {
            private set;
            get;
        }

        public string serverSionId
        {
            private set;
            get;
        }


        public string gateServerUin
        {
            private set;
            get;
        }

        public void Clean() {
            serverInfoDic.Clear(); 
        }

        public int curSelectIndex
        {
            private set;
            get;
        }

        public ServerInfo curSelectServer
        {
            private set;
            get;
        }

       public string[] StateString = { "流畅", "繁忙", "爆满" };
       public const string serverKey = "Server";
       public const string serverStateKey = "ServerState";    


        public void SetLabelColor(UILabel label, ServerState state)
       {
           switch (state)
           {
               case SelectServerData.ServerState.Busy:
                   label.color = Color.yellow;
                   break;
               case SelectServerData.ServerState.Fluent:
                   label.color = Color.green;
                   break;
               case SelectServerData.ServerState.HouseFull:
                   label.color = Color.red;
                   break;
           }
       }

        public void SetServerList(int index,string name,ServerState state,string addr,int port, string area="") {
            ServerInfo info = new ServerInfo();
            info.index = index;
            info.name = name;
            info.state = state;
            info.addr = addr;
            info.port = port;
            info.area = area;
            serverInfoDic.Add(index, info);
        }

        
        public void SetDefaultServer()
        {
            int index = 0;
            if (PlayerPrefs.HasKey(serverKey))
            {
                string name = PlayerPrefs.GetString(serverKey);
                for (int i = 0; i < GetServerDicInfo().Count; i++)
                {
                    if (name.CompareTo(GetServerDicInfo().ElementAt(i).Value.name) == 0)
                    {
                        index = i;
                        break;
                    }
                }
            }
            else
            {
                index = 0;
            }
            SetSelectServer(index);
        }

        public Dictionary<int, ServerInfo> GetServerDicInfo()
        {
            return serverInfoDic;
        } 

        public void SetServerInfo(int plafrom, string uin, string sionId) {
            serverPlafrom = plafrom;
            serverUin = uin;
            serverSionId = sionId;
        }

        public void SetGateServerUin(string uin) {
            gateServerUin = uin;
        }

        public void SetSelectServer(int index) {
            curSelectIndex = index;
            curSelectServer = serverInfoDic.ElementAt(index).Value;
        }
    }
}
