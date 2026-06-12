using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.MainPlayers
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
