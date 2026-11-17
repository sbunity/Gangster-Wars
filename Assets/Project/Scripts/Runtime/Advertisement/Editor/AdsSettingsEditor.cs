using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SBabchuk.EditorTools;
using UnityEditor;
using UnityEngine;

namespace SBabchuk.Runtime.Advertisement
{
    [CustomEditor(typeof(AdsSettings))]
    public sealed class AdsSettingsEditor : Editor
    {
        private const float SectionSpacing = 8f;

        private EditorAdsContainer[] _adsContainers;
        private string[] _availableProviderNames;

        private SerializedProperty _activeProviderProperty;
        private SerializedProperty _debugModeProperty;
        private SerializedProperty _testDevicesProperty;
        private SerializedProperty _loadAdsOnStartProperty;
        private SerializedProperty _bannerEnabledProperty;
        private SerializedProperty _interstitialEnabledProperty;
        private SerializedProperty _rewardedVideoEnabledProperty;
        private SerializedProperty _interstitialFirstStartDelayProperty;
        private SerializedProperty _interstitialStartDelayProperty;
        private SerializedProperty _interstitialShowingDelayProperty;
        private SerializedProperty _loadingAdDurationProperty;
        private SerializedProperty _loadingMessageProperty;
        private SerializedProperty _privacyLinkProperty;
        private SerializedProperty _termsOfUseLinkProperty;

        private void OnEnable()
        {
            _activeProviderProperty = serializedObject.FindProperty("_activeProvider");
            _debugModeProperty = serializedObject.FindProperty("_debugMode");
            _testDevicesProperty = serializedObject.FindProperty("_testDevices");
            _loadAdsOnStartProperty = serializedObject.FindProperty("_loadAdsOnStart");
            _bannerEnabledProperty = serializedObject.FindProperty("_bannerEnabled");
            _interstitialEnabledProperty = serializedObject.FindProperty("_interstitialEnabled");
            _rewardedVideoEnabledProperty = serializedObject.FindProperty("_rewardedVideoEnabled");
            _interstitialFirstStartDelayProperty = serializedObject.FindProperty("_interstitialFirstStartDelay");
            _interstitialStartDelayProperty = serializedObject.FindProperty("_interstitialStartDelay");
            _interstitialShowingDelayProperty = serializedObject.FindProperty("_interstitialShowingDelay");
            _loadingAdDurationProperty = serializedObject.FindProperty("_loadingAdDuration");
            _loadingMessageProperty = serializedObject.FindProperty("_loadingMessage");
            _privacyLinkProperty = serializedObject.FindProperty("_privacyLink");
            _termsOfUseLinkProperty = serializedObject.FindProperty("_termsOfUseLink");

            EnsureContainersPopulated();
            BuildEditorContainers();

            serializedObject.ApplyModifiedProperties();
        }

        private void EnsureContainersPopulated()
        {
            var containersProp = serializedObject.FindProperty("_providerContainers");
            var containerTypes = TypeCache.GetTypesDerivedFrom<AdsProviderContainer>()
                .Where(type => !type.IsAbstract);

            var modified = false;

            foreach (var type in containerTypes)
            {
                var exists = false;
                for (var i = 0; i < containersProp.arraySize; i++)
                {
                    if (containersProp.GetArrayElementAtIndex(i).managedReferenceValue?.GetType() == type)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    containersProp.arraySize++;
                    containersProp.GetArrayElementAtIndex(containersProp.arraySize - 1).managedReferenceValue =
                        Activator.CreateInstance(type);
                    modified = true;
                }
            }

            if (modified)
                serializedObject.ApplyModifiedProperties();
        }

        private void BuildEditorContainers()
        {
            var containersProp = serializedObject.FindProperty("_providerContainers");

            var editorContainerMap = new Dictionary<Type, Type>();
            foreach (var editorType in TypeCache.GetTypesDerivedFrom<EditorAdsContainer>())
            {
                if (editorType.IsAbstract) continue;

                var attr = editorType.GetCustomAttribute<AdsEditorContainerAttribute>();
                if (attr != null)
                    editorContainerMap[attr.ContainerType] = editorType;
            }

            var providerNames = new List<string>();
            for (var i = 0; i < containersProp.arraySize; i++)
            {
                if (containersProp.GetArrayElementAtIndex(i).managedReferenceValue is AdsProviderContainer container)
                    providerNames.Add(container.ProviderName);
            }
            _availableProviderNames = providerNames.ToArray();

            var containers = new List<EditorAdsContainer>();
            for (var i = 0; i < containersProp.arraySize; i++)
            {
                var element = containersProp.GetArrayElementAtIndex(i);
                var containerType = element.managedReferenceValue?.GetType();
                if (containerType == null) continue;

                if (editorContainerMap.TryGetValue(containerType, out var editorType))
                {
                    var editorContainer = (EditorAdsContainer)Activator.CreateInstance(editorType);
                    editorContainer.Init(element);
                    containers.Add(editorContainer);
                }
            }

            _adsContainers = containers.ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSettingsSection();
            DrawLegalSection();

            DrawProviderSection();

            GUILayout.Space(SectionSpacing);

            if (_adsContainers != null)
            {
                foreach (var container in _adsContainers)
                    container.DrawContainer();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettingsSection()
        {
            EditorBoxGroup.BeginBoxGroup("Settings");

            EditorGUILayout.PropertyField(_debugModeProperty);

            if (_debugModeProperty.boolValue)
                EditorGUILayout.PropertyField(_testDevicesProperty, true);

            EditorGUILayout.PropertyField(_loadAdsOnStartProperty);

            EditorBoxGroup.BeginBoxGroup("Ad Types");
            EditorGUILayout.PropertyField(_bannerEnabledProperty);
            EditorGUILayout.PropertyField(_interstitialEnabledProperty);
            EditorGUILayout.PropertyField(_rewardedVideoEnabledProperty);
            EditorBoxGroup.EndBoxGroup();

            EditorBoxGroup.BeginBoxGroup("Interstitial");
            EditorGUILayout.PropertyField(_interstitialFirstStartDelayProperty);
            EditorGUILayout.PropertyField(_interstitialStartDelayProperty);
            EditorGUILayout.PropertyField(_interstitialShowingDelayProperty);
            EditorBoxGroup.EndBoxGroup();

            EditorBoxGroup.BeginBoxGroup("Delay");
            EditorGUILayout.PropertyField(_loadingAdDurationProperty);
            EditorGUILayout.PropertyField(_loadingMessageProperty);
            EditorBoxGroup.EndBoxGroup();

            EditorBoxGroup.EndBoxGroup();
        }

        private void DrawLegalSection()
        {
            EditorBoxGroup.BeginBoxGroup("Legal");
            EditorGUILayout.PropertyField(_privacyLinkProperty);
            EditorGUILayout.PropertyField(_termsOfUseLinkProperty);
            EditorBoxGroup.EndBoxGroup();
        }

        private void DrawProviderSection()
        {
            if (_availableProviderNames == null || _availableProviderNames.Length == 0)
                return;

            EditorBoxGroup.BeginBoxGroup("Active Provider");

            var currentIndex = Array.IndexOf(_availableProviderNames, _activeProviderProperty.stringValue);

            if (currentIndex < 0)
            {
                EditorGUILayout.HelpBox($"Unknown provider '{_activeProviderProperty.stringValue}'. Select a valid one.", MessageType.Error);
                currentIndex = 0;
            }

            var newIndex = EditorGUILayout.Popup("Provider", currentIndex, _availableProviderNames);

            if (newIndex != currentIndex)
                _activeProviderProperty.stringValue = _availableProviderNames[newIndex];

            EditorBoxGroup.EndBoxGroup();
        }
    }
}
