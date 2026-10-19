using System.IO;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public static class SaveFileLocations
    {
        private const string ProgressFileName = "progress.json";

        public static string Root => Application.persistentDataPath;
        public static string Progress => Path.Combine(Root, ProgressFileName);
    }
}
