
namespace EasyFramework.Samples
{
    public class EFMainWindow : WindowResources
    {
        protected override void OnOpen()
        {
            FDebug.Log($"EFMainWindow - Open");
        }

        protected override void OnClose()
        {
            FDebug.Log($"EFMainWindow - Close");
        }
    }
}