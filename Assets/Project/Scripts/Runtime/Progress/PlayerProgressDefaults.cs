using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [CreateAssetMenu(menuName = "Databases/Create PlayerProgressDefaults", fileName = "PlayerProgressDefaults")]
    public sealed class PlayerProgressDefaults : ScriptableObject
    {
        [Header("Economy")]
        [SerializeField, Min(0)] private int _coins = 100;

        [Header("Levels")]
        [SerializeField, Min(0)] private int _levelId;
        [SerializeField, Min(0)] private int _unlockedChapterId;

        [Header("Starter Equipment")]
        [SerializeField] private WeaponsName _starterWeapon = WeaponsName.Weapon_1;
        [SerializeField, Min(0)] private int _starterWeaponAmmo = 70;
        [SerializeField] private GrenadesName _selectedGrenade = GrenadesName.Grenade_1;
        [SerializeField] private DefencesName _starterDefence = DefencesName.Defence_1;

        [Header("Cosmetics")]
        [SerializeField] private CarSkinsName _starterCarSkin = CarSkinsName.Black;

        [Header("Settings")]
        [SerializeField] private bool _musicEnabled = true;
        [SerializeField] private bool _soundEnabled = true;
        [SerializeField] private bool _vibrationEnabled = true;

        public int Coins => _coins;
        public int LevelId => _levelId;
        public int UnlockedChapterId => _unlockedChapterId;
        public int StarterWeaponId => (int)_starterWeapon;
        public int StarterWeaponAmmo => _starterWeaponAmmo;
        public int SelectedGrenadeId => (int)_selectedGrenade;
        public int StarterDefenceId => (int)_starterDefence;
        public int StarterCarSkinId => (int)_starterCarSkin;
        public bool MusicEnabled => _musicEnabled;
        public bool SoundEnabled => _soundEnabled;
        public bool VibrationEnabled => _vibrationEnabled;
    }
}
