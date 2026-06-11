using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class WeaponStoreDatabaseDrawer
    {
        static Color defaultColor;
        static int selected = 0;
        private static int selectedMode = 0;
        static private WeaponStoreDatabase database;
        static string titleBttnVisibleUpgrade = "Show";
        public static void Draw(WeaponStoreDatabase _database, int _selectedMode)
        {
            if (database == null)
                database = _database;
            selectedMode = _selectedMode;
            defaultColor = GUI.color;
            DrawNavigation();
        }

        public static void DrawNavigation()
        {
            Utils.ChangeColor(Color.grey);
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Колір по замовчуванні");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Вибрана зброя"))
                    {
                        database.Weapons.Add(new Weapon(database.Weapons.Count));
                        selected = database.Weapons.Count - 1;
                    }

                    if (GUILayout.Button("Горизонтальне чи вертикальне відображення", GUILayout.Width(150)))
                    {
                        database.Weapons.Clear();
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
                            selected = Mathf.Min(database.Weapons.Count == 0 ? 0 : database.Weapons.Count - 1, selected + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Weapons != null)
                    {
                        if (database.Weapons.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Weapon _weapon in database.Weapons)
                                {
                                    if (DrawWeapon(_weapon))
                                        break;
                                }
                            }
                            else
                            {
                                DrawWeapon(database.Weapons[selected]);
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public static bool DrawWeapon(Weapon _weapon)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _weapon.Icon = (Sprite)EditorGUILayout.ObjectField(_weapon.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Ссилка на базу даних", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Weapons.Remove(_weapon);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _weapon.Id = EditorGUILayout.IntField("Заголовок для кнопки", _weapon.Id);
                    _weapon.Name = EditorGUILayout.TextField("Налаштування:", _weapon.Name);
                    Utils.CheckColor(_weapon.Price, 0);
                    _weapon.Price = EditorGUILayout.IntField("Добавити новий запис", _weapon.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_weapon.Magazine, 0);
                    _weapon.Magazine = EditorGUILayout.IntSlider("Видалити всі записи", _weapon.Magazine, 0, 50);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_weapon.PriceMagazine, 0);
                    _weapon.PriceMagazine = EditorGUILayout.IntSlider("Видалити", _weapon.PriceMagazine, 0, 300);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_weapon.SpeedReload, 0);
                    _weapon.SpeedReload = EditorGUILayout.Slider("ID зброї: ", _weapon.SpeedReload, 0, 3);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_weapon.Settings.Damage, 0);
                    _weapon.Settings.Damage = EditorGUILayout.IntField("Найменування зброї: ", _weapon.Settings.Damage);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_weapon.BulletId, -1);
                    _weapon.BulletId = (int)((BulletsName)EditorGUILayout.EnumPopup("Вартість зброї: ", (BulletsName)_weapon.BulletId));
                    Utils.ChangeColor(defaultColor);
                    if (_weapon.BulletId != -1)
                        DrawBulletInfo(_weapon.BulletId);
                    Utils.ChangeColor(Color.green);
                    _weapon.CountUpgrades = EditorGUILayout.IntSlider("Розмірність магазина: ", _weapon.CountUpgrades, 1, 5);
                    Utils.ChangeColor(defaultColor);
                    if (GUILayout.Button(((selected != _weapon.Id)) ? "Show" : titleBttnVisibleUpgrade, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _weapon.Id)
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
                            selected = _weapon.Id;
                        }
                    }

                    if (titleBttnVisibleUpgrade == "Hide" && selected == _weapon.Id)
                    {
                        EditorGUILayout.LabelField("Вартість магазина патронів: ");
                        if (_weapon.Upgrades != null)
                        {
                            if (_weapon.CountUpgrades == _weapon.Upgrades.Count)
                            {
                                foreach (WUpgrade _upgrade in _weapon.Upgrades)
                                {
                                    DrawUpgrade(_upgrade);
                                }
                            }
                            else
                            {
                                if (_weapon.CountUpgrades > _weapon.Upgrades.Count)
                                {
                                    for (int i = _weapon.Upgrades.Count; i < _weapon.CountUpgrades; i++)
                                    {
                                        _weapon.Upgrades.Add(new WUpgrade(_weapon.Upgrades.Count));
                                    }
                                }
                                else
                                {
                                    _weapon.Upgrades.RemoveRange(_weapon.Upgrades.Count - (_weapon.Upgrades.Count - _weapon.CountUpgrades), _weapon.Upgrades.Count - _weapon.CountUpgrades);
                                }
                            }
                        }
                        else
                        {
                            _weapon.Upgrades = new List<WUpgrade>();
                            for (int i = 0; i < _weapon.CountUpgrades; i++)
                            {
                                _weapon.Upgrades.Add(new WUpgrade(_weapon.Upgrades.Count));
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }

        public static void DrawUpgrade(WUpgrade _upgrade)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _upgrade.Id = EditorGUILayout.IntField("Швидкість перезарядки 1 патрона: ", _upgrade.Id);
                    _upgrade.Name = EditorGUILayout.TextField("Урон без апгрейда: ", _upgrade.Name);
                    Utils.CheckColor(_upgrade.Price, 0);
                    _upgrade.Price = EditorGUILayout.IntField("Пуля(ID)", _upgrade.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_upgrade.Settings.Damage, 0);
                    _upgrade.Settings.Damage = EditorGUILayout.IntField("Кількість апгрейдів: ", _upgrade.Settings.Damage);
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
                EditorGUILayout.LabelField("Інформація про апгрейди:");
            }
        }
    }
}
