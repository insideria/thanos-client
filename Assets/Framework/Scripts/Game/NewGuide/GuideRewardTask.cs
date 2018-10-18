using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Resource;

namespace Thanos.GuideDate
{
    public class GuideRewardTask : GuideTaskBase
    {
        private CRewardTask rewardTask = null;
        private GameObject objEffect = null;

        public GuideRewardTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {
            //读取数据

           rewardTask = ConfigReader.GetRewardTaskInfo(task);
           //objEffect = GameObject.Instantiate(Resources.Load(rewardTask.EffectPath)) as GameObject;

           ResourceItem objEffectUnit = ResourcesManager.Instance.loadImmediate(rewardTask.EffectPath, ResourceType.PREFAB);
           objEffect = GameObject.Instantiate(objEffectUnit.Asset) as GameObject;


           if (rewardTask.EffectPos == Vector3.zero) {
               objEffect.transform.parent = PlayerManager.Instance.LocalPlayer.realObject.transform;
               objEffect.transform.localPosition = Vector3.zero;
           }
           else {
//                if (UIPlay.Instance != null)
//                {
//                    objEffect.transform.parent = UIPlay.Instance.transform;
//                }
               objEffect.transform.localPosition = rewardTask.EffectPos;
           }
        }

        public override void EnterTask()
        {
            //if (rewardTask.RewardResult != 0) { 
            //    //发送消息请求加钱
            //}
        }


        public override void ExcuseTask()
        {

        }

        public override void ClearTask()
        {
            base.ClearTask();
            if (objEffect != null) {
                GameObject.DestroyObject(objEffect);
            }
            objEffect = null;
        }

    }


}
