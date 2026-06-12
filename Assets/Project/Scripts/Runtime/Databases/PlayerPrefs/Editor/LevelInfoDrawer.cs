using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Levels;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    public class LevelInfoDrawer
    {
        static Color defaultColor;
        static PlayerPrefsDatabase database;
        static string titleLevel = "Show Levels";
        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.Database;
            defaultColor = PlayerPrefsDatabaseDrawer.DefaultColor;
            DrawTittle();
        }

        public static void DrawTittle()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(titleLevel, GUILayout.Width(125), GUILayout.Height(20)))
                {
                    if (titleLevel == "Show Levels")
                    {
                        titleLevel = "Hide Levels";
                    }
                    else
                    {
                        titleLevel = "Show Levels";
                    }
                }

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.Levels.Clear();
                }
            }

            GUILayout.EndHorizontal();
            if (titleLevel == "Hide Levels")
            {
                GUI.color = Color.grey;
                GUILayout.BeginHorizontal("box");
                {
                    GUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Інформація про персонажів:");
                        if (database.PlayerPrefs.Levels != null)
                        {
                            if (database.PlayerPrefs.Levels.Count == EditorDatabaseLookup.Get<LevelDatabase>().Levels.Count)
                            {
                                foreach (LevelShortInfo _level in database.PlayerPrefs.Levels)
                                {
                                    DrawInfo(_level);
                                }
                            }
                            else
                            {
                                database.PlayerPrefs.Levels.Clear();
                                foreach (Level _level in EditorDatabaseLookup.Get<LevelDatabase>().Levels)
                                {
                                    database.PlayerPrefs.Levels.Add(new LevelShortInfo(_level));
                                }
                            }
                        }
                        else
                        {
                            database.PlayerPrefs.Levels = new List<LevelShortInfo>();
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
        }

        public static void DrawInfo(LevelShortInfo _value)
        {
            Level _record = EditorDatabaseLookup.Get<LevelDatabase>().GetLevel(_value.Id);
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }

                    GUILayout.EndVertical();
                    if (_value.IsCompleted == mySwitch.On)
                        Utils.ChangeColor(Color.green);
                    GUILayout.BeginVertical();
                    {
                        _record.Id = EditorGUILayout.IntField("ID: ", _record.Id);
                        _record.Name = EditorGUILayout.TextField("Найменування: ", _record.Name);
                        _value.IsCompleted = ((mySwitch)EditorGUILayout.EnumPopup("Чи продений: ", (mySwitch)_value.IsCompleted));
                        if (_value.IsCompleted == mySwitch.On)
                            _value.Stars = EditorGUILayout.IntSlider("Успішність проходження: ", _value.Stars, 0, 3);
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}
