using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using SBabchuk.Runtime.Bootstrap;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class LegacySaveFilesCleanup : IBootstrapStep
    {
        private const string LegacySavePattern = "Main_*.pso";

        public UniTask ExecuteAsync(CancellationToken cancellationToken)
        {
            DeleteLegacySaves();
            return UniTask.CompletedTask;
        }

        private static void DeleteLegacySaves()
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
