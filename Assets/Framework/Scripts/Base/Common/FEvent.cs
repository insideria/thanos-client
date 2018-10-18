using System;
using System.Collections.Generic;

namespace Thanos
{
    public class FEvent
    {
        private Int32 eventId;
        private Dictionary<string, object> mParamList;

        public FEvent()
        {
            mParamList = new Dictionary<string, object>();
        }

        public FEvent(Int32 id)
        {
            eventId = id;
            mParamList = new Dictionary<string, object>();
        }

        //获取消息类型
        public Int32 GetEventId()
        {
            return eventId;
        }

        public void AddParam(string name, object value)
        {
            mParamList[name] = value;
        }

        public object GetParam(string name)
        {
            if (mParamList.ContainsKey(name))
            {
                return mParamList[name];
            }
            return null;
        }

        public bool HasParam(string name)
        {
            if (mParamList.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

        public int GetParamCount()
        {
            return mParamList.Count;
        }

        public Dictionary<string, object> GetParamList()
        {
            return mParamList;
        }
    }
}