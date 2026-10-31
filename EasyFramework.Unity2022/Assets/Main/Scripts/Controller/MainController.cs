
namespace EasyFramework.Samples
{
    public class MainController : Controller<MainController>
    {
        protected override async ETask OnEnterAsync()
        {
            await F.WindowManager.OpenAsync<MainWindow>();
            await F.WindowManager.CloseAsync<AwakeWindow>();
        }
    }
}