using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class GrenadeInfoDrawer
    {
        static Color defaultColor;
        static PlayerPrefsDatabase database;
        static string titleGrenade = "Show Grenades";
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
                    database.PlayerPrefs.Grenades.Clear();
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
                        EditorGUILayout.LabelField("Р†РЅС„РѕСЂРјР°С†С–СЏ РїСЂРѕ РіСЂР°РЅР°С‚Рё:");
                        if (database.PlayerPrefs.Grenades != null)
                        {
                            if (database.PlayerPrefs.Grenades.Count == EditorDatabaseLookup.Get<BombStoreDatabase>().Grenades.Count)
                            {
                                foreach (GrenadeShortInfo _grenade in database.PlayerPrefs.Grenades)
                                {
                                    DrawInfo(_grenade);
                                }
                            }
                            else
                            {
                                database.PlayerPrefs.Grenades.Clear();
                                foreach (Grenade _grenade in EditorDatabaseLookup.Get<BombStoreDatabase>().Grenades)
                                {
                                    database.PlayerPrefs.Grenades.Add(new GrenadeShortInfo(_grenade));
                                }
                            }
                        }
                        else
                        {
                            database.PlayerPrefs.Grenades = new List<GrenadeShortInfo>();
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
        }

        public static void DrawInfo(GrenadeShortInfo _value)
        {
            Grenade _grenade = EditorDatabaseLookup.Get<BombStoreDatabase>().GetGrenade(_value.Id);
            GUI.color = defaultColor;
            GUILayout.BeginVertical("box");
            {
                if (_value.IsBuy == mySwitch.On)
                    GUI.color = Color.cyan;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        _grenade.Icon = (Sprite)EditorGUILayout.ObjectField(_grenade.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        _grenade.Id = EditorGUILayout.IntField("ID: ", _grenade.Id);
                        _grenade.Name = EditorGUILayout.TextField("РќР°Р№РјРµРЅСѓРІР°РЅРЅСЏ: ", _grenade.Name);
                        if (_value.IsBuy == mySwitch.On)
                            GUI.color = Color.green;
                        _value.IsBuy = ((mySwitch)EditorGUILayout.EnumPopup("Р§Рё РєСѓРїР»РµРЅa: ", (mySwitch)_value.IsBuy));
                        GUI.color = Color.green;
                        _value.Count = EditorGUILayout.IntField("РљС–Р»СЊРєС–СЃС‚СЊ РіСЂР°РЅР°С‚ С†СЊРѕРіРѕ С‚РёРїСѓ РЅР° СЂСѓРєР°С…: ", _value.Count);
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
