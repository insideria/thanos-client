namespace Thanos.Effect
{
    public class OnlySoundEffect : IEffect
    {
        public OnlySoundEffect()
        {
        }
        public override void Update()
        {
            if (isDead)
                return;
                   
            base.Update();
        }
    }
}

