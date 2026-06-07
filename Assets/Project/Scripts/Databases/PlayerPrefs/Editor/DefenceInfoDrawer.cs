using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class DefenceInfoDrawer
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
        static string titleDefence = "Show Defences";

        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.database;

            defaultColor = PlayerPrefsDatabaseDrawer.defaultColor;

            DrawTittle();
        }


        /// <summary>
        /// Показуєм перепони
        /// </summary>
        public static void DrawTittle()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(titleDefence, GUILayout.Width(125), GUILayout.Height(20)))
                {
                    if (titleDefence == "Show Defences")
                    {
                        titleDefence = "Hide Defences";
                    }
                    else
                    {
                        titleDefence = "Show Defences";
                    }
                }

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.defences.Clear();
                }
            }
            GUILayout.EndHorizontal();

            if (titleDefence == "Hide Defences")
            {
                GUI.color = Color.grey;
                GUILayout.BeginHorizontal("box");
                {
                    GUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Інформація про перепони:");

                        if (database.PlayerPrefs.defences != null)
                        {
                            if (database.PlayerPrefs.defences.Count == DefenseStoreDatabase.GetDatabase().defenses.Count)
                            {
                                foreach (DefenceShortInfo _defence in database.PlayerPrefs.defences)
                                {
                                    DrawInfo(_defence);
                                }
                            }
                            else
                            {
                                Debug.Log("database.PlayerPrefs.defences == 0");

                                database.PlayerPrefs.defences.Clear();

                                foreach (Defense _defence in DefenseStoreDatabase.GetDatabase().defenses)
                                {
                                    database.PlayerPrefs.defences.Add(new DefenceShortInfo(_defence));
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("database.PlayerPrefs.defences == null");

                            database.PlayerPrefs.defences = new List<DefenceShortInfo>();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Показуєм заголовок інфи про перепони
        /// </summary>
        /// <param name="_value"></param>
        public static void DrawInfo(DefenceShortInfo _value)
        {
            Defense _defence = DefenseStoreDatabase.GetDatabase().GetDefense(_value.id);

            GUI.color = defaultColor;

            GUILayout.BeginVertical("box");
            {
                if (_value.isBuy == mySwitch.On)
                    GUI.color = Color.cyan;

                if (database.PlayerPrefs.selectedDefenceID == _defence.id)
                    GUI.color = Color.green;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _defence.ico = (Sprite)EditorGUILayout.ObjectField(_defence.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        _defence.id = EditorGUILayout.IntField("ID: ", _defence.id);

                        _defence.name = EditorGUILayout.TextField("Найменування: ", _defence.name);

                        _value.isBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи доступна: ", (mySwitch)_value.isBuy));

                        if (_value.isBuy == mySwitch.On)
                        {
                            GUI.color = Color.green;
                            _value.upgradeID = EditorGUILayout.IntField("ID апгрейда: ", _value.upgradeID);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                if (_value.isBuy == mySwitch.On)
                {
                    GUILayout.BeginHorizontal("box");
                    {
                        DrawSettings(_defence, _value.upgradeID);
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм властивості перепони
        /// </summary>
        /// <param name="_stuff"></param>
        /// <param name="_upgradeID"></param>
        public static void DrawSettings(Defense _defence, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                DUpgrade _upgrade = DefenseStoreDatabase.GetDatabase().GetUpgrade(_defence, _upgradeID);

                if (_upgrade != null)
                {
                    _upgrade.settings.health = EditorGUILayout.IntSlider("К-сть життів: ", _upgrade.settings.health, 0, 1000);
                }
                else
                {
                    _defence.settings.health = EditorGUILayout.IntSlider("К-сть життів(без апгрейда): ", _defence.settings.health, 0, 1000);
                }
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

    }
}
