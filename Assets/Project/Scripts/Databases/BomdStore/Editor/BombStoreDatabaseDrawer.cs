using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class BombStoreDatabaseDrawer
    {
        [Header("Колір по замовчуванні")]
        static Color defaultColor;

        [Header("Вибрана зброя")]
        static int selected = 0;

        [Header("Горизонтальне чи вертикальне відображення")]
        private static int selectedMode = 0;

        [Header("Ссилка на базу даних")]
        static private BombStoreDatabase database;

        [Header("Заголовок для кнопки")]
        static string titleBttnVisibleUpgrade = "Show";

        /// <summary>
        /// Головний метод промальовки в редакторі
        /// </summary>
        /// <param name="_database"></param>
        /// <param name="selectedMode"></param>
        public static void Draw(BombStoreDatabase _database, int _selectedMode)
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
                        database.grenades.Add(new Grenade(database.grenades.Count));
                        selected = database.grenades.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(150)))
                    {
                        database.grenades.Clear();
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
                            selected = Mathf.Min(database.grenades.Count == 0 ? 0 : database.grenades.Count - 1, selected + 1);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (database)
                {
                    if (database.grenades != null)
                    {
                        if (database.grenades.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Grenade _grenade in database.grenades)
                                {
                                    if (DrawGrenade(_grenade))
                                        break;
                                }
                            }
                            else
                            {
                                DrawGrenade(database.grenades[selected]);
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
        public static bool DrawGrenade(Grenade _record)
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
                        database.grenades.Remove(_record);
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

                    Utils.CheckColor(_record.damage, 0);
                    _record.damage = EditorGUILayout.IntSlider("Урон: ", _record.damage, 0, 100);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.delay, 0);
                    _record.delay = EditorGUILayout.Slider("Затримка до зриву: ", _record.delay, 0, 10);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.time, 0);
                    _record.time = EditorGUILayout.Slider("Час дії(для молотова): ", _record.time, 0, 10);
                    Utils.ChangeColor(defaultColor);

                    Utils.CheckColor(_record.radius, 0);
                    _record.radius = EditorGUILayout.Slider("Радіус дії: ", _record.radius, 0, 10);
                    Utils.ChangeColor(defaultColor);

                    _record.collision = ((CollisionsName)EditorGUILayout.EnumPopup("Партікл зриву", (CollisionsName)_record.collision));
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            return false;
        }
    }
}
