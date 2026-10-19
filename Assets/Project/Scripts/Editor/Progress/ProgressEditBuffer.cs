using SBabchuk.Runtime.Progress;
using UnityEngine;

namespace SBabchuk.EditorTools
{
    public sealed class ProgressEditBuffer : ScriptableObject
    {
        public const string ProgressPropertyName = nameof(_progress);

        [SerializeField] private PlayerProgress _progress;

        public PlayerProgress Progress { get => _progress; set => _progress = value; }
    }
}
