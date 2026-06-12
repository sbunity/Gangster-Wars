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
            LevelDatabaseDrawer.Draw((LevelDatabase)Database, SelectedMode);
        }
    }
}
