using GameDefine;
using Thanos.GameEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player {


    protected override void Start()
    {
        base.Start();
    }

    public override void OnFreeState()
    {
        OnCameraUpdatePosition();
        VirtualStickUI.Instance.SetVirtualStickUsable(true);      
        base.OnFreeState();
    }

    public override void OnRuntate()
    {
        GameObject entity = gameObject;
        Animation ani = this.gameObject.GetComponent<Animation>();
        if (ani != null)
        {
            ani.Play("walk");
        }
        Quaternion DestQuaternion = Quaternion.LookRotation(GOSSI.sServerDir);
        Quaternion sMidQuater = Quaternion.Lerp(entity.transform.rotation, DestQuaternion, 10 * Time.deltaTime);   
        entity.transform.rotation = sMidQuater;//实体方向旋转

        float fTimeSpan = Time.realtimeSinceStartup - GOSSI.fBeginTime;//时间间隔
        float fMoveDist = fTimeSpan * GOSSI.fServerSpeed;//在此间隔内移动的距离

        GOSSI.sServerSyncPos = GOSSI.sServerBeginPos + GOSSI.sServerDir * fMoveDist;//新位置


        //同步2D位置处理
        Vector3 serverPos2d = new Vector3(GOSSI.sServerSyncPos.x, 60, GOSSI.sServerSyncPos.z);
        Vector3 entityPos2d = new Vector3(entity.transform.position.x, 60, entity.transform.position.z);

        Vector3 sSyncDir = serverPos2d - entityPos2d;
        sSyncDir.Normalize();
        float fDistToServerPos = Vector3.Distance(serverPos2d, entityPos2d);

        if (fDistToServerPos > 5)
        {
            entity.transform.position = GOSSI.sServerSyncPos;//移动到新位置上
             OnCameraUpdatePosition();//摄像机更新跟随
            return;
        }
        Vector3 pos = entityPos2d + sSyncDir * GOSSI.fServerSpeed * Time.deltaTime;//在原位置上移动一帧后的新位置 ???
        entity.transform.position = pos;//????

        if (entity.GetComponent<CharacterController>())
        {
            entity.GetComponent<CharacterController>().Move(sSyncDir * GOSSI.fServerSpeed * Time.deltaTime);
        }
        else
        {
            entity.AddComponent<CharacterController>().Move(sSyncDir * GOSSI.fServerSpeed * Time.deltaTime);
        }
        entity.transform.position = new Vector3(entity.transform.position.x, 60, entity.transform.position.z);
       
        OnCameraUpdatePosition();
    }

    public override void OnDeadState()
    {
        VirtualStickUI.Instance.SetVirtualStickUsable(false); //禁用虚拟摇杆
        Animation ani = this.gameObject.GetComponent<Animation>();
        ani.Play("death");     
        //血条不可见  可以在显示对象的时候重新设置血条可见
        this.heroLife.SetActive(false);
        mHasLifeBar = false;
    }

    public override void OnEntityReleaseSkill()
    {
        base.OnEntityReleaseSkill();
    }

    public override void UpdateHp(Player player)
    {
        base.UpdateHp(player);
    }
    public override void UpdateMp(Player player)
    {
        base.UpdateMp(player);
    }

    protected void OnCameraUpdatePosition()
    {
        GameObject camera = GameObject.Find("Main Camera");
        if (camera!=null)
        {
            camera.transform.position = new Vector3(3,30,-26) + this.transform.position;
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
