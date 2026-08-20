using EasyFramework;

namespace Game.Logic
{
    [GameState]
    public class GameInitState : FSMState
    {
        protected override void OnEnter()
        {
            FDebug.Log($"GameInitState - OnEnter");
            
            GLogic.FSM.Enter<GameLoginState>();
        }

        protected override void OnExit()
        {
            FDebug.Log($"GameInitState - OnExit");
        }
    }
}