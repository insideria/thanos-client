using Thanos.GameEntity;

namespace Thanos.FSM
{
	public class EntityReleaseSkillFSM : IEntityFSM
	{
		public static readonly IEntityFSM Instance = new EntityReleaseSkillFSM();
		public FsmStateEnum State{
			get
			{
				return FsmStateEnum.RELEASE;
			}
		}
		public bool CanNotStateChange{
			set;get;
		}
		
		public bool StateChange(IEntity entity , IEntityFSM fsm){
			return CanNotStateChange;
		}
		
		public void Enter(IEntity entity , float last){
			
			entity.OnEntityReleaseSkill();
		}
		
		public void Execute(IEntity entity){
            //entity.OnEntityPrepareAttack ();
		}

		public void Exit(IEntity entity){
		    
		}
	}
}

