using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Samples
{
    public class Startup : MonoBehaviour
    {
        public string assemblyName = "EasyFramework.Samples";
        
        IEnumerator Start()
        {
            Application.runInBackground = true;
            
            F.WindowManager.Open<EFAwakeWindow>();
            
            yield return new WaitForSeconds(2);
            
            // 模拟进入热更新
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            HotUpdateHelper.Enter(assembly);
        }
    }
}