using Thanos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : UnitySingleton<PlayersManager>
{
    public Player LocalPlayer { set; get; }
    public Player targetPlayer { get; set; }
    public  Dictionary<UInt64, Player> PlayerDic = new Dictionary<UInt64, Player>();

    //将Player添加到AccountDic中 
    public void AddDic(UInt64 sGUID, Player entity)
    {
        if (PlayerDic.ContainsKey(sGUID))
        {
            return;
        }
        PlayerDic.Add(sGUID, entity);
    }
}
