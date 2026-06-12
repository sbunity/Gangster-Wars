using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.Bullets
{
    [CustomEditor(typeof(BulletDatabase))]
    public class BulletDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            BulletDatabaseDrawer.Draw((BulletDatabase)Database, SelectedMode);
        }
    }
}
