using Thanos.GameEntity;

namespace Thanos.FSM
{
    public class EntityIdleFSM : IEntityFSM
	{
        public static readonly IEntityFSM Instance = new EntityIdleFSM();
		
		public FsmStateEnum State
		{
			get
            {
                return FsmStateEnum.IDLE;
			}
		}
		
		public bool CanNotStateChange{
			set;get;
		}
		
		public bool StateChange(IEntity entity , IEntityFSM fsm)
        {
			return CanNotStateChange;
		}
		
		public void Enter(IEntity entity , float last)
        {
            entity.OnEnterIdle();
		}
		
		public void Execute(IEntity entity)
        {      
            if (EntityStrategyHelper.IsTick(entity, 3.0f))//3秒后进入free状态
            {
                entity.OnFSMStateChange(EntityFreeFSM.Instance);
            }
		}
		
		public void Exit(IEntity entity){
			
		}
	}
}

