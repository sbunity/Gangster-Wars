using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    public class WeaponInfoDrawer
    {
        static Color defaultColor;
        static PlayerPrefsDatabase database;
        static string titleWeapon = "Show Weapons";
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
                if (GUILayout.Button(titleWeapon, GUILayout.Width(125), GUILayout.Height(20)))
                {
                    if (titleWeapon == "Show Weapons")
                    {
                        titleWeapon = "Hide Weapons";
                    }
                    else
                    {
                        titleWeapon = "Show Weapons";
                    }
                }

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.Weapons.Clear();
                }
            }

            GUILayout.EndHorizontal();
            if (titleWeapon == "Hide Weapons")
            {
                GUI.color = Color.grey;
                GUILayout.BeginHorizontal("box");
                {
                    GUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Інформація про зброю:");
                        if (database.PlayerPrefs.Weapons != null)
                        {
                            if (database.PlayerPrefs.Weapons.Count == EditorDatabaseLookup.Get<WeaponStoreDatabase>().Weapons.Count)
                            {
                                foreach (WeaponShortInfo _weapon in database.PlayerPrefs.Weapons)
                                {
                                    DrawInfo(_weapon);
                                }
                            }
                            else
                            {
                                database.PlayerPrefs.Weapons.Clear();
                                foreach (Weapon _weapon in EditorDatabaseLookup.Get<WeaponStoreDatabase>().Weapons)
                                {
                                    database.PlayerPrefs.Weapons.Add(new WeaponShortInfo(_weapon));
                                }
                            }
                        }
                        else
                        {
                            database.PlayerPrefs.Weapons = new List<WeaponShortInfo>();
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
        }

        public static void DrawInfo(WeaponShortInfo _value)
        {
            Weapon _weapon = EditorDatabaseLookup.Get<WeaponStoreDatabase>().GetWeapon(_value.Id);
            if (_weapon != null)
            {
                if (_value.IsBuy == mySwitch.On)
                    GUI.color = Color.cyan;
                if (database.PlayerPrefs.SelectedWeaponId == _weapon.Id)
                    GUI.color = Color.green;
                GUILayout.BeginVertical("box");
                {
                    GUI.color = defaultColor;
                    GUILayout.BeginHorizontal("box");
                    {
                        GUILayout.BeginVertical();
                        {
                            _weapon.Icon = (Sprite)EditorGUILayout.ObjectField(_weapon.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                        }

                        GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                        {
                            _weapon.Id = EditorGUILayout.IntField("ID: ", _weapon.Id);
                            _weapon.Name = EditorGUILayout.TextField("Найменування зброї: ", _weapon.Name);
                            if (_value.IsBuy == mySwitch.On)
                                GUI.color = Color.green;
                            _value.IsBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи купленa: ", (mySwitch)_value.IsBuy));
                            if (_value.IsBuy == mySwitch.On)
                            {
                                GUI.color = Color.yellow;
                                _value.AmmoCount = EditorGUILayout.IntField("Кількість патронів які знайшли: ", _value.AmmoCount);
                                _value.UpgradeId = EditorGUILayout.IntSlider(" Апгрейд (ID): ", _value.UpgradeId, -1, _weapon.Upgrades.Count - 1);
                            }

                            GUI.color = defaultColor;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();
                    if (_value.IsBuy == mySwitch.On)
                    {
                        GUILayout.BeginHorizontal("box");
                        {
                            DrawSettings(_weapon, _value.UpgradeId);
                        }

                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndHorizontal();
                GUI.color = defaultColor;
            }
            else
            {
                EditorGUILayout.LabelField("Не знайдено посоха");
            }
        }

        public static void DrawSettings(Weapon _weapon, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                WUpgrade _upgrade = EditorDatabaseLookup.Get<WeaponStoreDatabase>().GetUpgrade(_weapon, _upgradeID);
                if (_upgrade != null)
                {
                    _upgrade.Settings.Damage = EditorGUILayout.IntSlider("Урон: ", _upgrade.Settings.Damage, 0, 10);
                }
                else
                {
                    _weapon.Settings.Damage = EditorGUILayout.IntSlider("Урон: ", _weapon.Settings.Damage, 0, 10);
                }

                GUI.color = defaultColor;
            }

            GUILayout.EndVertical();
        }
    }
}
