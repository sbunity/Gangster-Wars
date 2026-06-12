using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.WeaponStore
{
    [CustomEditor(typeof(WeaponStoreDatabase))]
    public class WeaponStoreDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            WeaponStoreDatabaseDrawer.Draw((WeaponStoreDatabase)Database, SelectedMode);
        }
    }
}
