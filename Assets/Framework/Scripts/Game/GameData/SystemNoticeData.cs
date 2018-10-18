using UnityEngine;
using System.Collections;
using GameDefine;
using System.Collections.Generic;
using System.Linq;

public class SystemNoticeData {

    private static SystemNoticeData instance = null;
    public static SystemNoticeData Instance
    {
        get {
            if (instance == null)
            {
                instance = new SystemNoticeData();
            }
            return instance;
        }
    }

    public void Clear()
    {
        if (systemNotList.Count != 0)
        {
            systemNotList.Clear();
        }
    }

    public List<SystemNoticeType> systemNotList = new List<SystemNoticeType>();
    public void SetSystemNotList(string title, NoticeIdentify identify, NoticeState state, int sort, string content)
    {
        SystemNoticeType notice = new SystemNoticeType();
        notice.SetSystemNotice(title, identify, state, sort, content);
        SystemNoticeType temp = null;
        foreach (var item in systemNotList)
        {
            if (item.mNoticeSort == sort)
            {
                item.SetNoticChildSort(item.mNoticChildSort);
            }
        }
        for (int i = 0; i < systemNotList.Count; i++)
        {
            if (systemNotList[i].mNoticeSort <= notice.mNoticeSort && 
                systemNotList[i].mNoticChildSort > notice.mNoticChildSort)//当新公告优先级高于
            {
                temp = systemNotList[i];
                systemNotList[i] = notice;
                notice = temp;
            }
        }
        systemNotList.Add(notice);
    }

    Dictionary<int, SystemNoticeType> systemNotDic = new Dictionary<int, SystemNoticeType>();

    public void SetSystemNotDic(string title, NoticeIdentify identify, NoticeState state, int sort, string content)
    {
        SystemNoticeType notice = new SystemNoticeType();
        notice.SetSystemNotice(title, identify, state, sort, content);

        List<KeyValuePair<int, SystemNoticeType>> myList = new List<KeyValuePair<int, SystemNoticeType>>(systemNotDic);

        myList.Sort(delegate(KeyValuePair<int, SystemNoticeType> s1, KeyValuePair<int, SystemNoticeType> s2)
        {
            return s1.Value.mNoticeSort.CompareTo(s2.Value.mNoticeSort);
        });

        systemNotDic.Clear();

        foreach (KeyValuePair<int, SystemNoticeType> pair in myList)
        {
            systemNotDic.Add(pair.Key, pair.Value);
        }
 
        //foreach(string key in dic.Keys)
        // {
        //     Response.Write(dic[key] +"<br />");
        // }
        //foreach(var item in systemNotDic.OrderByDescending)
    }



    public class SystemNoticeType
    {

        public string mNoticeTitle//公告标题
        {
            private set;
            get;
        }
        public NoticeIdentify mNoticeIdentify//公告标识
        {
            private set;
            get;
        }

        public NoticeState mNoticeState // 公告状态
        {
            private set;
            get;
        }

        public int mNoticeSort//公告排序
        {
            private set;
            get;
        }

        public int mNoticChildSort
        {
            private set;
            get;
        }
                
        public string mNoticeContent//公告内容
        {
            private set;
            get;
        }

        public void SetNoticChildSort(int childSort)
        {
            mNoticChildSort = (++childSort);
        }

        public void SetSystemNotice(string title, NoticeIdentify identify, NoticeState state, int sort, string content)
        {
            mNoticChildSort = 0;
            mNoticeTitle = title;
            mNoticeIdentify = identify;
            mNoticeState = state;
            mNoticeSort = sort;
            mNoticeContent = content;
        }
    }

    
}
