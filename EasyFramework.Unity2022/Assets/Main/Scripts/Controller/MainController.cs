
namespace EasyFramework.Samples
{
    public class MainController : Controller<MainController>
    {
        protected override async ETask OnEnterAsync()
        {
            await F.WindowManager.OpenAsync<MainWindow>();
            F.WindowManager.Close<AwakeWindow>();
        }
    }
}