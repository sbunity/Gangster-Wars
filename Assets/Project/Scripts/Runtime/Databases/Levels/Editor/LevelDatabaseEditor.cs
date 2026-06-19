using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.Levels
{
    [CustomEditor(typeof(LevelDatabase))]
    public class LevelDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            serializedObject.Update();

            var chapters = serializedObject.FindProperty("_chapters");
            EditorGUILayout.PropertyField(chapters, true);

            serializedObject.ApplyModifiedProperties();

            var levelDatabase = (LevelDatabase)Database;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Total Chapters", levelDatabase.Chapters?.Count.ToString() ?? "0");
            EditorGUILayout.LabelField("Total Levels", levelDatabase.Levels.Count.ToString());
        }
    }
}
