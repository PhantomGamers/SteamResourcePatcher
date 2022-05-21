using Bluegrams.Application;

namespace SteamResourcePatcher.Models
{
    internal class SettingsModel
    {
        static SettingsModel()
        {
            PortableJsonSettingsProvider.SettingsFileName = $"{Path.GetFileNameWithoutExtension(Environment.ProcessPath)}.config";
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);
        }
    }
}
