using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Thanos.Resource
{
    public enum ResourceType
    {
        ASSET,
        PREFAB,
        LEVELASSET,
        LEVEL,
    }

    public class ResourceItem : IDisposable
    {
        private string mPath;
        private Object mAsset;
        private ResourceType mResourceType;
        private List<ResourceItem> mNextLevelAssets;
        private AssetBundle mAssetBundle;
        private int mAssetBundleSize;
        private int mReferenceCount;
        //private int mAllDependencesAssetSize;

        internal ResourceItem(AssetBundle assetBundle, int assetBundleSize, Object asset, string path, ResourceType resourceType/*, int allDependencesAssetSize*/)
        {
            mPath = path;
            mAsset = asset;
            mResourceType = resourceType;
            mNextLevelAssets = new List<ResourceItem>();
            mAssetBundle = assetBundle;
            mAssetBundleSize = assetBundleSize;
            mReferenceCount = 0;
            //mAllDependencesAssetSize = allDependencesAssetSize;
        }

        public Object Asset
        {
            get
            {
                return mAsset;
            }

            internal set
            {
                mAsset = value;
            }
        }

        public ResourceType resourceType
        {
            get
            {
                return mResourceType;
            }
        }

        public List<ResourceItem> NextLevelAssets
        {
            get
            {
                return mNextLevelAssets;
            }

            internal set
            {
                foreach (ResourceItem asset in value)
                {
                    mNextLevelAssets.Add(asset);
                }
            }
        }

        public AssetBundle Assetbundle
        {
            get
            {
                return mAssetBundle;
            }
            set
            {
                mAssetBundle = value;
            }
        }

        public int AssetBundleSize
        {
            get
            {
                return mAssetBundleSize;
            }
        }

        public int ReferenceCount
        {
            get
            {
                return mReferenceCount;
            }
        }

        public void dumpNextLevel()
        {
            string info = mPath + " the mReferenceCount : " + mReferenceCount + "\n";
            foreach (ResourceItem ru in mNextLevelAssets)
            {
                ru.dumpNextLevel();
                info += ru.mPath + "\n";
            }
            DebugEx.Log(info, ResourceCommon.DEBUGTYPENAME);
        }

        public void addReferenceCount()
        {
            ++mReferenceCount;
            foreach (ResourceItem asset in mNextLevelAssets)
            {
                asset.addReferenceCount();
            }
        }

        public void reduceReferenceCount()
        {
            --mReferenceCount;

            foreach (ResourceItem asset in mNextLevelAssets)
            {
                asset.reduceReferenceCount();
            }
            if (isCanDestory())
            {
                //ResourcesManager.Instance.mLoadedResourceItem.Remove(ResourceCommon.getFileName(mPath, true));
                Dispose();
            }
        }

        public bool isCanDestory() { return (0 == mReferenceCount); }

        public void Dispose()
        {
            ResourceCommon.Log("Destory " + mPath);

            if (null != mAssetBundle)
            {
                //mAssetBundle.Unload(true);
                mAssetBundle = null;
            }
            mNextLevelAssets.Clear();
            mAsset = null;
        }
    }

}