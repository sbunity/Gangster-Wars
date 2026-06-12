using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk.Runtime.Databases.BombStore
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
                EditorGUILayout.LabelField("Налаштування:");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.Grenades.Add(new Grenade(database.Grenades.Count));
                        selected = database.Grenades.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(150)))
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
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
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
                    _record.Name = EditorGUILayout.TextField("Найменування: ", _record.Name);
                    Utils.CheckColor(_record.Price, 0);
                    _record.Price = EditorGUILayout.IntField("Вартість: ", _record.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Damage, 0);
                    _record.Damage = EditorGUILayout.IntSlider("Урон: ", _record.Damage, 0, 100);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Delay, 0);
                    _record.Delay = EditorGUILayout.Slider("Затримка до зриву: ", _record.Delay, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Time, 0);
                    _record.Time = EditorGUILayout.Slider("Час дії(для молотова): ", _record.Time, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Radius, 0);
                    _record.Radius = EditorGUILayout.Slider("Радіус дії: ", _record.Radius, 0, 10);
                    Utils.ChangeColor(defaultColor);
                    _record.Collision = ((CollisionsName)EditorGUILayout.EnumPopup("Партікл зриву", (CollisionsName)_record.Collision));
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }
    }
}
