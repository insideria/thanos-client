using System;
using Thanos.GameEntity;
using GameDefine;

namespace Thanos.View
{
    public class SkillDescribleWindow : BaseWindow
    {
        public SkillDescribleWindow()
        {
            mScenesType = EScenesType.EST_Play;
            mResName = GameConstDefine.SkillDestribe;
            mResident = false;
        }

        ////////////////////////////继承接口/////////////////////////
        //类对象初始化
        public override void Init()
        {
            EventCenter.AddListener<bool,  SkillTypeEnum,  IPlayer >((Int32)GameEventEnum.GameEvent_SkillDescribleType, ShowDescribleByType);
            EventCenter.AddListener<bool, int, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleId, ShowDescribleById);
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

        //类对象释放
        public override void Realse()
        {
            EventCenter.RemoveListener<bool, SkillTypeEnum, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleType, ShowDescribleByType);
            EventCenter.RemoveListener<bool, int, IPlayer>((Int32)GameEventEnum.GameEvent_SkillDescribleId, ShowDescribleById);
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_GamePlayExit, Hide);
        }

        //窗口控件初始化
        protected override void InitWidget()
        {
            mSkill2CD = mRoot.Find("Skill_Cooldown").GetComponent<UILabel>();

            mSkillDes = mRoot.Find("Skill_Describe").GetComponent<UILabel>();

            mSkillLv = mRoot.Find("Skill_Level").GetComponent<UILabel>();

            mSkillName = mRoot.Find("Skill_Name").GetComponent<UILabel>();

            mSkillDis = mRoot.Find("Skill_Distance").GetComponent<UILabel>();

            mSkillMpCost = mRoot.Find("Skill_MP").GetComponent<UILabel>();

            mSkillHpCost = mRoot.Find("Skill_HP").GetComponent<UILabel>();

            mSkillCpCost = mRoot.Find("Skill_CP").GetComponent<UILabel>();
        }

        //窗口控件释放
        protected override void RealseWidget()
        { 
        }

        //游戏事件注册
        protected override void OnAddListener()
        {
            EventCenter.AddListener((Int32)GameEventEnum.GameEvent_SkillDescribleUpdate, UpdateDescrible);
        }

        //游戏事件注消
        protected override void OnRemoveListener()
        {
            EventCenter.RemoveListener((Int32)GameEventEnum.GameEvent_SkillDescribleUpdate, UpdateDescrible);
        }

        //显示
        public override void OnEnable()
        {
            currentSkill = SkillTypeEnum.SKILL_NULL;
        }

        //隐藏
        public override void OnDisable()
        {

        }


        public override void Update(float deltaTime)
        {
        
        }
       
        public SkillTypeEnum currentSkill
        {
            private set;
            get;
        }

        private UILabel mSkillName = null;
        private UILabel mSkillDes = null;
        private UILabel mSkill2CD = null;
        private UILabel mSkillDis = null;
        private UILabel mSkillLv = null;
        private UILabel mSkillMpCost = null;
        private UILabel mSkillCpCost = null;
        private UILabel mSkillHpCost = null;


        private void ShowDescribleByType(bool show, SkillTypeEnum skillType, IPlayer player)
        {
            if (show)
            {
                Show();
                SetSkillDestribe(skillType, (ISelfPlayer)player);
                currentSkill = skillType;
            }
            else
            {
                Hide();
                currentSkill = SkillTypeEnum.SKILL_NULL;
            }
        }

        public void ShowDescribleById(bool show, int skillId, IPlayer player)
        {
            if (show)
            {
                Show();
                SetSkillDestribe(skillId, player);
            }
            else
            {
                Hide();
                currentSkill = SkillTypeEnum.SKILL_NULL;
            }
        }

        private void UpdateDescrible()
        {
            if (currentSkill != SkillTypeEnum.SKILL_NULL)
            {
                UpdateDestribe(currentSkill, PlayerManager.Instance.LocalPlayer);
            }
        }


        public void UpdateDestribe(SkillTypeEnum skillType, ISelfPlayer player)
        {
            int skillId = player.SkillIdDic[skillType];

            UpdateDestribe(skillId, (IPlayer)player);

        }

        public void UpdateDestribe(int skillId, IPlayer player)
        {
            SkillManagerConfig skillconfig = ConfigReader.GetSkillManagerCfg(skillId);
            if (skillconfig == null) return;
            
            mSkill2CD.text = (skillconfig.coolDown / 1000f).ToString();//冷却时间
            mSkillDes.text = DestribeWithAttribue(skillconfig.info, player);//技能描述

            int bet = skillconfig.id % 10;
            if (bet == 0)
                bet = 1;
            mSkillLv.text = bet.ToString();//技能等级

            mSkillName.text = skillconfig.name;//技能名称

            mSkillDis.text = "(" + skillconfig.range.ToString() + "施法距离)";
            ShowCost(skillconfig);//技能消耗（hp，mp，cp其中一个）

        }


        void ShowCost(SkillManagerConfig skillconfig)
        {
            //初始化都为false
            mSkillHpCost.transform.gameObject.SetActive(false);
            mSkillCpCost.transform.gameObject.SetActive(false);
            mSkillMpCost.transform.gameObject.SetActive(false);
            //技能释放的消耗只能是Hp，Mp，Cp中的一个
            if (skillconfig.mpUse != 0)
            {
                mSkillMpCost.text = skillconfig.mpUse.ToString();
                mSkillMpCost.transform.gameObject.SetActive(true);
            }
            else if (skillconfig.hpUse != 0)
            {
                mSkillHpCost.text = skillconfig.hpUse.ToString();
                mSkillHpCost.transform.gameObject.SetActive(true);
            }
            else if (skillconfig.cpUse != 0)
            {
                mSkillCpCost.text = skillconfig.cpUse.ToString();
                mSkillCpCost.transform.gameObject.SetActive(true);
            }
        }


        public void SetSkillDestribe(SkillTypeEnum skillType, ISelfPlayer player)
        {
            if (!player.SkillIdDic.ContainsKey(skillType) || !player.BaseSkillIdDic.ContainsKey(skillType))
                return;
            int skillId = player.SkillIdDic[skillType];

            SetSkillDestribe(skillId, (IPlayer)player);
        }

        public void SetSkillDestribe(int skillId, IPlayer player)
        {
            SkillManagerConfig skillconfig = ConfigReader.GetSkillManagerCfg(skillId);
            if (skillconfig == null) return;
           
            UpdateDestribe(skillId, player);
        }

        string DestribeWithAttribue(string str, IPlayer player)
        {
            string tempStr = "";
            tempStr = str;
            if (!(str.Contains("mag") || str.Contains("phy")))
                return str;
            for (int i = 0; i < tempStr.Length; i++)
            {
                if (tempStr[i] != ']')
                {
                    continue;
                }
                int index = tempStr.LastIndexOf('[', i);
                string addStr = tempStr.Substring(index, i - index + 1);
                string[] strArray;
                if (addStr.Contains("mag") || addStr.Contains("phy"))
                {
                    strArray = addStr.Split(',');
                    strArray[1] = strArray[1].Remove(strArray[1].Length - 1, 1);
                    float attr = float.Parse(strArray[1]);
                    if (player == null)
                    {
                        return null;
                    }
                    float phyAttr = player.PhyAtk;
                    float magAttr = player.MagAtk;
                    attr = addStr.Contains("mag") ? (attr * magAttr) : (attr * phyAttr);
                    string attrAdd = attr >= 0 ? ("+" + attr.ToString()) : ("-" + attr.ToString());
                    attrAdd = "[00FF00]" + attrAdd + "[-]";
                    tempStr = tempStr.Replace(addStr, attrAdd);
                }

            }
            return tempStr;
        }

    }
}



