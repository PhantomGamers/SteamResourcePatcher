using Microsoft.Extensions.Caching.Memory;

using SteamResourcePatcher.Models;

namespace SteamResourcePatcher
{
    internal class Program
    {
        private static readonly string s_customPath = Path.Join(Path.GetDirectoryName(Environment.ProcessPath), "custom");
        private static readonly IEnumerable<string> s_customFiles;

        static Program()
        {
            if (Directory.Exists(s_customPath))
            {
                s_customFiles = Directory.EnumerateFiles(s_customPath, "*.*", SearchOption.AllDirectories);
            }
            else
            {
                s_customFiles = Array.Empty<string>();
            }
        }
        private static async Task Main()
        {
            _ = new SettingsModel();

            if (!Directory.Exists(s_customPath))
            {
                ErrorAndExit($"{s_customPath} does not exist");
            }

            if (!s_customFiles.Any())
            {
                ErrorAndExit("No custom files found");
            }

            if (string.IsNullOrWhiteSpace(SteamModel.ResourceDir) || !Directory.Exists(SteamModel.ResourceDir))
            {
                ErrorAndExit($"{SteamModel.SteamDir} does not exist");
            }

            await PatchModel.ReplaceAllFiles(s_customFiles, s_customPath);

            FSWModel.CreateAndStartWatcher(SteamModel.ResourceDir!, OnWatcherEvent, GetFileNamesOnly(s_customFiles));

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static async void OnWatcherEvent(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.TokenExpired)
            {
                return;
            }

            if (value is FileSystemEventArgs eventArgs)
            {
                FSWModel.RemoveFilter(eventArgs.Name!);
                await PatchModel.ReplaceFile(eventArgs.FullPath, GetCustomPath(eventArgs.FullPath));
                FSWModel.AddFilter(eventArgs.Name!);
            }
        }

        private static string GetCustomPath(string file)
        {
            return Path.Join(s_customPath, Path.GetRelativePath(SteamModel.ResourceDir!, file));
        }
        internal static IEnumerable<string> GetFileNamesOnly(IEnumerable<string> files)
        {
            List<string> fileNames = new();
            foreach (string? file in files)
            {
                fileNames.Add(Path.GetFileName(file));
            }
            return fileNames;
        }

        private static void ErrorAndExit(string error)
        {
            Console.WriteLine(error);
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
            Environment.Exit(-1);
        }
    }
}
