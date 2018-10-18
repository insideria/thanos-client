namespace Thanos.GameEntity
{
	public enum EntityTypeEnum {
		Monster = 1,
		Soldier,
		Building,
		Player,
        AltarSoldier,
	}

    public enum NPCCateChildEnum
    {
        None = 0,
        NPC_Per_AtkBuilding, //攻击箭塔1
        NPC_Per_Bomb,  //炮兵  2
        SmallMonster, //小野怪 3
        HugeMonster,  //大野怪 4
        BUILD_Altar = 10,  //祭坛
        BUILD_Base,        //基地
        BUILD_Shop,        //商店
        BUILD_Tower,       //箭塔
        BUILD_Summon = 20,
        Ohter,
    }
}
