using UnityEngine;
using System.Collections;
using GameDefine;
using Thanos.Resource;

namespace Thanos.GameData
{
    public class LoadBaseDate
    {

        public GameObject BaseA
        {
            private set;
            get;
        }
        public GameObject BaseB
        {
            private set;
            get;
        }
        private static LoadBaseDate instance = null;
        public static LoadBaseDate Instance()
        {
            if (instance == null)
            {
                instance = new LoadBaseDate();
            }
            return instance;
        }

        //加载基地销毁特效，并设置为禁用
        public void LoadBase()
        {
            //基地A销毁特效
            ResourceItem BaseAUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.LoadGameBuildingEffectPath + "jidideath_A", ResourceType.PREFAB);
            BaseA = GameObject.Instantiate(BaseAUnit.Asset) as GameObject;

            //基地B销毁特效
            ResourceItem BaseBUnit = ResourcesManager.Instance.loadImmediate(GameConstDefine.LoadGameBuildingEffectPath + "jidideath_B", ResourceType.PREFAB);
            BaseB = GameObject.Instantiate(BaseBUnit.Asset) as GameObject;

            if (BaseA == null)
            {
                Debug.LogError("error  BaseA is null");
                return;
            }
            if (BaseB == null)
            {
                Debug.LogError("error  BaseB is null");
                return;
            }
            BaseB.gameObject.SetActive(false);
            BaseA.gameObject.SetActive(false);
           
        }
     
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
