using UnityEngine;

namespace SBabchuk
{
    public static class EditorDatabaseLookup
    {
        public static T Get<T>() where T : Object
        {
            return Utils.GetAsset<T>();
        }
    }
}
