using UnityEngine;
using System.Collections;
using Thanos.GameEntity;
using GameDefine;
using Thanos.GameData;
using Thanos.Tools;
using Thanos.GuideDate;
using System;

public class UIMiniMapPlayerElement : UIMiniMapElement
{
    private string smallIconName;

    private UISprite spriteSmallIcon;

    private Vector2 orignalSize = new Vector2();

    private Vector2 scaleSize = new Vector2(40f, 40f);

    private UISprite headBg;

    protected override void Awake()
    {
        base.Awake();
        spriteSmallIcon = gameMapPitch.GetComponent<UISprite>();//获取 UISprite组件
        orignalSize = new Vector2(spriteSmallIcon.width, spriteSmallIcon.height);
        smallIconName = spriteSmallIcon.spriteName;
        headBg = spriteSmallIcon.transform.Find("Sprite").GetComponent<UISprite>();
        headBg.gameObject.SetActive(false);
    }

    public override void CreateMiniMapElement(UInt64 guid, float x, float y, float z)
    {
        base.CreateMiniMapElement(guid, x, y, z);
    }


    private void OnEnable()
    {
        EventCenter.AddListener((Int32)GameEventEnum.GameEvent_ChangeMapState, ChangeMapState);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_ChangeMapState, ChangeMapState);
    }


    //改变地图状态 其实就是将图片禁用
    void ChangeMapState() { 

        UISprite sprite = headBg.transform.Find("Sprite").GetComponent<UISprite>();

        if (!EntityManager.AllEntitys.ContainsKey(mapTarget))
        {
            sprite.gameObject.SetActive(false);
            return;
        }

        IEntity entity = EntityManager.AllEntitys[mapTarget];
        //实体为空 或实体对象空  或 实体对象未激活
        if (entity == null || entity.realObject == null || !entity.realObject.activeInHierarchy)
        {
            sprite.gameObject.SetActive(false);
            return;
        }
        //全敌对类型的地图元素禁用
        if (entity.entityType != EntityTypeEnum.Player && entity.entityType != EntityTypeEnum.Soldier && entity.entityType != EntityTypeEnum.AltarSoldier)
        {
            sprite.gameObject.SetActive(false);
            return;
        }

        //实体为死亡状态
        if (entity.FSM != null && entity.FSM.State == Thanos.FSM.FsmStateEnum.DEAD)
        {
            sprite.gameObject.SetActive(false);
            return;
        }

        Vector3 pos = entity.realObject.transform.position; //获取实体位置
        UpdatePosDirect(pos.x, pos.y, pos.z);//将对象设置到目标点

       
        spriteSmallIcon.spriteName = smallIconName;
        spriteSmallIcon.SetDimensions((int)orignalSize.x, (int)orignalSize.y);//设置规模大小
        headBg.gameObject.SetActive(false);           
     
    }
}
    
