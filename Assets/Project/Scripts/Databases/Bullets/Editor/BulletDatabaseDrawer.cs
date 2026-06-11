using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class BulletDatabaseDrawer
    {
        static int selected = 0;
        static Color defaultColor;
        static private BulletDatabase database;
        public static void Draw(BulletDatabase _database, int selectedMode)
        {
            if (database == null)
                database = _database;
            defaultColor = GUI.color;
            Utils.ChangeColor(Color.grey);
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Налаштування:");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.Bullets.Add(new Bullet(database.Bullets.Count));
                        selected = database.Bullets.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(175)))
                    {
                        database.Bullets.Clear();
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
                            selected = Mathf.Min(database.Bullets.Count == 0 ? 0 : database.Bullets.Count - 1, selected + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Bullets != null)
                    {
                        if (database.Bullets.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Bullet _bullet in database.Bullets)
                                {
                                    if (DrawInfo(_bullet))
                                        break;
                                }
                            }
                            else
                            {
                                DrawInfo(database.Bullets[selected]);
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public static bool DrawInfo(Bullet _bullet)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _bullet.Icon = (Sprite)EditorGUILayout.ObjectField(_bullet.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Bullets.Remove(_bullet);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _bullet.Id = EditorGUILayout.IntField("ID: ", _bullet.Id);
                    _bullet.Name = EditorGUILayout.TextField("Найменування: ", _bullet.Name);
                    Utils.CheckColor(_bullet.SpeedMove, 0);
                    _bullet.SpeedMove = EditorGUILayout.FloatField("Швидкість: ", _bullet.SpeedMove);
                    Utils.ChangeColor(defaultColor);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }
    }
}
