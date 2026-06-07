using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class PersonageInfoDrawer 
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
        static string titlePersonage = "Show Personages";

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
                if (GUILayout.Button(titlePersonage, GUILayout.Width(125), GUILayout.Height(20)))
                {
                    if (titlePersonage == "Show Personages")
                    {
                        titlePersonage = "Hide Personages";
                    }
                    else
                    {
                        titlePersonage = "Show Personages";
                    }
                }

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.personages.Clear();
                }
            }
            GUILayout.EndHorizontal();

            if (titlePersonage == "Hide Personages")
            {
                GUI.color = Color.grey;
                GUILayout.BeginHorizontal("box");
                {
                    GUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Інформація про персонажів:");

                        if (database.PlayerPrefs.personages != null)
                        {
                            if (database.PlayerPrefs.personages.Count == MainPlayerDatabase.GetDatabase().personages.Count)
                            {
                                foreach (PersonageShortInfo _personage in database.PlayerPrefs.personages)
                                {
                                    DrawInfo(_personage);
                                }
                            }
                            else
                            {
                                Debug.Log("database.PlayerPrefs.personages == 0");

                                database.PlayerPrefs.personages.Clear();

                                foreach (Personage _personage in MainPlayerDatabase.GetDatabase().personages)
                                {
                                    database.PlayerPrefs.personages.Add(new PersonageShortInfo(_personage));
                                }
                                Debug.Log("$$$ " + database.PlayerPrefs.personages.Count);
                            }
                        }
                        else
                        {
                            Debug.Log("database.PlayerPrefs.personages == null");

                            database.PlayerPrefs.personages = new List<PersonageShortInfo>();
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
        public static void DrawInfo(PersonageShortInfo _value)
        {
            Personage _record = MainPlayerDatabase.GetDatabase().GetPersonage(_value.id);

            GUI.color = defaultColor;

            GUILayout.BeginVertical("box");
            {
                if (_value.isBuy == mySwitch.On)
                    Utils.ChangeColor(Color.cyan);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _record.ico = (Sprite)EditorGUILayout.ObjectField(_record.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        _record.id = EditorGUILayout.IntField("ID: ", _record.id);

                        _record.name = EditorGUILayout.TextField("Найменування: ", _record.name);

                        _value.isBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи доступна: ", (mySwitch)_value.isBuy));

                        if (_value.isBuy == mySwitch.On)
                        {
                            _value.upgradeID = EditorGUILayout.IntSlider(" Апгрейд (ID): ", _value.upgradeID, -1, _record.upgrades.Count - 1);

                            DrawSettings(_record, _value.upgradeID);
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Показуєм властивості зброї
        /// </summary>
        /// <param name="_stuff"></param>
        /// <param name="_upgradeID"></param>
        public static void DrawSettings(Personage _personage, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                PUpgrade _upgrade = MainPlayerDatabase.GetDatabase().GetUpgrade(_personage, _upgradeID);

                if (_upgrade != null)
                {
                    _upgrade.settings.damage = EditorGUILayout.IntSlider("Урон: ", _upgrade.settings.damage, 0, 100);

                    _upgrade.settings.speedAtack = EditorGUILayout.Slider("Швидкість стрельби: ", _upgrade.settings.speedAtack, 0, 10);
                }
                else
                {
                    _personage.settings.damage = EditorGUILayout.IntSlider("Урон: ", _personage.settings.damage, 0, 100);

                    _personage.settings.speedAtack = EditorGUILayout.Slider("Швидкість стрiльби: ", _personage.settings.speedAtack, 0, 10);
                }
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }
    }
}