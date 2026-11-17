using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SBabchuk.EditorTools.Defines
{
    public sealed class DefineManagerWindow : EditorWindow
    {
        private const float ToggleWidth = 20f;
        private const float RemoveButtonSize = 18f;

        private Define[] _projectDefines;

        private bool _isDefinesSame;
        private bool _isRequireInit;

        [MenuItem("Tools/Define Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<DefineManagerWindow>(true);
            window.minSize = new Vector2(300, 200);
            window.titleContent = new GUIContent("Define Manager");
        }

        private void OnEnable()
        {
            _isRequireInit = true;

            CacheVariables();
        }

        private void CacheVariables()
        {
            var defines = new List<Define>();

            var registeredDefines = DefineManager.GetDynamicDefines();

            foreach (var currentDefine in GetCurrentDefines())
            {
                var type = registeredDefines.FindIndex(x => x.Define == currentDefine) != -1
                    ? Define.Type.Auto
                    : Define.Type.ThirdParty;

                if (defines.FindIndex(x => x.Name == currentDefine) == -1)
                    defines.Add(new Define(currentDefine, type, true));
            }

            _projectDefines = defines.ToArray();
        }

        private string GetActiveDefinesLine()
        {
            return string.Join(";", _projectDefines.Where(define => define.IsEnabled).Select(define => define.Name));
        }

        private static string[] GetCurrentDefines()
        {
            return PlayerSettings.GetScriptingDefineSymbols(DefineManager.ActiveBuildTarget)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void SaveDefines(string definesLine)
        {
            PlayerSettings.SetScriptingDefineSymbols(DefineManager.ActiveBuildTarget, definesLine);
        }

        private bool CompareDefines()
        {
            var currentDefines = GetCurrentDefines();

            foreach (var define in _projectDefines)
            {
                var isActive = Array.IndexOf(currentDefines, define.Name) != -1;
                if (define.IsEnabled != isActive)
                    return false;
            }

            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (_projectDefines != null && _projectDefines.Length > 0)
            {
                EditorGUI.BeginChangeCheck();

                foreach (var define in _projectDefines)
                {
                    EditorGUILayout.BeginHorizontal();

                    switch (define.DefineType)
                    {
                        case Define.Type.Auto:
                            define.IsEnabled = EditorGUILayout.Toggle(define.IsEnabled, GUILayout.Width(ToggleWidth));
                            EditorGUILayout.LabelField(define.Name + " (Auto)");
                            break;
                        case Define.Type.ThirdParty:
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle(true, GUILayout.Width(ToggleWidth));
                            EditorGUI.EndDisabledGroup();

                            EditorGUILayout.LabelField(define.Name + " (Third Party)");

                            GUILayout.FlexibleSpace();

                            var backupColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;

                            if (GUILayout.Button("X", GUILayout.Height(RemoveButtonSize), GUILayout.Width(RemoveButtonSize)))
                            {
                                if (EditorUtility.DisplayDialog("Remove define", "Are you sure you want to remove define?", "Remove", "Cancel"))
                                    SaveDefines(string.Join(";", GetCurrentDefines().Where(name => name != define.Name)));
                            }

                            GUI.backgroundColor = backupColor;
                            break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (EditorGUI.EndChangeCheck())
                    _isRequireInit = true;
            }
            else
            {
                EditorGUILayout.LabelField("There are no defines in project.");
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (_isRequireInit)
            {
                _isDefinesSame = CompareDefines();

                _isRequireInit = false;
            }

            EditorGUI.BeginDisabledGroup(_isDefinesSame);

            if (GUILayout.Button("Apply Defines"))
            {
                SaveDefines(GetActiveDefinesLine());

                return;
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Check Auto Defines"))
            {
                DefineManager.CheckAutoDefines();

                return;
            }

            EditorGUILayout.EndVertical();

            if (EditorApplication.isCompiling)
                EditorGUILayout.HelpBox("Compiling...", MessageType.Info);
        }

        private sealed class Define
        {
            public string Name { get; }
            public Type DefineType { get; }
            public bool IsEnabled { get; set; }

            public Define(string name, Type defineType, bool isEnabled = false)
            {
                Name = name;
                DefineType = defineType;
                IsEnabled = isEnabled;
            }

            public enum Type
            {
                ThirdParty = 2,
                Auto = 3
            }
        }
    }
}
