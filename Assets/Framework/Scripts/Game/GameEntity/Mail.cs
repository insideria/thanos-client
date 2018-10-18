namespace Thanos.GameEntity
{ 
    public enum MailCurtStateEnum
    {
        None = 0,
        New = 1,
        LookedButNotGotGift = 2,
        Look = 3,
        Del = 4,
    }

    public enum MailGiftTypeEnum
    {
        None = 0,
        Gold = 1,
        Diamond = 2, 
        Runne = 3,
        Hero = 4,
        Skin = 5,
        Goods = 6,
        Exp = 7,
    }

    public class Mail
    {
        public int mId;
        public string mTitle;
        public string mContent;
        public string mGift;
        public string mSender;
        public string mCreateTime;
        public int mType;
        public MailCurtStateEnum CurrentState;
    }
}
