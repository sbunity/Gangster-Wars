using System.Collections.Generic;
using SBabchuk.Runtime.Databases.Enemies;
using UnityEditor;
using UnityEngine;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    public class SeenEnemyInfoDrawer
    {
        private static Color defaultColor;
        private static PlayerPrefsDatabase database;
        private static string titleSeenEnemies = "Show Seen Enemies";

        public static void Draw()
        {
            database = PlayerPrefsDatabaseDrawer.Database;
            defaultColor = PlayerPrefsDatabaseDrawer.DefaultColor;
            DrawTitle();
        }

        private static void DrawTitle()
        {
            EnsureSeenEnemyIds();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(titleSeenEnemies, GUILayout.Width(155), GUILayout.Height(20)))
                {
                    titleSeenEnemies = titleSeenEnemies == "Show Seen Enemies"
                        ? "Hide Seen Enemies"
                        : "Show Seen Enemies";
                }

                GUILayout.Label("Count: " + database.PlayerPrefs.SeenEnemyIds.Count, GUILayout.Width(80));

                if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    database.PlayerPrefs.SeenEnemyIds.Clear();
                }
            }

            GUILayout.EndHorizontal();

            if (titleSeenEnemies == "Hide Seen Enemies")
                DrawSeenEnemies();
        }

        private static void DrawSeenEnemies()
        {
            GUI.color = Color.grey;
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Seen enemies:");

                if (database.PlayerPrefs.SeenEnemyIds.Count == 0)
                {
                    EditorGUILayout.LabelField("No seen enemies saved.");
                    GUILayout.EndVertical();
                    return;
                }

                var enemyDatabase = EditorDatabaseLookup.Get<EnemyDatabase>();
                for (var i = database.PlayerPrefs.SeenEnemyIds.Count - 1; i >= 0; i--)
                {
                    var enemyId = database.PlayerPrefs.SeenEnemyIds[i];
                    var enemy = enemyDatabase != null ? enemyDatabase.GetEnemy(enemyId) : null;
                    DrawInfo(i, enemyId, enemy);
                }
            }

            GUILayout.EndVertical();
            GUI.color = defaultColor;
        }

        private static void DrawInfo(int index, int enemyId, Enemy enemy)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical(GUILayout.Width(80));
                {
                    var icon = enemy != null ? enemy.Icon : null;
                    EditorGUILayout.ObjectField(icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.IntField("Enemy ID: ", enemyId);
                    EditorGUILayout.LabelField("Name: ", enemy != null ? enemy.Name : "Missing enemy record");

                    if (enemy == null)
                    {
                        GUI.color = Color.yellow;
                        EditorGUILayout.LabelField("This id is saved, but EnemyDatabase has no matching enemy.");
                        GUI.color = defaultColor;
                    }
                }

                GUILayout.EndVertical();

                if (GUILayout.Button("Remove", GUILayout.Width(90), GUILayout.Height(24)))
                    database.PlayerPrefs.SeenEnemyIds.RemoveAt(index);
            }

            GUILayout.EndHorizontal();
        }

        private static void EnsureSeenEnemyIds()
        {
            if (database?.PlayerPrefs == null)
                return;

            if (database.PlayerPrefs.SeenEnemyIds == null)
                database.PlayerPrefs.SeenEnemyIds = new List<int>();
        }
    }
}
