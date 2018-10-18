using System;
using UnityEngine; 
using Thanos;
using Thanos.GameState;

public class MiniMapManager : Singleton<MiniMapManager>
{
    const float updateTime = 1f;
    float timeStart = 0f;
    bool tagCheck = false;
    public MiniMapManager() {
        EventCenter.AddListener<Thanos.FEvent>((Int32)GameEventEnum.Loading, this.OnEvent);
         EventCenter.AddListener<UInt64>((Int32)GameEventEnum.GameOver, OnGameOver);         
    }

    public void Update() {
        if (!tagCheck)
            return;
        if (Time.time - timeStart >= updateTime) {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_UpdateMiniMap);
            timeStart = Time.time;
        }
    }

    private void OnGameOver(UInt64 BaseGuid)
    {
        tagCheck = false;
    }

    private void OnEvent(FEvent evt)
    {
        switch ((GameEventEnum)evt.GetEventId())
        {
            case GameEventEnum.Loading:
                {
                    GameStateTypeEnum stateType = (GameStateTypeEnum)evt.GetParam("NextState");
                    if (stateType != GameStateTypeEnum.Play)
                        return;
                    timeStart = Time.time;
                    tagCheck = true;
                }
                break;
        }
    }


    

}

