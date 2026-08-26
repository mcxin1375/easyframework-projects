using System;
using System.Collections;
using System.Linq;
using EasyFramework;
using UnityEngine;

namespace Main
{
    public class Startup : MonoBehaviour
    {
        public string assemblyName = "Game.Logic";
        
        IEnumerator Start()
        {
            F.WindowManager.Open<MainLogoWindow>();
            
            yield return new WaitForSeconds(2);
            
            // 模拟进入热更新
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            HotUpdateHelper.Enter(assembly);
        }
    }
}