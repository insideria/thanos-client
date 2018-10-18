using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Thanos.Resource
{
    public class ResourcesManager : UnitySingleton<ResourcesManager>
    {
        //是否通过assetbundle加载资源
        public bool UsedAssetBundle = false;

        private bool mInit = false;
        private int mFrameCount = 0;
        private ResourceRequest mCurrentRequest = null;
        private Queue<ResourceRequest> mAllRequests = new Queue<ResourceRequest>();

        //保存读取的Resource信息
        private AssetInfoManager mAssetInfoManager = null;
        private Dictionary<string, string> mResources = new Dictionary<string, string>();
        //加载的资源信息
        private Dictionary<string, ResourceItem> mLoadedResourceItem = new Dictionary<string, ResourceItem>();

        public delegate void HandleFinishLoad(ResourceItem resource);
        public delegate void HandleFinishLoadLevel();
        public delegate void HandleFinishUnLoadLevel();


        public void Init()
        {
            //使用assetbundle打包功能
            if (UsedAssetBundle)
            {
                mAssetInfoManager = new AssetInfoManager();
                mAssetInfoManager.LoadAssetInfo();

                ArchiveManager.Instance.Init();
            }

            mInit = true;
        }

        public void Update()
        {
            if (!mInit)
                return;

            if (null == mCurrentRequest && mAllRequests.Count > 0)
                handleRequest();

            ++mFrameCount;
            if (mFrameCount == 300)
            {
                mFrameCount = 0;
            }
        }

        private void handleRequest()
        {
            //使用assetbundle打包功能
            if (UsedAssetBundle)
            {
                mCurrentRequest = mAllRequests.Dequeue();

                //相对Asset的完整资源路径
                string fileName = mCurrentRequest.mFileName;

                switch (mCurrentRequest.mRequestType)
                {
                    case ResourceRequestType.LOAD:
                        {
                            switch (mCurrentRequest.mResourceType)
                            {
                                case ResourceType.ASSET:
                                case ResourceType.PREFAB:
                                    {
                                        if (mLoadedResourceItem.ContainsKey(fileName))
                                        {
                                            //(mLoadedResourceItem[fileName] as ResourceItem).addReferenceCount();

                                            mCurrentRequest.mResourceAsyncOperation.mComplete = true;
                                            mCurrentRequest.mResourceAsyncOperation.mResource = mLoadedResourceItem[fileName] as ResourceItem;

                                            if (null != mCurrentRequest.mHandle)
                                                mCurrentRequest.mHandle(mLoadedResourceItem[fileName] as ResourceItem);
                                            handleResponse();
                                        }
                                        else
                                        {
                                            //传入相对路径名称
                                            //StartCoroutine(_load(fileName, mCurrentRequest.mHandle, mCurrentRequest.mResourceType, mCurrentRequest.mResourceAsyncOperation));
                                        }
                                    }
                                    break;
                                case ResourceType.LEVELASSET:
                                    {
                                        DebugEx.LogError("do you real need a single level asset??? this is have not decide!!!", ResourceCommon.DEBUGTYPENAME);
                                    }
                                    break;
                                case ResourceType.LEVEL:
                                    {
                                        DebugEx.LogError("this is impossible!!!", ResourceCommon.DEBUGTYPENAME);
                                    }
                                    break;
                            }
                        }
                        break;
                    case ResourceRequestType.UNLOAD:
                        {
                            if (!mLoadedResourceItem.ContainsKey(fileName))
                                DebugEx.LogError("can not find " + fileName, ResourceCommon.DEBUGTYPENAME);
                            else
                            {
                                //(mLoadedResourceItem[fileName] as ResourceItem).reduceReferenceCount();
                            }
                            handleResponse();
                        }
                        break;
                    case ResourceRequestType.LOADLEVEL:
                        {
                            StartCoroutine(_loadLevel(fileName, mCurrentRequest.mHandleLevel, ResourceType.LEVEL, mCurrentRequest.mResourceAsyncOperation));
                        }
                        break;
                    case ResourceRequestType.UNLOADLEVEL:
                        {
                            if (!mLoadedResourceItem.ContainsKey(fileName))
                                DebugEx.LogError("can not find level " + fileName, ResourceCommon.DEBUGTYPENAME);
                            else
                            {
                                //(mLoadedResourceItem[fileName] as ResourceItem).reduceReferenceCount();

                                if (null != mCurrentRequest.mHandleUnloadLevel)
                                    mCurrentRequest.mHandleUnloadLevel();
                            }
                            handleResponse();
                        }
                        break;
                }
            }
            //不使用打包
            else
            {
                mCurrentRequest = mAllRequests.Dequeue();

                switch (mCurrentRequest.mRequestType)
                {
                    case ResourceRequestType.LOAD:
                        {
                            switch (mCurrentRequest.mResourceType)
                            {
                                case ResourceType.ASSET:
                                case ResourceType.PREFAB:
                                    {
                                        //暂时不处理，直接使用资源相对路径
                                        //StartCoroutine(_load(mCurrentRequest.mFileName, mCurrentRequest.mHandle, mCurrentRequest.mResourceType, mCurrentRequest.mResourceAsyncOperation));
                                    }
                                    break;
                                case ResourceType.LEVELASSET:
                                    {
                                        DebugEx.LogError("do you real need a single level asset??? this is have not decide!!!", ResourceCommon.DEBUGTYPENAME);
                                    }
                                    break;
                                case ResourceType.LEVEL:
                                    {
                                        DebugEx.LogError("this is impossible!!!", ResourceCommon.DEBUGTYPENAME);
                                    }
                                    break;
                            }
                        }
                        break;
                    case ResourceRequestType.UNLOAD:
                        {
                            handleResponse();
                        }
                        break;
                    case ResourceRequestType.LOADLEVEL:
                        {
                            StartCoroutine(_loadLevel(mCurrentRequest.mFileName, mCurrentRequest.mHandleLevel, ResourceType.LEVEL, mCurrentRequest.mResourceAsyncOperation));
                        }
                        break;
                    case ResourceRequestType.UNLOADLEVEL:
                        {
                            if (null != mCurrentRequest.mHandleUnloadLevel)
                                mCurrentRequest.mHandleUnloadLevel();
                            handleResponse();
                        }
                        break;
                }
            }
        }

        private void handleResponse()
        {
            mCurrentRequest = null;
        }

        //传入Resources下相对路径名称 例如Resources/Game/Effect1    传入Game/Effect1
        public ResourceItem loadImmediate(string filePathName, ResourceType resourceType, string archiveName = "Resources")
        {
            //使用assetbundle打包
            if (UsedAssetBundle)
            {
                //添加Resource
                string completePath = "Resources/" + filePathName;

                string completeName = ArchiveManager.Instance.getPath("Resources", completePath);

                //根据场景名称获取asset信息
                AssetInfo sceneAssetInfo = mAssetInfoManager.GetAssetInfo(completeName);

                //获取依赖的asset的索引
                foreach (int index in sceneAssetInfo.mDependencys)
                {
                    //根据索引获取依赖的Asset
                    AssetInfo depencyAsset = mAssetInfoManager.GetAssetInfo(index);
                    string depencyAssetName = depencyAsset.mName;

                    //加载场景依赖assetbundle


                    _LoadImmediate(depencyAssetName, ResourceType.ASSET);
                }

                //加载本身预制件
                ResourceItem unit = _LoadImmediate(completeName, resourceType);

                return unit;
            }
            //不使用
            else
            {
                Object asset = Resources.Load(filePathName);
                ResourceItem resource = new ResourceItem(null, 0, asset, null, resourceType);
                return resource;
            }
        }


        //fileName = "Scene/1"
        public ResourceAsyncOperation loadLevel(string fileName, HandleFinishLoadLevel handle, string archiveName = "Level")
        {
            //使用assetbundle打包
            if (UsedAssetBundle)
            {
                //获取完整路径
                string completeName = ArchiveManager.Instance.getPath(archiveName, fileName);
                if (mLoadedResourceItem.ContainsKey(completeName))
                {
                    DebugEx.LogError("why you load same level twice, maybe you have not unload last time!!!", ResourceCommon.DEBUGTYPENAME);
                    return null;
                }
                else
                {
                    ResourceAsyncOperation operation = new ResourceAsyncOperation(ResourceRequestType.LOADLEVEL);
                    mAllRequests.Enqueue(new ResourceRequest(completeName, ResourceType.LEVEL, handle, ResourceRequestType.LOADLEVEL, operation));
                    return operation;
                }
            }
            //不使用
            else
            {
                ResourceAsyncOperation operation = new ResourceAsyncOperation(ResourceRequestType.LOADLEVEL);
                mAllRequests.Enqueue(new ResourceRequest(fileName, ResourceType.LEVEL, handle, ResourceRequestType.LOADLEVEL, operation));
                return operation;
            }
        }

        private IEnumerator _loadLevel(string path, HandleFinishLoadLevel handle, ResourceType resourceType, ResourceAsyncOperation operation)
        {
            //使用assetbundle打包
            if (UsedAssetBundle)
            {
                //根据场景名称获取asset信息
                AssetInfo sceneAssetInfo = mAssetInfoManager.GetAssetInfo(path);
                //获取该包总大小
                operation.mAllDependencesAssetSize = mAssetInfoManager.GetAllAssetSize(sceneAssetInfo);

                //获取依赖的asset的索引
                foreach (int index in sceneAssetInfo.mDependencys)
                {
                    //根据索引获取依赖的Asset
                    AssetInfo depencyAsset = mAssetInfoManager.GetAssetInfo(index);
                    string depencyAssetName = depencyAsset.mName;

                    //加载场景依赖assetbundle
                    ResourceItem unit = _LoadImmediate(depencyAssetName, ResourceType.LEVEL);
                    operation.mLoadDependencesAssetSize += unit.AssetBundleSize;
                }

                //加载场景assetbundle     
                int scenAssetBundleSize = 0;
                byte[] binary = ResourceCommon.getAssetBundleFileBytes(path, ref scenAssetBundleSize);
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(binary);
                if (!assetBundle)
                    DebugEx.LogError("create scene assetbundle " + path + "in _LoadImmediate failed");

                //添加场景大小
                operation.mLoadDependencesAssetSize += scenAssetBundleSize;

                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(ResourceCommon.getFileName(path, false));
                operation.asyncOperation = asyncOperation;
                yield return asyncOperation;

                handleResponse();

                operation.asyncOperation = null;
                operation.mComplete = true;
                operation.mResource = null;

                if (null != handle)
                    handle();
            }
            //不使用
            else
            {
                ResourceItem level = new ResourceItem(null, 0, null, path, resourceType);

                //获取加载场景名称
                string sceneName = ResourceCommon.getFileName(path, true);
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                operation.asyncOperation = asyncOperation;
                yield return asyncOperation;

                handleResponse();

                operation.asyncOperation = null;
                operation.mComplete = true;

                if (null != handle)
                {
                    handle();
                }
            }
        }

        //单个资源加载
        ResourceItem _LoadImmediate(string fileName, ResourceType resourceType)
        {
            //没有该资源，加载
            if (!mLoadedResourceItem.ContainsKey(fileName))
            {
                //资源大小
                int assetBundleSize = 0;
                byte[] binary = ResourceCommon.getAssetBundleFileBytes(fileName, ref assetBundleSize);
                AssetBundle assetBundle = AssetBundle.LoadFromMemory(binary);
                if (!assetBundle)
                    DebugEx.LogError("create assetbundle " + fileName + "in _LoadImmediate failed");

                Object asset = assetBundle.LoadAsset(fileName);
                if (!asset)
                    DebugEx.LogError("load assetbundle " + fileName + "in _LoadImmediate failed");
                //调试用
                ResourceItem ru = new ResourceItem(assetBundle, assetBundleSize, asset, fileName, resourceType);
                //添加到资源中
                mLoadedResourceItem.Add(fileName, ru);
                return ru;
            }
            else
            {
                return mLoadedResourceItem[fileName];
            }
        }

        public static Stream Open(string path)
        {
            string localPath;
            //Andrio跟IOS环境使用沙箱目录
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                localPath = string.Format("{0}/{1}", Application.persistentDataPath, path + ResourceCommon.assetbundleFileSuffix);
            }
            //Window下使用assetbunlde资源目录
            else
            {
                localPath = ResourceCommon.assetbundleFilePath + path + ResourceCommon.assetbundleFileSuffix;
            }

            //首先检查沙箱目录中是否有更新资源
            if (File.Exists(localPath))
            {
                return File.Open(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            //没有的话原始包中查找
            else
            {
                TextAsset text = Resources.Load(path) as TextAsset;
                if (null == text)
                    DebugEx.LogError("can not find : " + path + " in OpenText", ResourceCommon.DEBUGTYPENAME);
                return new MemoryStream(text.bytes);
            }
        }

        public static StreamReader OpenText(string path)
        {
            return new StreamReader(Open(path), System.Text.Encoding.Default);
        }
    }
}