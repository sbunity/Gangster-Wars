namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBonusCollectTargetRegistry
    {
        void Register(ShortInfoName kind, int id, IBonusCollectTarget target);
        void Unregister(ShortInfoName kind, int id, IBonusCollectTarget target);
        bool TryGet(ShortInfoName kind, int id, out IBonusCollectTarget target);
    }
}
