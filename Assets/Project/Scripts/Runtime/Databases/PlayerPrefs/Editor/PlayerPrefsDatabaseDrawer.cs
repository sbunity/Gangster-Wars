using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    public class PlayerPrefsDatabaseDrawer
    {
        private static Color _defaultColor;
        public static Color DefaultColor { get => _defaultColor; set => _defaultColor = value; }

        private static PlayerPrefsDatabase _database;
        public static PlayerPrefsDatabase Database { get => _database; set => _database = value; }

        public static void Draw(PlayerPrefsDatabase database)
        {
            _defaultColor = GUI.color;
            if (database == null)
                return;

            _database = database;
            if (database.PlayerPrefs == null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити pPrefs"))
                    {
                        database.PlayerPrefs = new PlayerPrefs();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Видалити pPrefs"))
                    {
                        database.PlayerPrefs = null;
                        GUI.color = _defaultColor;
                        return;
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Зберегти зміни в pPrefs"))
                    {
                        EditorUtility.SetDirty(database);
                        AssetDatabase.SaveAssets();
                        SavePersistentDatabase(database);
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUI.color = _defaultColor;
                DrawInfo(_database);
            }
        }

        public static void DrawInfo(PlayerPrefsDatabase _database)
        {
            if (_database == null || _database.PlayerPrefs == null)
                return;

            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawSoundMusikInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawCoinInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawLevelInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawWeaponIDInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawGrenadeIDInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                DrawDefenceIDInfo();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = _defaultColor;
                WeaponInfoDrawer.Draw();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = _defaultColor;
                GrenadeInfoDrawer.Draw();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = _defaultColor;
                DefenceInfoDrawer.Draw();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = _defaultColor;
                PersonageInfoDrawer.Draw();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = _defaultColor;
                LevelInfoDrawer.Draw();
            }

            GUILayout.EndVertical();
            GUI.color = _defaultColor;
        }

        public static void DrawDefenceIDInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.SelectedDefenceId = EditorGUILayout.IntField("ID поточної перепони: ", _database.PlayerPrefs.SelectedDefenceId);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawGrenadeIDInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.SelectedGrenadeId = EditorGUILayout.IntField("ID поточної гранати: ", _database.PlayerPrefs.SelectedGrenadeId);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawWeaponIDInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.SelectedWeaponId = EditorGUILayout.IntField("ID поточної зброї: ", _database.PlayerPrefs.SelectedWeaponId);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawLevelInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.LevelId = EditorGUILayout.IntField("Рівень на якому зупинились: ", _database.PlayerPrefs.LevelId);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawCoinInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.Coin = EditorGUILayout.IntField("К-сть діамантів: ", _database.PlayerPrefs.Coin);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawSoundMusikInfo()
        {
            GUI.color = _defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUI.color = Color.yellow;
                _database.PlayerPrefs.Music = (mySwitch)EditorGUILayout.EnumPopup("Музика: ", _database.PlayerPrefs.Music);
                _database.PlayerPrefs.Sound = (mySwitch)EditorGUILayout.EnumPopup("Звуки: ", _database.PlayerPrefs.Sound);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }

        private static void SavePersistentDatabase(PlayerPrefsDatabase database)
        {
            var path = Path.Combine(Application.persistentDataPath, $"Main_{database.name}.pso");
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var formatter = new BinaryFormatter();
            using var file = File.Create(path);
            formatter.Serialize(file, JsonUtility.ToJson(database));
        }
    }
}
