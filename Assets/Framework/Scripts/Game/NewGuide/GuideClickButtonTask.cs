using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using GameDefine;
using Thanos.Resource;

namespace Thanos.GuideDate
{
    public class GuideClickButtonTask : GuideTaskBase
    {
        private GuideButtonClickInfo mTaskInfo;
        private GameObject GameObjectEffect;

        public GuideClickButtonTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

            //读取数据

        }

        public override void EnterTask()
        {
            EventCenter.AddListener<GameObject>((Int32)GameEventEnum.GameEvent_GuideLockTargetCanAbsorb, OnGuideTaskEvents);
            EventCenter.AddListener<GameObject>((Int32)GameEventEnum.GameEvent_GuideLockTargetCanNotAbsorb, OnGuideTaskRemoveEvent);
            if (!ConfigReader.GuideButtonClickXmlDict.TryGetValue(mTaskId , out mTaskInfo))
            {
                this.FinishTask();
            }
        }

        /// <summary>
        /// 引导事件任务
        /// </summary>
        private void OnGuideTaskEvents(GameObject gObj)
        {
            if (GameObjectEffect == null)
            {
                ResourceItem Unit = ResourcesManager.Instance.loadImmediate(mTaskInfo.mEffects, ResourceType.PREFAB);
                GameObjectEffect = GameObject.Instantiate(Unit.Asset) as GameObject;
            }
            GameObjectEffect.transform.parent = gObj.transform;
            GameObjectEffect.transform.localScale = Vector3.one;
            GameObjectEffect.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// 把提示对象销毁掉关掉
        /// </summary>
        private void OnGuideTaskRemoveEvent(GameObject gObj)
        {
            if (GameObjectEffect != null)
            {
                GameObject.Destroy(GameObjectEffect);
            }
                
        }

        private void PressButton(int ie, bool isPress)
        {

        }

        public override void ExcuseTask()
        {

        }

        public override void ClearTask()
        {
            base.ClearTask();
            if (GameObjectEffect != null)
            {
                GameObject.Destroy(GameObjectEffect);
            }
            EventCenter.RemoveListener<GameObject>((Int32)GameEventEnum.GameEvent_GuideLockTargetCanAbsorb, OnGuideTaskEvents);
            EventCenter.RemoveListener<GameObject>((Int32)GameEventEnum.GameEvent_GuideLockTargetCanNotAbsorb, OnGuideTaskRemoveEvent);
        }
    }


}
