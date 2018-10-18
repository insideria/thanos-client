using Thanos.GameEntity;

namespace Thanos.FSM
{
    public class EntityLastingFSM : IEntityFSM
	{
        public static readonly IEntityFSM Instance = new EntityLastingFSM();
		
		public FsmStateEnum State
		{
			get
            {
                return FsmStateEnum.LASTING;
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
            entity.OnEntityLastingSkill();
		}
		
		public void Execute(IEntity entity)
        {

		}
		
		public void Exit(IEntity entity){
			
		}
	}
}

