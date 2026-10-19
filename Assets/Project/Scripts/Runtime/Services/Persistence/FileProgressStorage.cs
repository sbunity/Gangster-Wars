using System;
using System.IO;
using SBabchuk.Runtime.Progress;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class FileProgressStorage : IProgressStorage
    {
        private const string TempSuffix = ".tmp";
        private const string CorruptedSuffix = ".corrupted";

        private readonly IDataSerializer _serializer;
        private readonly string _filePath;
        private readonly string _tempPath;

        public FileProgressStorage(IDataSerializer serializer, string filePath)
        {
            _serializer = serializer;
            _filePath = filePath;
            _tempPath = filePath + TempSuffix;
        }

        public bool Exists => File.Exists(_filePath) || File.Exists(_tempPath);

        public bool TryLoad(out PlayerProgress progress)
        {
            progress = null;
            RecoverInterruptedSave();

            if (!File.Exists(_filePath))
                return false;

            try
            {
                progress = _serializer.Deserialize<PlayerProgress>(File.ReadAllText(_filePath));
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to read progress from '{_filePath}': {exception.Message}");
                progress = null;
            }

            if (progress != null)
                return true;

            QuarantineCorruptedFile();
            return false;
        }

        public void Save(PlayerProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(_tempPath, _serializer.Serialize(progress));
            ReplaceWithTemp();
        }

        public void Delete()
        {
            DeleteIfExists(_filePath);
            DeleteIfExists(_tempPath);
        }

        private void RecoverInterruptedSave()
        {
            if (File.Exists(_tempPath) && !File.Exists(_filePath))
                File.Move(_tempPath, _filePath);
        }

        private void ReplaceWithTemp()
        {
            DeleteIfExists(_filePath);
            File.Move(_tempPath, _filePath);
        }

        private void QuarantineCorruptedFile()
        {
            var corruptedPath = _filePath + CorruptedSuffix;
            DeleteIfExists(corruptedPath);
            File.Move(_filePath, corruptedPath);
            Debug.LogWarning($"Corrupted progress moved to '{corruptedPath}'. Default progress will be used.");
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
