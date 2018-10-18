using System;
using UnityEngine;


namespace Thanos.Resource
{
    class ResourceItemMonitor : MonoBehaviour
    {
        public delegate void Construct(int instanceID, string name);
        public delegate void Destruct(int instanceID, string name);

        public static Construct mCHandle;
        public static Destruct mDHandle;

        void Awake()
        {
            mCHandle(gameObject.GetInstanceID(), gameObject.name);
        }

        void OnDestroy()
        {
            mDHandle(gameObject.GetInstanceID(), gameObject.name);
        }
    }
}
