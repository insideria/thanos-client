using UnityEngine;
using System.Collections;
using Thanos;
using GameDefine;

namespace Thanos.View
{
    public abstract class BaseWindow
    {
        protected Transform mRoot;

        protected EScenesType mScenesType; //场景类型
        protected string mResName;         //资源名
        protected bool mResident;          //是否常驻 
        protected bool mVisible = false;   //是否可见
      

        //类对象初始化
        public abstract void Init();

        //类对象释放
        public abstract void Realse();

        //窗口控制初始化
        protected abstract void InitWidget();

        //窗口控件释放
        protected abstract void RealseWidget();

        //游戏事件注册
        protected abstract void OnAddListener();

        //游戏事件注消
        protected abstract void OnRemoveListener();

        //显示初始化
        public abstract void OnEnable();

        //隐藏处理
        public abstract void OnDisable();

        //每帧更新
        public virtual void Update(float deltaTime) { }

        //取得所以场景类型
        public EScenesType GetScenseType()
        {
            return mScenesType;
        }
        //是否已打开
        public bool IsVisible() { return mVisible;  }

        //是否常驻
        public bool IsResident() { return mResident; }

        //显示
        public void Show()
        {
            //如果Mroot为空 创建窗体并初始化
            if (mRoot == null)
            {
                if (Create())
                {
                    // 获取窗口控件，添加UI事件
                    InitWidget();
                }
            }
            //窗口显示与游戏事件注册
            if (mRoot && mRoot.gameObject.activeSelf == false)
            {
                mRoot.gameObject.SetActive(true);
                mVisible = true;
                OnEnable();
                OnAddListener();
            }

        }

        //隐藏
        public void Hide()
        {
            if (mRoot && mRoot.gameObject.activeSelf == true)
            {
                OnRemoveListener();

                OnDisable();

                if (mResident)
                {
                    mRoot.gameObject.SetActive(false);
                }
                else
                {
                    RealseWidget();
                    Destroy();
                }
            }

            mVisible = false;
        }
        //预加载
        public void PreLoad()
        {
            if (mRoot == null)
            {
                if (Create())
                {
                    InitWidget();
                }
            }
        }

        //延时删除
        public void DelayDestory()
        {
            if (mRoot)
            {
                RealseWidget();
                Destroy();
            }
        }

        //创建窗体
        private bool Create()
        {
            if (mRoot)
            {
                Debug.LogError("Window Create Error Exist!");
                return false;
            }
            if (mResName == null || mResName == "")
            {
                Debug.LogError("Window Create Error ResName is empty!");
                return false;
            }
            if (GameMethod.GetUiCamera.transform== null)
            {
                Debug.LogError("Window Create Error GetUiCamera is empty! WindowName = " + mResName);
                return false;
            }

            //加载资源  根据每个Window中的资源名来加载（资源名在每个窗口的构造器中确定）
            GameObject obj = LoadUiResource.LoadRes(GameMethod.GetUiCamera.transform, mResName);
            if (obj == null)
            {
                Debug.LogError("Window Create Error LoadRes WindowName = " + mResName);
                return false;
            }
            mRoot = obj.transform;
            mRoot.gameObject.SetActive(false);
            return true;
        }

        //销毁窗体
        protected void Destroy()
        {
            if (mRoot)
            {
                LoadUiResource.DestroyLoad(mRoot.gameObject);
                mRoot = null;
            }
        }

        //取得根节点
        public Transform GetRoot()
        {
            return mRoot;
        }

    }
}

