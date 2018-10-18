namespace Thanos.Effect
{
    public class BuffEffect : IEffect
    {
        public BuffEffect()
        {
            mType = IEffect.ESkillEffectType.eET_Buff;
        }
        public Player entity;        
        public uint InstID;     
        public override void OnLoadComplete()
        {
            if (entity == null)
            {
                return;
            }
            GetTransform().parent = entity.RealEntity.transform;
            GetTransform().position = entity.objBuffPoint.transform.position;
        }

        public override void Update()
        {
            if (isDead)
                return;                   

            base.Update();
        }
    }
}
