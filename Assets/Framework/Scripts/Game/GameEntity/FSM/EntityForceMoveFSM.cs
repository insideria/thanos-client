using Thanos.GameEntity;

namespace Thanos.FSM
{
    public class EntityForceMoveFSM : IEntityFSM
    {
        public static readonly IEntityFSM Instance = new EntityForceMoveFSM();

        public FsmStateEnum State
        {
            get
            {
                return FsmStateEnum.FORCEMOVE;
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
            //entity.OnEnterEntityAdMove();
        }

        public void Execute(IEntity entity)
        {
            entity.OnExecuteForceMove();
        }

        public void Exit(IEntity entity)
        {

        }
    }
}

