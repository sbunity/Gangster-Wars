using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class MainPlayerDatabaseDrawer
    {
        [Header("Колір по замовчуванні")]
        static Color defaultColor;

        [Header("Вибрана зброя")]
        static int selected = 0;

        [Header("Горизонтальне чи вертикальне відображення")]
        private static int selectedMode = 0;

        [Header("Ссилка на базу даних")]
        static private MainPlayerDatabase database;

        [Header("Заголовок для кнопки")]
        static string titleBttnVisibleUpgrade = "Show";

        /// <summary>
        /// Головний метод промальовки в редакторі
        /// </summary>
        /// <param name="_database"></param>
        /// <param name="selectedMode"></param>
        public static void Draw(MainPlayerDatabase _database, int _selectedMode)
        {
            if (database == null) //Перевірка параметра на null
                database = _database; //Запам*ятовуєм базу даних

            defaultColor = GUI.color; //Зберігаєм колір по дефолту, для подальшого використання

            selectedMode = _selectedMode; //Запам*ятовуєм мод

            DrawNavigation(); //Промалбовуєм навігацію
        }

        /// <summary>
        /// Промалювання навігації(меню)
        /// </summary>
        public static void DrawNavigation()
        {
            GUILayout.BeginVertical("box");
            {
                Utils.ChangeColor(defaultColor);
                EditorGUILayout.LabelField("Налаштування:");

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.personages.Add(new Personage(database.personages.Count));
                        selected = database.personages.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(150)))
                    {
                        database.personages.Clear();
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
                            selected = Mathf.Min(database.personages.Count == 0 ? 0 : database.personages.Count - 1, selected + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.personages != null)
                    {
                        if (database.personages.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Personage _personage in database.personages)
                                {
                                    if (DrawPersonage(_personage))
                                        break;
                                }
                            }
                            else
                            {
                                DrawPersonage(database.personages[selected]);
                            }
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Промалбовуєм інформації про персонажа
        /// </summary>
        /// <param name="_weapon"></param>
        /// <returns></returns>
        public static bool DrawPersonage(Personage _record)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    //Малюєм іконку
                    _record.ico = (Sprite)EditorGUILayout.ObjectField(_record.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));

                    //Кнопка видалення поточного поля
                    if (GUILayout.Button("Видалити", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.personages.Remove(_record);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    _record.id = EditorGUILayout.IntField("ID: ", _record.id);

                    _record.name = EditorGUILayout.TextField("Найменування: ", _record.name);

                    Utils.CheckColor(_record.price, 0);
                    _record.price = EditorGUILayout.IntField("Вартість: ", _record.price);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.settings.speedAtack, 0);
                    _record.settings.speedAtack = EditorGUILayout.FloatField("Швидкість стрільби(без апгрейда): ", _record.settings.speedAtack);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.settings.damage, 0);
                    _record.settings.damage = EditorGUILayout.IntField("Урон(без апгрейда): ", _record.settings.damage);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.bulletID, -1);
                    _record.bulletID = (int)((BulletsName)EditorGUILayout.EnumPopup("Пуля(ID)", (BulletsName)_record.bulletID));
                    Utils.ChangeColor(defaultColor);

                    if (_record.bulletID != -1)
                        DrawBulletInfo(_record.bulletID);

                    Utils.ChangeColor(Color.green);
                    _record.countUpgrades = EditorGUILayout.IntSlider("Кількість апгрейдів: ", _record.countUpgrades, 1, 5);
                    Utils.ChangeColor(defaultColor);

                    if (GUILayout.Button(((selected != _record.id)) ? "Show" : titleBttnVisibleUpgrade, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _record.id)
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
                            selected = _record.id;
                        }
                    }

                    if (titleBttnVisibleUpgrade == "Hide" && selected == _record.id)
                    {
                        EditorGUILayout.LabelField("Інформація про апгрейди:");

                        if (_record.upgrades != null)
                        {
                            if (_record.countUpgrades == _record.upgrades.Count)
                            {
                                foreach (PUpgrade _upgrade in _record.upgrades)
                                {
                                    DrawUpgrade(_upgrade);
                                }
                            }
                            else
                            {
                                if (_record.countUpgrades > _record.upgrades.Count)
                                {
                                    for (int i = _record.upgrades.Count; i < _record.countUpgrades; i++)
                                    {
                                        _record.upgrades.Add(new PUpgrade(_record.upgrades.Count));
                                    }
                                }
                                else
                                {
                                    _record.upgrades.RemoveRange(_record.upgrades.Count - (_record.upgrades.Count - _record.countUpgrades), _record.upgrades.Count - _record.countUpgrades);
                                }
                            }
                        }
                        else
                        {
                            _record.upgrades = new List<PUpgrade>();

                            for (int i = 0; i < _record.countUpgrades; i++)
                            {
                                _record.upgrades.Add(new PUpgrade(_record.upgrades.Count));
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                return false;
            }
        }

        /// <summary>
        /// Промалбовуєм інформацію про апгрейди
        /// </summary>
        /// <param name="_upgrade"></param>
        public static void DrawUpgrade(PUpgrade _upgrade)
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

                    Utils.CheckColor(_upgrade.settings.speedAtack, 0);
                    _upgrade.settings.speedAtack = EditorGUILayout.FloatField("Швидкість стрільби: ", _upgrade.settings.speedAtack);
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
