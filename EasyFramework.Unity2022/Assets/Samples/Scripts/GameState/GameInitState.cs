
namespace EasyFramework.Samples
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
            await F.InitializeAsync();
            FTest.FSM.Enter<GameMainState>();
        }

    }
}