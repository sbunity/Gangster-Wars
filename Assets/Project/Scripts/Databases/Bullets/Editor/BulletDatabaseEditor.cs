using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(BulletDatabase))]
    public class BulletDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            BulletDatabaseDrawer.Draw((BulletDatabase)database, selectedMode);
        }
    }
}