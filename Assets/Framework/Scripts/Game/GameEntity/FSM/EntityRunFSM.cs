using Thanos.GameEntity;

namespace Thanos.FSM
{
	public class EntityRunFSM : IEntityFSM
	{
		public static readonly IEntityFSM Instance = new EntityRunFSM();
		
		public FsmStateEnum State{
			get
			{
				return FsmStateEnum.RUN;
			}
		}
		
		public bool CanNotStateChange{
			set;get;
		}
		
		public bool StateChange(IEntity entity , IEntityFSM fsm){
			return CanNotStateChange;
		}
		
		public void Enter(IEntity entity , float last){
            FSMHelper.ExecuteDeviation(entity);
            entity.OnEnterMove();
		}
		
		public void Execute(IEntity entity){      
                entity.OnExecuteMove();
		}
		
		public void Exit(IEntity entity){
			
		}
	}
}

