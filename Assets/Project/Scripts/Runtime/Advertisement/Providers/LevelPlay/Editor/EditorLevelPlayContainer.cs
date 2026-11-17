using UnityEditor;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [AdsEditorContainer(typeof(LevelPlayContainer))]
    public sealed class EditorLevelPlayContainer : EditorAdsContainer
    {
        private const float ButtonsSpacing = 8f;

        protected override string ContainerDisplayName => "LevelPlay";

        protected override void SpecialButtons()
        {
            GUILayout.Space(ButtonsSpacing);

            if (GUILayout.Button("Getting Started Guide"))
                Application.OpenURL(@"https://docs.unity.com/en-us/grow/levelplay/sdk/unity/get-started");

            if (GUILayout.Button("Integration Testing"))
                Application.OpenURL(@"https://docs.unity.com/en-us/grow/levelplay/sdk/unity/integration-test-suite");

            GUILayout.Space(ButtonsSpacing);

            EditorGUILayout.HelpBox("Tested with LevelPlay v9.3.0", MessageType.Info);
        }
    }
}
