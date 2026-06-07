using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    /// <summary>
    /// Базовий клас Editor
    /// </summary>
    public class BaseDatabaseEditor : Editor
    {
        /// <summary>
        /// База даних
        /// </summary>
        public ScriptableObject database;

        /// <summary>
        /// Горизонтальне чи вертикальне відображення
        /// </summary>
        public int selectedMode = 0;

        /// <summary>
        /// Як відображається режим на кнопці
        /// </summary>
        public string[] mode = { "|", "--" };

        [Header("Колір по замовчуванні")]
        static Color defaultColor;

        /// <summary>
        /// OnEnable - ініціалізація
        /// </summary>
        public void OnEnable()
        {
            database = (ScriptableObject)target;
        }

        /// <summary>
        /// Головний метод промальовки
        /// </summary>
        public override void OnInspectorGUI()
        {
            defaultColor = GUI.color;

            SetMode();

            if (database == null)
                database = (ScriptableObject)target;

            DrawButtonSave();

            Draw();

            Update();
        }

        /// <summary>
        /// Метод, де йде варіація вибора медотів промалбовки
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        /// Промальовка режима
        /// </summary>
        public void SetMode()
        {
            GUILayout.BeginHorizontal();
            {
                selectedMode = GUILayout.Toolbar(selectedMode, mode);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Оновлення
        /// </summary>
        public void Update()
        {
            serializedObject.Update();
            

            if (GUI.changed) //SetObjectDirty(database);
                DrawButtonSave();
        }

        public void DrawButtonSave()
        {
            if (GUILayout.Button("Зберегти", GUILayout.Height(20)))
            {
                serializedObject.ApplyModifiedProperties();
                SetObjectDirty(database);
            }
        }

        /// <summary>
        /// Збереження змін
        /// </summary>
        /// <param name="obj"></param>
        public static void SetObjectDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }
    }
}
