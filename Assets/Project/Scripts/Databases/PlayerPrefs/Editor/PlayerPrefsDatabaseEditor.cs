using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SBabchuk
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