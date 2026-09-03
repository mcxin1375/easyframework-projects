
namespace EasyFramework.Samples
{
    public class MainController : ControllerBase
    {
        protected override async ETask OnEnterAsync()
        {
            await F.WindowManager.OpenAsync<EFMainWindow>();
            await F.WindowManager.CloseAsync<EFAwakeWindow>();
        }
    }
}