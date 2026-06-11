using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(EnemyDatabase))]
    public class EnemyDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            EnemyDatabaseDrawer.Draw((EnemyDatabase)Database, SelectedMode);
        }
    }
}