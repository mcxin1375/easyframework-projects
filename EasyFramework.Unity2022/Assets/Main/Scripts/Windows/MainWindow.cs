
namespace EasyFramework.Samples
{

    public class MainWindowUI : WindowComponent
    {
        public UIScrollRectBehaviour SR_List;

        public readonly UIScrollRectList<MainWindowItem, int> List = new();

        protected override void OnWindowLoaded()
        {
            List.Initialize(SR_List);
        }
    }

    public class MainWindowItem : UIGridItemBehaviour<int>
    {
        
    }

    public partial class MainWindow : Window, IWindowUI<MainWindowUI>
    {
        protected override float KeepAliveTime => 1;

        protected override void OnOpen()
        {
            FDebug.Log($"MainWindow - Open");

            int[] arr = new int[100];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = i;
            }
            UI.List.Refresh(arr);
        }

        protected override void OnClose()
        {
            FDebug.Log($"MainWindow - Close");
        }
    }
}
