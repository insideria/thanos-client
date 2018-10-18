using System;
using System.Collections.Generic;
using Thanos.GameEntity;

namespace Thanos.Model
{
    public class MailModel : Singleton<MailModel>
    {
        private List<KeyValuePair<int, Mail>> mMailList = new List<KeyValuePair<int, Mail>>();

        public int defaultMailid;

        public void Clear()
        {
            mMailList.Clear();
        }

        public List<KeyValuePair<int, Mail>> MailList
        {
            get { return mMailList; }
        }

        private bool bIfNewMail;

        public bool BIfNewMail
        {
            get { return bIfNewMail; }
            set { bIfNewMail = value; }
        }
        /// <summary>
        /// 添加邮件标题列表
        /// </summary>
        /// <param name="mailType"></param>
        /// <param name="mailId"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailContent"></param>
        public void AddMailData(int mailType, int mailId, string mailTitle, MailCurtStateEnum curtState)
        {
            Mail mail = new Mail();
            mail.mId = mailId;
            mail.mTitle = mailTitle;
            mail.mType = mailType;
            mail.CurrentState = curtState;

            KeyValuePair<int, Mail> kv = new KeyValuePair<int, Mail>(mailId, mail);
            mMailList.Add(kv);
            bIfNewMail = true;
        }
        ///填充邮件具体内容///
        public void UpdateMailInfo(int mailId, string mailContent, string mailGift, string sender, string createtime)
        {
            Mail mdb = null;
            foreach (KeyValuePair<int, Mail> kv in mMailList)
            {
                if (mailId == kv.Key)
                {
                    kv.Value.mContent = mailContent;
                    kv.Value.mGift = mailGift;
                    kv.Value.mSender = sender;
                    kv.Value.mCreateTime = createtime;
                    kv.Value.CurrentState = MailCurtStateEnum.Look;

                    mdb = kv.Value;
                    break;
                }
            }
            SortMailList();

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateMailInfo, mdb);
        }

        ///查看单封邮件，是否删除该邮件 ，并且 从新排序邮件
        public void ResetMailList(int mailId, bool ifDel, bool ifSort)
        {
            foreach (KeyValuePair<int, Mail> kv in mMailList)
            {
                if (mailId == kv.Key)
                {
                    if (ifDel)
                    {
                        mMailList.Remove(kv);
                    }
                    else
                    {
                        kv.Value.CurrentState = MailCurtStateEnum.Del;
                    }
                    break;
                }
            }

            SortMailList();

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateMailList);

            bIfNewMail = false;
        }

        private void StartSortMailList(List<KeyValuePair<int, Mail>> myList)
        {
            myList.Sort(delegate(KeyValuePair<int, Mail> s1, KeyValuePair<int, Mail> s2)
            {
                return s2.Key.CompareTo(s1.Key);
            });
        }
        //排序规则：
        //1 没有查看的在最前面，并且按照mailId从大到小
        //2 查看了的排在后边，并且按照mailId从大到小 
        public void SortMailList()
        {
            List<KeyValuePair<int, Mail>> notViewMailList = new List<KeyValuePair<int, Mail>>();
            List<KeyValuePair<int, Mail>> hasViewMailList = new List<KeyValuePair<int, Mail>>();
            foreach (var item in mMailList)
            {
                if (item.Value.CurrentState == MailCurtStateEnum.New)
                {
                    notViewMailList.Add(item);
                }
                else
                {
                    hasViewMailList.Add(item);
                }
            }
            mMailList.Clear();

            StartSortMailList(notViewMailList);
            StartSortMailList(hasViewMailList);

            mMailList.AddRange(notViewMailList);
            mMailList.AddRange(hasViewMailList);
            //设置默认选择的邮件id
            if (notViewMailList.Count > 0)
            {
                defaultMailid = notViewMailList[0].Key;
            }
            else if (hasViewMailList.Count > 0 )
            {
                defaultMailid = hasViewMailList[0].Key;
            }
        }

        public bool CheckMailContentByMailId(int mailId)
        {
            if (mMailList == null || mMailList.Count < 1)
            {
                return false;
            }
            Mail mdb = null;
            foreach (var item in mMailList)
            {
                if (item.Key == mailId)
                {
                    if (null == item.Value.mContent)
                        return false;
                    mdb = item.Value;
                    break;
                }
            }

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateMailInfo, mdb);
            return true;
        }
    }
}