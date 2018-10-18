using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using Thanos.Resource;

using Thanos.View;

namespace Thanos.GuideDate
{
    public class GuidePathTask : GuideTaskBase
    {

        private const float betweenLength = 2f;
        private const float perLength = 0.5f;
        private const int maxCount = 50;
        private const float startOffset = 1.2f;
        private const float arrowHeight = 2f;
        private const string pathPrefab = "effect/other/jiantou";
        private const string arrowPath = "effect/other/jiantou_2";

        private ISelfPlayer mPlayer = null;
        private GuidePathInfo mPathInfo;
        private List<GameObject> mArrowList = new List<GameObject>();
        private Vector3 targetScale = new Vector3(500f, 500f, 500f);
        private Vector3 desPos;
        private Vector3 mDirectionToDes;
        private GameObject objTargetArrow;

        public GuidePathTask(int task, GuideTaskType type, GameObject mParent)
            : base(task, type, mParent)
        {

        }


        public override void EnterTask()
        {
            if (!ConfigReader.GuidePathInfoDict.TryGetValue(mTaskId, out mPathInfo))
            {
                this.FinishTask();
                return;
            }
            mPlayer = PlayerManager.Instance.LocalPlayer;
            ResourceItem objTargetArrowUnit = ResourcesManager.Instance.loadImmediate(arrowPath, ResourceType.PREFAB);
            objTargetArrow = GameObject.Instantiate(objTargetArrowUnit.Asset) as GameObject;

            objTargetArrow.transform.position = new Vector3(mPathInfo.mDesPos.x, mPathInfo.mDesPos.y + arrowHeight, mPathInfo.mDesPos.z);
            objTargetArrow.transform.localScale = targetScale;
        }


        private void CreateArrowPrefab(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(pathPrefab, ResourceType.PREFAB);
                GameObject obj = GameObject.Instantiate(objUnit.Asset) as GameObject;

                obj.transform.localScale = Vector3.one;
                if (mArrowList.Count > 0)
                {
                    obj.transform.position = mArrowList.ElementAt(mArrowList.Count - 1).transform.position + betweenLength * mDirectionToDes;
                }
                else
                {
                    obj.transform.position = desPos + mDirectionToDes * startOffset * betweenLength;
                }
                mArrowList.Add(obj);
            }
        }

        private void UpdateArrowPos()
        {
            for (int i = 0; i < mArrowList.Count; i++)
            {
                GameObject obj = mArrowList.ElementAt(i);
                obj.transform.position = desPos + i * (betweenLength + perLength) * mDirectionToDes;
                obj.transform.rotation = Quaternion.LookRotation( -mDirectionToDes);
            }
        }

        private void DeleteArrowPrefab(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject obj = mArrowList.ElementAt(mArrowList.Count - 1);
                GameObject.DestroyObject(obj);
                mArrowList.RemoveAt(mArrowList.Count - 1);
            }
        }

        public override void ExcuseTask()
        {
            desPos = new Vector3(mPathInfo.mDesPos.x, mPlayer.realObject.transform.position.y, mPathInfo.mDesPos.z);
            float dis = Vector3.Distance(mPlayer.realObject.transform.position, desPos);
            if (dis < mPathInfo.mDistance)
            {
                base.FinishTask();
                return;
            }
            mDirectionToDes = (mPlayer.realObject.transform.position - desPos).normalized;
            //desPos += mDirectionToDes * startOffset * betweenLength;

            int totalCount = (int)(dis / (betweenLength + perLength));
            if (totalCount > mArrowList.Count)
            {
                CreateArrowPrefab(totalCount - mArrowList.Count);
            }
            else if (totalCount < mArrowList.Count)
            {
                DeleteArrowPrefab(mArrowList.Count - totalCount);
            }
            UpdateArrowPos(); 
        }

        public override void ClearTask()
        {
            base.ClearTask();
            DeleteArrowPrefab(mArrowList.Count);
            if (objTargetArrow != null)
            {
                GameObject.DestroyObject(objTargetArrow);
            }
            objTargetArrow = null;
            mPathInfo = null;
        }

    }


}

