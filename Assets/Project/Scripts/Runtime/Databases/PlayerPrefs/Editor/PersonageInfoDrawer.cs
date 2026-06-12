using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.MainPlayers;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    public class PersonageInfoDrawer
    {
        static Color defaultColor;
        static PlayerPrefsDatabase database;
        static string titlePersonage = "Show Personages";
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
                    database.PlayerPrefs.Personages.Clear();
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
                        if (database.PlayerPrefs.Personages != null)
                        {
                            if (database.PlayerPrefs.Personages.Count == EditorDatabaseLookup.Get<MainPlayerDatabase>().Personages.Count)
                            {
                                foreach (PersonageShortInfo _personage in database.PlayerPrefs.Personages)
                                {
                                    DrawInfo(_personage);
                                }
                            }
                            else
                            {
                                database.PlayerPrefs.Personages.Clear();
                                foreach (Personage _personage in EditorDatabaseLookup.Get<MainPlayerDatabase>().Personages)
                                {
                                    database.PlayerPrefs.Personages.Add(new PersonageShortInfo(_personage));
                                }
                            }
                        }
                        else
                        {
                            database.PlayerPrefs.Personages = new List<PersonageShortInfo>();
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
        }

        public static void DrawInfo(PersonageShortInfo _value)
        {
            Personage _record = EditorDatabaseLookup.Get<MainPlayerDatabase>().GetPersonage(_value.Id);
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                if (_value.IsBuy == mySwitch.On)
                    Utils.ChangeColor(Color.cyan);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        _record.Id = EditorGUILayout.IntField("ID: ", _record.Id);
                        _record.Name = EditorGUILayout.TextField("Найменування: ", _record.Name);
                        _value.IsBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи доступна: ", (mySwitch)_value.IsBuy));
                        if (_value.IsBuy == mySwitch.On)
                        {
                            _value.UpgradeId = EditorGUILayout.IntSlider(" Апгрейд (ID): ", _value.UpgradeId, -1, _record.Upgrades.Count - 1);
                            DrawSettings(_record, _value.UpgradeId);
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        public static void DrawSettings(Personage _personage, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                PUpgrade _upgrade = EditorDatabaseLookup.Get<MainPlayerDatabase>().GetUpgrade(_personage, _upgradeID);
                if (_upgrade != null)
                {
                    _upgrade.Settings.Damage = EditorGUILayout.IntSlider("Урон: ", _upgrade.Settings.Damage, 0, 100);
                    _upgrade.Settings.AttackSpeed = EditorGUILayout.Slider("Швидкість стрельби: ", _upgrade.Settings.AttackSpeed, 0, 10);
                }
                else
                {
                    _personage.Settings.Damage = EditorGUILayout.IntSlider("Урон: ", _personage.Settings.Damage, 0, 100);
                    _personage.Settings.AttackSpeed = EditorGUILayout.Slider("Швидкість стрiльби: ", _personage.Settings.AttackSpeed, 0, 10);
                }

                GUI.color = defaultColor;
            }

            GUILayout.EndVertical();
        }
    }
}
