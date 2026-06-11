using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class LevelDatabaseDrawer : MonoBehaviour
    {
        static Waves wave;
        static EnemyOfWave enemyOfWave;
        static private LevelDatabase database;
        static Color defaultColor;
        static int selectedIndexLevel = 0;
        static int selectedIndexWave = 0;
        static int selectedIndexEnemyOfWave = 0;
        static string titleBttnVisibleLevel = "Show";
        static string titleBttnVisibleWave = "Show";

        static int verticalModeWave = 0;
        static int verticalModeEnemy = 0;
        static string[] mode = { "|", "--" };

        public static void Draw(LevelDatabase _database, int selectedMode)
        {
            if (database == null)
                database = _database;

            defaultColor = GUI.color;

            Utils.ChangeColor(Color.grey);

            GUILayout.BeginVertical("box");
            {
                Utils.ChangeColor(defaultColor);
                EditorGUILayout.LabelField("Налаштування:");

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити новий запис"))
                    {
                        database.levels.Add(new Level(database.levels.Count));
                        selectedIndexLevel = database.levels.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(175)))
                    {
                        database.levels.Clear();
                        selectedIndexLevel = 0;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    if (selectedMode == 1)
                    {
                        if (GUILayout.Button("<--"))
                        {
                            selectedIndexLevel = Mathf.Max(0, selectedIndexLevel - 1);
                        }
                        if (GUILayout.Button("-->"))
                        {
                            selectedIndexLevel = Mathf.Min(database.levels.Count == 0 ? 0 : database.levels.Count - 1, selectedIndexLevel + 1);
                        }
                    }
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Level: " + selectedIndexLevel);

                if (database)
                {
                    if (database.levels != null)
                    {
                        if (database.levels.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Level _level in database.levels)
                                {
                                    if (Draw(_level))
                                        break;
                                }
                            }
                            else
                            {
                                selectedIndexLevel = selectedIndexLevel != -1 ? selectedIndexLevel : 0;
                                Draw(database.levels[selectedIndexLevel]);
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Немає записів");
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }

        public static bool Draw(Level _level)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        database.levels.Remove(_level);
                        selectedIndexLevel = Mathf.Max(0, selectedIndexLevel - 1);
                        return true;
                    }
                    if (GUILayout.Button(((selectedIndexLevel != _level.id)) ? "Show" : titleBttnVisibleLevel, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selectedIndexLevel == _level.id)
                        {
                            if (titleBttnVisibleLevel == "Show")
                            {
                                titleBttnVisibleLevel = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleLevel = "Show";
                                selectedIndexLevel = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleLevel = "Hide";
                            selectedIndexLevel = _level.id;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    _level.ico = (Sprite)EditorGUILayout.ObjectField(_level.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    GUILayout.BeginVertical();
                    {
                        _level.id = EditorGUILayout.IntField("ID рівень: ", _level.id);
                        _level.name = EditorGUILayout.TextField("Найменування рівня: ", _level.name);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                if (titleBttnVisibleLevel == "Hide" && selectedIndexLevel == _level.id)
                {
                    #region Waves
                    DrawWaves(_level);
                    #endregion
                }
                //				}
                GUILayout.EndVertical();
            }
            return false;
        }

        /// <summary>
        /// Малюєм хвилі
        /// </summary>
        public static void DrawWaves(Level level)
        {
            GUI.color = Color.cyan;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Хвилі:");

                GUILayout.BeginHorizontal();
                {
                    verticalModeWave = GUILayout.Toolbar(verticalModeWave, mode);
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити нову хвилю"))
                    {
                        level.waves.Add(new Waves(level.waves.Count));
                        selectedIndexWave = level.waves.Count - 1;
                    }
                    if (verticalModeWave == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selectedIndexWave = Mathf.Max(0, selectedIndexWave - 1);
                        }
                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selectedIndexWave = Mathf.Min(level.waves.Count == 0 ? 0 : level.waves.Count - 1, selectedIndexWave + 1);
                        }
                    }
                    if (GUILayout.Button("Видалити всі", GUILayout.Width(75)))
                    {
                        level.waves.Clear();
                        selectedIndexWave = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (level.waves.Count > 0)
                {
                    if (verticalModeWave == 0)
                    {
                        foreach (Waves _wave in level.waves)
                        {
                            if (DrawWave(level.waves, _wave))
                                break;
                        }
                    }
                    else
                    {
                        selectedIndexWave = selectedIndexWave != -1 ? selectedIndexWave : 0;
                        DrawWave(level.waves, level.waves[selectedIndexWave]);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Малюєм хвилю
        /// </summary>
        public static bool DrawWave(List<Waves> waves, Waves wave)
        {
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        waves.Remove(wave);
                        selectedIndexWave = Mathf.Max(0, selectedIndexWave - 1);
                        return true;
                    }
                    if (GUILayout.Button(((selectedIndexWave != wave.id)) ? "Show" : titleBttnVisibleWave, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selectedIndexWave == wave.id)
                        {
                            if (titleBttnVisibleWave == "Show")
                            {
                                titleBttnVisibleWave = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleWave = "Show";
                                selectedIndexWave = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleWave = "Hide";
                            selectedIndexWave = wave.id;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                wave.id = EditorGUILayout.IntField("ID хвилі: ", wave.id);
                wave.startDelay = EditorGUILayout.FloatField("Затримка при старті хвилі: ", wave.startDelay);
                wave.delay = EditorGUILayout.FloatField("Час на проходження: ", wave.delay);

                if (titleBttnVisibleWave == "Hide" && selectedIndexWave == wave.id)
                {
                    DrawEnemyOfWave(wave);
                }
            }
            GUILayout.EndVertical();
            return false;
        }

        /// <summary>
        /// Малюєм таблицю, юнітів для певної хвилі
        /// </summary>
        public static void DrawEnemyOfWave(Waves wave)
        {
            GUI.color = Color.gray;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Хвилі:");

                GUILayout.BeginHorizontal();
                {
                    verticalModeEnemy = GUILayout.Toolbar(verticalModeEnemy, mode);
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Добавити нового юніта"))
                    {
                        wave.enemies.Add(new EnemyOfWave());
                        selectedIndexEnemyOfWave = wave.enemies.Count - 1;
                    }
                    if (verticalModeEnemy == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selectedIndexEnemyOfWave = Mathf.Max(0, selectedIndexEnemyOfWave - 1);
                        }
                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selectedIndexEnemyOfWave = Mathf.Min(wave.enemies.Count == 0 ? 0 : wave.enemies.Count - 1, selectedIndexEnemyOfWave + 1);
                        }
                    }
                    if (GUILayout.Button("Delete All", GUILayout.Width(75)))
                    {
                        wave.enemies.Clear();
                        selectedIndexEnemyOfWave = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (wave.enemies.Count > 0)
                {
                    if (verticalModeEnemy == 0)
                    {
                        foreach (EnemyOfWave _enemyOfWave in wave.enemies)
                        {
                            if (DrawEnemy(wave.enemies, _enemyOfWave))
                                break;
                        }
                    }
                    else
                    {
                        selectedIndexEnemyOfWave = selectedIndexEnemyOfWave != -1 ? selectedIndexEnemyOfWave : 0;
                        DrawEnemy(wave.enemies, wave.enemies[selectedIndexEnemyOfWave]);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Малюєм інформаціє про ворога в плані хвилі, кількість інтервал і так далі
        /// </summary>
        public static bool DrawEnemy(List<EnemyOfWave> enemies, EnemyOfWave enemyOfWave)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        enemies.Remove(enemyOfWave);
                        selectedIndexEnemyOfWave = Mathf.Max(0, selectedIndexEnemyOfWave - 1);
                        return true;
                    }
                }
                GUILayout.EndHorizontal();

                enemyOfWave.enemyID = (int)((EnemiesName)EditorGUILayout.EnumPopup("Тип юніта(ID)", (EnemiesName)enemyOfWave.enemyID));

                DrawEnemyInfo(enemyOfWave.enemyID);

                enemyOfWave.countEnemy = EditorGUILayout.IntSlider("Кількість ворогів (спавняться одночасно): ", enemyOfWave.countEnemy, 0, 10);
                enemyOfWave.interval = EditorGUILayout.Slider("Інтервал (через скільки часу ворог спавниться): ", enemyOfWave.interval, 0, 30);
                enemyOfWave.changeCraft = EditorGUILayout.IntSlider("Йморівність випадання монетки (%): ", enemyOfWave.changeCraft, 0, 100);
            }
            GUILayout.EndVertical();
            return false;
        }

        /// <summary>
        /// Малюєм інформацію, про самого ворога, для наглядності
        /// </summary>
        public static void DrawEnemyInfo(int index)
        {
            if (EditorDatabaseLookup.Get<EnemyDatabase>().enemies.Count > 0)
            {
                foreach (Enemy enemy in EditorDatabaseLookup.Get<EnemyDatabase>().enemies)
                {
                    if (enemy.id == index)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                enemy.ico = (Sprite)EditorGUILayout.ObjectField(enemy.ico, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                                GUILayout.BeginVertical();
                                {
                                    enemy.id = EditorGUILayout.IntField("ID юніта: ", enemy.id);
                                    enemy.name = EditorGUILayout.TextField("Найменування юніта: ", enemy.name);

                                }
                                GUILayout.EndVertical();
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
