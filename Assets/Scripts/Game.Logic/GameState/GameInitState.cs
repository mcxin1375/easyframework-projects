using EasyFramework;

namespace Game.Logic
{
    [GameState]
    public class GameInitState : FSMState
    {
        protected override void OnEnter()
        {
            FDebug.Log($"GameInitState - OnEnter");

            _ = InitializeAsync();
        }

        protected override void OnExit()
        {
            FDebug.Log($"GameInitState - OnExit");
        }

        private async ETask InitializeAsync()
        {
            await F.ResLoader.PreInitializeAsync();
            
            GLogic.FSM.Enter<GameLoginState>();
        }

    }
}