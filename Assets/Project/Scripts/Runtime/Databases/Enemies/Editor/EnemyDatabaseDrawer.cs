using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Bullets;

namespace SBabchuk.Runtime.Databases.Enemies
{
    public class EnemyDatabaseDrawer
    {
        static int selectedEnemy = 0;
        static Color defaultColor;
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
                        database.Enemies.Add(new Enemy(database.Enemies.Count));
                        selectedEnemy = database.Enemies.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(75)))
                    {
                        database.Enemies.Clear();
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
                            selectedEnemy = Mathf.Min(database.Enemies.Count == 0 ? 0 : database.Enemies.Count - 1, selectedEnemy + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Enemies != null)
                    {
                        if (database.Enemies.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Enemy _enemy in database.Enemies)
                                {
                                    if (DrawInfo(_enemy))
                                        break;
                                }
                            }
                            else
                            {
                                DrawInfo(database.Enemies[selectedEnemy]);
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
                    _enemy.Icon = (Sprite)EditorGUILayout.ObjectField(_enemy.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Enemies.Remove(_enemy);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _enemy.Id = EditorGUILayout.IntField("ID: ", _enemy.Id);
                    _enemy.Name = EditorGUILayout.TextField("Найменування: ", _enemy.Name);
                    Utils.CheckColor(_enemy.Gold, 0);
                    _enemy.Gold = EditorGUILayout.IntField("Кошти за смерть: ", _enemy.Gold);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.Health, 0);
                    _enemy.Health = EditorGUILayout.IntField("Кількість життів: ", _enemy.Health);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.SpeedMove, 0);
                    _enemy.SpeedMove = EditorGUILayout.Slider("Швидкість руху: ", _enemy.SpeedMove, 0, 20);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.AttackSpeed, 0);
                    _enemy.AttackSpeed = EditorGUILayout.Slider("Швидкість атаки: ", _enemy.AttackSpeed, 0, 5);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.Damage, 0);
                    _enemy.Damage = EditorGUILayout.IntField("Урон Юніта: ", _enemy.Damage);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.AttackRadius, 0);
                    _enemy.AttackRadius = EditorGUILayout.Slider("Радіус атаки: ", _enemy.AttackRadius, 0, 100);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_enemy.BulletId, -1);
                    _enemy.BulletId = (int)((BulletsName)EditorGUILayout.EnumPopup("Пуля(ID)", (BulletsName)_enemy.BulletId));
                    Utils.ChangeColor(defaultColor);
                    if (_enemy.BulletId != -1)
                        DrawBulletInfo(_enemy.BulletId);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }

        public static void DrawBulletInfo(int index)
        {
            if (EditorDatabaseLookup.Get<BulletDatabase>().Bullets.Count > 0)
            {
                foreach (Bullet bullet in EditorDatabaseLookup.Get<BulletDatabase>().Bullets)
                {
                    if (bullet.Id == index)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                bullet.Icon = (Sprite)EditorGUILayout.ObjectField(bullet.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndVertical();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("ID пулі: ");
            }
        }
    }
}
