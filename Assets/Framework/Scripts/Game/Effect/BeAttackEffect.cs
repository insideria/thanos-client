using UnityEngine;

//技能受击效果
namespace Thanos.Effect
{
    public class BeAttackEffect : IEffect
    {
        public BeAttackEffect()
        {
            projectID = EffectManager.Instance.GetLocalId();
            mType = IEffect.ESkillEffectType.eET_BeAttack;
        }

        public override void Update()
        {
            if (isDead)
                return;

            base.Update();
        }

        public override void OnLoadComplete()
        {
            //判断enTarget
            Player enTarget;
             PlayersManager.Instance.PlayerDic.TryGetValue(enTargetKey, out enTarget);

            if (enTarget != null && obj != null)
            {
                Transform hitpoit = enTarget.RealEntity.transform.Find("hitpoint");
                if (hitpoit != null)
                {
                    GetTransform().parent = hitpoit;
                    GetTransform().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }

            if (skillID == 0)
            {
                return;
            }
        }
    }	
}

