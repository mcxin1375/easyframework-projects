using UnityEngine;

namespace EasyFramework.Samples
{
    public class Startup : MonoBehaviour
    {
        void Start()
        {
            Application.runInBackground = true;
            Run().Forget();
        }

        private void Awake()
        {
            
        }

        private void OnDestroy()
        {
            
        }

        private async ETask Run()
        {
            F.WindowManager.Open<AwakeWindow>();

            await ETask.DelaySeconds(2);

            await F.InitializeAsync();
            await MainController.Instance.EnterAsync();
        }
    }
}