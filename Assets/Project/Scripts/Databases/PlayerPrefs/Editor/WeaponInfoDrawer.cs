using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class WeaponInfoDrawer
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
        static string titleWeapon = "Show Weapons";

        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.database;

            defaultColor = PlayerPrefsDatabaseDrawer.defaultColor;

            DrawTittle();
        }

        /// <summary>
        /// Показуєм заголовок для зброї
        /// </summary>
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
                    database.PlayerPrefs.weapons.Clear();
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

                        if (database.PlayerPrefs.weapons != null)
                        {
                            if (database.PlayerPrefs.weapons.Count == WeaponStoreDatabase.GetDatabase().weapons.Count)
                            {
                                foreach (WeaponShortInfo _weapon in database.PlayerPrefs.weapons)
                                {
                                    DrawInfo(_weapon);
                                }
                            }
                            else
                            {
                                Debug.Log("database.PlayerPrefs.stuffs == 0");

                                database.PlayerPrefs.weapons.Clear();

                                foreach (Weapon _weapon in WeaponStoreDatabase.GetDatabase().weapons)
                                {
                                    database.PlayerPrefs.weapons.Add(new WeaponShortInfo(_weapon));
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("database.PlayerPrefs.stuffs == null");

                            database.PlayerPrefs.weapons = new List<WeaponShortInfo>();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Показуєм інформацію для зброї
        /// </summary>
        /// <param name="_value"></param>
        public static void DrawInfo(WeaponShortInfo _value)
        {
            Weapon _weapon = WeaponStoreDatabase.GetDatabase().GetWeapon(_value.id);

            if (_weapon != null)
            {
                if (_value.isBuy == mySwitch.On)
                    GUI.color = Color.cyan;

                if (database.PlayerPrefs.selectedWeaponID == _weapon.id)
                    GUI.color = Color.green;

                GUILayout.BeginVertical("box");
                {
                    GUI.color = defaultColor;
                    GUILayout.BeginHorizontal("box");
                    {
                        GUILayout.BeginVertical();
                        {
                            _weapon.ico = (Sprite)EditorGUILayout.ObjectField(_weapon.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                        }
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical();
                        {
                            _weapon.id = EditorGUILayout.IntField("ID: ", _weapon.id);

                            _weapon.name = EditorGUILayout.TextField("Найменування зброї: ", _weapon.name);


                            if (_value.isBuy == mySwitch.On)
                                GUI.color = Color.green;
                            _value.isBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи купленa: ", (mySwitch)_value.isBuy));

                            if (_value.isBuy == mySwitch.On)
                            {
                                GUI.color = Color.yellow;

                                _value.countPatrons = EditorGUILayout.IntField("Кількість патронів які знайшли: ", _value.countPatrons);

                                _value.upgradeID = EditorGUILayout.IntSlider(" Апгрейд (ID): ", _value.upgradeID, -1, _weapon.upgrades.Count - 1);
                            }

                            GUI.color = defaultColor;
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();

                    if (_value.isBuy == mySwitch.On)
                    {
                        GUILayout.BeginHorizontal("box");
                        {
                            DrawSettings(_weapon, _value.upgradeID);
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

        /// <summary>
        /// Показуєм властивості зброї
        /// </summary>
        /// <param name="_stuff"></param>
        /// <param name="_upgradeID"></param>
        public static void DrawSettings(Weapon _weapon, int _upgradeID)
        {
            GUILayout.BeginVertical();
            {
                GUI.color = Color.yellow;
                WUpgrade _upgrade = WeaponStoreDatabase.GetDatabase().GetUpgrade(_weapon, _upgradeID);

                if (_upgrade != null)
                {
                    _upgrade.settings.damage = EditorGUILayout.IntSlider("Урон: ", _upgrade.settings.damage, 0, 10);
                }
                else
                {
                    _weapon.settings.damage = EditorGUILayout.IntSlider("Урон: ", _weapon.settings.damage, 0, 10);
                }
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

    }
}
