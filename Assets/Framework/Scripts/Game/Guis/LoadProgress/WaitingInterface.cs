using System;
using UnityEngine;

public class WaitingInterface : MonoBehaviour {


    /// <summary>
    /// 连接实例
    /// </summary>
    public static WaitingInterface Instance
    {
        private set;
        get;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Instance = this;
        EventCenter.AddListener((Int32)GameEventEnum.GameEvent_EndWaiting, DestorySelf);   
    }

    void OnDisble()
    {
        EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_EndWaiting, DestorySelf);   
    }

    void OnDestroy()
    {
        EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_EndWaiting, DestorySelf);   
    }

    void DestorySelf()
    {
         DestroyImmediate(gameObject);
    }
}
