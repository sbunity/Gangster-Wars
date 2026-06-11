using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class DefenceInfoDrawer
    {
        static Color defaultColor;
        static PlayerPrefsDatabase database;
        static string titleDefence = "Show Defences";
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
                    database.PlayerPrefs.Defences.Clear();
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
                        if (database.PlayerPrefs.Defences != null)
                        {
                            if (database.PlayerPrefs.Defences.Count == EditorDatabaseLookup.Get<DefenseStoreDatabase>().Defenses.Count)
                            {
                                foreach (DefenceShortInfo _defence in database.PlayerPrefs.Defences)
                                {
                                    DrawInfo(_defence);
                                }
                            }
                            else
                            {
                                database.PlayerPrefs.Defences.Clear();
                                foreach (Defense _defence in EditorDatabaseLookup.Get<DefenseStoreDatabase>().Defenses)
                                {
                                    database.PlayerPrefs.Defences.Add(new DefenceShortInfo(_defence));
                                }
                            }
                        }
                        else
                        {
                            database.PlayerPrefs.Defences = new List<DefenceShortInfo>();
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
        }

        public static void DrawInfo(DefenceShortInfo _value)
        {
            Defense _defence = EditorDatabaseLookup.Get<DefenseStoreDatabase>().GetDefense(_value.Id);
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                if (_value.IsBuy == mySwitch.On)
                    GUI.color = Color.cyan;
                if (database.PlayerPrefs.SelectedDefenceId == _defence.Id)
                    GUI.color = Color.green;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _defence.Icon = (Sprite)EditorGUILayout.ObjectField(_defence.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        _defence.Id = EditorGUILayout.IntField("ID: ", _defence.Id);
                        _defence.Name = EditorGUILayout.TextField("Найменування: ", _defence.Name);
                        _value.IsBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи доступна: ", (mySwitch)_value.IsBuy));
                        if (_value.IsBuy == mySwitch.On)
                        {
                            GUI.color = Color.green;
                            _value.UpgradeId = EditorGUILayout.IntField("ID апгрейда: ", _value.UpgradeId);
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
                if (_value.IsBuy == mySwitch.On)
                {
                    GUILayout.BeginHorizontal("box");
                    {
                        DrawSettings(_defence, _value.UpgradeId);
                    }

                    GUILayout.EndHorizontal();
                }

                GUI.color = defaultColor;
            }

            GUILayout.EndVertical();
        }

        public static void DrawSettings(Defense _defence, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                DUpgrade _upgrade = EditorDatabaseLookup.Get<DefenseStoreDatabase>().GetUpgrade(_defence, _upgradeID);
                if (_upgrade != null)
                {
                    _upgrade.Settings.Health = EditorGUILayout.IntSlider("К-сть життів: ", _upgrade.Settings.Health, 0, 1000);
                }
                else
                {
                    _defence.Settings.Health = EditorGUILayout.IntSlider("К-сть життів(без апгрейда): ", _defence.Settings.Health, 0, 1000);
                }

                GUI.color = defaultColor;
            }

            GUILayout.EndVertical();
        }
    }
}
