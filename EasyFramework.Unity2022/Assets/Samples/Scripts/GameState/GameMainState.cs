
namespace EasyFramework.Samples
{
    [GameState]
    public class GameMainState : FSMState
    {
        protected override void OnEnter()
        {
            FDebug.Log($"GameMainState - OnEnter");

            F.ControllerManager.EnterAsync<MainController>();
        }

        protected override void OnExit()
        {
            FDebug.Log($"GameMainState - OnExit");
        }
    }
}