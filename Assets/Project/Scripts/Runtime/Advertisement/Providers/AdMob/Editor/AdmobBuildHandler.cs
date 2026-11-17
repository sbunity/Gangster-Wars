using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if MODULE_ADMOB
using UnityEditor;
using UnityEngine;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    public sealed class AdmobBuildHandler : IPreprocessBuildWithReport
    {
        private const string SettingsFilePath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
        private const string AndroidAppIdPropertyPath = "adMobAndroidAppId";
        private const string IOSAppIdPropertyPath = "adMobIOSAppId";
        private const string OurAndroidAppIdPropertyPath = "_androidAppId";
        private const string OurIOSAppIdPropertyPath = "_iosAppId";
        private const string ProviderContainersPropertyPath = "_providerContainers";

        public int callbackOrder => -5;

        public void OnPreprocessBuild(BuildReport report)
        {
#if MODULE_ADMOB
            var adsData = SBabchuk.Utils.GetAsset<AdsSettings>();

            if (adsData == null)
            {
                Debug.LogError("AdsData don`t exist.");
                return;
            }

            var settingsFile = AssetDatabase.LoadMainAssetAtPath(SettingsFilePath);

            if (settingsFile == null)
            {
                GoogleMobileAds.Editor.GoogleMobileAdsSettingsEditor.OpenInspector();
                settingsFile = AssetDatabase.LoadMainAssetAtPath(SettingsFilePath);

                if (settingsFile == null)
                {
                    Debug.LogError("Failed to create instance of GoogleMobileAdsSettings.");
                    return;
                }
            }

            var adsDataObject = new SerializedObject(adsData);
            var settingsObject = new SerializedObject(settingsFile);

            var adMobContainerProperty = FindAdMobContainer(adsDataObject);
            if (adMobContainerProperty == null)
                return;

            var changed = false;

            if (settingsObject.FindProperty(AndroidAppIdPropertyPath).stringValue.Length == 0)
            {
                var ourAndroidAppId = adMobContainerProperty.FindPropertyRelative(OurAndroidAppIdPropertyPath).stringValue;
                if (ourAndroidAppId.Length != 0)
                {
                    settingsObject.FindProperty(AndroidAppIdPropertyPath).stringValue = ourAndroidAppId;
                    changed = true;
                }
            }

            if (settingsObject.FindProperty(IOSAppIdPropertyPath).stringValue.Length == 0)
            {
                var ourIOSAppId = adMobContainerProperty.FindPropertyRelative(OurIOSAppIdPropertyPath).stringValue;
                if (ourIOSAppId.Length != 0)
                {
                    settingsObject.FindProperty(IOSAppIdPropertyPath).stringValue = ourIOSAppId;
                    changed = true;
                }
            }

            if (changed)
                settingsObject.ApplyModifiedProperties();
#endif
        }

#if MODULE_ADMOB
        private static SerializedProperty FindAdMobContainer(SerializedObject adsDataObject)
        {
            var containers = adsDataObject.FindProperty(ProviderContainersPropertyPath);
            for (var i = 0; i < containers.arraySize; i++)
            {
                var element = containers.GetArrayElementAtIndex(i);
                if (element.managedReferenceValue is AdMobContainer)
                    return element;
            }

            return null;
        }
#endif
    }
}
