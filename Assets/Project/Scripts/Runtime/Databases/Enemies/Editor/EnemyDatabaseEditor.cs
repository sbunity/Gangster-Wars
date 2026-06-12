using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.Enemies
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
