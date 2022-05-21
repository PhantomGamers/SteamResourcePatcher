using Bluegrams.Application;

namespace SteamResourcePatcher.Models
{
    internal class SettingsModel
    {
        public SettingsModel()
        {
            PortableJsonSettingsProvider.SettingsFileName = $"{Path.GetFileNameWithoutExtension(Environment.ProcessPath)}.config";
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);
            Properties.Settings.Default.SteamDirectory = Properties.Settings.Default.SteamDirectory;
            Properties.Settings.Default.Save();
        }
    }
}
