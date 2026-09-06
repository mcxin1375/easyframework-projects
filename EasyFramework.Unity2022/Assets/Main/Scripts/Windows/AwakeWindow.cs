
namespace EasyFramework.Samples
{
    public class AwakeWindow : WindowResources
    {
        protected override void OnOpen()
        {
            FDebug.Log($"EFAwakeWindow - Open");
        }

        protected override void OnClose()
        {
            FDebug.Log($"EFAwakeWindow - Close");
        }
    }
}