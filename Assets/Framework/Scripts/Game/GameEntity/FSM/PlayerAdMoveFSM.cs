using Thanos.GameEntity;

namespace Thanos.FSM
{
    public class PlayerAdMoveFSM : IEntityFSM
    {
        public static readonly IEntityFSM Instance = new PlayerAdMoveFSM();

        public FsmStateEnum State
        {
            get
            {
                return FsmStateEnum.ADMOVE;
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
            entity.OnEnterEntityAdMove();//无
        }

        public void Execute(IEntity entity)
        {
            entity.OnExecuteEntityAdMove(); //ientity中空  在Iselfplayer
        }

        public void Exit(IEntity entity)
        {

        }
    }
}

