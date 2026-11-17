using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if !MODULE_IAP
using UnityEditor;
using UnityEngine;
#endif

namespace SBabchuk.Runtime.Purchasing
{
    public sealed class IAPDummyModeBuildWarning : IPreprocessBuildWithReport
    {
        private const string SuppressKey = "SBabchuk.BuildWarning.IAPMode.Suppress";
        private const int DontShowAgainChoice = 1;
        private const int CancelChoice = 2;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if ((report.summary.options & UnityEditor.BuildOptions.Development) != 0) return;

#if !MODULE_IAP
            const string message = "IAP module is in Dummy mode — purchases won't work in production. Configure the module before releasing to stores.";

            if (Application.isBatchMode)
            {
                Debug.LogWarning("[IAP Manager]: " + message);
                return;
            }

            if (EditorPrefs.GetBool(SuppressKey, false)) return;

            var choice = EditorUtility.DisplayDialogComplex(
                "Dummy IAP Mode Active",
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
                throw new BuildFailedException("Build cancelled: Dummy IAP mode is active.");
#endif
        }
    }
}
