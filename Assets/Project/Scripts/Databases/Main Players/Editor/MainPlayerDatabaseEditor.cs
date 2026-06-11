using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(MainPlayerDatabase))]
    public class MainPlayerDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            MainPlayerDatabaseDrawer.Draw((MainPlayerDatabase)Database, SelectedMode);
        }
    }
}