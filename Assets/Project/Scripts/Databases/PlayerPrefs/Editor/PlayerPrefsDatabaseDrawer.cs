using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
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
            if (database.PlayerPrefs == null)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Р”РѕР±Р°РІРёС‚Рё pPrefs"))
                    {
                        database.PlayerPrefs = new PlayerPrefs();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _database = database;
                GUI.color = Color.red;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Р’РёРґР°Р»РёС‚Рё pPrefs"))
                    {
                        database.PlayerPrefs = null;
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUI.color = Color.green;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Р—Р±РµСЂРµРіС‚Рё Р·РјС–РЅРё РІ pPrefs"))
                    {
                        EditorUtility.SetDirty(database);
                        AssetDatabase.SaveAssets();
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUI.color = _defaultColor;
                DrawInfo(_database);
            }
        }

        public static void DrawInfo(PlayerPrefsDatabase _database)
        {
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
                _database.PlayerPrefs.SelectedDefenceId = EditorGUILayout.IntField("ID РїРѕС‚РѕС‡РЅРѕС— РїРµСЂРµРїРѕРЅРё: ", _database.PlayerPrefs.SelectedDefenceId);
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
                _database.PlayerPrefs.SelectedGrenadeId = EditorGUILayout.IntField("ID РїРѕС‚РѕС‡РЅРѕС— РіСЂР°РЅР°С‚Рё: ", _database.PlayerPrefs.SelectedGrenadeId);
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
                _database.PlayerPrefs.SelectedWeaponId = EditorGUILayout.IntField("ID РїРѕС‚РѕС‡РЅРѕС— Р·Р±СЂРѕС—: ", _database.PlayerPrefs.SelectedWeaponId);
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
                _database.PlayerPrefs.LevelId = EditorGUILayout.IntField("Р С–РІРµРЅСЊ РЅР° СЏРєРѕРјСѓ Р·СѓРїРёРЅРёР»РёСЃСЊ: ", _database.PlayerPrefs.LevelId);
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
                _database.PlayerPrefs.Coin = EditorGUILayout.IntField("Рљ-СЃС‚СЊ РґС–Р°РјР°РЅС‚С–РІ: ", _database.PlayerPrefs.Coin);
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
                _database.PlayerPrefs.Music = (mySwitch)EditorGUILayout.EnumPopup("РњСѓР·РёРєР°: ", _database.PlayerPrefs.Music);
                _database.PlayerPrefs.Sound = (mySwitch)EditorGUILayout.EnumPopup("Р—РІСѓРєРё: ", _database.PlayerPrefs.Sound);
                GUI.color = _defaultColor;
            }

            GUILayout.EndVertical();
        }
    }
}
