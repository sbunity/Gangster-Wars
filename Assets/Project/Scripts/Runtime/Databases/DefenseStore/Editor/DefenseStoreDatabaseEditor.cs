using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.DefenseStore
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
