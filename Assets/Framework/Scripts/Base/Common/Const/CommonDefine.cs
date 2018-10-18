using System;
#pragma warning disable 414

namespace Thanos
{
    namespace GameData
    {
        public class ConstDefine
        {
            private static UInt32 s_un32DefaultNameLen = 30;
            private static UInt32 s_un32DefaultPasswordLen = 33;
            private static UInt32 s_un32DefaultDeviceKeyLen = 150;
            private static Int32 s_n32MaxShipInBuilding = 6;
            private static Int32 s_n32MaxAttackProjectileNumInShip = 3;
            private static Int32 s_n32MaxDefenseProjectileNumInShip = 3;
            private static Int32 s_n32ProjectileMaxChildProjectileType = 3;

            private static UInt32 s_un32MaxProduceBuildingDemandNum = 6;
            private static Int32 s_n32DefaultMaxHP = 100;
            private static UInt32 s_un32DefaultUpgradeNeedSecond = 30;
            private static Int32 s_n32DefaultLaunchCDSecond = 6;
            private static ObjectTypeEnum s_eDefaultProjectileTypeID = (ObjectTypeEnum)52001;
            private static Int64 s_n64DefaultFoodCostForBuilding = 1000;
            private static Int64 s_n64DefaultSteelCostForBuilding = 1000;
            private static Int64 s_n64DefaultOilCostForBuilding = 1000;
            private static Int64 s_n64DefaultFoodCostForShip = 600;
            private static Int64 s_n64DefaultSteelCostForShip = 600;
            private static Int64 s_n64DefaultOilCostForShip = 600;
            private static Int32 s_n32PrepareAttackSecond = 30;
            private static Int32 s_n32AttackSecond = 600;
            private static Int32 s_n32DefaultShipZDist = 4000;
            private static Int32 s_n32DefaultMissileMiniYAltitude = 5;
            private static Int32 s_n32DefaultMissileYAltitude = 200;
            private static Int32 s_n32DefaultCannonBallAltitude = 300;
            private static Int32 s_n32DefaultShipXRange = 200;
            private static Int32 s_n32DefaultAircraftFighterYAltitude = 1000;
            private static Int32 s_n32DefaultAircraftBomberYAltitude = 1000;
            private static Int32 s_n32DefaultAircraftAsistantYAltitude = 1000;
            private static Int32 s_n32DefaultAircraftRotateRadius = 400;
            private static Int32 s_n32DefaultMissileRotateRadius = 400;
            private static float s_fDefaultNearAttackDistance = 2500;
            private static float s_fDefaultModerateAttackDistance = 3000;
            private static float s_fDefaultFarAttackDistance = 3500;
            private static Int32 s_n32DefaultRecoverHPEverySec = 10;
            private static Int32 s_n32MiniDistanceToTargetCity = 18520;

            public static UInt32 DefaultNameLen { get { return s_un32DefaultNameLen; } }
            public static UInt32 DefaultPasswordLen { get { return s_un32DefaultPasswordLen; } }
            public static UInt32 DefaultDeviceKeyLen { get { return s_un32DefaultDeviceKeyLen; } }
            public static Int32 MaxShipInBuilding { get { return s_n32MaxShipInBuilding; } }

            public static UInt32 MaxProduceBuildingDemandNum { get { return s_un32MaxProduceBuildingDemandNum; } }
            public static Int32 DefaultMaxHP { get { return s_n32DefaultMaxHP; } }
            public static UInt32 DefaultUpgradeNeedSecond { get { return s_un32DefaultUpgradeNeedSecond; } }
            public static Int32 DefaultLaunchCDSecond { get { return s_n32DefaultLaunchCDSecond; } }
            public static ObjectTypeEnum DefaultProjectileTypeID { get { return s_eDefaultProjectileTypeID; } }
            public static Int32 ProjectileMaxChildProjectileType { get { return s_n32ProjectileMaxChildProjectileType; } }
            public static Int64 DefaultFoodCostForBuilding { get { return s_n64DefaultFoodCostForBuilding; } }
            public static Int64 DefaultSteelCostForBuilding { get { return s_n64DefaultSteelCostForBuilding; } }
            public static Int64 DefaultOilCostForBuilding { get { return s_n64DefaultOilCostForBuilding; } }
            public static Int64 DefaultFoodCostForShip { get { return s_n64DefaultFoodCostForShip; } }
            public static Int64 DefaultSteelCostForShip { get { return s_n64DefaultSteelCostForShip; } }
            public static Int64 DefaultOilCostForShip { get { return s_n64DefaultOilCostForShip; } }
            public static Int32 PrepareAttackSecond { get { return s_n32PrepareAttackSecond; } }
            public static Int32 AttackSecond { get { return s_n32AttackSecond; } }
            public static Int32 DefaultShipZDist { get { return s_n32DefaultShipZDist; } }
            public static Int32 DefaultMissileMiniYAltitude { get { return s_n32DefaultMissileMiniYAltitude; } }
            public static Int32 DefaultMissileYAltitude { get { return s_n32DefaultMissileYAltitude; } }
            public static Int32 DefaultCannonBallAltitude { get { return s_n32DefaultCannonBallAltitude; } }
            public static Int32 DefaultShipXRange { get { return s_n32DefaultShipXRange; } }
            public static Int32 DefaultAircraftFighterYAltitude { get { return s_n32DefaultAircraftFighterYAltitude; } }
            public static Int32 DefaultAircraftBomberYAltitude { get { return s_n32DefaultAircraftBomberYAltitude; } }
            public static Int32 DefaultAircraftAsistantYAltitude { get { return s_n32DefaultAircraftAsistantYAltitude; } }
            public static Int32 DefaultAircraftRotateRadius { get { return s_n32DefaultAircraftRotateRadius; } }
            public static Int32 DefaultMissileRotateRadius { get { return s_n32DefaultMissileRotateRadius; } }
            public static float DefaultNearAttackDistance { get { return s_fDefaultNearAttackDistance; } }
            public static float DefaultModerateAttackDistance { get { return s_fDefaultModerateAttackDistance; } }
            public static float DefaultFarAttackDistance { get { return s_fDefaultFarAttackDistance; } }
            public static Int32 DefaultRecoverHPEverySec { get { return s_n32DefaultRecoverHPEverySec; } }
            public static Int32 MiniDistanceToTargetCity { get { return s_n32MiniDistanceToTargetCity; } }
        }

        public enum ObjectTypeEnum
        {
            None = 0,
            Guild,
            User,
            HeroBegin = ConstEnum.Level1Inter * 1,
            NPCBegin = ConstEnum.Level1Inter * 2,
            GoodsBegin = ConstEnum.Level1Inter * 3,
            AiRobotBegin = ConstEnum.Level1Inter * 4,
        }
    }
}