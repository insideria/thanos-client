using GSToGC;
using Thanos;
using Thanos.GameEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player:MonoBehaviour{

    public Dictionary<SkillTypeEnum, int> skillDic = new Dictionary<SkillTypeEnum, int>();
    public Dictionary<int, GameObject> heroLifeDic = new Dictionary<int, GameObject>();
    public bool isRuning=false;
    private CGameObjectSyncInfo m_pcGOSSI = new CGameObjectSyncInfo();  
    public CGameObjectSyncInfo GOSSI
    {
        get
        {
            return m_pcGOSSI;
        }
    }
    public Transform objTransform = null;
    public UInt64 sGUID { get; set; }
    public UInt64 GameObjGUID { get; set; }
    public UInt64 ObjTypeID { get; set; }
    public GameObject LocalPlayer { get; set; }
    public Vector3 EntityFSMPosition{ get;set; }
    public Vector3 EntityFSMDirection { get; set; }
    public int EntitySkillID { get; set; }
    public  Player entitySkillTarget { get; set; }
     public GameObject  RealEntity
    {
        set;
        get;
    }
    public Transform objAttackPoint { private set; get; }//攻击点
    public Transform objBuffPoint { private set; get; }//buff点
    public Transform objPoint { private set; get; }//
    
    
    public float mPlayerSpeed { get; set; }
    public float Hp { get; set; }
    public float HpMax { private set; get; }
    public float Mp { private set; get; }
    public float MpMax { set; get; }
    public uint mPlayerID { get; set; }
    public GameObject heroLife;
    public bool mHasLifeBar = false;
    public bool mIsSkillCD = false;
    public bool mHasSkill1used = false;
    public bool mHasSkill2used = false;
    private UISprite mpSprite = null;
    private UISprite hpSprite = null;
    public Transform mSkill1Foreground = null;
    public Transform mSkill2Foreground = null;
    private float mSkill1CD = 10f;
    private float mSkill2CD=18f;
    
    //设置Mp   
    public virtual void SetMp(float curMp)
    {
        Mp = curMp;
    }
    //设置Hp
    public virtual void SetHp(float curHp)
    {
        Hp = curHp;
    }
    //设置Hp最大值
    public virtual void SetHpMax(float curHpMax)
    {
        HpMax = curHpMax;
    }
    //设置Mp最大值
    public virtual void SetMpMax(float curMpMax)
    {
        MpMax = curMpMax;
    }
    public virtual void SetSkillCD(float curCDTime){
        mSkill2CD = curCDTime;
    }
    
   
    public virtual void OnReliveState()
    {
        
        if (!mHasLifeBar)
        {
            this.heroLife.SetActive(true);
            mHasLifeBar = true;
        }

    }
    public virtual void OnFreeState(){
        if (RealEntity == null) return;
        if (!mHasLifeBar)
        {
            this.heroLife.SetActive(true);
            mHasLifeBar = true;
        }
        Vector2 serverPos2D = new Vector2(m_pcGOSSI.sServerBeginPos.x, m_pcGOSSI.sServerBeginPos.z);
        Vector2 objPos2D = new Vector2(objTransform.position.x, objTransform.position.z);
        float fDistToServerPos = Vector2.Distance(serverPos2D, objPos2D);
        if (fDistToServerPos > 10)//因为服务器可能对玩家的位置会有调整，所以调整位置
        {
            objTransform.position = m_pcGOSSI.sServerBeginPos;//按服务器的位置设置      
            objTransform.rotation = Quaternion.LookRotation(EntityFSMDirection);//方向调整。   
        }
        RealEntity.GetComponent<Animation>().Play("free");
    }

    public virtual void OnRuntate(){
        Animation ani = this.gameObject.GetComponent<Animation>();
        if (ani!=null)
        {
            ani.Play("walk");
        }
        float fThisRealTimeSinceStartup = Time.realtimeSinceStartup;//以秒计，自游戏开始的实时时间
        float fTimeSpan = fThisRealTimeSinceStartup - GOSSI.fBeginTime; //时间间隔

        float fThisFrameTimeSpan = fThisRealTimeSinceStartup - GOSSI.fLastSyncSecond;
        GOSSI.fLastSyncSecond = fThisRealTimeSinceStartup;

        float fMoveDist = fTimeSpan * GOSSI.fServerSpeed;//移动距离
        m_pcGOSSI.sServerSyncPos = GOSSI.sServerBeginPos + GOSSI.sServerDir * fMoveDist;//新的位置

        //2D计算
        Vector3 serverPos2D = new Vector3(m_pcGOSSI.sServerSyncPos.x, 60, m_pcGOSSI.sServerSyncPos.z);//要同步的位置，新的位置
        Vector3 realPos2D = new Vector3(objTransform.position.x, 60, objTransform.position.z);//实体现在的位置

        //需要同步的情况
        float fDistToSyncPos = Vector3.Distance(serverPos2D, realPos2D);// 距离差

        //大于10 的情况 直接赋值
        if (fDistToSyncPos > 10)
        {
            objTransform.position = m_pcGOSSI.sServerSyncPos; //原位置+方向上*距离差
            objTransform.rotation = Quaternion.LookRotation(EntityFSMDirection);
            m_pcGOSSI.sLocalSyncDir = EntityFSMDirection;
            return;
        }

        Vector3 sCrossPoint = 5 * m_pcGOSSI.sServerDir + m_pcGOSSI.sServerSyncPos;
        Vector3 sThisSyncDir = sCrossPoint - realPos2D;
        sThisSyncDir.y = 0;
        sThisSyncDir.Normalize();


        float fAngle = Vector3.Angle(sThisSyncDir, m_pcGOSSI.sLocalSyncDir);
        if (5 < fAngle)
        {
            float fLerpVlaue = fAngle / 100;
            Vector3 sTempThisSyncDir = Vector3.Lerp(m_pcGOSSI.sLocalSyncDir, sThisSyncDir, fLerpVlaue);
            sThisSyncDir = sTempThisSyncDir;
        }

        float fThisMoveSpeed = m_pcGOSSI.fServerSpeed;
        Vector3 sTempSyncDir = m_pcGOSSI.sServerSyncPos - realPos2D;
        sTempSyncDir.y = 0;
        sTempSyncDir.Normalize();


        fAngle = Vector3.Angle(sTempSyncDir, m_pcGOSSI.sLocalSyncDir);
        if (90 < fAngle)
        {
            if (1 > fDistToSyncPos)
            {
                fThisMoveSpeed = m_pcGOSSI.fServerSpeed;
            }
            else if (2 > fDistToSyncPos)
            {
                fThisMoveSpeed -= m_pcGOSSI.fServerSpeed * 0.01f;
            }
            else if (5 > fDistToSyncPos)
            {
                fThisMoveSpeed -= fThisMoveSpeed * 0.05f;
            }
            else if (6 > fDistToSyncPos)
            {
                fThisMoveSpeed -= fThisMoveSpeed * 0.10f;
            }
            fThisMoveSpeed = m_pcGOSSI.fServerSpeed;
        }
        else
        {
            if (1 > fDistToSyncPos)
            {
                fThisMoveSpeed = m_pcGOSSI.fServerSpeed;
            }
            else if (2 > fDistToSyncPos)
            {
                fThisMoveSpeed += fThisMoveSpeed * 0.05f;
            }
            else if (4 > fDistToSyncPos)
            {
                fThisMoveSpeed += fThisMoveSpeed * 0.10f;
            }
            else if (6 > fDistToSyncPos)
            {
                fThisMoveSpeed += fThisMoveSpeed * 0.20f;
            }
            fThisMoveSpeed = m_pcGOSSI.fServerSpeed;
        }

        float fDistDiff = Mathf.Abs(fThisMoveSpeed - m_pcGOSSI.fServerSpeed);
        if (0.1 <= fDistDiff)
        {
            Debug.Log("GO " + GameObjGUID + " fThisMoveSpeed:" + fThisMoveSpeed);
        }

        //位置计算
        float fThisSyncDist = fThisMoveSpeed * fThisFrameTimeSpan;//距离差  移动速度*时间间隔
        Vector3 sNewPos = sThisSyncDir * fThisSyncDist + realPos2D;//新位置     原位置+方向上*距离差

        if (sNewPos.magnitude > 0)
        {
            objTransform.position = sNewPos;  //位置设定
        }
        m_pcGOSSI.sLocalSyncDir = sThisSyncDir;
        Quaternion DestQuaternion = Quaternion.LookRotation(sThisSyncDir);
        Quaternion sMidQuater = Quaternion.Lerp(objTransform.rotation, DestQuaternion, 3 * Time.deltaTime);
        objTransform.rotation = sMidQuater;
    }

    public virtual void OnEntityReleaseSkill()
    {
        SkillManagerConfig skillManagerConfig = ConfigReader.GetSkillManagerCfg(EntitySkillID);
        Animation ani = RealEntity.GetComponent<Animation>();
        if (skillManagerConfig.isNormalAttack == 1)//判断是否是普通攻击
        {      
           ani.Play("attack");
        }
        else//技能攻击
        {
            ani.Play(skillManagerConfig.rAnimation.ToString());//播放释放技能动画
            //如果此动画不是循环模式
            if (RealEntity.GetComponent<Animation>()[skillManagerConfig.rAnimation] != null && RealEntity.GetComponent<Animation>()[skillManagerConfig.rAnimation].wrapMode != WrapMode.Loop)
            {
                ani.CrossFadeQueued("free");//淡入free动画
            }       
        } 
        objTransform.rotation = Quaternion.LookRotation(EntityFSMDirection);
    }

    public virtual void OnDeadState()
    {
        //目标死亡主角相关处理
        Player player = PlayersManager.Instance.LocalPlayer;
        Animation ani = this.gameObject.GetComponent<Animation>();//播放死亡动画
        ani.Play("death");    
    }

    public void EntityChangedata(Vector3 mvPos, Vector3 mvDir)
    {

        EntityFSMPosition = mvPos;
        EntityFSMDirection = mvDir;
    }

    public void EntityChangeDataOnPrepareSkill(Vector3 mvPos, Vector3 mvDir, int skillID, Player targetID)
    {
        EntityFSMPosition = mvPos;
        EntityFSMDirection = mvDir;
        EntitySkillID = skillID;
        entitySkillTarget = targetID;
    }

    public void EntityFSMChangeDataOnDead(Vector3 mvPos, Vector3 mvDir)
    {
        EntityFSMPosition = mvPos;
        EntityFSMDirection = mvDir;
    }

    public void showHeroLifePlate(GOAppear.AppearInfo info)
    {

        String path = "Prefab/HeroLifePlateRed";
        if (PlayersManager.Instance.LocalPlayer)
        {
            if (PlayersManager.Instance.LocalPlayer.GameObjGUID == info.objguid)
            {
                path = "Prefab/HeroLifePlateGreen";
            }
        }
        mHasLifeBar = true;
        GameObject heroLifeModel = Resources.Load(path) as GameObject;
        heroLife = GameObject.Instantiate(heroLifeModel) as GameObject;
        heroLifeDic.Add((int)info.obj_type_id,heroLife);
        hpSprite = heroLife.transform.Find("Control_Hp/Foreground").GetComponent<UISprite>();//绿
        mpSprite = heroLife.transform.Find("Control_Mp/Foreground").GetComponent<UISprite>();
    }

    public virtual void  UpdateHp(Player player)
    {
        hpSprite.fillAmount =player.Hp/player.HpMax ;
    }

    public virtual void UpdateMp(Player player)
    {
        mpSprite.fillAmount = player.Mp / player.MpMax;
    }
    Vector3 WorldToUI(int heroId)
    {
        Vector3 ff= new Vector3 (0,0,0);
        float height = GetXueTiaoHeight((uint)heroId);
        Player player = null;
        foreach (var playerItem in PlayersManager.Instance.PlayerDic)
        {
            if (playerItem.Value.ObjTypeID == (ulong)heroId)
            {
                player = playerItem.Value;
                break;
            }
        }
        Vector3 pt = player.RealEntity.transform.position;
        pt += new Vector3(0f, height, 0f);
        var posScreen = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(pt);

        GameObject xuetiao;
        heroLifeDic.TryGetValue((int)heroId, out xuetiao);
        if (player)
        {
            ff = NGUITools.FindCameraForLayer(xuetiao.layer).ScreenToWorldPoint(posScreen);
            ff.z = 0;
        }
        return ff;
    }

    float GetXueTiaoHeight(UInt64 guidType)
    {
        HeroConfigInfo info = ConfigReader.GetHeroInfo((int)ObjTypeID);
        return info.HeroXueTiaoHeight;
    }

    public void InitSkillDic()
    {
        int id = (int)ObjTypeID;
        HeroConfigInfo heroInfo = ConfigReader.GetHeroInfo(id);
        skillDic.Add(SkillTypeEnum.SKILL_TYPE1, heroInfo.HeroSkillType1);
        skillDic.Add(SkillTypeEnum.SKILL_TYPE2, heroInfo.HeroSkillType2);
        skillDic.Add(SkillTypeEnum.SKILL_TYPE3, heroInfo.HeroSkillType3);
        skillDic.Add(SkillTypeEnum.SKILL_TYPE4, heroInfo.HeroSkillType4);
        skillDic.Add(SkillTypeEnum.SKILL_TYPEABSORB1, 0);
        skillDic.Add(SkillTypeEnum.SKILL_TYPEABSORB2, 0);



    }

    public void InitSkillBG()
    {
        mSkill1Foreground.gameObject.SetActive(false);
        mSkill2Foreground.gameObject.SetActive(false);
    }

    public void UpdateHpChange(byte reason, float curHp)
    {
        float changeValue = Hp - curHp;
        SetHp((float)curHp);
       
    }

    protected virtual void Start()
    {
        objAttackPoint = transform.Find("hitpoint");
        objBuffPoint = transform.Find("buffpoint");
        objPoint = transform.Find("point");

        mSkill1Foreground = GameObject.Find("button_skill_1").transform.Find("Foreground");
        mSkill2Foreground = GameObject.Find("button_skill_2").transform.Find("Foreground");
        InitSkillBG();
       
    }
    float timer1=0;
    float timer2 = 0;
    protected virtual void Update()
    {
        if (isRuning)
        {
            OnRuntate();
        }

        if (heroLifeDic.Count > 0)
        {
            foreach (var item in heroLifeDic)
            {
                item.Value.transform.position = WorldToUI(item.Key);
            }
        }

        if (mHasSkill1used)
        {
            timer1 += Time.deltaTime;
            if (timer1 >= mSkill2CD)
            {
                timer1 = mSkill2CD;
            }
            mSkill1Foreground.gameObject.GetComponent<UISprite>().fillAmount = 1 - timer1 / mSkill1CD;            
        }
        if (mHasSkill2used)
        {
            timer2 += Time.deltaTime;
            if (timer2 >= mSkill2CD)
            {
                timer2 = mSkill2CD;
            }
            mSkill2Foreground.gameObject.GetComponent<UISprite>().fillAmount =1- timer2/ mSkill2CD;
        }

        if (mSkill1Foreground.gameObject.GetComponent<UISprite>().fillAmount <= 0.05)
        {
            mHasSkill1used = false;
            mSkill1Foreground.gameObject.SetActive(false);
            timer1 = 0;
        }

        if (mSkill2Foreground.gameObject.GetComponent<UISprite>().fillAmount <= 0.05)
        {
            mHasSkill2used = false;
            mSkill2Foreground.gameObject.SetActive(false);
            timer2 = 0;
        }
    }
}
public class CGameObjectSyncInfo
{
    public float fServerSpeed;
    public Vector3 sServerBeginPos;
    public Vector3 sServerSyncPos;
    public Vector3 sServerDir;
    public Vector3 sLocalSyncDir;
    public float fLastSyncSecond;
    public float fBeginTime;
    public float fDistMoved;
    public float fForceMoveSpeed;
};