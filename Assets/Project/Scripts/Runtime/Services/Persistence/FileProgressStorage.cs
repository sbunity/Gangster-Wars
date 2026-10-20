using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        public async UniTask<PlayerProgress> LoadAsync(CancellationToken cancellationToken)
        {
            RecoverInterruptedSave();

            if (!File.Exists(_filePath))
                return null;

            var progress = await ReadAsync(cancellationToken);
            if (progress != null)
                return progress;

            QuarantineCorruptedFile();
            return null;
        }

        public UniTask SaveAsync(PlayerProgress progress, CancellationToken cancellationToken)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            return WriteAsync(_serializer.Serialize(progress), cancellationToken);
        }

        public void Delete()
        {
            DeleteIfExists(_filePath);
            DeleteIfExists(_tempPath);
        }

        private async UniTask<PlayerProgress> ReadAsync(CancellationToken cancellationToken)
        {
            try
            {
                var raw = await File.ReadAllTextAsync(_filePath, cancellationToken);
                return _serializer.Deserialize<PlayerProgress>(raw);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogError($"Failed to read progress from '{_filePath}': {exception.Message}");
                return null;
            }
        }

        private async UniTask WriteAsync(string raw, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(_tempPath, raw, cancellationToken);
            ReplaceWithTemp();
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
