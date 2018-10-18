using UnityEngine;

namespace Thanos.Effect
{
    public class NormalEffect : IEffect
    {
        public enum NormalEffectType
        {
            eNE_Leading,
            eNE_Move,
        }
        public int modelID;
        public NormalEffectType NEType;

        public GameObject Target;

        public NormalEffect()
        {
            projectID = EffectManager.Instance.GetLocalId();
            Target = null;
            mType = IEffect.ESkillEffectType.eET_Normal;
        }

        public override void Update()
        {
            if (isDead)
                return;
                   
            base.Update();
        }

        public override void OnLoadComplete()
        {
            if (Target != null)
            {
                GetTransform().parent = Target.transform;
            }
            
            //无偏移位置，旋转
            GetTransform().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            GetTransform().localRotation = Quaternion.identity;
        }     
    }
}
