using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class AdsDummyModeBuildWarning : IPreprocessBuildWithReport
    {
        private const string SuppressKey = "SBabchuk.BuildWarning.AdsMode.Suppress";
        private const string DummyProviderName = "Dummy";
        private const int DontShowAgainChoice = 1;
        private const int CancelChoice = 2;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if ((report.summary.options & BuildOptions.Development) != 0) return;

            var guids = AssetDatabase.FindAssets("t:AdsSettings");
            if (guids.Length == 0) return;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var adsSettings = AssetDatabase.LoadAssetAtPath<AdsSettings>(path);

            if (adsSettings == null || adsSettings.ActiveProvider != DummyProviderName) return;

            const string message = "Ads module is set to Dummy provider — ads won't show in production. Configure the module before releasing to stores.";

            if (Application.isBatchMode)
            {
                Debug.LogWarning("[AdsManager]: " + message);
                return;
            }

            if (EditorPrefs.GetBool(SuppressKey, false)) return;

            var choice = EditorUtility.DisplayDialogComplex(
                "Dummy Ads Mode Active",
                message + "\n\nContinue build?",
                "Continue",
                "Don't show again",
                "Cancel"
            );

            if (choice == DontShowAgainChoice)
            {
                EditorPrefs.SetBool(SuppressKey, true);
                return;
            }

            if (choice == CancelChoice)
                throw new BuildFailedException("Build cancelled: Dummy Ads mode is active.");
        }
    }
}
