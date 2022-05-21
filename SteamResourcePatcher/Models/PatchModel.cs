namespace SteamResourcePatcher.Models
{
    internal class PatchModel
    {
        private const string PatchedText = "// PATCHED\n";

        internal static async Task<bool> ReplaceFile(string oldFile, string replacementFile)
        {
            if (!File.Exists(oldFile))
            {
                Console.WriteLine($"{oldFile} does not exist");
                return false;
            }

            if (!File.Exists(replacementFile))
            {
                Console.WriteLine($"{replacementFile} does not exist");
                return false;
            }

            try
            {
                string? file = await File.ReadAllTextAsync(oldFile);
                if (file.StartsWith(PatchedText))
                {
                    Console.WriteLine($"{oldFile} is already patched.");
                    return false;
                }
                file = await File.ReadAllTextAsync(replacementFile);
                file = PatchedText + file;
                await File.WriteAllTextAsync(oldFile, file);
                Console.WriteLine($"Patched {oldFile}");
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not patch file {oldFile}");
            }
            return false;
        }

        internal static async Task ReplaceAllFiles(IEnumerable<string> customFiles, string customPath)
        {
            Console.WriteLine("Patching files...");
            List<Task> tasks = new();
            foreach (string? file in customFiles)
            {
                string? relativeFile = Path.GetRelativePath(customPath, file);
                string? steamFile = Path.Join(SteamModel.ResourceDir, relativeFile);
                if (!File.Exists(steamFile))
                {
                    Console.WriteLine($"{steamFile} does not exist, skipping...");
                    continue;
                }
                tasks.Add(ReplaceFile(steamFile, file));
            }
            await Task.WhenAll(tasks);
        }
    }
}
