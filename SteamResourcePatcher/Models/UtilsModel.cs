using System.Runtime.Versioning;
using Microsoft.Win32;

namespace SteamResourcePatcher.Models
{
    internal class UtilsModel
    {
        [SupportedOSPlatform("windows")]
        public static object? GetRegistryData(string aKey, string aValueName)
        {
            using RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(aKey);
            object? value = null;
            object? regValue = registryKey?.GetValue(aValueName);
            if (regValue != null)
            {
                value = regValue;
            }

            return value;
        }
    }
}
