using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class EnemyDatabaseDrawer 
    {
        /// <summary>
        /// Вибраний ворог
        /// </summary>
        static int selectedEnemy = 0;

        /// <summary>
        /// Змінна для зберігання кольрі по дефолту
        /// </summary>
        static Color defaultColor;

        /// <summary>
        /// База даних
        /// </summary>
        static private EnemyDatabase database;

        public static void Draw(EnemyDatabase _database, int selectedMode)
        {
            if (database == null)
                database = _database;

            defaultColor = GUI.color;
            GUI.color = Color.grey;

            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Налаштування:");

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.enemies.Add(new Enemy(database.enemies.Count));
                        selectedEnemy = database.enemies.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(75)))
                    {
                        database.enemies.Clear();
                        selectedEnemy = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    if (selectedMode == 1)
                    {
                        if (GUILayout.Button("<--"))
                        {
                            selectedEnemy = Mathf.Max(0, selectedEnemy - 1);
                        }
                        if (GUILayout.Button("-->"))
                        {
                            selectedEnemy = Mathf.Min(database.enemies.Count == 0 ? 0 : database.enemies.Count - 1, selectedEnemy + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.enemies != null)
                    {
                        if (database.enemies.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Enemy _enemy in database.enemies)
                                {
                                    if (DrawInfo(_enemy))
                                        break;
                                }
                            }
                            else
                            {
                                DrawInfo(database.enemies[selectedEnemy]);
                            }
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        public static bool DrawInfo(Enemy _enemy)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _enemy.ico = (Sprite)EditorGUILayout.ObjectField(_enemy.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.enemies.Remove(_enemy);
                        return true;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _enemy.id = EditorGUILayout.IntField("ID: ", _enemy.id);

                    _enemy.name = EditorGUILayout.TextField("Найменування: ", _enemy.name);

                    Utils.CheckColor(_enemy.gold, 0);
                    _enemy.gold = EditorGUILayout.IntField("Кошти за смерть: ", _enemy.gold);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.health, 0);
                    _enemy.health = EditorGUILayout.IntField("Кількість життів: ", _enemy.health);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.speedMove, 0);
                    _enemy.speedMove = EditorGUILayout.Slider("Швидкість руху: ", _enemy.speedMove, 0, 20);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.speedAtack, 0);
                    _enemy.speedAtack = EditorGUILayout.Slider("Швидкість атаки: ", _enemy.speedAtack, 0, 5);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.damage, 0);
                    _enemy.damage = EditorGUILayout.IntField("Урон Юніта: ", _enemy.damage);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.radiusAtack, 0);
                    _enemy.radiusAtack = EditorGUILayout.Slider("Радіус атаки: ", _enemy.radiusAtack, 0, 100);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_enemy.bulletID, -1);
                    _enemy.bulletID = (int)((BulletsName)EditorGUILayout.EnumPopup("Пуля(ID)", (BulletsName)_enemy.bulletID));
                    Utils.ChangeColor(defaultColor);

                    if (_enemy.bulletID != -1)
                        DrawBulletInfo(_enemy.bulletID);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            return false;
        }

        /// <summary>
		/// Вимальовує інформацію про пулю в інформації про ворога
		/// </summary>
		/// <param name="index">Index.</param>
		public static void DrawBulletInfo(int index)
        {
            if (EditorDatabaseLookup.Get<BulletDatabase>().bullets.Count > 0)
            {
                foreach (Bullet bullet in EditorDatabaseLookup.Get<BulletDatabase>().bullets)
                {
                    if (bullet.id == index)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                bullet.ico = (Sprite)EditorGUILayout.ObjectField(bullet.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
  //                              GUILayout.BeginVertical();
  //                              {
 //                                 bullet.id = EditorGUILayout.IntField("ID пулі: ", bullet.id);
 //                                   bullet.name = EditorGUILayout.TextField("Найменування пулі: ", bullet.name);
  //                                  bullet.speedMove = EditorGUILayout.FloatField("Швидкість руху: ", bullet.speedMove);
  //                                  bullet.damage = EditorGUILayout.IntField("Урон: ", bullet.damage);
  //                              }
  //                              GUILayout.EndVertical();
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Немає записів");
            }
        }

    }
}
