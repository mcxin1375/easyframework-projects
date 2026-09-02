using EasyFramework;

namespace Game.Logic
{
    [GameState]
    public class GameLoginState : FSMState
    {
        protected override void OnEnter()
        {
            FDebug.Log($"GameLoginState - OnEnter");

            F.ControllerManager.EnterAsync<LoginController>();
        }

        protected override void OnExit()
        {
            FDebug.Log($"GameLoginState - OnExit");
        }
    }
}