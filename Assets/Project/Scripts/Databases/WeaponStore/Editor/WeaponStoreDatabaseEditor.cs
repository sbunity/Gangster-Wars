using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
{
    [CustomEditor(typeof(WeaponStoreDatabase))]
    public class WeaponStoreDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            WeaponStoreDatabaseDrawer.Draw((WeaponStoreDatabase)database, selectedMode);
        }
    }
}
