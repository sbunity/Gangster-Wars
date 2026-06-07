using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class BulletDatabaseDrawer
    {
        /// <summary>
        /// Вибрана пуля
        /// </summary>
        static int selected = 0;

        /// <summary>
        /// Змінна для зберігання кольрі по дефолту
        /// </summary>
        static Color defaultColor;

        /// <summary>
        /// База даних
        /// </summary>
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
                        database.bullets.Add(new Bullet(database.bullets.Count));
                        selected = database.bullets.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(175)))
                    {
                        database.bullets.Clear();
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
                            selected = Mathf.Min(database.bullets.Count == 0 ? 0 : database.bullets.Count - 1, selected + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.bullets != null)
                    {
                        if (database.bullets.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Bullet _bullet in database.bullets)
                                {
                                    if (DrawInfo(_bullet))
                                        break;
                                }
                            }
                            else
                            {
                                DrawInfo(database.bullets[selected]);
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
                    _bullet.ico = (Sprite)EditorGUILayout.ObjectField(_bullet.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));

                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.bullets.Remove(_bullet);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _bullet.id = EditorGUILayout.IntField("ID: ", _bullet.id);
                    _bullet.name = EditorGUILayout.TextField("Найменування: ", _bullet.name);

                    Utils.CheckColor(_bullet.speedMove, 0);
                    _bullet.speedMove = EditorGUILayout.FloatField("Швидкість: ", _bullet.speedMove);
                    Utils.ChangeColor(defaultColor);
                    //_bullet.damage = EditorGUILayout.IntField ("Урон: ", _bullet.damage);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            return false;
        }

    }
}
