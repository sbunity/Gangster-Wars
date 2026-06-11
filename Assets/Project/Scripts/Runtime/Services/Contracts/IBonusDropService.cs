namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IBonusDropService
    {
        // Returns a droppable bonus id, or -1 when nothing is eligible.
        int GetAvailableBonusId();
    }
}
