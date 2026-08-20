using EasyFramework;

namespace Game.Logic
{
    public class LoginWindow : Window
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