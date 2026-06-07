using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using DG.Tweening;

namespace SBabchuk
{
	static public class Utils
	{
#if UNITY_ANDROID || UNITY_IPHONE
        public static T GetAsset2<T>() where T : Object
        {
            return Resources.Load<T>("Databases/" + typeof(T).Name);
        }
#endif

#if UNITY_EDITOR
        public static T GetAsset<T>() where T : Object
		{
			string[] assets = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
			if (assets.Length > 0)
			{
				return (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]), typeof(T));
			}

			return default(T);
		}

		public static T[] GetAssets<T>() where T : Object
		{
			string[] assetsPath = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
			if (assetsPath.Length > 0)
			{
				T[] assets = new T[assetsPath.Length];

				for (int i = 0; i < assets.Length; i++)
				{
					assets[i] = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assetsPath[i]), typeof(T));
				}

				return assets;
			}

			return null;
		}

		public static bool HasAsset<T>() where T : Object
		{
			string[] assets = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
			if (assets.Length > 0)
			{
				return true;
			}

			return false;
		}

		public static T CreateAsset<T>(System.Type type, string path, bool refresh = false) where T : ScriptableObject
		{
			T scriptableObject = (T)ScriptableObject.CreateInstance(type);

			string itemPath = path + ".asset";

			UnityEditor.AssetDatabase.CreateAsset(scriptableObject, itemPath);

			UnityEditor.AssetDatabase.SaveAssets();

			if (refresh)
				UnityEditor.AssetDatabase.Refresh();

			return scriptableObject;
		}

		public static T CreateAsset<T>(string path, bool refresh = false) where T : ScriptableObject
		{
			T scriptableObject = (T)ScriptableObject.CreateInstance(typeof(T));

			string itemPath = path + ".asset";

			UnityEditor.AssetDatabase.CreateAsset(scriptableObject, itemPath);

			UnityEditor.AssetDatabase.SaveAssets();

			if (refresh)
				UnityEditor.AssetDatabase.Refresh();

			return scriptableObject;
		}
#endif

		public static bool IsMobilePlatform ()
		{
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA
			return true;
#else
			return false;
#endif
		}

        /// <summary>
        /// Зміга кольра стилю промальовки
        /// </summary>
        /// <param name="_color">Новий колір</param>
        public static void ChangeColor(Color _color)
        {
            GUI.color = _color;
        }

        /// <summary>
        /// Зміга кольра стилю промальовки
        /// </summary>
        /// <param name="_color">Новий колір</param>
        public static void CheckColor(float _field = -10, float defaultValue = 0)
        {
            if (_field == -10)
            {
                ChangeColor(Color.green);
            }
            else
            {
                if (_field == defaultValue)
                {
                    ChangeColor(Color.yellow);
                }
                else
                {
                    ChangeColor(Color.green);
                }
            }

        }

        /// <summary>
        /// Зміга кольра стилю промальовки
        /// </summary>
        /// <param name="_color">Новий колір</param>
        public static void CheckColor(bool _value, bool _value2)
        {
            if (_value == _value2)
            {
                ChangeColor(Color.green);
            }
            else
            {
                ChangeColor(Color.yellow);
            }
        }

        /// <summary>
        /// Зупиняєм твін
        /// </summary>
        /// <param name="_tween"></param>
        public static void StopTween(Tween _tween)
        {
            if (_tween != null)
            {
                _tween.Kill();

                _tween = null;
            }
        }
    }   
}