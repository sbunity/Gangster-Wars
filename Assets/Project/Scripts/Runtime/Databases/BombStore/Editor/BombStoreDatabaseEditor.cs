using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.BombStore
{
    [CustomEditor(typeof(BombStoreDatabase))]
    public class BombStoreDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            BombStoreDatabaseDrawer.Draw((BombStoreDatabase)Database, SelectedMode);
        }
    }
}
