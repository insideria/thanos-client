using System.Collections.Generic;

namespace Thanos.GameState
{
    //状态分类
    public enum GameStateTypeEnum
    {
        Continue,
        Login,
        User,
        Lobby,
        Room,
        Hero,
        Loading,
        Play,
        Over,
    }

    // 
    public class GameStateManager : Singleton<GameStateManager>
    {
        //存储所有的状态，在创建GameStartManager时在构造函数中初始化，加载所有的状态。
        Dictionary<GameStateTypeEnum, IGameState> mStatesDict;
        IGameState mCurrentState;//当前状态

        public GameStateManager()
        {
            mStatesDict = new Dictionary<GameStateTypeEnum, IGameState>();

            IGameState gameState;

            gameState = new LoginState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new UserState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new LobbyState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new RoomState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new HeroState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new LoadingState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new PlayState();
            mStatesDict.Add(gameState.GetStateType(), gameState);

            gameState = new OverState();
            mStatesDict.Add(gameState.GetStateType(), gameState);
        }

        //获取当前状态
        public IGameState GetCurState()
        {
            return mCurrentState;
        }

        //改变状态
        //根据监测状态变化，如果currentState有变化，对应进入相应的状态类，然后进入enter中，广播事件，添加监听器等等
        public void ChangeGameStateTo(GameStateTypeEnum stateType)
        {
            //如果当前状态与即将转换的状态一致，那么返回，不需要在转换了
            if (mCurrentState != null && mCurrentState.GetStateType() != GameStateTypeEnum.Loading && mCurrentState.GetStateType() == stateType)
                return;

            //在转换状态前，需要退出当前的状态，每种状态都有各自的退出的方法。
            if (mStatesDict.ContainsKey(stateType))
            {
                if (mCurrentState != null)
                {
                    mCurrentState.Exit();
                }
                //转换状态
                mCurrentState = mStatesDict[stateType];
                //根据添加的状态，调用相应的进入的方法，例如如果是登入状态，进入LoginState的Enter方法，每个状态都有相应的状态类
                mCurrentState.Enter();
            }
        }
        //一开始默认就是登陆状态（总共八个状态）
        public void EnterDefaultState()
        {
            ChangeGameStateTo(GameStateTypeEnum.Login);
        }


        //普通方法，但是在FixedUpdate中调用
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (mCurrentState != null)
            {
                mCurrentState.FixedUpdate(fixedDeltaTime);
            }
        }
        
        // 更新游戏状态机 在HolyTechGame中的Update中调用。实时监测
        // GameStateManager.Instance.Update(Time.deltaTime);
        public void Update(float fDeltaTime)
        {
            GameStateTypeEnum nextStateType = GameStateTypeEnum.Continue;
            if (mCurrentState != null)//如果状态不为空，那么就获取当前状态。
            {
                nextStateType = mCurrentState.Update(fDeltaTime);
            }
            if (nextStateType > GameStateTypeEnum.Continue)//如果下一个状态不是Continue状态，那么就转换状态
            {
                ChangeGameStateTo(nextStateType);//一旦转换状态，那么就加载对应的状态，然后进入enter中。
            }

        }
        //获取状态
        public IGameState getState(GameStateTypeEnum type)
        {
            if (!mStatesDict.ContainsKey(type))
            {
                return null;
            }
            return mStatesDict[type];
        }
    }
}
