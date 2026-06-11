using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(DefenseStoreDatabase))]
    public class DefenseStoreDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            DefenseStoreDatabaseDrawer.Draw((DefenseStoreDatabase)Database, SelectedMode);
        }
    }
}