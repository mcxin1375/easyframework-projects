using EasyFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Logic
{
    /*
     * 通过F.ResLoader 加载同名资源，资源名必须跟类型名一致
     */
    public class HelloWorldWindow1 : Window
    {
        protected override void OnOpen()
        {
            FDebug.Log($"HelloWorldWindow - Open");
        }

        protected override void OnClose()
        {
            FDebug.Log($"HelloWorldWindow - Close");
        }

        // 可重写自定义加载
        protected override GameObject CreateWindowObject(Transform parent)
        {
            return base.CreateWindowObject(parent);
        }
        protected override ETask<GameObject> CreateWindowObjectAsync(Transform parent)
        {
            return base.CreateWindowObjectAsync(parent);
        }
    }

    // 通过名字自动绑定
    public class HelloWorldWindow2UI
    {
        public Image TestImg;
        public Text TestText;
    }
    /*
     * UI Generate By Roslyn
     */
    public partial class HelloWorldWindow2 : Window, IWindowUI<HelloWorldWindow2UI>
    {
        protected override void OnOpen()
        {
            FDebug.Log(UI.TestText.text);
        }
    }

    public struct HelloWorldWindow3Params
    {
        public int Value1;
    }
    /*
     * ITParams 支持3个参数
     * T1,T2,T3
     */
    public partial class HelloWorldWindow3 : Window, ITParams<HelloWorldWindow3Params, string>
    {
        protected override void OnOpen()
        {
            FDebug.Log(T1.Value1);
            FDebug.Log(T2);
        }
    }

    // 默认Resources.Load({Type.Name});
    [WindowResourcesPath("Windows/HelloWorldResourcesWindow")]
    public class HelloWorldResourcesWindow : WindowResources
    {
        
    }

    public static class WindowTests
    {
        public static async ETask Test()
        {
            await F.WindowManager.OpenAsync<HelloWorldWindow1>(UILayer.HUD);
            await F.WindowManager.OpenAsync<HelloWorldWindow3, HelloWorldWindow3Params, string>(UILayer.HUD, new HelloWorldWindow3Params(), "123");
        }
    }
}