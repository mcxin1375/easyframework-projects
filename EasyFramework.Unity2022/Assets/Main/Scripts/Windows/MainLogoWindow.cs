using EasyFramework;

namespace Main
{
    public class MainLogoWindow : WindowResources
    {
        protected override void OnOpen()
        {
            FDebug.Log($"MainLogoWindow - Open");
        }

        protected override void OnClose()
        {
            FDebug.Log($"MainLogoWindow - Close");
        }
    }
}