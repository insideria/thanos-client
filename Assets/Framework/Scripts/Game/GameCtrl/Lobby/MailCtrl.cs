using System;
using UnityEngine;
using Thanos.Network;
using Thanos.Model;
using Thanos.GameEntity;

namespace Thanos.Ctrl
{ 
    public class MailCtrl : Singleton<MailCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MailEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_MailExit);
        }

        //关闭邮件
        public void DelOrSortMailList(int mailId, bool ifDel, bool ifSort)
        {
            MailModel.Instance.ResetMailList(mailId, ifDel, ifSort);
        }
        public void DelOrSortMailList(GSToGC.DelAndSortMail pMsg)
        { 
            DelOrSortMailList(pMsg.mailid, pMsg.mailidDel, pMsg.sort);
        }
        //新增邮件
        public void AddMail(GSToGC.NotifyMailList pMsg)
        { 
            foreach(GSToGC.ShortMail mail in pMsg.mailList)
            {
                MailModel.Instance.AddMailData((int)mail.mailType, mail.mailid, mail.mailTitle, (MailCurtStateEnum)mail.mailState);
            } 
            //从新排序
            MailModel.Instance.SortMailList();

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_AddNewMailReq);

            Debug.Log("=====11111111111========");
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateMailList);
            Debug.Log("=====11111111111========");
        }
        //更新单封邮件
        public void UpdateMailInfo(GSToGC.MailInfo pMsg)
        {
            MailModel.Instance.UpdateMailInfo(pMsg.mailid, pMsg.mailcontent, pMsg.mailgift, pMsg.sender, pMsg.createTime); 
        }

        //获取单封邮件内容
        public void GetMailContent(int mailId)
        {
            if (mailId < 1)
            {
                Debug.LogWarning("mailId < 1" );
                return;
            }
            if (MailModel.Instance.CheckMailContentByMailId(mailId))
            {
                return;
            }

            GCToCS.AskMailInfo pMsg = new GCToCS.AskMailInfo()
            {
                mailId = mailId
            };
            NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);

            Debug.LogWarning("AskMailInfo:" + mailId);
        }
        //关闭 或者 领取奖励
        public void CloseOrGetMailGift(int mailId)
        {
            Debug.Log("CloseOrGetMailGift:" + mailId);

            GCToCS.AskGetMailGift pMsg = new GCToCS.AskGetMailGift()
            {
                mailId = mailId
            };
            NetworkManager.Instance.SendMsg(pMsg, (int)pMsg.msgnum);
            
        } 
        
    }
}
