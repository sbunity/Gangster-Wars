using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SBabchuk.EditorTools
{
    [InitializeOnLoad]
    public static class PlayModeStartScene
    {
        static PlayModeStartScene()
        {
            EditorBuildSettings.sceneListChanged += Apply;
            EditorApplication.delayCall += Apply;
        }

        private static void Apply()
        {
            var firstScene = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled);
            EditorSceneManager.playModeStartScene = firstScene != null
                ? AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScene.path)
                : null;
        }
    }
}
