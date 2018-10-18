using Thanos.GameEntity;

namespace Thanos.Skill
{
    public class Buff
    {
        //buff的实例id
        public uint ID
        {
            get;
            set;
        }
        //buff的typeid
        public uint TypeID
        {
            get;
            set;
        }
        //buff剩余时间
        public float Time
        {
            get;
            set;
        }

        public IEntity Entity
        {
            get;
            set;
        }

        public string GetSpriteName()
        {

            BuffConfigInfo buffInfo = ConfigReader.GetBuffInfo(TypeID);
            return buffInfo == null ? "" : ConfigReader.GetBuffInfo(TypeID).IconRes;
        }

        public float GetTotalTime()
        {
            BuffConfigInfo buffInfo = ConfigReader.GetBuffInfo(TypeID);
            return buffInfo == null ? 1000000 : ConfigReader.GetBuffInfo(TypeID).TotalTime;
        }

        public void Update()
        {
            Time -= UnityEngine.Time.deltaTime;
        }
    }
}
