using UnityEngine;

namespace SBabchuk.Runtime.Audio
{
    [CreateAssetMenu(menuName = "Audio/Create UISoundsConfig", fileName = "UISoundsConfig")]
    public sealed class UISoundsConfig : ScriptableObject
    {
        [SerializeField] private SoundConfig _click;

        public SoundConfig Click => _click;
    }
}
