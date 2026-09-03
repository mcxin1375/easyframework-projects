using EasyFramework.Editor;

namespace EasyFramework.Samples.Editor
{
    public class EFSamplesDLC : IAssetBundleBuilderSettings
    {
        public string[] BuildDirectories { get; } = new string[]
        {
            "Assets/Samples/DLC",
        };
    }
}