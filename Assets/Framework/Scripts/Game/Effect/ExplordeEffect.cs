using UnityEngine;
using Thanos.GameEntity;

//技能爆炸效果，通常应对范围技能
namespace Thanos.Effect
{
    class ExplordeEffect : IEffect
    {
        public override void OnLoadComplete()
        {
            IEntity enOwner;
            EntityManager.AllEntitys.TryGetValue(enOwnerKey, out enOwner);

            if (obj != null && enOwner != null)
            {
                SkillConfigInfo skillinfo = ConfigReader.GetSkillInfo(skillID);
                Transform point = enOwner.RealEntity.objAttackPoint;

                if (point != null)
                {
                    if (skillinfo.SkillType == (int)SkillTypeEnum.FixDistanceArea)
                    {
                        Vector3 temp = fixPosition;
                        temp.y = point.transform.position.y;
                        fixPosition = temp;
                        GetTransform().position = fixPosition;
                    }
                    else
                    {
                        GetTransform().position = point.transform.position;
                    }


                    IEntity enTarget;
                    EntityManager.AllEntitys.TryGetValue(enTargetKey, out enTarget);
                    
                    //Debug.LogError("fly effect init pos:" + root.transform.position.x + ":" + root.transform.position.y + ":" + root.transform.position.z);
                    if (mType == IEffect.ESkillEffectType.eET_FlyEffect && enTarget != null)
                    {
                        Quaternion rt = Quaternion.LookRotation(enTarget.RealEntity.transform.position - GetTransform().position);
                        GetTransform().rotation = rt;
                    }
                    else
                    {
                        if (dir == Vector3.zero)
                        {
                            dir = Vector3.one;
                        }
                        Quaternion rt = Quaternion.LookRotation(dir);
                        GetTransform().rotation = rt;
                    }
                }
            }
        }

        public override void Update()
        {
            if (isDead)
                return;

            GetTransform().position = fixPosition;
            base.Update();              
        }
    }
}
