using Thanos.GameEntity;

namespace Thanos.FSM
{
	using Thanos.GameEntity;

    public class EntityLeadingFSM : IEntityFSM
	{
        public static readonly IEntityFSM Instance = new EntityLeadingFSM();
		
		public FsmStateEnum State
		{
			get
            {
                return FsmStateEnum.LEADING;
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
            entity.OnEntityLeadingSkill();
		}
		
		public void Execute(IEntity entity)
        {

		}
		
		public void Exit(IEntity entity){
			
		}
	}
}

