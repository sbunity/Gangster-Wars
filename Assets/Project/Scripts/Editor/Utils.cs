using UnityEngine;

namespace SBabchuk
{
    public static class Utils
    {
        public static T GetAsset<T>() where T : Object
        {
            var assets = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (assets.Length > 0)
                return (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T));

            return default;
        }

        public static T[] GetAssets<T>() where T : Object
        {
            var assetPaths = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (assetPaths.Length == 0)
                return null;

            var assets = new T[assetPaths.Length];
            for (var i = 0; i < assets.Length; i++)
                assets[i] = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assetPaths[i]), typeof(T));

            return assets;
        }

        public static bool HasAsset<T>() where T : Object
        {
            return UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name).Length > 0;
        }

        public static T CreateAsset<T>(System.Type type, string path, bool refresh = false) where T : ScriptableObject
        {
            var scriptableObject = (T)ScriptableObject.CreateInstance(type);
            UnityEditor.AssetDatabase.CreateAsset(scriptableObject, path + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();

            if (refresh)
                UnityEditor.AssetDatabase.Refresh();

            return scriptableObject;
        }

        public static T CreateAsset<T>(string path, bool refresh = false) where T : ScriptableObject
        {
            return CreateAsset<T>(typeof(T), path, refresh);
        }

        public static void ChangeColor(Color color)
        {
            GUI.color = color;
        }

        public static void CheckColor(float field = -10, float defaultValue = 0)
        {
            if (field == -10)
            {
                ChangeColor(Color.green);
                return;
            }

            ChangeColor(field == defaultValue ? Color.yellow : Color.green);
        }

        public static void CheckColor(bool value, bool expected)
        {
            ChangeColor(value == expected ? Color.green : Color.yellow);
        }
    }
}
