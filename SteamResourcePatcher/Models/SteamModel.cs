using System.Runtime.InteropServices;

namespace SteamResourcePatcher.Models
{
    internal class SteamModel
    {
        public static string? SteamDir
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SteamDirectory))
                {
                    return Properties.Settings.Default.SteamDirectory;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return UtilsModel.GetRegistryData(@"SOFTWARE\Valve\Steam", "SteamPath")?.ToString()?.Replace(@"/", @"\");
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "steam");
                }

                // OSX
                return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam", "Steam.AppBundle", "Steam", "Contents", "MacOS");
            }
        }

        public static string? ResourceDir = Path.Join(SteamDir, "resource");
    }
}
