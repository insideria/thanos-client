using UnityEngine;
using System.Collections.Generic;
using Thanos;

public class CPChanggeManager : MonoBehaviour {

    public const string cp_path = "Guis/Play/CPAddItem";

    public static CPChanggeManager Instance
    {
        private set;
        get;
    }

    public CPAddUI cpadd
    {
        private set;
        get;
    }
    int index = 5;
    public List<CPAddUI> CpAddList = new List<CPAddUI>(); //金币
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < index; i ++ )
        {
            CPAddUI addui = LoadCPAddPrefab(cp_path);
            addui.gameObject.transform.parent = transform;
            addui.gameObject.transform.localScale = Vector3.one;
            addui.gameObject.transform.localPosition = Vector3.zero;
            addui.gameObject.SetActive(false);
            CpAddList.Add(addui);
        }
    }
   
        public void ClearList()
    {
        foreach (var item in CpAddList)
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.localScale = Vector3.one;
            item.gameObject.transform.localPosition = Vector3.zero;
        }
    }


    public void CreateCPAdd(int count = 0)
    {
     
        if (count == 0)
        {
            return;
        }
       foreach (var item in CpAddList)
       {           
           if(item.gameObject.activeSelf == false)
           {
               item.Init();
               AudioManager.Instance.GetMoneyAudio();
               item.SetCPAdd_Count(count.ToString(), item.gameObject);
               return;
           }
       }
     
    }

     CPAddUI LoadCPAddPrefab(string path)
    {    
        GameObject obj = ObjectPool.Instance.GetGO(path);

        CPAddUI cpadd = obj.GetComponent<CPAddUI>();
        return cpadd;
    }
}
