using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace SBabchuk.EditorTools.Defines
{
    public static class DefineManager
    {
        public static NamedBuildTarget ActiveBuildTarget
            => NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

        public static bool HasDefine(string define)
        {
            var definesLine = PlayerSettings.GetScriptingDefineSymbols(ActiveBuildTarget);
            return Array.FindIndex(definesLine.Split(';'), x => x == define) != -1;
        }

        public static void EnableDefine(string define)
        {
            var definesString = new DefineString();
            definesString.AddDefine(define);
            definesString.ApplyDefines();
        }

        public static void DisableDefine(string define)
        {
            var definesString = new DefineString();
            definesString.RemoveDefine(define);
            definesString.ApplyDefines();
        }

        public static void CheckAutoDefines(string[] deletedAssets = null)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => CheckAutoDefines(deletedAssets);
                return;
            }

            bool CheckDeletedAssets(string filePath)
            {
                if (string.IsNullOrEmpty(filePath) || deletedAssets == null)
                    return false;

                foreach (var deletedAsset in deletedAssets)
                {
                    if (deletedAsset.EndsWith(filePath, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var markedDefines = new List<DefineState>();

            foreach (var registeredDefine in GetDynamicDefines())
            {
                if (CheckDeletedAssets(registeredDefine.FilePath))
                {
                    markedDefines.Add(new DefineState(registeredDefine.Define, false));
                    continue;
                }

                var defineFound = false;
                foreach (var assembly in assemblies)
                {
                    if (assembly.GetType(registeredDefine.AssemblyType, false) != null)
                    {
                        defineFound = true;
                        break;
                    }
                }

                markedDefines.Add(new DefineState(registeredDefine.Define, defineFound));
            }

            ChangeAutoDefinesState(markedDefines);
        }

        public static void ChangeAutoDefinesState(List<DefineState> defineStates)
        {
            if (EditorApplication.isCompiling || defineStates == null || defineStates.Count == 0)
                return;

            var definesUpdated = false;

            var sb = new StringBuilder();
            sb.Append("[Define Manager]: Dependencies change is detected. Updating Scripting Define Symbols..");
            sb.AppendLine();

            var definesString = new DefineString();
            foreach (var defineState in defineStates)
            {
                if (defineState.State)
                {
                    if (!definesString.HasDefine(defineState.Define))
                    {
                        definesUpdated = true;
                        definesString.AddDefine(defineState.Define);
                        sb.AppendLine();
                        sb.Append(defineState.Define);
                        sb.Append(" - added");
                    }
                }
                else
                {
                    if (definesString.HasDefine(defineState.Define))
                    {
                        definesUpdated = true;
                        definesString.RemoveDefine(defineState.Define);
                        sb.AppendLine();
                        sb.Append(defineState.Define);
                        sb.Append(" - removed");
                    }
                }
            }
            sb.AppendLine();

            if (definesUpdated)
                Debug.Log(sb.ToString());

            definesString.ApplyDefines();
        }

        public static ModuleDefineSettings[] GetModuleDefineSettings()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(ModuleDefineSettings));
            var result = new List<ModuleDefineSettings>();

            foreach (var guid in guids)
            {
                var settings = AssetDatabase.LoadAssetAtPath<ModuleDefineSettings>(AssetDatabase.GUIDToAssetPath(guid));

                if (settings != null)
                    result.Add(settings);
            }

            return result.ToArray();
        }

        public static List<RegisteredDefine> GetDynamicDefines()
        {
            var registeredDefines = new List<RegisteredDefine>();

            foreach (var settings in GetModuleDefineSettings())
            {
                if (string.IsNullOrEmpty(settings.Define) || string.IsNullOrEmpty(settings.DetectionType)) continue;

                if (registeredDefines.FindIndex(x => x.Define == settings.Define) == -1)
                    registeredDefines.Add(new RegisteredDefine(settings.Define, settings.DetectionType, settings.FilePath));
            }

            return registeredDefines;
        }
    }
}
