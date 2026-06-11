using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class PlayerPrefsDatabaseDrawer
    {
        /// <summary>
        /// Дефолтний колір
        /// </summary>
        public static Color defaultColor;

        /// <summary>
        /// База даних
        /// </summary>
        public static PlayerPrefsDatabase database;

        /// <summary>
        /// Перевірка чи є створений PPref
        /// </summary>
        /// <param name="_database"></param>
        public static void Draw(PlayerPrefsDatabase _database)
        {
            defaultColor = GUI.color;
            if (_database.PlayerPrefs == null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити pPrefs"))
                    {
                        _database.PlayerPrefs = new PlayerPrefs();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                database = _database;
                GUI.color = Color.red;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Видалити pPrefs"))
                    {
                        _database.PlayerPrefs = null;
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Зберегти зміни в pPrefs"))
                    {
                        EditorUtility.SetDirty(_database);
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUI.color = defaultColor;

                DrawInfo(_database);
            }
        }

        /// <summary>
        /// Основний метод промалювання
        /// </summary>
        /// <param name="_database"></param>
        public static void DrawInfo(PlayerPrefsDatabase _database)
        {
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawSoundMusikInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawCoinInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawLevelInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawWeaponIDInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawGrenadeIDInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawDefenceIDInfo();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                WeaponInfoDrawer.Draw();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                GrenadeInfoDrawer.Draw();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                DefenceInfoDrawer.Draw();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                PersonageInfoDrawer.Draw();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                LevelInfoDrawer.Draw();
            }
            GUILayout.EndVertical();
            GUI.color = defaultColor;
        }

        /// <summary>
        /// Показуєм ID поточної гранати
        /// </summary>
        public static void DrawDefenceIDInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.selectedDefenceID = EditorGUILayout.IntField("ID поточної перепони: ", database.PlayerPrefs.selectedDefenceID);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм ID поточної гранати
        /// </summary>
        public static void DrawGrenadeIDInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.selectedGrenadeID = EditorGUILayout.IntField("ID поточної гранати: ", database.PlayerPrefs.selectedGrenadeID);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм ID поточної зброї
        /// </summary>
        public static void DrawWeaponIDInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.selectedWeaponID = EditorGUILayout.IntField("ID поточної зброї: ", database.PlayerPrefs.selectedWeaponID);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм рівень на якому зупинились
        /// </summary>
        public static void DrawLevelInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.levelID = EditorGUILayout.IntField("Рівень на якому зупинились: ", database.PlayerPrefs.levelID);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм кількість діамантів
        /// </summary>
        public static void DrawCoinInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.coin = EditorGUILayout.IntField("К-сть діамантів: ", database.PlayerPrefs.coin);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();

        }

        /// <summary>
        /// Показуєм параметри включеності музики і звуків
        /// </summary>
        public static void DrawSoundMusikInfo()
        {
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                database.PlayerPrefs.musik = (mySwitch)EditorGUILayout.EnumPopup("Музика: ", database.PlayerPrefs.musik);
                database.PlayerPrefs.sound = (mySwitch)EditorGUILayout.EnumPopup("Звуки: ", database.PlayerPrefs.sound);
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }
    }
}
