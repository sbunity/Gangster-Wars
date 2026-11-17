using UnityEditor;
using UnityEditor.Callbacks;

namespace SBabchuk.EditorTools.Defines
{
    public sealed class DefinePostprocessor : AssetPostprocessor
    {
        private const string PrefsKey = "SBabchuk.DefinesCheck";

        [DidReloadScripts]
        private static void AssemblyReload()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += AssemblyReload;
                return;
            }

            EditorApplication.delayCall += () => DefineManager.CheckAutoDefines();
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            ValidateRequirement(importedAssets, deletedAssets);

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths, didDomainReload);
                return;
            }

            if (EditorPrefs.GetBool(PrefsKey, false))
            {
                DefineManager.CheckAutoDefines(deletedAssets);
                EditorPrefs.SetBool(PrefsKey, false);
            }
        }

        private static void ValidateRequirement(string[] importedAssets, string[] deletedAssets)
        {
            if (ContainsCode(importedAssets) || ContainsCode(deletedAssets))
                EditorPrefs.SetBool(PrefsKey, true);
        }

        private static bool ContainsCode(string[] assets)
        {
            if (assets == null)
                return false;

            foreach (var asset in assets)
            {
                if (asset.EndsWith(".cs") || asset.EndsWith(".dll"))
                    return true;
            }

            return false;
        }
    }
}
