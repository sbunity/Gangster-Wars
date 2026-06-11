using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{

    public class GrenadeInfoDrawer
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
        static string titleGrenade = "Show Grenades";

        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.database;

            defaultColor = PlayerPrefsDatabaseDrawer.defaultColor;

            DrawTittle();
        }


        /// <summary>
        /// Показуєм гранати
        /// </summary>
        public static void DrawTittle()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(titleGrenade, GUILayout.Width(125), GUILayout.Height(20)))
                {
                    if (titleGrenade == "Show Grenades")
                    {
                        titleGrenade = "Hide Grenades";
                    }
                    else
                    {
                        titleGrenade = "Show Grenades";
                    }
                }

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.grenades.Clear();
                }
            }
            GUILayout.EndHorizontal();

            if (titleGrenade == "Hide Grenades")
            {
                GUI.color = Color.grey;
                GUILayout.BeginHorizontal("box");
                {
                    GUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Інформація про гранати:");

                        if (database.PlayerPrefs.grenades != null)
                        {
                            if (database.PlayerPrefs.grenades.Count == EditorDatabaseLookup.Get<BombStoreDatabase>().grenades.Count)
                            {
                                foreach (GrenadeShortInfo _grenade in database.PlayerPrefs.grenades)
                                {
                                    DrawInfo(_grenade);
                                }
                            }
                            else
                            {
                                Debug.Log("database.PlayerPrefs.bonuses == 0");

                                database.PlayerPrefs.grenades.Clear();

                                foreach (Grenade _grenade in EditorDatabaseLookup.Get<BombStoreDatabase>().grenades)
                                {
                                    database.PlayerPrefs.grenades.Add(new GrenadeShortInfo(_grenade));
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("database.PlayerPrefs.bonuses == null");

                            database.PlayerPrefs.grenades = new List<GrenadeShortInfo>();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Показуєм заголовок інфи про гранати
        /// </summary>
        /// <param name="_value"></param>
        public static void DrawInfo(GrenadeShortInfo _value)
        {
            Grenade _grenade = EditorDatabaseLookup.Get<BombStoreDatabase>().GetGrenade(_value.id);

            GUI.color = defaultColor;

            GUILayout.BeginVertical("box");
            {
                if (_value.isBuy == mySwitch.On)
                    GUI.color = Color.cyan;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _grenade.ico = (Sprite)EditorGUILayout.ObjectField(_grenade.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        _grenade.id = EditorGUILayout.IntField("ID: ", _grenade.id);

                        _grenade.name = EditorGUILayout.TextField("Найменування: ", _grenade.name);

                        if (_value.isBuy == mySwitch.On)
                            GUI.color = Color.green;
                        _value.isBuy = ((mySwitch)EditorGUILayout.EnumPopup("Чи купленa: ", (mySwitch)_value.isBuy));


                        GUI.color = Color.green;
                        _value.count = EditorGUILayout.IntField("Кількість гранат цього типу на руках: ", _value.count);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUI.color = defaultColor;
            }
            GUILayout.EndVertical();
        }

    }
}
