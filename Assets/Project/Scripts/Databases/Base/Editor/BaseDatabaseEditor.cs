using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class BaseDatabaseEditor : Editor
    {
        [SerializeField]
        [FormerlySerializedAs("database")]
        private ScriptableObject _database;
        public ScriptableObject Database { get => _database; set => _database = value; }

        [SerializeField]
        [FormerlySerializedAs("selectedMode")]
        private int _selectedMode = 0;
        public int SelectedMode { get => _selectedMode; set => _selectedMode = value; }

        [SerializeField]
        [FormerlySerializedAs("mode")]
        private string[] _mode =
        {
            "|",
            "--"
        };
        public string[] Mode { get => _mode; set => _mode = value; }

        static Color defaultColor;
        private void OnEnable()
        {
            _database = (ScriptableObject)target;
        }

        public override void OnInspectorGUI()
        {
            defaultColor = GUI.color;
            SetMode();
            if (_database == null)
                _database = (ScriptableObject)target;
            DrawButtonSave();
            Draw();
            Update();
        }

        public virtual void Draw()
        {
        }

        public void SetMode()
        {
            GUILayout.BeginHorizontal();
            {
                _selectedMode = GUILayout.Toolbar(_selectedMode, _mode);
            }

            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            serializedObject.Update();
            if (GUI.changed)
                DrawButtonSave();
        }

        public void DrawButtonSave()
        {
            if (GUILayout.Button("Зберегти", GUILayout.Height(20)))
            {
                serializedObject.ApplyModifiedProperties();
                SetObjectDirty(_database);
            }
        }

        public static void SetObjectDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
    }
}
