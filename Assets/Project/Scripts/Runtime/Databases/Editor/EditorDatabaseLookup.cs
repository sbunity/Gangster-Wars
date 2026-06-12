using UnityEngine;

namespace SBabchuk.Runtime.Databases
{
    public static class EditorDatabaseLookup
    {
        public static T Get<T>()
            where T : Object
        {
            return Utils.GetAsset<T>();
        }
    }
}
