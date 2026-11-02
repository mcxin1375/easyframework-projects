using UnityEngine;

namespace EasyFramework.Samples
{
    public class Startup : MonoBehaviour
    {
        public string assemblyName = "EasyFramework.Samples";
        
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
            
            FDebug.LogError(@"

wefjeriojgiorjegije
reg
er
g
er
g
erg
er
g
reg
er
g
r
g
rg
rg
er
gh
er
h
erth
er
th
erh


er

h
















erherh
");
        }
    }
}