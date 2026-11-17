using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS && !UNITY_EDITOR_WIN
using System.IO;
using UnityEditor.iOS.Xcode;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    public static class AdsPostBuild
    {
        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToXcode)
        {
            if (buildTarget == BuildTarget.iOS)
                AddPListValues(pathToXcode);
        }

        private static void AddPListValues(string pathToXcode)
        {
#if UNITY_IOS && !UNITY_EDITOR_WIN
            var plistPath = pathToXcode + "/Info.plist";

            var plistObj = new PlistDocument();
            plistObj.ReadFromString(File.ReadAllText(plistPath));

            var plistRoot = plistObj.root;

            plistRoot.SetString("NSUserTrackingUsageDescription", "Your data will be used to deliver personalized ads to you.");

            File.WriteAllText(plistPath, plistObj.WriteToString());
#endif
        }
    }
}
