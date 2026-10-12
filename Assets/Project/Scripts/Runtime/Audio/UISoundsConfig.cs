using UnityEngine;

namespace SBabchuk.Runtime.Audio
{
    [CreateAssetMenu(menuName = "Audio/Create UISoundsConfig", fileName = "UISoundsConfig")]
    public sealed class UISoundsConfig : ScriptableObject
    {
        [SerializeField] private SoundConfig _click;
        [SerializeField] private SoundConfig _purchase;
        [SerializeField] private SoundConfig _supply;

        public SoundConfig Click => _click;
        public SoundConfig Purchase => _purchase;
        public SoundConfig Supply => _supply;
    }
}
