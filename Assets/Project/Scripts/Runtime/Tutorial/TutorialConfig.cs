using UnityEngine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Tutorial
{
    [CreateAssetMenu(menuName = "Databases/Create TutorialConfig", fileName = "TutorialConfig")]
    public sealed class TutorialConfig : ScriptableObject
    {
        [Header("Level")]
        [SerializeField, Min(0)] private int _tutorialLevelId;

        [Header("Enemy Target")]
        [SerializeField, FormerlySerializedAs("_shootTargetWorldSize")] private Vector2 _enemyTargetWorldSize = new(2.4f, 3.4f);

        [Header("Shoot")]
        [SerializeField] private string _shootCaption = "Hold and drag to shoot";
        [SerializeField, Min(0f)] private float _shootDelayAfterSpawn = 1f;
        [SerializeField, Range(0.05f, 1f)] private float _shootTimeScale = 0.2f;

        [Header("Reload")]
        [SerializeField] private string _reloadCaption = "Release to reload";
        [SerializeField, Min(1)] private int _reloadCompleteRounds = 3;
        [SerializeField, Min(0f)] private float _reloadMaxDuration = 6f;

        [Header("Barricade")]
        [SerializeField] private string _barricadeCaption = "Protect the barricade!";
        [SerializeField, Min(0f)] private float _barricadeHintDuration = 2.5f;
        [SerializeField, Range(0f, 1f)] private float _barricadeMinHealth = 0.2f;

        [Header("Wave Break")]
        [SerializeField, Min(0f)] private float _minSecondsUntilNextWave = 5f;
        [SerializeField, Min(0f)] private float _waveBreakSeconds = 6f;
        [SerializeField] private string _newEnemyCaption = "New enemy! Tap to see";
        [SerializeField] private string _nextWaveCaption = "Call the wave early for coins";

        [Header("Win")]
        [SerializeField] private string _starsCaption = "More barricade health, more stars";

        [Header("Menu")]
        [SerializeField] private WeaponsName _upgradeWeapon = WeaponsName.Weapon_1;
        [SerializeField] private string _upgradeCaption = "Upgrade your weapon";
        [SerializeField] private string _playCaption = "Play the next level";
        [SerializeField] private string _selectLevelCaption = "Tap to play";

        [Header("Grenade")]
        [SerializeField, Min(0)] private int _grenadeLevelId = 1;
        [SerializeField] private GrenadesName _giftGrenade = GrenadesName.Grenade_1;
        [SerializeField, Min(0)] private int _giftGrenadeCount = 2;
        [SerializeField] private string _grenadeCaption = "Drag the grenade at enemies";
        [SerializeField, Min(0f)] private float _grenadeDelayAfterSpawn = 1f;
        [SerializeField, Range(0.05f, 1f)] private float _grenadeTimeScale = 0.25f;
        [SerializeField, Min(0f)] private float _grenadeHintMaxDuration = 8f;

        [Header("Bonus")]
        [SerializeField] private string _bonusCaption = "Bonus! Tap to grab it faster";
        [SerializeField] private Vector2 _bonusTargetWorldSize = new(1.4f, 1.4f);
        [SerializeField, Min(0f)] private float _bonusSlotHintDuration = 1.5f;

        [Header("Switch Weapon")]
        [SerializeField] private string _switchWeaponCaption = "Tap to switch weapon";
        [SerializeField, Min(0f)] private float _switchWeaponHintDuration = 6f;

        public int TutorialLevelId => _tutorialLevelId;

        public Vector2 EnemyTargetWorldSize => _enemyTargetWorldSize;

        public string ShootCaption => _shootCaption;
        public float ShootDelayAfterSpawn => _shootDelayAfterSpawn;
        public float ShootTimeScale => _shootTimeScale;

        public string ReloadCaption => _reloadCaption;
        public int ReloadCompleteRounds => _reloadCompleteRounds;
        public float ReloadMaxDuration => _reloadMaxDuration;

        public string BarricadeCaption => _barricadeCaption;
        public float BarricadeHintDuration => _barricadeHintDuration;
        public float BarricadeMinHealth => _barricadeMinHealth;

        public float MinSecondsUntilNextWave => _minSecondsUntilNextWave;
        public float WaveBreakSeconds => _waveBreakSeconds;
        public string NewEnemyCaption => _newEnemyCaption;
        public string NextWaveCaption => _nextWaveCaption;

        public string StarsCaption => _starsCaption;

        public WeaponsName UpgradeWeapon => _upgradeWeapon;
        public string UpgradeCaption => _upgradeCaption;
        public string PlayCaption => _playCaption;
        public string SelectLevelCaption => _selectLevelCaption;

        public int GrenadeLevelId => _grenadeLevelId;
        public GrenadesName GiftGrenade => _giftGrenade;
        public int GiftGrenadeCount => _giftGrenadeCount;
        public string GrenadeCaption => _grenadeCaption;
        public float GrenadeDelayAfterSpawn => _grenadeDelayAfterSpawn;
        public float GrenadeTimeScale => _grenadeTimeScale;
        public float GrenadeHintMaxDuration => _grenadeHintMaxDuration;

        public string BonusCaption => _bonusCaption;
        public Vector2 BonusTargetWorldSize => _bonusTargetWorldSize;
        public float BonusSlotHintDuration => _bonusSlotHintDuration;

        public string SwitchWeaponCaption => _switchWeaponCaption;
        public float SwitchWeaponHintDuration => _switchWeaponHintDuration;
    }
}
