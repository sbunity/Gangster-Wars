namespace SBabchuk.Runtime.Services.Contracts
{
    public interface IDataSerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string raw);
    }
}
