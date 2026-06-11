using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
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