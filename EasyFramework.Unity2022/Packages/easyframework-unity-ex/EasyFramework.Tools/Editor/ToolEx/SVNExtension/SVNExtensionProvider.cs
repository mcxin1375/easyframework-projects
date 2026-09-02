/*----------------------------------------------------------------
// author:Cookie(mcx)
// date:2023/6/23
// describe:
//----------------------------------------------------------------*/

using UnityEditor;

namespace EasyFramework.Editor
{
    public class SVNExtensionProvider : ProjectSettingsProvider<SVNExtensionSettings>
    {
        private const string SettingsPath = ToolsProvider.SettingPath + "/" + nameof(SVNExtension);
        
        [SettingsProvider]
        public static SettingsProvider Create() => Singleton<SVNExtensionProvider>.Instance;
        public SVNExtensionProvider() : base(SettingsPath) { }
    }
}