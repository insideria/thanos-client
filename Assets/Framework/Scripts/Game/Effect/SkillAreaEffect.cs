using UnityEngine;

namespace Thanos.Effect
{
    public class SkillAreaEffect : IEffect
    {
        public SkillAreaEffect()
        {
            mType = IEffect.ESkillEffectType.eET_Area;
        }

        public override void OnLoadComplete()
        {
            SkillAreaConfig skillInfo = ConfigReader.GetSkillAreaConfig(skillID);
            Quaternion rt = Quaternion.LookRotation(dir);
            GetTransform().rotation = rt;
            GetTransform().position = fixPosition;           
        }

        public override void Update()
        {
            if (isDead)
                return;                   
          
            base.Update();
        }
    }
}
