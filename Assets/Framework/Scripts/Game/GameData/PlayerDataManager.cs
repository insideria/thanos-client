using Thanos.GameEntity;

namespace Thanos.GameData
{
    public class PlayerDataManager: Singleton<PlayerDataManager>
    {
        public IPlayer Player
        {
            private set;
            get;
        }

        public int DeathTime
        {
            private set;
            get;
        }

        public int CampID
        {
            private set;
            get;
        }

        public void SetDeathTime(IPlayer player, int deathTime)
        {
            Player = player;
            DeathTime = deathTime;
        }

        public void SetPersonInfo(IPlayer player, int campId)
        {
            Player = player;
            CampID = campId;
        }
    }
}
