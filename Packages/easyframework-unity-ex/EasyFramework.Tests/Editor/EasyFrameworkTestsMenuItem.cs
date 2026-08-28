/*----------------------------------------------------------------
// author:Cookie(mcx)
// date:2024/5/8
// describe:
//----------------------------------------------------------------*/

using System.IO;
using UnityEditor;

namespace EasyFramework.Editor
{
    public static class EasyFrameworkToolsExMenuItem
    {
        private const int Priority = MenuItemOrder.Tests;

        [MenuItem("EasyFramework/Tests/AssetBundleBuilder - Test", priority = Priority + 100)]
        private static void AssetBundleBuilderTest()
        {
            var dataPath = AssetBundleBuilder.Instance.ProjectPlatformPath;
            var targetFile = $"{dataPath}/Cube.ab";
            for (int i = 0; i < 1000; i++)
            {
                File.Copy(targetFile, $"{dataPath}/Cube_{i}.ab", true);
            }
        }
    }
}