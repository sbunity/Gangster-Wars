using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class LevelInfoDrawer
    {
        /// <summary>
        /// Дефолтний колір
        /// </summary>
        static Color defaultColor;

        /// <summary>
        /// База даних
        /// </summary>
        static PlayerPrefsDatabase database;

        /// <summary>
        /// Заголовок для кнопки
        /// </summary>
        static string titleLevel = "Show Levels";

        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.database;

            defaultColor = PlayerPrefsDatabaseDrawer.defaultColor;

            DrawTittle();
        }

        /// <summary>
        /// Показуєм заголовок
        /// </summary>
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
                    database.PlayerPrefs.levels.Clear();
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

                        if (database.PlayerPrefs.levels != null)
                        {
                            if (database.PlayerPrefs.levels.Count == EditorDatabaseLookup.Get<LevelDatabase>().levels.Count)
                            {
                                foreach (LevelShortInfo _level in database.PlayerPrefs.levels)
                                {
                                    DrawInfo(_level);
                                }
                            }
                            else
                            {
                                Debug.Log("database.PlayerPrefs.levels == 0");

                                database.PlayerPrefs.levels.Clear();

                                foreach (Level _level in EditorDatabaseLookup.Get<LevelDatabase>().levels)
                                {
                                    database.PlayerPrefs.levels.Add(new LevelShortInfo(_level));
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("database.PlayerPrefs.defences == null");

                            database.PlayerPrefs.levels = new List<LevelShortInfo>();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Показуєм інформацію
        /// </summary>
        /// <param name="_value"></param>
        public static void DrawInfo(LevelShortInfo _value)
        {
            Level _record = EditorDatabaseLookup.Get<LevelDatabase>().GetLevel(_value.id);

            GUI.color = defaultColor;

            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _record.ico = (Sprite)EditorGUILayout.ObjectField(_record.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }
                    GUILayout.EndVertical();

                    if (_value.isCompleted == mySwitch.On)
                        Utils.ChangeColor(Color.green);

                    GUILayout.BeginVertical();
                    {
                        _record.id = EditorGUILayout.IntField("ID: ", _record.id);

                        _record.name = EditorGUILayout.TextField("Найменування: ", _record.name);

                        //_value.isOpened = ((mySwitch)EditorGUILayout.EnumPopup("Чи відкритий: ", (mySwitch)_value.isOpened));

                        _value.isCompleted = ((mySwitch)EditorGUILayout.EnumPopup("Чи продений: ", (mySwitch)_value.isCompleted));

                        if (_value.isCompleted == mySwitch.On)
                            _value.stars = EditorGUILayout.IntSlider("Успішність проходження: ", _value.stars, 0, 3);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
