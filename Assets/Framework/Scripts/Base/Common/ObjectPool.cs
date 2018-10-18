using UnityEngine;
using System;
using System.Collections.Generic;
using Thanos;
using Thanos.Resource;
using Thanos.Effect;

//缓存物体类型
public enum PoolObjectTypeEnum
{
    Effect,
    MiniMap,
    Entity,
    UITip,
    BloodBar,
}

//缓存GameObject
public class ObjectItemInfo
{
    public string name;
    //缓存时间
    public float mCacheTime = 0.0f;
    //缓存物体类型
    public PoolObjectTypeEnum type;
    //如果是特效，特效的显示等级
    public EffectLodLevel lodLevel;

    #region //拖尾处理
    //可以重用
    public bool mCanUse = true;
    //重置时间
    public float mResetTime = .0f;
    //拖尾原始时间
    public Dictionary<TrailRenderer, float> mTrailTimes = new Dictionary<TrailRenderer, float>();
    #endregion
}

public class ObjectPool : Singleton<ObjectPool>
{
    //GameObject缓存池
    class ObjectInfo
    {
        //缓存队列
        public Dictionary<GameObject, ObjectItemInfo> mQueue = new Dictionary<GameObject, ObjectItemInfo>();
    }
    //缓存GameObject Map
    private Dictionary<String, ObjectInfo> mPoolDic = new Dictionary<String, ObjectInfo>();
    //缓存GameObject节点
    private GameObject objectsPool;

    private float mCachTime = 1800.0f;

    //删除队列
    private List<GameObject> mDestoryPoolGameObjects = new List<GameObject>();

    //取得可用对象
    private bool TryGetObject(ObjectInfo poolInfo, out KeyValuePair<GameObject, ObjectItemInfo> objPair)
    {
        if (poolInfo.mQueue.Count > 0)
        {
            foreach (KeyValuePair<GameObject, ObjectItemInfo> pair in poolInfo.mQueue)
            {
                GameObject go = pair.Key;
                ObjectItemInfo info = pair.Value;

                if (info.mCanUse)
                {
                    objPair = pair;
                    return true;
                }
            }
        }

        objPair = new KeyValuePair<GameObject, ObjectItemInfo>();
        return false;
    }

    //获取缓存物体
    public GameObject GetGO(String res)
    {
        //有效性检查
        if (null == res)
        {
            return null;
        }

        //查找对应pool，如果没有缓存
        ObjectInfo poolInfo = null;
        KeyValuePair<GameObject, ObjectItemInfo> pair;
        if (!mPoolDic.TryGetValue(res, out poolInfo) || !TryGetObject(poolInfo, out pair))
        {
            //新创建
            //Debug.LogError("res = " + res);
            ResourceItem unit = ResourcesManager.Instance.loadImmediate(res, ResourceType.PREFAB);
            if (unit.Asset == null)
            {
                Debug.Log("can not find the resource" + res);
            }
            return GameObject.Instantiate(unit.Asset) as GameObject;
        }

        //出队列数据
        GameObject go = pair.Key;
        ObjectItemInfo info = pair.Value;

        poolInfo.mQueue.Remove(go);

        //使有效
        EnablePoolGameObject(go, info);

        //返回缓存Gameobjec
        return go;
    }


    public void ReleaseGO(String res, GameObject go, PoolObjectTypeEnum type)
    {
        //获取缓存节点,设置为不可见位置
        if (objectsPool == null)
        {
            objectsPool = new GameObject("ObjectPool");
            objectsPool.AddComponent<UIPanel>();
            objectsPool.transform.localPosition = new Vector3(0, -5000, 0);
        }

        if (null == res || null == go) return;


        //拖尾处理
        if (go.tag == "nopool")
        {
            GameObject.Destroy(go);
            return;
        }

        ObjectInfo poolInfo = null;
        //没有创建 
        if (!mPoolDic.TryGetValue(res, out poolInfo))
        {
            poolInfo = new ObjectInfo();
            mPoolDic.Add(res, poolInfo);
        }


        ObjectItemInfo poolGameObjInfo = new ObjectItemInfo();
        poolGameObjInfo.type = type;
        poolGameObjInfo.name = res;

        //无效缓存物体
        DisablePoolGameObject(go, poolGameObjInfo);

        //保存缓存GameObject,会传入相同的go, 有隐患
        //poolInfo.mQueue.Add(go,poolGameObjInfo);           
        poolInfo.mQueue[go] = poolGameObjInfo;
    }

    //设置缓存物体无效
    public void EnablePoolGameObject(GameObject go, ObjectItemInfo info)
    {
        //特效Enable          
        if (info.type == PoolObjectTypeEnum.Effect)
        {
            go.SetActive(true);

            //prewarm的情况需要模拟一个周期运行
            if (go.tag == "prewarm")
            {
                go.transform.localPosition = new Vector3(0, -5000, 0);

                ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem part in particles)
                {
                    if (part.gameObject.tag == "prewarm" && part.main.loop)
                    {
                        part.Simulate(part.main.duration);
                        part.Play();
                    }
                }
                go.transform.localPosition = new Vector3(0, 0, 0);
            }

            //拖尾处理
            if (go.tag == "trail")
            {
                ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem part in particles)
                {
                    part.gameObject.SetActive(true);
                }

                XffectComponent[] xffects = go.GetComponentsInChildren<XffectComponent>(true);
                foreach (XffectComponent xffect in xffects)
                {
                    xffect.Active();
                }

                TrailRenderer[] trailRenders = go.GetComponentsInChildren<TrailRenderer>(true);
                foreach (TrailRenderer trail in trailRenders)
                {
                    if (info.mTrailTimes.ContainsKey(trail))
                        trail.time = info.mTrailTimes[trail];
                }

                MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>(true);
                foreach (MeshRenderer render in renders)
                {
                    render.gameObject.SetActive(true);
                }
            }

            go.transform.parent = null;
        }
        else if (info.type == PoolObjectTypeEnum.MiniMap)
        {
            go.SetActive(true);
            go.transform.parent = null;
        }
        else if (info.type == PoolObjectTypeEnum.Entity)
        {
            go.SetActive(true);
            go.transform.parent = null;
        }
        else if (info.type == PoolObjectTypeEnum.UITip)
        {
            go.SetActive(true);
            go.transform.parent = null;
        }
        else if (info.type == PoolObjectTypeEnum.BloodBar)
        {
            //do nothing
        }

        info.mCacheTime = 0.0f;
    }

    //设置缓存物体无效
    public void DisablePoolGameObject(GameObject go, ObjectItemInfo info)
    {
        //特效Disable
        if (info.type == PoolObjectTypeEnum.Effect)
        {
            ParticleSystem[] particles = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem part in particles)
            {
                part.Clear();
            }

            //解绑到poolGameobject节点
            go.transform.parent = objectsPool.transform;

            //拖尾处理
            if (go.tag == "trail")
            {
                info.mCanUse = false;

                TrailRenderer[] trailRenders = go.GetComponentsInChildren<TrailRenderer>();
                foreach (TrailRenderer trail in trailRenders)
                {
                    info.mTrailTimes[trail] = trail.time;
                    trail.time = -10;
                }

                XffectComponent[] xffects = go.GetComponentsInChildren<XffectComponent>(true);
                foreach (XffectComponent xffect in xffects)
                {
                    xffect.DoFinish();
                    xffect.gameObject.SetActive(false);
                }

                MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>(true);
                foreach (MeshRenderer render in renders)
                {
                    render.gameObject.SetActive(false);
                }

                foreach (ParticleSystem part in particles)
                {
                    part.gameObject.SetActive(false);
                }
            }
            else
            {
                info.mCanUse = true;
                go.SetActive(false);
            }

        }
        else if (info.type == PoolObjectTypeEnum.MiniMap)
        {
            //go.transform.parent = objectsPool.transform;   
            go.SetActive(false);
        }
        else if (info.type == PoolObjectTypeEnum.Entity)
        {
            //解绑到poolGameobject节点
            go.transform.parent = objectsPool.transform;
            go.SetActive(false);
        }
        else if (info.type == PoolObjectTypeEnum.UITip)
        {
            //解绑到poolGameobject节点
            go.transform.parent = objectsPool.transform;
            go.SetActive(false);
        }
        else if (info.type == PoolObjectTypeEnum.BloodBar)
        {
            go.transform.parent = objectsPool.transform;
            BloodBarUI xt = go.GetComponent<BloodBarUI>();
            xt.SetVisible(false);
        }
    }

    //清除
    public void Clear()
    {
        mPoolDic.Clear();
        mDestoryPoolGameObjects.Clear();
        objectsPool = null;
    }


    float mTotalTime = 0;
    public void OnUpdate()
    {
        //每隔0.1更新一次
        mTotalTime += Time.deltaTime;
        if (mTotalTime <= 0.1f)
        {
            return;
        }
        else
        {
            mTotalTime = 0;
        }



        float deltaTime = Time.deltaTime;

        //遍历数据
        foreach (ObjectInfo poolInfo in mPoolDic.Values)
        {
            //死亡列表
            mDestoryPoolGameObjects.Clear();

            foreach (KeyValuePair<GameObject, ObjectItemInfo> pair in poolInfo.mQueue)
            {
                GameObject obj = pair.Key;
                ObjectItemInfo info = pair.Value;

                info.mCacheTime += deltaTime;

                float mAllCachTime = mCachTime;

                //POT_UITip,缓存3600秒
                if (info.type == PoolObjectTypeEnum.UITip)
                    mAllCachTime = 3600;

                //缓存时间到
                if (info.mCacheTime >= mAllCachTime)
                {
                    mDestoryPoolGameObjects.Add(obj);
                }

                //拖尾重置计时
                if (!info.mCanUse)
                {
                    info.mResetTime += deltaTime;

                    if (info.mResetTime > 1.0f)
                    {
                        info.mResetTime = .0f;
                        info.mCanUse = true;

                        obj.SetActive(false);
                    }
                }
            }

            //移除
            for (int k = 0; k < mDestoryPoolGameObjects.Count; k++)
            {
                GameObject obj = mDestoryPoolGameObjects[k];
                //obj.transform.parent = null;
                GameObject.DestroyImmediate(obj);

                poolInfo.mQueue.Remove(obj);
            }
        }
    }
}