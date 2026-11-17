using SBabchuk.EditorTools;
using UnityEditor;
using UnityEngine;

#if MODULE_ADMOB
using System.Reflection;
#endif

namespace SBabchuk.Runtime.Advertisement
{
    [AdsEditorContainer(typeof(AdMobContainer))]
    public sealed class EditorAdMobContainer : EditorAdsContainer
    {
        private const string SettingsFilePath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";
        private const string TestAppId = "ca-app-pub-3940256099942544~3347511713";
        private const string AndroidAppIdPropertyPath = "adMobAndroidAppId";
        private const string OurAndroidAppIdPropertyPath = "_androidAppId";
        private const string IOSAppIdPropertyPath = "adMobIOSAppId";
        private const string OurIOSAppIdPropertyPath = "_iosAppId";

        private const string BannerTypePropertyPath = "_bannerType";
        private const string BannerPositionPropertyPath = "_bannerPosition";
        private const string AndroidBannerIdPropertyPath = "_androidBannerID";
        private const string AndroidInterstitialIdPropertyPath = "_androidInterstitialID";
        private const string AndroidRewardedVideoIdPropertyPath = "_androidRewardedVideoID";
        private const string IOSBannerIdPropertyPath = "_iOSBannerID";
        private const string IOSInterstitialIdPropertyPath = "_iOSInterstitialID";
        private const string IOSRewardedVideoIdPropertyPath = "_iOSRewardedVideoID";
        private const string UseAppOpenAdPropertyPath = "_useAppOpenAd";
        private const string AndroidAppOpenAdIdPropertyPath = "_androidAppOpenAdID";
        private const string IOSAppOpenAdIdPropertyPath = "_iOSAppOpenAdID";

        private const float WarningIconWidth = 24f;
        private const float ButtonsSpacing = 8f;

        private SerializedProperty _androidAppIdProperty;
        private SerializedProperty _iOSAppIdProperty;
        private SerializedProperty _ourAndroidAppIdProperty;
        private SerializedProperty _ourIOSAppIdProperty;
        private Object _settingsFile;
        private SerializedObject _serializedObject;
        private bool _fileLoaded;

        private SerializedProperty _bannerTypeProperty;
        private SerializedProperty _bannerPositionProperty;
        private SerializedProperty _androidBannerIdProperty;
        private SerializedProperty _androidInterstitialIdProperty;
        private SerializedProperty _androidRewardedVideoIdProperty;
        private SerializedProperty _iOSBannerIdProperty;
        private SerializedProperty _iOSInterstitialIdProperty;
        private SerializedProperty _iOSRewardedVideoIdProperty;

        private SerializedProperty _useAppOpenAdProperty;
        private SerializedProperty _androidAppOpenAdIDProperty;
        private SerializedProperty _iosAppOpenAdIDProperty;

        private GUIContent _testIdContent;
        private GUIStyle _groupStyle;

        protected override string ContainerDisplayName => "AdMob";

        public override void Init(SerializedProperty containerProp)
        {
            base.Init(containerProp);

            _bannerTypeProperty = _containerProperty.FindPropertyRelative(BannerTypePropertyPath);
            _bannerPositionProperty = _containerProperty.FindPropertyRelative(BannerPositionPropertyPath);
            _androidBannerIdProperty = _containerProperty.FindPropertyRelative(AndroidBannerIdPropertyPath);
            _androidInterstitialIdProperty = _containerProperty.FindPropertyRelative(AndroidInterstitialIdPropertyPath);
            _androidRewardedVideoIdProperty = _containerProperty.FindPropertyRelative(AndroidRewardedVideoIdPropertyPath);
            _iOSBannerIdProperty = _containerProperty.FindPropertyRelative(IOSBannerIdPropertyPath);
            _iOSInterstitialIdProperty = _containerProperty.FindPropertyRelative(IOSInterstitialIdPropertyPath);
            _iOSRewardedVideoIdProperty = _containerProperty.FindPropertyRelative(IOSRewardedVideoIdPropertyPath);

            _useAppOpenAdProperty = _containerProperty.FindPropertyRelative(UseAppOpenAdPropertyPath);
            _androidAppOpenAdIDProperty = _containerProperty.FindPropertyRelative(AndroidAppOpenAdIdPropertyPath);
            _iosAppOpenAdIDProperty = _containerProperty.FindPropertyRelative(IOSAppOpenAdIdPropertyPath);

            _fileLoaded = false;
            _ourAndroidAppIdProperty = _containerProperty.FindPropertyRelative(OurAndroidAppIdPropertyPath);
            _ourIOSAppIdProperty = _containerProperty.FindPropertyRelative(OurIOSAppIdPropertyPath);

            _testIdContent = new GUIContent("", EditorGUIUtility.IconContent("console.warnicon.sml").image, "You are using test app id value.");
            _groupStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerLeft
            };

            LoadFile();
        }

        private void LoadFile()
        {
            _settingsFile = AssetDatabase.LoadMainAssetAtPath(SettingsFilePath);

            if (_settingsFile != null)
            {
                _serializedObject = new SerializedObject(_settingsFile);
                _androidAppIdProperty = _serializedObject.FindProperty(AndroidAppIdPropertyPath);
                _iOSAppIdProperty = _serializedObject.FindProperty(IOSAppIdPropertyPath);
                _fileLoaded = true;
            }
            else
            {
#if MODULE_ADMOB
                var assembly = typeof(GoogleMobileAds.Editor.GoogleMobileAdsSettingsEditor).Assembly;
                var settingsType = assembly.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings");
                if (settingsType != null)
                {
                    var loadInstanceMethod = settingsType.GetMethod("LoadInstance", BindingFlags.NonPublic | BindingFlags.Static);

                    loadInstanceMethod?.Invoke(null, null);
                }
                else
                {
                    GoogleMobileAds.Editor.GoogleMobileAdsSettingsEditor.OpenInspector();
                }

                LoadFile();

                if (_fileLoaded)
                {
                    _serializedObject.Update();

                    if ((_androidAppIdProperty.stringValue.Length == 0) && (_ourAndroidAppIdProperty.stringValue.Length != 0))
                        _androidAppIdProperty.stringValue = _ourAndroidAppIdProperty.stringValue;
                    else if ((_androidAppIdProperty.stringValue.Length != 0) && (_ourAndroidAppIdProperty.stringValue.Length == 0))
                        _ourAndroidAppIdProperty.stringValue = _androidAppIdProperty.stringValue;

                    if ((_iOSAppIdProperty.stringValue.Length == 0) && (_ourIOSAppIdProperty.stringValue.Length != 0))
                        _iOSAppIdProperty.stringValue = _ourIOSAppIdProperty.stringValue;
                    else if ((_iOSAppIdProperty.stringValue.Length != 0) && (_ourIOSAppIdProperty.stringValue.Length == 0))
                        _ourIOSAppIdProperty.stringValue = _iOSAppIdProperty.stringValue;

                    _serializedObject.ApplyModifiedProperties();
                    _ourAndroidAppIdProperty.serializedObject.ApplyModifiedProperties();
                }
#endif
            }
        }

        public override void DrawContainer()
        {
            _containerProperty.isExpanded = EditorBoxGroup.BeginExpandBoxGroup(ContainerDisplayName, _containerProperty.isExpanded);

            if (_containerProperty.isExpanded)
            {
                DrawAppSection();
                DrawAddUnitsSection();

                DrawUsefulSection();

                _containerProperty.serializedObject.ApplyModifiedProperties();

                if (_fileLoaded)
                    _serializedObject.ApplyModifiedProperties();
            }

            EditorBoxGroup.EndBoxGroup();
        }

        private void DrawAppSection()
        {
            EditorGUILayout.LabelField("Application ID", _groupStyle);

            DrawIdProperty(_ourAndroidAppIdProperty, TestAppId);
            DrawIdProperty(_ourIOSAppIdProperty, TestAppId);

            if (_fileLoaded)
            {
                _androidAppIdProperty.stringValue = _ourAndroidAppIdProperty.stringValue;
                _iOSAppIdProperty.stringValue = _ourIOSAppIdProperty.stringValue;
            }
        }

        private void DrawIdProperty(SerializedProperty property, string testValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);

            if (property.stringValue.Equals(testValue))
                EditorGUILayout.LabelField(_testIdContent, GUILayout.MaxWidth(WarningIconWidth));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAddUnitsSection()
        {
            EditorGUILayout.LabelField("Banner ID", _groupStyle);
            DrawIdProperty(_androidBannerIdProperty, AdMobContainer.AndroidBannerTestId);
            DrawIdProperty(_iOSBannerIdProperty, AdMobContainer.IOSBannerTestId);

            EditorGUILayout.PropertyField(_bannerTypeProperty);
            EditorGUILayout.PropertyField(_bannerPositionProperty);

            EditorGUILayout.LabelField("Interstitial ID", _groupStyle);
            DrawIdProperty(_androidInterstitialIdProperty, AdMobContainer.AndroidInterstitialTestId);
            DrawIdProperty(_iOSInterstitialIdProperty, AdMobContainer.IOSInterstitialTestId);

            EditorGUILayout.LabelField("Rewarded Video ID", _groupStyle);
            DrawIdProperty(_androidRewardedVideoIdProperty, AdMobContainer.AndroidRewardedVideoTestId);
            DrawIdProperty(_iOSRewardedVideoIdProperty, AdMobContainer.IOSRewardedVideoTestId);

            EditorGUILayout.LabelField("App Open AD", _groupStyle);
            EditorGUILayout.PropertyField(_useAppOpenAdProperty);
            if (_useAppOpenAdProperty.boolValue)
            {
                DrawIdProperty(_androidAppOpenAdIDProperty, AdMobContainer.AndroidOpenTestId);
                DrawIdProperty(_iosAppOpenAdIDProperty, AdMobContainer.IOSOpenTestId);
            }

            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            if (GUILayout.Button("Set test app id"))
            {
                _ourAndroidAppIdProperty.stringValue = TestAppId;
                _ourIOSAppIdProperty.stringValue = TestAppId;
                _containerProperty.serializedObject.ApplyModifiedProperties();

                if (_fileLoaded)
                {
                    _androidAppIdProperty.stringValue = _ourAndroidAppIdProperty.stringValue;
                    _iOSAppIdProperty.stringValue = _ourIOSAppIdProperty.stringValue;
                    _serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button("Set test ids"))
            {
                _androidBannerIdProperty.stringValue = AdMobContainer.AndroidBannerTestId;
                _iOSBannerIdProperty.stringValue = AdMobContainer.IOSBannerTestId;

                _androidInterstitialIdProperty.stringValue = AdMobContainer.AndroidInterstitialTestId;
                _iOSInterstitialIdProperty.stringValue = AdMobContainer.IOSInterstitialTestId;

                _androidRewardedVideoIdProperty.stringValue = AdMobContainer.AndroidRewardedVideoTestId;
                _iOSRewardedVideoIdProperty.stringValue = AdMobContainer.IOSRewardedVideoTestId;

                _androidAppOpenAdIDProperty.stringValue = AdMobContainer.AndroidOpenTestId;
                _iosAppOpenAdIDProperty.stringValue = AdMobContainer.IOSOpenTestId;
            }
        }

        private void DrawUsefulSection()
        {
            EditorGUILayout.LabelField("Useful", EditorStyles.boldLabel);

            if (GUILayout.Button("Download AdMob plugin"))
                Application.OpenURL(@"https://github.com/googleads/googleads-mobile-unity/releases");

            if (GUILayout.Button("AdMob Dashboard"))
                Application.OpenURL(@"https://apps.admob.com/v2/home");

            if (GUILayout.Button("AdMob Quick Start Guide"))
                Application.OpenURL(@"https://developers.google.com/admob/unity/start");

            GUILayout.Space(ButtonsSpacing);

            EditorGUILayout.HelpBox("Tested with AdMob Plugin v11.0.0", MessageType.Info);
        }

        protected override void SpecialButtons()
        {
        }
    }
}
