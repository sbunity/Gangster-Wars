using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class BombStoreDatabaseDrawer
    {
        static Color defaultColor;
        static int selected = 0;
        private static int selectedMode = 0;
        static private BombStoreDatabase database;
        static string titleBttnVisibleUpgrade = "Show";
        public static void Draw(BombStoreDatabase _database, int _selectedMode)
        {
            if (database == null)
                database = _database;
            selectedMode = _selectedMode;
            defaultColor = GUI.color;
            DrawNavigation();
        }

        public static void DrawNavigation()
        {
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Р СњР В°Р В»Р В°РЎв‚¬РЎвЂљРЎС“Р Р†Р В°Р Р…Р Р…РЎРЏ:");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Р вЂќР С•Р В±Р В°Р Р†Р С‘РЎвЂљР С‘ Р Р…Р С•Р Р†Р С‘Р в„– Р В·Р В°Р С—Р С‘РЎРѓ"))
                    {
                        database.Grenades.Add(new Grenade(database.Grenades.Count));
                        selected = database.Grenades.Count - 1;
                    }

                    if (GUILayout.Button("Р вЂ™Р С‘Р Т‘Р В°Р В»Р С‘РЎвЂљР С‘ Р Р†РЎРѓРЎвЂ“ Р В·Р В°Р С—Р С‘РЎРѓР С‘", GUILayout.Width(150)))
                    {
                        database.Grenades.Clear();
                        selected = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    if (selectedMode == 1)
                    {
                        if (GUILayout.Button("<--"))
                        {
                            selected = Mathf.Max(0, selected - 1);
                        }

                        if (GUILayout.Button("-->"))
                        {
                            selected = Mathf.Min(database.Grenades.Count == 0 ? 0 : database.Grenades.Count - 1, selected + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Grenades != null)
                    {
                        if (database.Grenades.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Grenade _grenade in database.Grenades)
                                {
                                    if (DrawGrenade(_grenade))
                                        break;
                                }
                            }
                            else
                            {
                                DrawGrenade(database.Grenades[selected]);
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public static bool DrawGrenade(Grenade _record)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Р вЂ™Р С‘Р Т‘Р В°Р В»Р С‘РЎвЂљР С‘", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Grenades.Remove(_record);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _record.Id = EditorGUILayout.IntField("ID: ", _record.Id);
                    _record.Name = EditorGUILayout.TextField("Р СњР В°Р в„–Р СР ВµР Р…РЎС“Р Р†Р В°Р Р…Р Р…РЎРЏ: ", _record.Name);
                    Utils.CheckColor(_record.Price, 0);
                    _record.Price = EditorGUILayout.IntField("Р вЂ™Р В°РЎР‚РЎвЂљРЎвЂ“РЎРѓРЎвЂљРЎРЉ: ", _record.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Damage, 0);
                    _record.Damage = EditorGUILayout.IntSlider("Р Р€РЎР‚Р С•Р Р…: ", _record.Damage, 0, 100);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Delay, 0);
                    _record.Delay = EditorGUILayout.Slider("Р вЂ”Р В°РЎвЂљРЎР‚Р С‘Р СР С”Р В° Р Т‘Р С• Р В·РЎР‚Р С‘Р Р†РЎС“: ", _record.Delay, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Time, 0);
                    _record.Time = EditorGUILayout.Slider("Р В§Р В°РЎРѓ Р Т‘РЎвЂ“РЎвЂ”(Р Т‘Р В»РЎРЏ Р СР С•Р В»Р С•РЎвЂљР С•Р Р†Р В°): ", _record.Time, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Radius, 0);
                    _record.Radius = EditorGUILayout.Slider("Р В Р В°Р Т‘РЎвЂ“РЎС“РЎРѓ Р Т‘РЎвЂ“РЎвЂ”: ", _record.Radius, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    _record.Collision = ((CollisionsName)EditorGUILayout.EnumPopup("Р СџР В°РЎР‚РЎвЂљРЎвЂ“Р С”Р В» Р В·РЎР‚Р С‘Р Р†РЎС“", (CollisionsName)_record.Collision));
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }
    }
}
