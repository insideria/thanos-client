using Thanos.GameEntity;

namespace Thanos.GameData
{
    public class PackageDataManager: Singleton<PackageDataManager>
    {   
        public IPlayer Player
        {
            private set;
            get;
        }
        public int Seat
        {
            private set;
            get;
        }
        public int Id
        {
            private set;
            get;
        }
        public int Num
        {
            private set;
            get;
        }
        public float TotalTime
        {
            private set;
            get;
        }
        public float LastTime
        {
            private set;
            get;
        }
        public void SetPackageData(IPlayer player, int seat, int id, int num, float totalTime, float lastTime)
        {
            Player = player;
            Seat = seat;
            Id = id;
            Num = num;
            TotalTime = totalTime;
            LastTime = lastTime;
        }
        // Use this for initialization
        void Start()
        {
        }
        
        // Update is called once per frame
        void Update()
        {
        }
    }
}
