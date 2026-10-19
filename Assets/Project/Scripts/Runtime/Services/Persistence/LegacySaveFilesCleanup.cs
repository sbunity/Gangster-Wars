using System.IO;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.Services
{
    public sealed class LegacySaveFilesCleanup : IInitializable
    {
        private const string LegacySavePattern = "Main_*.pso";

        public void Initialize()
        {
            var root = SaveFileLocations.Root;
            if (!Directory.Exists(root))
                return;

            foreach (var path in Directory.GetFiles(root, LegacySavePattern))
            {
                try
                {
                    File.Delete(path);
                }
                catch (IOException exception)
                {
                    Debug.LogWarning($"Failed to delete legacy save '{path}': {exception.Message}");
                }
            }
        }
    }
}
