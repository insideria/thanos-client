using Thanos.GameEntity;
namespace Thanos.FSM
{
	public class EntityFreeFSM : IEntityFSM
	{
		public static readonly IEntityFSM Instance = new EntityFreeFSM();
		
		public FsmStateEnum State
		{
			get{
				return FsmStateEnum.FREE;
			}
		}
		
		public bool CanNotStateChange{
			set;get;
		}
		
		public bool StateChange(IEntity entity , IEntityFSM fsm){
			return CanNotStateChange;
		}
		
		public void Enter(IEntity entity , float last){
            entity.OnEnterFree();
		}
		
		public void Execute(IEntity entity)
        {           
           entity.OnExecuteFree();
		}
		
		public void Exit(IEntity entity){
			
		}
	}
}

