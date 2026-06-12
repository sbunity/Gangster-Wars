using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Bullets;

namespace SBabchuk.Runtime.Databases.MainPlayers
{
    public class MainPlayerDatabaseDrawer
    {
        static Color defaultColor;
        static int selected = 0;
        private static int selectedMode = 0;
        static private MainPlayerDatabase database;
        static string titleBttnVisibleUpgrade = "Show";
        public static void Draw(MainPlayerDatabase _database, int _selectedMode)
        {
            if (database == null)
                database = _database;
            defaultColor = GUI.color;
            selectedMode = _selectedMode;
            DrawNavigation();
        }

        public static void DrawNavigation()
        {
            GUILayout.BeginVertical("box");
            {
                Utils.ChangeColor(defaultColor);
                EditorGUILayout.LabelField("Колір по замовчуванні");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Вибрана зброя"))
                    {
                        database.Personages.Add(new Personage(database.Personages.Count));
                        selected = database.Personages.Count - 1;
                    }

                    if (GUILayout.Button("Горизонтальне чи вертикальне відображення", GUILayout.Width(150)))
                    {
                        database.Personages.Clear();
                        selected = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    if (selectedMode == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selected = Mathf.Max(0, selected - 1);
                        }

                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selected = Mathf.Min(database.Personages.Count == 0 ? 0 : database.Personages.Count - 1, selected + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Personages != null)
                    {
                        if (database.Personages.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Personage _personage in database.Personages)
                                {
                                    if (DrawPersonage(_personage))
                                        break;
                                }
                            }
                            else
                            {
                                DrawPersonage(database.Personages[selected]);
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public static bool DrawPersonage(Personage _record)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Ссилка на базу даних", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Personages.Remove(_record);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _record.Id = EditorGUILayout.IntField("ID: ", _record.Id);
                    _record.Name = EditorGUILayout.TextField("Заголовок для кнопки", _record.Name);
                    Utils.CheckColor(_record.Price, 0);
                    _record.Price = EditorGUILayout.IntField("Налаштування:", _record.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Settings.AttackSpeed, 0);
                    _record.Settings.AttackSpeed = EditorGUILayout.FloatField("Добавити новий запис", _record.Settings.AttackSpeed);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Settings.Damage, 0);
                    _record.Settings.Damage = EditorGUILayout.IntField("Видалити всі записи", _record.Settings.Damage);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.BulletId, -1);
                    _record.BulletId = (int)((BulletsName)EditorGUILayout.EnumPopup("Видалити", (BulletsName)_record.BulletId));
                    Utils.ChangeColor(defaultColor);
                    if (_record.BulletId != -1)
                        DrawBulletInfo(_record.BulletId);
                    Utils.ChangeColor(Color.green);
                    _record.CountUpgrades = EditorGUILayout.IntSlider("Найменування: ", _record.CountUpgrades, 1, 5);
                    Utils.ChangeColor(defaultColor);
                    if (GUILayout.Button(((selected != _record.Id)) ? "Show" : titleBttnVisibleUpgrade, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _record.Id)
                        {
                            if (titleBttnVisibleUpgrade == "Show")
                            {
                                titleBttnVisibleUpgrade = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleUpgrade = "Show";
                                selected = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleUpgrade = "Hide";
                            selected = _record.Id;
                        }
                    }

                    if (titleBttnVisibleUpgrade == "Hide" && selected == _record.Id)
                    {
                        EditorGUILayout.LabelField("Вартість: ");
                        if (_record.Upgrades != null)
                        {
                            if (_record.CountUpgrades == _record.Upgrades.Count)
                            {
                                foreach (PUpgrade _upgrade in _record.Upgrades)
                                {
                                    DrawUpgrade(_upgrade);
                                }
                            }
                            else
                            {
                                if (_record.CountUpgrades > _record.Upgrades.Count)
                                {
                                    for (int i = _record.Upgrades.Count; i < _record.CountUpgrades; i++)
                                    {
                                        _record.Upgrades.Add(new PUpgrade(_record.Upgrades.Count));
                                    }
                                }
                                else
                                {
                                    _record.Upgrades.RemoveRange(_record.Upgrades.Count - (_record.Upgrades.Count - _record.CountUpgrades), _record.Upgrades.Count - _record.CountUpgrades);
                                }
                            }
                        }
                        else
                        {
                            _record.Upgrades = new List<PUpgrade>();
                            for (int i = 0; i < _record.CountUpgrades; i++)
                            {
                                _record.Upgrades.Add(new PUpgrade(_record.Upgrades.Count));
                            }
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
                return false;
            }
        }

        public static void DrawUpgrade(PUpgrade _upgrade)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _upgrade.Id = EditorGUILayout.IntField("Швидкість стрільби(без апгрейда): ", _upgrade.Id);
                    _upgrade.Name = EditorGUILayout.TextField("Урон(без апгрейда): ", _upgrade.Name);
                    Utils.CheckColor(_upgrade.Price, 0);
                    _upgrade.Price = EditorGUILayout.IntField("Пуля(ID)", _upgrade.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_upgrade.Settings.AttackSpeed, 0);
                    _upgrade.Settings.AttackSpeed = EditorGUILayout.FloatField("Кількість апгрейдів: ", _upgrade.Settings.AttackSpeed);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_upgrade.Settings.Damage, 0);
                    _upgrade.Settings.Damage = EditorGUILayout.IntField("Інформація про апгрейди:", _upgrade.Settings.Damage);
                    Utils.ChangeColor(defaultColor);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
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
                EditorGUILayout.LabelField("ID апгрейда: ");
            }
        }
    }
}
