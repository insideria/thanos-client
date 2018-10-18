namespace Thanos.FSM
{
    //英雄状态
	public enum FsmStateEnum 
	{
		FREE,//自由 0
		RUN,//跑 1
        SING,//唱歌 2
		RELEASE,//释放3
        LEADING,//离开 4
        LASTING,//
		DEAD,//死亡 6
        ADMOVE,//
        FORCEMOVE,//被迫移动  8
        RELIVE,//重生  9
        IDLE,//默认状态  10
	}
}