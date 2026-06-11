using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(LevelDatabase))]
    public class LevelDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            LevelDatabaseDrawer.Draw((LevelDatabase)Database, SelectedMode);
        }
    }
}