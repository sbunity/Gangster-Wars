using UnityEditor;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [AdsEditorContainer(typeof(ApplovinContainer))]
    public sealed class EditorApplovinContainer : EditorAdsContainer
    {
        private const float ButtonsSpacing = 8f;

        protected override string ContainerDisplayName => "AppLovin";

        protected override void SpecialButtons()
        {
            GUILayout.Space(ButtonsSpacing);

            EditorGUILayout.HelpBox("Tested with Applovin v8.6.2", MessageType.Info);
        }
    }
}
