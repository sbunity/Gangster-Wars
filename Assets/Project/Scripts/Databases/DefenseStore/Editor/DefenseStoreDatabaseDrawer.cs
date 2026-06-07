using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class DefenseStoreDatabaseDrawer
    {
        [Header("Колір по замовчуванні")]
        static Color defaultColor;

        [Header("Вибрана зброя")]
        static int selected = 0;

        [Header("Горизонтальне чи вертикальне відображення")]
        private static int selectedMode = 0;

        [Header("Ссилка на базу даних")]
        static private DefenseStoreDatabase database;

        [Header("Заголовок для кнопки")]
        static string titleBttnVisibleUpgrade = "Show";

        [Header("Заголовок для кнопки")]
        static string titleBttnVisibleIcons = "Show";

        /// <summary>
        /// Головний метод промальовки в редакторі
        /// </summary>
        /// <param name="_database"></param>
        /// <param name="selectedMode"></param>
        public static void Draw(DefenseStoreDatabase _database, int _selectedMode)
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
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Налаштування:");

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.defenses.Add(new Defense(database.defenses.Count));
                        selected = database.defenses.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(150)))
                    {
                        database.defenses.Clear();
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
                            selected = Mathf.Min(database.defenses.Count == 0 ? 0 : database.defenses.Count - 1, selected + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.defenses != null)
                    {
                        if (database.defenses.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Defense _defense in database.defenses)
                                {
                                    if (DrawDefense(_defense))
                                        break;
                                }
                            }
                            else
                            {
                                DrawDefense(database.defenses[selected]);
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
        public static bool DrawDefense(Defense _record)
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
                        database.defenses.Remove(_record);
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

                    Utils.CheckColor(_record.settings.health, 0);
                    _record.settings.health = EditorGUILayout.IntField("К-сть життів (без апгрейда): ", _record.settings.health);
                    Utils.ChangeColor(defaultColor);

                    GUI.color = Color.green;
                    _record.countUpgrades = EditorGUILayout.IntSlider("Кількість апгрейдів: ", _record.countUpgrades, 1, 5);
                    GUI.color = defaultColor;

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
                                foreach (DUpgrade _upgrade in _record.upgrades)
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
                                        _record.upgrades.Add(new DUpgrade(_record.upgrades.Count));
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
                            _record.upgrades = new List<DUpgrade>();

                            for (int i = 0; i < _record.countUpgrades; i++)
                            {
                                _record.upgrades.Add(new DUpgrade(_record.upgrades.Count));
                            }
                        }
                    }

                    GUI.color = Color.green;
                    _record.countIcons = EditorGUILayout.IntSlider("Кількість іконок: ", _record.countIcons, 1, 4);
                    GUI.color = defaultColor;

                    if (GUILayout.Button(((selected != _record.id)) ? "Show" : titleBttnVisibleIcons, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _record.id)
                        {
                            if (titleBttnVisibleIcons == "Show")
                            {
                                titleBttnVisibleIcons = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleIcons = "Show";
                                selected = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleIcons = "Hide";
                            selected = _record.id;
                        }
                    }

                    if (titleBttnVisibleIcons == "Hide" && selected == _record.id)
                    {
                        EditorGUILayout.LabelField("Інформація про апгрейди:");

                        if (_record.icons != null)
                        {
                            if (_record.countIcons == _record.icons.Count)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    foreach (Ico _ico in _record.icons)
                                    {
                                        DrawIco(_ico);
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                if (_record.countIcons > _record.icons.Count)
                                {
                                    for (int i = _record.icons.Count; i < _record.countIcons; i++)
                                    {
                                        _record.icons.Add(new Ico(_record.icons.Count));
                                    }
                                }
                                else
                                {
                                    _record.icons.RemoveRange(_record.icons.Count - (_record.icons.Count - _record.countIcons), _record.icons.Count - _record.countIcons);
                                }
                            }
                        }
                        else
                        {
                            _record.icons = new List<Ico>();

                            for (int i = 0; i < _record.countIcons; i++)
                            {
                                _record.icons.Add(new Ico(_record.icons.Count));
                            }
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            return false;
        }

        public static void DrawIco(Ico _record)
        {
            _record.ico = (Sprite)EditorGUILayout.ObjectField(_record.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
        }

        /// <summary>
        /// Промалбовуєм інформацію про апгрейди
        /// </summary>
        /// <param name="_upgrade"></param>
        public static void DrawUpgrade(DUpgrade _upgrade)
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

                    Utils.CheckColor(_upgrade.settings.health, 0);
                    _upgrade.settings.health = EditorGUILayout.IntField("К-сть життів (без апгрейда): ", _upgrade.settings.health);
                    Utils.ChangeColor(defaultColor);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Зміга кольра стилю промальовки
        /// </summary>
        /// <param name="_color">Новий колір</param>
        public static void ChangeColor(Color _color)
        {
            GUI.color = _color;
        }
    }
}
