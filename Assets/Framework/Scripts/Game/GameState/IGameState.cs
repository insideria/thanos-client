namespace Thanos.GameState
{
    public interface IGameState
    {
        GameStateTypeEnum GetStateType();
        void SetStateTo(GameStateTypeEnum gsType);
        void Enter();
        GameStateTypeEnum Update(float fDeltaTime);
        void FixedUpdate(float fixedDeltaTime);
        void Exit();
    }
}
