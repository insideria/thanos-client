using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Holoville.HOTween;
using Thanos.Resource;

namespace Thanos.GuideDate

{
    public class GuideCameraTask : GuideTaskBase
    { 
        CameraMoveInfo mCamInfo;
        /// <summary>
        /// 要后续的摄像机移动的Id
        /// </summary>
        private int LastCameraMoveId;

        private int CameraMoveNextId;

        private GameObject objMark;

        //const int bTipsId = 1001;
        public GuideCameraTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {
            CameraMoveNextId = task;
            //CGLCtrl_GameLogic.Instance.EmsgToss_AskStopMove();
        }

        public override void EnterTask()
        {
            base.EnterTask();
            ResourceItem objMarkUnit = ResourcesManager.Instance.loadImmediate("Guis/Over/superwan", ResourceType.PREFAB);
            objMark = GameObject.Instantiate(objMarkUnit.Asset) as GameObject;
            objMark.transform.parent = GameMethod.GetUiCamera.transform;
            objMark.transform.localScale = Vector3.one;
            objMark.transform.localPosition = Vector3.zero;

            GameMethod.GetMainCamera.CameraMoving = DeltCameraMoveInfo() ? true : false;
            mUpTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 处理摄像机移动数据
        /// </summary>
        private bool DeltCameraMoveInfo()
        {
            if (!ConfigReader.GuideCameraMoveXmlInfoDict.TryGetValue(CameraMoveNextId, out mCamInfo))
            {
                this.FinishTask();
                return false;
            }
            GameMethod.GetMainCamera.transform.position = mCamInfo.mStartPos;
            Vector3 mLkDir = mCamInfo.mAspect - mCamInfo.mStartPos;
            GameMethod.GetMainCamera.transform.rotation = Quaternion.LookRotation(mLkDir.normalized);
            EnterCameraMoveTask();
            return true;
        }

        /// <summary>
        /// 摄像机开始阶段 移动
        /// </summary>
        private void EnterCameraMoveTask()
        {
            TweenParms parms = new TweenParms();
            parms.Prop("position", mCamInfo.mEndPos);
            Tweener tweener = HOTween.To(GameMethod.GetMainCamera.transform, mCamInfo.mNextTime, parms);
            tweener.easeType = EaseType.Linear;
        }

        /// <summary>
        /// 摄像机移动结束
        /// </summary>
        private void OnCameraMoveCompelect()
        {
            CameraMoveNextId = mCamInfo.mGoon;
            GameMethod.GetMainCamera.CameraMoving = DeltCameraMoveInfo() ? true : false;
        }


        float mUpTime;
        public override void ExcuseTask()
        {
            if (mCamInfo == null)
            {
                return;
            }
            if (Time.realtimeSinceStartup - mUpTime >= mCamInfo.mDurTime)
            {
                OnCameraMoveCompelect();
                mUpTime = Time.realtimeSinceStartup;
            }
        }

        //清理任务
        public override void ClearTask()
        {
            if (objMark != null)
            {
                GameObject.Destroy(objMark);
            }
            base.ClearTask();
            mCamInfo = null;
        }
    }


}
