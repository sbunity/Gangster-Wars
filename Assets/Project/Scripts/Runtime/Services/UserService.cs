using System;
using System.Security.Cryptography;
using System.Text;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class UserService : IUserService
    {
        private const int GuidBytesLength = 16;

        private readonly IPlayerProgressRepository _repository;

        private string _customId;

        public UserService(IPlayerProgressRepository repository)
        {
            _repository = repository;
        }

        public string LocalId => LoadOrCreateLocalId();
        public string CustomId => _customId;
        public bool HasCustomId => !string.IsNullOrWhiteSpace(_customId);

        public string GetId() => HasCustomId ? _customId : LocalId;

        public void SetCustomId(string customId)
        {
            _customId = Sanitize(customId);
        }

        public string GetIdSha256Hex() => Sha256Hex(GetId());

        public Guid GetIdAsGuidFromSha256()
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(GetId() ?? ""));
            var guidBytes = new byte[GuidBytesLength];

            Buffer.BlockCopy(hash, 0, guidBytes, 0, GuidBytesLength);

            return new Guid(guidBytes);
        }

        private string LoadOrCreateLocalId()
        {
            var progress = _repository.Progress;
            if (string.IsNullOrEmpty(progress.UserLocalId))
            {
                progress.UserLocalId = Guid.NewGuid().ToString("N");
                _repository.Save();
            }

            return progress.UserLocalId;
        }

        private static string Sanitize(string value)
        {
            value = value?.Trim();

            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static string Sha256Hex(string value)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? ""));

            var builder = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }
    }
}
