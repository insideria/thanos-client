using System;
using System.Collections.Generic;
using Thanos.GameEntity;
using GameDefine;

namespace Thanos.Skill
{
    class BuffManager: Singleton<BuffManager>
    {
        enum BuffEffectEnum
        {
            XuanYun = 16,
            ShuFu = 22,
        }

        public delegate void OnStopBuffAdd(UInt64 key);
        static public int chenmoID = 1017;

        public Dictionary<uint, Buff> mBuffDict = new Dictionary<uint, Buff>();
        //主角是否有不能移动的buff
        public bool isHaveStopBuff(UInt64 key)
        {
            foreach (Buff buff in mBuffDict.Values)
            {
                if (buff == null || buff.Entity == null || buff.Entity.GameObjGUID != key)
                {
                    continue;
                }

                BuffConfigInfo bi = ConfigReader.GetBuffInfo(buff.TypeID);
                if (null == bi)
                {
                    continue;
                }

                if (bi.effectID == (int)BuffEffectEnum.XuanYun || bi.effectID == (int)BuffEffectEnum.ShuFu)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isSelfHaveBuffType(int typeID)
        {
            foreach (Buff buff in mBuffDict.Values)
            {
                if (buff == null || buff.Entity == null)
                {
                    continue;
                }
                if (buff.TypeID == typeID && PlayerManager.Instance.LocalPlayer.ObjType == ObPlayerOrPlayer.PlayerType && buff.Entity.GameObjGUID == PlayerManager.Instance.LocalPlayer.GameObjGUID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHaveBuff(uint instID)
        {
            return mBuffDict.ContainsKey(instID);
        }

        public Buff AddBuff(uint instID, uint typeID, float remainTime, IEntity entity)
        {
            if (IsHaveBuff(instID))
            {
                return mBuffDict[instID];
            }

            if (entity == null)
            {
                return null;
            }

            if (isHaveStopBuff(entity.GameObjGUID) == false)
            {
                BuffConfigInfo bi = ConfigReader.GetBuffInfo(typeID);
                if (null != bi)
                {
                    if (bi.effectID == (int)BuffEffectEnum.XuanYun || bi.effectID == (int)BuffEffectEnum.ShuFu)
                    {
                        entity.OnEntityGetAstrictBuff();
                    }
                }
            }
            
            Buff buff = new Buff();
            buff.ID = instID;
            buff.TypeID = typeID;
            buff.Time = remainTime;
            buff.Entity = entity;

            //buffDict[instID] = b;
            mBuffDict.Add(instID, buff);

            if (isSelfHaveBuffType(chenmoID) == true)
            {
                EventCenter.Broadcast<bool>((Int32)GameEventEnum.GameEvent_LocalPlayerSilence, true);
            }
            //refresh ui
            if (UIBuffUnityInterface.Instance != null)
            {
                UIBuffUnityInterface.Instance.RefreshUIItem();
            }
            return buff;
        }

        public void RemoveBuff(uint instID)
        {
            if (mBuffDict.ContainsKey(instID))
            {
                Buff b = mBuffDict[instID];
                BuffConfigInfo bi = ConfigReader.GetBuffInfo(b.TypeID);

                mBuffDict.Remove(instID);
                if (bi != null)
                {
                    if (bi.effectID == (int)BuffEffectEnum.XuanYun || bi.effectID == (int)BuffEffectEnum.ShuFu)
                    {
                        if (b.Entity != null)
                        {
                            if (isHaveStopBuff(b.Entity.GameObjGUID) == false)
                            {
                                b.Entity.OnEntityRomoveAstrictBuff();
                            }
                        }
                    }
                }
                //如果该Buffer带有BeatFlyMotion效果,立即取消BeatFlyMotion效果
                if (b != null && bi.eFlyEffectID != 0)
                {
                    //获取击飞信息                
                    SkillFlyConfig skillFlycfg = ConfigReader.GetSkillFlyConfig(bi.eFlyEffectID);
                    
                    if (skillFlycfg != null && b.Entity != null)
                    {
                        b.Entity.BeatFlyFallDown(bi.BuffID);
                    }
                }

                if (UIBuffUnityInterface.Instance != null)
                {
                    UIBuffUnityInterface.Instance.RefreshUIItem();
                }

                if (isSelfHaveBuffType(chenmoID) == false)
                {
                    EventCenter.Broadcast<bool>((Int32)GameEventEnum.GameEvent_LocalPlayerSilence, false);
                }
            }
        }
        //
        public void Update()
        {
            foreach (Buff buff in mBuffDict.Values)
            {
                buff.Update();
            }

            if (UIBuffUnityInterface.Instance != null)
            {
                UIBuffUnityInterface.Instance.UpdateUIItem();
            }
        }
    }
}
