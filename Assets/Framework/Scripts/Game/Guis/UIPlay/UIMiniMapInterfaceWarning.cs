using UnityEngine;
using System.Collections;
using Thanos.GameEntity;
using GameDefine;
using Thanos.GameData;
using Thanos.Tools;
using System;

public class UIMiniMapInterfaceWarning : UIMiniMapElement
{
    protected bool startShow = false;

    protected float startShowTime = 0f;

    protected float showTimeLimit = 3f;   

    public override void CreateMiniMapElement(UInt64 guid, float x, float y, float z)
    {
        base.CreateMiniMapElement(guid, x, y, z);
        startShowTime = Time.time;
        startShow  = true;
    }  

    void Update() { 
        if(!startShow)
            return ;

        EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_RemoveMiniWarning, mapTarget);
    }
}
