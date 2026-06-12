using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SBabchuk.Runtime.Databases.Base;

namespace SBabchuk.Runtime.Databases.PlayerPrefs
{
    [CustomEditor(typeof(PlayerPrefsDatabase))]
    public class PlayerPrefsDatabaseEditor : BaseDatabaseEditor
    {
        public override void Draw()
        {
            PlayerPrefsDatabaseDrawer.Draw((PlayerPrefsDatabase)Database);
        }
    }
}
