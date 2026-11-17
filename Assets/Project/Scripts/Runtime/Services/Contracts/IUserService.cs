using System;

namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IUserService
    {
        string LocalId { get; }
        string CustomId { get; }
        bool HasCustomId { get; }

        string GetId();
        void SetCustomId(string customId);
        string GetIdSha256Hex();
        Guid GetIdAsGuidFromSha256();
    }
}
