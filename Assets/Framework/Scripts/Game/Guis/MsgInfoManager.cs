using UnityEngine;
using GameDefine;
using Thanos.GameEntity;
using System.Collections.Generic;
using Thanos;
using System;
using Thanos.Resource;

public class MsgInfoManager
{
    public static MsgInfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MsgInfoManager();
            }
            return instance;
        }
    }
    public const float Alpha = 255f;
    private static MsgInfoManager instance;
    private const string res = "Guis/" + "JumpDigit";
    private Msg localMsg;
    private Queue<Msg> worldMsg = new Queue<Msg>();
    private Queue<KillsDate> KillMsgQueue = new Queue<KillsDate>(10);
    private MsgKill killmsginfo;
    public static bool isPlayEffect = true;
    public static float EffectTime = 0f;
    private KillsDate data;
    public enum MsgType
    {
        MsgType1 = 1,//本地提示
        MsgType2 = 2,//世界系统提示,阵营系统提示
        MsgType4 = 4,
        //MsgType3,//击杀提示
    }
    public enum eKillMsgType
    {
        eKillBuild,
        eKillNormal,
        eKillDouble,
        eKillThree,
        eFirstBlood,
    }
    struct KillsDate
    {
        public eKillMsgType KillsType;
        public bool isAced;
        public bool isFirstBlood;
        public string NameKill;
        public string NameDeath;
        public MsgKill msgkill;
        public string ReadXml;
        public EHeroKillTitle Title;
    }
    public enum AudioPlayType
    {
        KillAudio = 1,//按比例播放
        FuhuoAudio,
        TwentySconde,
    }
    public string GetNameGame(UInt64 targetID)
    {
        string namedead = null;
        namedead = BattleingData.Instance.GetGUIDName(targetID, EntityCampTypeEnum.A);
        if (namedead == null)
            namedead = BattleingData.Instance.GetGUIDName(targetID, EntityCampTypeEnum.B);
        return namedead;
    }

    public void RemoveKillMsg()
    {
        for (int i = KillMsgQueue.Count; i > 0; i--)
        {
            KillMsgQueue.Dequeue();
        }
    }
    public void SetAuido(eKillMsgType type, bool isaced )
    {
        switch ((eKillMsgType)type)
        {
            case eKillMsgType.eKillNormal:
                break;
            case eKillMsgType.eKillDouble:
                AddAuido("Double_kill");
                break;
            case eKillMsgType.eKillThree:
                AddAuido("Triple_kill");
                break;
            case eKillMsgType.eFirstBlood:
                AddAuido("First_blood");
                break;
            case eKillMsgType.eKillBuild:
                break;
        }
        if (isaced)
        {
            AddAuido("Aced");
        }
        
    }
    void AddAuido(string audio_name)
    {
        string path = "Audio/AudioKill/";
        if (audio_name == null || path == null) return;
        path = path + audio_name;
        
        //AudioClip clip = (AudioClip)Resources.Load(path) as AudioClip;
        //AudioManager.Instance.PlayGameKillAudio(clip);


        ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.ASSET);
        AudioClip clip = clipUnit.Asset as AudioClip;

        AudioManager.Instance.PlayGameKillAudio(clip);
    }
    public void SetKillDeathPlayer(Byte killstate, int campID, UInt64 targetID, UInt64 killID, bool Aced, EHeroKillTitle Title)
    {
        MsgInfoManager.eKillMsgType m_type = (eKillMsgType)killstate;
        string namekill = null;
        string readXml = null;
        EntityCampTypeEnum Type = (EntityCampTypeEnum)campID;
        string namedead = null;
        SetAuido(m_type, Aced);
        if (Type == EntityCampTypeEnum.Bad)
        {
            namekill = "中立势力";
            readXml = ConfigReader.GetMsgInfo(10009).content;
            m_type = MsgInfoManager.eKillMsgType.eKillBuild;
        }
        else
        {
            readXml = ConfigReader.GetMsgInfo(10008).content;
        }

        if ((int)campID % 2 == 0) { Type = EntityCampTypeEnum.B; }
        else { Type = EntityCampTypeEnum.A; }
        namedead = GetNameGame(targetID);
        if(namekill == null)
            namekill = GetNameGame(killID);
        if (namekill != null)
        {
            MsgInfoManager.Instance.SetAudioPlay(killID, AudioPlayType.TwentySconde);
        }
        else
        {
            if (Type == EntityCampTypeEnum.A)
            {
                namekill = "精灵势力";
            }
            else
            {
                namekill = "亡灵势力";
            }
            m_type = MsgInfoManager.eKillMsgType.eKillBuild;
        }
        if (namekill == null || namedead == null || readXml == null)
            return;
        if (Aced)
        {
            MsgInfoManager.Instance.SetKills(m_type, Aced, namekill, namedead, readXml, Title);
            Aced = false;
        }

        MsgInfoManager.Instance.SetKills(m_type, Aced, namekill, namedead, readXml, Title);
    }
    public void SetKills(eKillMsgType killType, bool isaced, string nameKill, string nameDeath, string readXml, EHeroKillTitle Title = EHeroKillTitle.eNoneTitel)
    {
        KillsDate killdate = new KillsDate();
        killdate.KillsType = killType;
        killdate.isAced = isaced;
        killdate.NameKill = nameKill;
        killdate.NameDeath = nameDeath;
        killdate.ReadXml = readXml;
        killdate.Title = Title;
        killdate.msgkill = new MsgKill();
        if (KillMsgQueue.Count <= 5)
        {
            KillMsgQueue.Enqueue(killdate);
            SetKillShow();
        }
    }
    void SetKillShow()
    {
        if (isPlayEffect && KillMsgQueue != null && KillMsgQueue.Count > 0)
        {
            KillsDate temp = KillMsgQueue.Dequeue();
            data = temp;
            temp.msgkill.Init();
            temp.msgkill.ShowMsgInfo(temp.KillsType, temp.isAced, temp.NameKill, temp.NameDeath, temp.ReadXml,temp.Title);
        }
    }

    public class MsgKill
    {
        public static string res = "Guis/play/" + "PlayerKillTip";
        public GameObject root;
        UILabel NameLabelKill;
        UILabel NameLabelDead;
        GameObject objKill;
        GameObject objAced;
        GameObject objDoubleKill;
        GameObject objThrebleKill;
        GameObject objFirstBlood;
        UILabel NameDate;
        UILabel SerialKill = null;
        Animation Anima;


        public void Init()
        {
            if (root == null)
            {
                //ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(res, ResourceType.PREFAB);
                //if (objUnit != null && objUnit.Asset != null)
                //{
                //    root = GameObject.Instantiate(objUnit.Asset) as GameObject;
                //}
            

                root = ObjectPool.Instance.GetGO(res);
            }
            NameLabelKill = root.transform.Find("name/Name1").gameObject.GetComponent<UILabel>();
            NameLabelDead = root.transform.Find("name/Name2").gameObject.GetComponent<UILabel>();
            NameDate = root.transform.Find("name/NameDate").GetComponent<UILabel>();
            SerialKill = root.transform.Find("name/SerialKill").GetComponent<UILabel>();
            Anima = NameDate.transform.parent.GetComponent<Animation>();
            Anima.enabled = false;
            objKill = root.transform.Find("kill").gameObject;
            objAced = root.transform.Find("aced").gameObject;
            objDoubleKill = root.transform.Find("dk").gameObject;
            objThrebleKill = root.transform.Find("tk").gameObject;
            objFirstBlood = root.transform.Find("fb").gameObject;
            objKill.SetActive(false);
            objAced.SetActive(false);
            objDoubleKill.SetActive(false);
            objThrebleKill.SetActive(false);
            objFirstBlood.SetActive(false);
            root.transform.parent = GameMethod.GetUiCamera.transform;
            root.transform.localScale = Vector3.one;
            root.transform.localPosition = Vector3.zero;
        }
        void SetLabel(bool isVib)
        {
            NameLabelKill.gameObject.SetActive(isVib);
            NameLabelDead.gameObject.SetActive(isVib);
            objKill.SetActive(isVib);
            NameDate.gameObject.SetActive(!isVib);
        }

        public void ShowMsgInfo(eKillMsgType type, bool isaced, string namekill, string namedeath, string readXml, EHeroKillTitle Title)
        {
            SetLabel(type != eKillMsgType.eKillBuild);
            NameLabelKill.text = namekill;
            NameLabelDead.text = namedeath;
            string audio_name = null;
            SerialKill.gameObject.SetActive(false);
            if (Title != 0 && Title != EHeroKillTitle.eNoneTitel)
            {
                SerialKill.gameObject.SetActive(true);
                SerialKill.text = ConfigReader.GetMsgInfo((int)Title).content;
            }
            if (isaced == false)
            {
                switch ((eKillMsgType)type)
                {
                    case eKillMsgType.eKillNormal:
                        break;
                    case eKillMsgType.eKillDouble:
                        objDoubleKill.SetActive(true);
                        //audio_name = "Double_kill";
                        break;
                    case eKillMsgType.eKillThree:
                        objThrebleKill.SetActive(true);
                        //audio_name = "Triple_kill";
                        break;
                    case eKillMsgType.eFirstBlood:
                        objFirstBlood.SetActive(true);
                        //audio_name = "First_blood";
                        break;
                    case eKillMsgType.eKillBuild:
                        SetLabel(type == eKillMsgType.eKillBuild);
                        namedeath = string.Format(readXml, namekill, namedeath);
                        NameDate.text = namedeath;
                        break;
                }
            }
            else if (isaced)
            {
                objAced.SetActive(true);
                //audio_name = "Aced";
            }
            Anima.enabled = true;
            EffectTime = 3f;
            isPlayEffect = false;
        }
    }
    class Msg
    {
        private UILabel Content;
        public UISprite frame;
        private GameObject root;
        UITweener UitPos;
        UITweener UitRot;
        UITweener UitSca;
        UITweener UitAlp;
        public MsgSettingConfigInfo SettingEnd;
        public MsgConfigInfo msgInfo;
        public int msgID;
        UITweener tweenEnd = null;
        public System.Action<Msg> msgEndEvent;
        public void Init()
        {
            if (root == null)
            {
                //GameObject obj = Resources.Load(res) as GameObject;
                //if (obj != null)
                //{
                //    root = GameObject.Instantiate(obj) as GameObject;
                //    Content = root.GetComponentInChildren<UILabel>();
                //    frame = root.transform.FindChild("digitLable/frame").GetComponent<UISprite>();
                //}

                ResourceItem objUnit = ResourcesManager.Instance.loadImmediate(res, ResourceType.PREFAB);
                if (objUnit != null && objUnit.Asset != null)
                {
                    root = GameObject.Instantiate(objUnit.Asset) as GameObject;
                    Content = root.GetComponentInChildren<UILabel>();
                    frame = root.transform.Find("digitLable/frame").GetComponent<UISprite>();               
                }
            }
        }
        public void MsgTypeTip(MsgConfigInfo info, MsgSettingConfigInfo setting)
        {
            msgInfo = info;
            SettingEnd = setting;
            Content.text = info.content;
            //Debug.LogError("text  =" + Content.text + "width = " + Content.width + "height = " + Content.height);
            SetLabelShow(Content, setting, info);
            SetShowInfo(setting);
        }

        void SetLabelShow(UILabel worldLocalLabel, MsgSettingConfigInfo setting, MsgConfigInfo info)
        {
            worldLocalLabel.fontSize = setting.font_size;
            worldLocalLabel.color = setting.color;
            worldLocalLabel.alpha = setting.alpha_start_in / Alpha;
            worldLocalLabel.effectStyle = (UILabel.Effect)setting.font_effect;
            worldLocalLabel.effectColor = setting.fontEffect_color;
            worldLocalLabel.effectDistance = setting.fontEffect_distance;
            worldLocalLabel.depth = 100;
            root.transform.parent = GameMethod.GetUiCamera.transform;
            root.transform.localScale = setting.scale_start_in;
            root.transform.localPosition = setting.position_start_in;
            root.gameObject.SetActive(true);
            if (setting.if_frame != 0)
            {
                frame.SetDimensions((info.content.Length * setting.font_size + 80), setting.font_size + 15);
                frame.gameObject.SetActive(true);
            }
            else
                frame.gameObject.SetActive(false);
        }

        void SetShowInfo(MsgSettingConfigInfo setting)
        {
            ClearEvent();
            UitPos = TweenPosition.Begin(root, setting.position_time_in, setting.position_end_in);
            EventDelegate.Add(UitPos.onFinished, ShowInfoTweenPosition);
            UitRot = TweenRotation.Begin(root, setting.rotation_time_in, Quaternion.Euler(setting.rotation_end_in));
            EventDelegate.Add(UitRot.onFinished, ShowInfoTweenRotation, true);
            UitSca = TweenScale.Begin(root, setting.scale_time_in, setting.scale_end_in);
            EventDelegate.Add(UitSca.onFinished, ShowInfoTweenScale, true);
            UitAlp = TweenAlpha.Begin(Content.gameObject, setting.alpha_time_in, setting.alpha_end_in / Alpha);
            EventDelegate.Add(UitAlp.onFinished, ShowInfoTweenAlpha, true);
        }
        void ShowInfoTweenPosition()
        {
            EventDelegate.Remove(UitPos.onFinished, ShowInfoTweenPosition);
            UITweener tween = TweenPosition.Begin(root, SettingEnd.position_time_out, SettingEnd.position_end_out);
            tween.delay = SettingEnd.stayTime;
            DoTweenEnd(tween);
        }
        void ShowInfoTweenRotation()
        {
            EventDelegate.Remove(UitRot.onFinished, ShowInfoTweenRotation);
            UITweener tween = TweenRotation.Begin(root, SettingEnd.rotation_time_out, Quaternion.Euler(SettingEnd.rotation_end_out));
            tween.delay = SettingEnd.stayTime;
            DoTweenEnd(tween);
        }

        void ShowInfoTweenScale()
        {
            EventDelegate.Remove(UitSca.onFinished, ShowInfoTweenScale);
            UITweener tween = TweenScale.Begin(root, SettingEnd.scale_time_out, SettingEnd.scale_end_out);
            tween.delay = SettingEnd.stayTime;
            DoTweenEnd(tween);
        }

        void ShowInfoTweenAlpha()
        {
            EventDelegate.Remove(UitAlp.onFinished, ShowInfoTweenAlpha);
            UITweener tween = TweenAlpha.Begin(Content.gameObject, SettingEnd.alpha_time_out, SettingEnd.alpha_end_out / Alpha);
            tween.delay = SettingEnd.stayTime;
            DoTweenEnd(tween);
        }


        void DoTweenEnd(UITweener tween)
        {
            if (tween == null) return;
            if (tweenEnd != null) return;
            tweenEnd = tween;

            EventDelegate.Add(tweenEnd.onFinished, ShowInfoEnd, true);
        }
        void ShowInfoEnd()
        {
            if (tweenEnd != null)
            {
                EventDelegate.Remove(tweenEnd.onFinished, ShowInfoEnd);
            }
            root.SetActive(false);
            if (msgEndEvent != null)
            {
                msgEndEvent(this);
                NGUITools.DestroyImmediate(root);
            }
        }

        void ClearEvent()
        {
            if (tweenEnd != null)
            {
                tweenEnd.delay = 0;
                EventDelegate.Remove(tweenEnd.onFinished, ShowInfoEnd);
            }
            tweenEnd = null;
            if (UitPos != null)
            {
                UitPos.delay = 0;
                EventDelegate.Remove(UitPos.onFinished, ShowInfoTweenPosition);
            }
            if (UitRot != null)
            {
                UitRot.delay = 0;
                EventDelegate.Remove(UitRot.onFinished, ShowInfoTweenRotation);
            }
            if (UitSca != null)
            {
                UitSca.delay = 0;
                EventDelegate.Remove(UitSca.onFinished, ShowInfoTweenScale);
            }
            if (UitAlp != null)
            {
                UitAlp.delay = 0;
                EventDelegate.Remove(UitAlp.onFinished, ShowInfoTweenAlpha);
            }
        }
    }
    float startTime = 0f;
    public void Update()
    {
        if (!isPlayEffect)
        {
            if (Time.time - startTime >= 1f)
            {
                startTime = Time.time;
                if (EffectTime <= 0)
                {
                    isPlayEffect = true;
                    data.msgkill.root.SetActive(false);


                    //处理MsgKill的释放
                    //GameObject.DestroyImmediate(data.msgkill.root);
                    ObjectPool.Instance.ReleaseGO(MsgKill.res, data.msgkill.root, PoolObjectTypeEnum.UITip);
                    
                    data.msgkill.root = null;
                    SetKillShow();
                }
                else
                    EffectTime--;
            }
        }
    }

    public void ShowMsg(MsgConfigInfo msginfo) {
        MsgSettingConfigInfo setting = null;
        if (msginfo == null)
        {
            //GameMethod.DebugError("error :msgID" + msgID);
            return;
        }
        setting = ConfigReader.GetMsgSettingInfo(msginfo.effect);
        if (setting == null)
        {
            //GameMethod.DebugError("error :msginfo.effect");
            return;
        }
        switch ((MsgType)msginfo.msgType)
        {
            case MsgType.MsgType1:
                if (localMsg == null)
                {
                    localMsg = new Msg();       
                }
                localMsg.Init();
                localMsg.MsgTypeTip(msginfo, setting);
                break;
            case MsgType.MsgType2:
                Msg msg = new Msg();
                msg.msgID = msginfo.id;
                worldMsg.Enqueue(msg);
                CheckMsgEnd();
                break;
            case MsgType.MsgType4:

                break;
        }
    }

    public void ShowMsg(int msgID)
    {
        MsgConfigInfo msginfo = ConfigReader.GetMsgInfo(msgID);//读取的是 “请选择英雄”
        ShowMsg(msginfo);
    }
    public bool currentIsEnd = true;
    void CheckMsgEnd()
    {
        if (currentIsEnd && worldMsg != null && worldMsg.Count != 0)
        {
            Msg msg = worldMsg.Dequeue();
            msg.Init();
            MsgConfigInfo msginfo = ConfigReader.GetMsgInfo(msg.msgID);
            MsgSettingConfigInfo setting = ConfigReader.GetMsgSettingInfo(msginfo.effect);
            msg.MsgTypeTip(msginfo, setting);
            currentIsEnd = false;
            msg.msgEndEvent += MsgEnd;
        }
    }

    void MsgEnd(Msg msg)
    {
        currentIsEnd = true;
        CheckMsgEnd();
    }
    public void SetAudioManager(string path, string audio_name)
    {
        if (audio_name == null || path == null) return;
        path = path + audio_name;
        
        //AudioClip clip = (AudioClip)GameObject.Instantiate(Resources.Load(path)) as AudioClip;
        //AudioManager.Instance.PlayEffectAudio(clip);

        ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.ASSET);

        AudioClip clip = clipUnit.Asset as AudioClip;
        AudioManager.Instance.PlayEffectAudio(clip);
    }
    struct AudioRand
    {
        public string AudioName;
        public float RandNum;
    }
    List<AudioRand> SetAudioDate(UInt64 heroID)
    {
        if (!EntityManager.AllEntitys.ContainsKey(heroID))
        {
            return null;
        }

        IEntity sEntity = EntityManager.AllEntitys[heroID];

        HeroConfigInfo info = ConfigReader.GetHeroInfo((int)sEntity.ObjTypeID);
        string[] val = info.HeroScript1.Split(',');
        string[] val1 = info.HeroScript1Rate.Split(',');
        List<AudioRand> randa = new List<AudioRand>();
        for (int i = 0; i < val.Length; i++)
        {
            AudioRand temp = new AudioRand();
            temp.AudioName = val[i];
            temp.RandNum = Convert.ToSingle(val1[i]) % 90000;
            randa.Add(temp);
        }
        return randa;
    }
    public void SetAudioPlay(UInt64 heroID, AudioPlayType audiotype = AudioPlayType.KillAudio)
    {
        if (!EntityManager.AllEntitys.ContainsKey(heroID))
        {
            return;
        }
        IEntity sEntity = EntityManager.AllEntitys[heroID];

        if (sEntity.GetDistanceToPos(PlayerManager.Instance.LocalPlayer.realObject.transform.position) > 30)
        {
            return;
        }

        HeroConfigInfo info = ConfigReader.GetHeroInfo((int)sEntity.ObjTypeID);
        if (info != null)
        {
            switch (audiotype)
            {
                case AudioPlayType.KillAudio:
                case AudioPlayType.TwentySconde:
                    {
                        System.Random rand = new System.Random();
                        int randomdata = rand.Next(2);
                        if (randomdata == 1)
                        {
                            string path = "Audio/HeroSelect/";
                            string adio_name = "";
                            int audio_num = rand.Next(100);
                            List<AudioRand> reand = SetAudioDate(heroID);
                            if (reand.Count < randomdata + 1)
                            {
                                return;
                            }
                            if (audio_num > reand[randomdata].RandNum)
                            {
                                adio_name = reand[1].AudioName;
                            }
                            else
                            {
                                adio_name = reand[0].AudioName;
                            }

                            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(path + adio_name, ResourceType.ASSET);
                            AudioClip clip = clipUnit.Asset as AudioClip;
                                
                            AudioManager.Instance.PlayGameKillAudio(clip);
                        }
                       
                    }
                    break;
                case AudioPlayType.FuhuoAudio:
                    {
                        if (info.HeroScript1 != null)
                        {
                            string path = "Audio/HeroSelect/";
                            string adio_name = info.HeroScript1.Substring(info.HeroScript1.IndexOf(",") + 1, info.HeroScript1.Length - info.HeroScript1.IndexOf(",") - 1);

                            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(path + adio_name, ResourceType.ASSET);
                            AudioClip clip = clipUnit.Asset as AudioClip;

                            AudioManager.Instance.PlayHeroLinesAudio(heroID, clip);
                        }
                    }
                    break;
            }
        }
    }
}