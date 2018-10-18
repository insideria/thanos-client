using Thanos.GameEntity;
using UnityEngine;

namespace Thanos.FSM
{
    public static class FSMHelper
    {
        //执行偏差
        public static bool ExecuteDeviation(IEntity entity)
        {
            if (entity.realObject == null)
            {
                return true;
            }
            float disToFsmPos = Vector3.Distance(entity.realObject.transform.position, entity.EntityFSMPosition);
            if (disToFsmPos <= FSMVariable.IgnoreDistance)
            {
                return true;
            }
        
            return true;
        }
    }
}
