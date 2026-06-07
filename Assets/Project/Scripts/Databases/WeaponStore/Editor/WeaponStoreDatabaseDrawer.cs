using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class WeaponStoreDatabaseDrawer
    {
        [Header("Колір по замовчуванні")]
        static Color defaultColor;

        [Header("Вибрана зброя")]
        static int selected = 0;

        [Header("Горизонтальне чи вертикальне відображення")]
        private static int selectedMode = 0;

        [Header("Ссилка на базу даних")]
        static private WeaponStoreDatabase database;

        [Header("Заголовок для кнопки")]
        static string titleBttnVisibleUpgrade = "Show";

        /// <summary>
        /// Головний метод промальовки в редакторі
        /// </summary>
        /// <param name="_database"></param>
        /// <param name="selectedMode"></param>
        public static void Draw(WeaponStoreDatabase _database, int _selectedMode)
        {
            if (database == null) //Перевірка параметра на null
                database = _database; //Запам*ятовуєм базу даних

            selectedMode = _selectedMode; //Запам*ятовуєм мод

            defaultColor = GUI.color; //Зберігаєм колір по дефолту, для подальшого використання

            DrawNavigation(); //Промалбовуєм навігацію
        }

        /// <summary>
        /// Промалювання навігації(меню)
        /// </summary>
        public static void DrawNavigation()
        {
            Utils.ChangeColor(Color.grey);
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Налаштування:");

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.weapons.Add(new Weapon(database.weapons.Count));
                        selected = database.weapons.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(150)))
                    {
                        database.weapons.Clear();
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
                            selected = Mathf.Min(database.weapons.Count == 0 ? 0 : database.weapons.Count - 1, selected + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.weapons != null)
                    {
                        if (database.weapons.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Weapon _weapon in database.weapons)
                                {
                                    if (DrawWeapon(_weapon))
                                        break;
                                }
                            }
                            else
                            {
                                DrawWeapon(database.weapons[selected]);
                            }
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Промалбовуєм інформації про Зброю
        /// </summary>
        /// <param name="_weapon"></param>
        /// <returns></returns>
        public static bool DrawWeapon(Weapon _weapon)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    //Малюєм іконку
                    _weapon.ico = (Sprite)EditorGUILayout.ObjectField(_weapon.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));

                    //Кнопка видалення поточного поля
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.weapons.Remove(_weapon);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    _weapon.id = EditorGUILayout.IntField("ID зброї: ", _weapon.id);
                    _weapon.name = EditorGUILayout.TextField("Найменування зброї: ", _weapon.name);

                    Utils.CheckColor(_weapon.price, 0);
                    _weapon.price = EditorGUILayout.IntField("Вартість зброї: ", _weapon.price);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_weapon.magazine, 0);
                    _weapon.magazine = EditorGUILayout.IntSlider("Розмірність магазина: ", _weapon.magazine, 0, 50);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_weapon.priceMagazine, 0);
                    _weapon.priceMagazine = EditorGUILayout.IntSlider("Вартість магазина патронів: ", _weapon.priceMagazine, 0, 300);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_weapon.speedReload, 0);
                    _weapon.speedReload = EditorGUILayout.Slider("Швидкість перезарядки 1 патрона: ", _weapon.speedReload, 0, 3);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_weapon.settings.damage, 0);
                    _weapon.settings.damage = EditorGUILayout.IntField("Урон без апгрейда: ", _weapon.settings.damage);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_weapon.bulletID, -1);
                    _weapon.bulletID = (int)((BulletsName)EditorGUILayout.EnumPopup("Пуля(ID)", (BulletsName)_weapon.bulletID));
                    Utils.ChangeColor(defaultColor);

                    if (_weapon.bulletID != -1)
                        DrawBulletInfo(_weapon.bulletID);

                    Utils.ChangeColor(Color.green);
                    _weapon.countUpgrades = EditorGUILayout.IntSlider("Кількість апгрейдів: ", _weapon.countUpgrades, 1, 5);
                    Utils.ChangeColor(defaultColor);

                    if (GUILayout.Button(((selected != _weapon.id)) ? "Show" : titleBttnVisibleUpgrade, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _weapon.id)
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
                            selected = _weapon.id;
                        }
                    }
                   
                    if (titleBttnVisibleUpgrade == "Hide" && selected == _weapon.id)
                    {
                       
                        EditorGUILayout.LabelField("Інформація про апгрейди:");
                        if (_weapon.upgrades != null)
                        {
                            if (_weapon.countUpgrades == _weapon.upgrades.Count)
                            {
                                foreach (WUpgrade _upgrade in _weapon.upgrades)
                                {
                                    DrawUpgrade(_upgrade);
                                }
                            }
                            else
                            {
                                if (_weapon.countUpgrades > _weapon.upgrades.Count)
                                {
                                    for (int i = _weapon.upgrades.Count; i < _weapon.countUpgrades; i++)
                                    {
                                        _weapon.upgrades.Add(new WUpgrade(_weapon.upgrades.Count));
                                    }
                                }
                                else
                                {
                                    _weapon.upgrades.RemoveRange(_weapon.upgrades.Count - (_weapon.upgrades.Count - _weapon.countUpgrades), _weapon.upgrades.Count - _weapon.countUpgrades);
                                }
                            }
                        }
                        else
                        {
                            _weapon.upgrades = new List<WUpgrade>();

                            for (int i = 0; i < _weapon.countUpgrades; i++)
                            {
                                _weapon.upgrades.Add(new WUpgrade(_weapon.upgrades.Count));
                            }
                        }
                    }

                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            return false;
        }

        /// <summary>
        /// Промалбовуєм інформацію про апгрейди
        /// </summary>
        /// <param name="_upgrade"></param>
        public static void DrawUpgrade(WUpgrade _upgrade)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _upgrade.id = EditorGUILayout.IntField("ID апгрейда: ", _upgrade.id);
                    _upgrade.name = EditorGUILayout.TextField("Найменування апгрейда: ", _upgrade.name);

                    Utils.CheckColor(_upgrade.price, 0);
                    _upgrade.price = EditorGUILayout.IntField("Вартість апгрейда: ", _upgrade.price);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_upgrade.settings.damage, 0);
                    _upgrade.settings.damage = EditorGUILayout.IntField("Урон: ", _upgrade.settings.damage);
                    Utils.ChangeColor(defaultColor);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
		/// Вимальовує інформацію про пулю в інформації про ворога
		/// </summary>
		/// <param name="index">Index.</param>
		public static void DrawBulletInfo(int index)
        {
            if (BulletDatabase.GetDatabase().bullets.Count > 0)
            {
                foreach (Bullet bullet in BulletDatabase.GetDatabase().bullets)
                {
                    if (bullet.id == index)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                bullet.ico = (Sprite)EditorGUILayout.ObjectField(bullet.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
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
