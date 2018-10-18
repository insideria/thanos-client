using System;
using UnityEngine;
using System.Collections.Generic;
using GameDefine;
using Thanos.GameEntity;
using System.Linq;
using Thanos.Resource;
using Thanos.Model;

namespace Thanos.Ctrl
{
    public class HeroCtrl : Singleton<HeroCtrl>
    {
        public void Enter()
        {
            Instance.SetSelectState(HeroSelectState.EnterSelect);
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroEnter);
        }

        public void Exit()
        {
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroExit);

            if (mAudioSource != null)
                mAudioSource.Stop();
        }

		public void SetSelectState(HeroSelectState state){
			selectState = state;
			switch(state){
			case HeroSelectState.InitSelect:  //初始化选择
				Init();//获取所有的英雄信息
				break;
			case HeroSelectState.EnterSelect://进入选择
				EnterSelectHero();//清除临时选择的英雄列表，以及确定选择的英雄列表
                break;
			case HeroSelectState.RandomSelect://随机选择
				EnterRandomHero();//随机选择
				break;
			case HeroSelectState.OutSelect://退出选择
				//Exit();
                Debug.LogError("HeroSelectState.OutSelect");
				break;
			}
		}

		public void AddPreSelectHero(uint index,int heroId)
        {
            //玩家预选择英雄
			foreach(var item in PlayerManager.Instance.AccountDic.Values){
				if(index == item.GameUserSeat){

					if(heroSelectDic.ContainsKey(item)){
						heroSelectDic.Remove(item);
					}
					heroSelectDic.Add(item,heroId);
					break;
				}
			}
            //调用英雄选择列表UpdateHeroListSelect
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroPreSelect);
        }
       
        //玩家确定英雄
        public void AddRealSelectHero(uint index,int heroId)
        {      
			foreach(var item in PlayerManager.Instance.AccountDic.Values){
                if (index != item.GameUserSeat) continue;
                 
                //判断heroSelectDic中是否包含这个玩家，如果包含，则移除掉
                if (heroSelectDic.ContainsKey(item))
                {
					heroSelectDic.Remove(item);
				}

                if (index == PlayerManager.Instance.LocalAccount.GameUserSeat)
                {
                    if (selectState == HeroSelectState.RandomSelect)
                    {
                        EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroRealSelect, heroId);
                    }
                    else
                    {
                       //根据选择的英雄播放对应英雄口号
                        PlayLocalSelectSound(heroId);
                    }
                }
				heroSureSelectDic.Add(item,heroId);//确定英雄列表
				heroSelectDic.Add(item,heroId);//英雄选择列表
				break;				
			}
            //广播选择影响消息   更新列表与队伍    显示已选定   禁用按钮
            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroAddSelect);
		}

        public void ReconnectPreSelect(int heroId)
        {
            EventCenter.Broadcast<int>((Int32)GameEventEnum.GameEvent_HeroReconnectPreSelect,heroId);
        }

       private void Init()
        {
			heroSelectDic = new Dictionary<IPlayer, int>();			
			heroSureSelectDic = new Dictionary<IPlayer, int>();	
			heroInfoList = new List<HeroSelectConfigInfo>();//英雄信息列表
			for(int i= 0 ;i < ConfigReader.HeroSelectXmlInfoDict.Count;i++)
			{
				#region 获得所有英雄信息
				HeroSelectConfigInfo item = ConfigReader.HeroSelectXmlInfoDict.ElementAt(i).Value;
                if (item.IsGuideHero == 1)
                    continue;
				heroInfoList.Add(item);
				#endregion
			}
            SortByBuyed();            
		}

        void SortByBuyed() {
            for (int i = 0; i < heroInfoList.Count; i++)
            {
                for (int j = 0; j < heroInfoList.Count - 1 - i; j++)
                {
                    HeroSelectConfigInfo infoA = heroInfoList[j];
                    HeroSelectConfigInfo infoB = heroInfoList[j + 1];
                    if (!GameUserModel.Instance.CanChooseHeroList.Contains(infoA.HeroSelectNum) && GameUserModel.Instance.CanChooseHeroList.Contains(infoB.HeroSelectNum))
                    {
                        HeroSelectConfigInfo temp = heroInfoList[j];
                        heroInfoList[j] = heroInfoList[j + 1];
                        heroInfoList[j + 1] = temp;
                    }
                }
            }
        }

		private void EnterSelectHero(){
			heroSelectDic.Clear();
			heroSureSelectDic.Clear(); 
		}
		
        
		private void EnterRandomHero()
        {
         
            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(AudioDefine.PATH_HERO_SELECT_COUNTDOWN, ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;
            mAudioSource = AudioManager.Instance.PlayEffectAudio(clip);

            EventCenter.Broadcast((Int32)GameEventEnum.GameEvent_HeroEnterRandom);
		}
		
        private void PlayLocalSelectSound(int heroId) {
            HeroSelectConfigInfo info = ConfigReader.GetHeroSelectInfo(heroId);
            string path = "Audio/HeroSelect/";
            path = path + info.HeroSelectSound;
            

            //AudioClip clip = (AudioClip) Resources.Load(path) as AudioClip;
            ResourceItem clipUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.ASSET);
            AudioClip clip = clipUnit.Asset as AudioClip;

            AudioSource source = AudioManager.Instance.PlayEffectAudio(clip);
        }


		public HeroCtrl(){
			SetSelectState(HeroSelectState.InitSelect);
		} 

		#region public
		//临时选择的英雄  某个玩家选择了某个英雄
		public Dictionary<IPlayer,int> heroSelectDic{
			get;
			private set;
		}

		public Dictionary<IPlayer,int> heroSureSelectDic{
			get;
			private set;
		}
		//所有英雄信息
		public List<HeroSelectConfigInfo> heroInfoList{
			get;
			private set;
		}

		public enum HeroSelectState{
			InitSelect,//初始化选择
			EnterSelect,//进入选择
			RandomSelect,//随机选择
			OutSelect//退出选择
		}
		#endregion

		#region private
		HeroSelectState selectState = HeroSelectState.InitSelect ;
        AudioSource mAudioSource = null;
		#endregion
	}
}
