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
        static string[] mode =
        {
            "|",
            "--"
        };
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
                        database.Levels.Add(new Level(database.Levels.Count));
                        selectedIndexLevel = database.Levels.Count - 1;
                    }

                    if (GUILayout.Button("Видалити всі записи", GUILayout.Width(175)))
                    {
                        database.Levels.Clear();
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
                            selectedIndexLevel = Mathf.Min(database.Levels.Count == 0 ? 0 : database.Levels.Count - 1, selectedIndexLevel + 1);
                        }
                    }
                }

                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Level: " + selectedIndexLevel);
                if (database)
                {
                    if (database.Levels != null)
                    {
                        if (database.Levels.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Level _level in database.Levels)
                                {
                                    if (Draw(_level))
                                        break;
                                }
                            }
                            else
                            {
                                selectedIndexLevel = selectedIndexLevel != -1 ? selectedIndexLevel : 0;
                                Draw(database.Levels[selectedIndexLevel]);
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
                        database.Levels.Remove(_level);
                        selectedIndexLevel = Mathf.Max(0, selectedIndexLevel - 1);
                        return true;
                    }

                    if (GUILayout.Button(((selectedIndexLevel != _level.Id)) ? "Show" : titleBttnVisibleLevel, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selectedIndexLevel == _level.Id)
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
                            selectedIndexLevel = _level.Id;
                        }
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    _level.Icon = (Sprite)EditorGUILayout.ObjectField(_level.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    GUILayout.BeginVertical();
                    {
                        _level.Id = EditorGUILayout.IntField("ID рівень: ", _level.Id);
                        _level.Name = EditorGUILayout.TextField("Найменування рівня: ", _level.Name);
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
                if (titleBttnVisibleLevel == "Hide" && selectedIndexLevel == _level.Id)
                {
                    DrawWaves(_level);
                }

                GUILayout.EndVertical();
            }

            return false;
        }

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
                        level.Waves.Add(new Waves(level.Waves.Count));
                        selectedIndexWave = level.Waves.Count - 1;
                    }

                    if (verticalModeWave == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selectedIndexWave = Mathf.Max(0, selectedIndexWave - 1);
                        }

                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selectedIndexWave = Mathf.Min(level.Waves.Count == 0 ? 0 : level.Waves.Count - 1, selectedIndexWave + 1);
                        }
                    }

                    if (GUILayout.Button("Видалити всі", GUILayout.Width(75)))
                    {
                        level.Waves.Clear();
                        selectedIndexWave = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (level.Waves.Count > 0)
                {
                    if (verticalModeWave == 0)
                    {
                        foreach (Waves _wave in level.Waves)
                        {
                            if (DrawWave(level.Waves, _wave))
                                break;
                        }
                    }
                    else
                    {
                        selectedIndexWave = selectedIndexWave != -1 ? selectedIndexWave : 0;
                        DrawWave(level.Waves, level.Waves[selectedIndexWave]);
                    }
                }
            }

            GUILayout.EndVertical();
        }

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

                    if (GUILayout.Button(((selectedIndexWave != wave.Id)) ? "Show" : titleBttnVisibleWave, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selectedIndexWave == wave.Id)
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
                            selectedIndexWave = wave.Id;
                        }
                    }
                }

                GUILayout.EndHorizontal();
                wave.Id = EditorGUILayout.IntField("ID хвилі: ", wave.Id);
                wave.StartDelay = EditorGUILayout.FloatField("Затримка при старті хвилі: ", wave.StartDelay);
                wave.Delay = EditorGUILayout.FloatField("Час на проходження: ", wave.Delay);
                if (titleBttnVisibleWave == "Hide" && selectedIndexWave == wave.Id)
                {
                    DrawEnemyOfWave(wave);
                }
            }

            GUILayout.EndVertical();
            return false;
        }

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
                        wave.Enemies.Add(new EnemyOfWave());
                        selectedIndexEnemyOfWave = wave.Enemies.Count - 1;
                    }

                    if (verticalModeEnemy == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selectedIndexEnemyOfWave = Mathf.Max(0, selectedIndexEnemyOfWave - 1);
                        }

                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selectedIndexEnemyOfWave = Mathf.Min(wave.Enemies.Count == 0 ? 0 : wave.Enemies.Count - 1, selectedIndexEnemyOfWave + 1);
                        }
                    }

                    if (GUILayout.Button("Delete All", GUILayout.Width(75)))
                    {
                        wave.Enemies.Clear();
                        selectedIndexEnemyOfWave = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (wave.Enemies.Count > 0)
                {
                    if (verticalModeEnemy == 0)
                    {
                        foreach (EnemyOfWave _enemyOfWave in wave.Enemies)
                        {
                            if (DrawEnemy(wave.Enemies, _enemyOfWave))
                                break;
                        }
                    }
                    else
                    {
                        selectedIndexEnemyOfWave = selectedIndexEnemyOfWave != -1 ? selectedIndexEnemyOfWave : 0;
                        DrawEnemy(wave.Enemies, wave.Enemies[selectedIndexEnemyOfWave]);
                    }
                }
            }

            GUILayout.EndVertical();
        }

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
                enemyOfWave.EnemyId = (int)((EnemiesName)EditorGUILayout.EnumPopup("Тип юніта(ID)", (EnemiesName)enemyOfWave.EnemyId));
                DrawEnemyInfo(enemyOfWave.EnemyId);
                enemyOfWave.CountEnemy = EditorGUILayout.IntSlider("Кількість ворогів (спавняться одночасно): ", enemyOfWave.CountEnemy, 0, 10);
                enemyOfWave.Interval = EditorGUILayout.Slider("Інтервал (через скільки часу ворог спавниться): ", enemyOfWave.Interval, 0, 30);
                enemyOfWave.DropChance = EditorGUILayout.IntSlider("Йморівність випадання монетки (%): ", enemyOfWave.DropChance, 0, 100);
            }

            GUILayout.EndVertical();
            return false;
        }

        public static void DrawEnemyInfo(int index)
        {
            if (EditorDatabaseLookup.Get<EnemyDatabase>().Enemies.Count > 0)
            {
                foreach (Enemy enemy in EditorDatabaseLookup.Get<EnemyDatabase>().Enemies)
                {
                    if (enemy.Id == index)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                enemy.Icon = (Sprite)EditorGUILayout.ObjectField(enemy.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                                GUILayout.BeginVertical();
                                {
                                    enemy.Id = EditorGUILayout.IntField("ID юніта: ", enemy.Id);
                                    enemy.Name = EditorGUILayout.TextField("Найменування юніта: ", enemy.Name);
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
