using Thanos.GameEntity;

namespace Thanos.FSM
{
    public class EntityReliveFSM : IEntityFSM
    {
        public static readonly IEntityFSM Instance = new EntityReliveFSM();
        public FsmStateEnum State
        {
            get
            {
                return FsmStateEnum.RELIVE;
            }
        }
        public bool CanNotStateChange
        {
            set;
            get;
        }

        public bool StateChange(IEntity entity, IEntityFSM fsm)
        {
            return CanNotStateChange;
        }

        public void Enter(IEntity entity, float last)
        {
            entity.OnEnterRelive();
        }

        public void Execute(IEntity entity)
        {
        }

        public void Exit(IEntity entity)
        {
            entity.OnExitRelive();
        }
    }
}

