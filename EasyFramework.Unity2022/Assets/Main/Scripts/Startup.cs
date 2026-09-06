using System.Collections;
using UnityEngine;

namespace EasyFramework.Samples
{
    public class Startup : MonoBehaviour
    {
        public string assemblyName = "EasyFramework.Samples";
        
        IEnumerator Start()
        {
            Application.runInBackground = true;
            
            F.WindowManager.Open<AwakeWindow>();
            
            yield return new WaitForSeconds(2);

            F.ControllerManager.EnterAsync<MainController>();
        }
    }
}