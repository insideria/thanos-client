using UnityEngine;

namespace Thanos.GameEntity {
	public static class EntityStrategyHelper
	{
		public const float MoveCheckBlockTick = 1.0f;
        public const float IdleTimeTick = 10.0f;

		public static bool IsTick(IEntity self , float tick){
			if (Time.time - self.StrategyTick >= tick) {
				self.StrategyTick = Time.time;
				return true;	
			}
			return false;
		}
	}
}
