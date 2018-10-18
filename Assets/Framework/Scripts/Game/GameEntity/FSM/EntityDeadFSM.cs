using Thanos.GameEntity;
using UnityEngine;

namespace Thanos.FSM
{
    public class EntityDeadFSM : IEntityFSM
    {
        public static readonly IEntityFSM Instance = new EntityDeadFSM();

        public EntityDeadFSM()
        {
        }

        public FsmStateEnum State
        {
            get
            {
                return FsmStateEnum.DEAD;
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
            entity.OnEnterDead();
        }

        public void Execute(IEntity entity)
        {
            entity.OnExecuteDead();
        }

        public void Exit(IEntity entity)
        {
            if (entity.FSM != null && entity.FSM.State == FsmStateEnum.DEAD)
            {
                entity.objTransform.position = entity.EntityFSMPosition;
                entity.objTransform.rotation = Quaternion.LookRotation(entity.EntityFSMDirection);
                entity.OnReborn();
            }
        }
    }
}

