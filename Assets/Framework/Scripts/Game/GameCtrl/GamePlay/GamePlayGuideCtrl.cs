using System;
using Thanos.Model;

namespace Thanos.Ctrl
{
    public class GamePlayGuideCtrl : Singleton<GamePlayGuideCtrl>
    {
        public void Enter()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayGuideEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_PlayGuideExit);
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化

        /// <summary>
        /// 向服务器发送要执行的任务Id
        /// </summary>
        public void AskSSGuideStepComp(GCToSS.AskSSGuideStepComp.edotype ebornsolder, int mTaskId)
        {
            HolyGameLogic.Instance.AskSSGuideStepComp(ebornsolder, mTaskId);
        }

        /// <summary>
        /// 开始执行模块的引导
        /// </summary>
        /// <param name="modelId"></param>
        public void StartModelTask(mGuideStepInfo modelId)
        {
            GamePlayGuideModel.Instance.StartModelTask(modelId);
        }

        /// <summary>
        /// 完成模块Id
        /// </summary>
        /// <param name="mdId"></param>
        /// <param name="comp"></param>
        public void GuideCSStepComplete(int mdId , bool allComp)
        {
            HolyGameLogic.Instance.GuideCSStepComplete(mdId, allComp);
            if (allComp)
            {
                UIGuideModel.Instance.mIsGuideComp = allComp;
                UIGuideModel.Instance.ClearModelData();
                GamePlayGuideModel.Instance.ClearModelData();
            }
        }

    }
}

