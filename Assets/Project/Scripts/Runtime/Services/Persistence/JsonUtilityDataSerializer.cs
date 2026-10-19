using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;

namespace SBabchuk.Runtime.Services
{
    public sealed class JsonUtilityDataSerializer : IDataSerializer
    {
        public string Serialize<T>(T data)
            => JsonUtility.ToJson(data, true);

        public T Deserialize<T>(string raw)
            => JsonUtility.FromJson<T>(raw);
    }
}
