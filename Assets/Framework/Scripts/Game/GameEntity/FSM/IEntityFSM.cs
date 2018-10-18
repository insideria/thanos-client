using Thanos.GameEntity;

namespace Thanos.FSM
{
	public interface IEntityFSM
	{
		bool CanNotStateChange{set;get;}
		FsmStateEnum State { get; }
		void Enter(IEntity entity , float stateLast);
		bool StateChange(IEntity entity , IEntityFSM state);
		void Execute(IEntity entity);
		void Exit(IEntity Ientity);
    }
} 
