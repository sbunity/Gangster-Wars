using UnityEngine;

namespace SBabchuk.EditorTools.Defines
{
    [CreateAssetMenu(menuName = "Defines/Create ModuleDefineSettings", fileName = "ModuleDefine")]
    public sealed class ModuleDefineSettings : ScriptableObject
    {
        [SerializeField] private string _define;
        public string Define => _define;

        [SerializeField] private string _detectionType;
        public string DetectionType => _detectionType;

        [Tooltip("Optional file or DLL path. When it is deleted, the define is disabled before the next domain reload.")]
        [SerializeField] private string _filePath;
        public string FilePath => _filePath;
    }
}
