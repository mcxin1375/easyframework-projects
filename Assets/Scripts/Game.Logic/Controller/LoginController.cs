using EasyFramework;
using Main;

namespace Game.Logic
{
    public class LoginController : ControllerBase
    {
        protected override void OnEnter()
        {
            FDebug.Log($"LoginController - OnEnter");
            

        }

        protected override async ETask OnEnterAsync()
        {
            FDebug.Log($"LoginController - OnEnterAsync");
            
            await F.WindowManager.OpenAsync<LoginWindow>();
            F.WindowManager.Close<MainLogoWindow>();

            await F.ResLoader.CreateObjAsync("Cube");
        }

        protected override void OnExit()
        {
            FDebug.Log($"LoginController - OnExit");
        }
    }
}