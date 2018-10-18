using Thanos.GameEntity;

namespace Thanos.FSM
{
    class EntitySingFSM : IEntityFSM
    {
        public static readonly EntitySingFSM Instance = new EntitySingFSM();
        public FsmStateEnum State
        {
            get
            {
                return FsmStateEnum.RUN;
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
            entity.OnEnterSing();
        }

        public void Execute(IEntity entity)
        {
        }

        public void Exit(IEntity entity)
        {
            entity.OnExitSing();
        }
    }
}
