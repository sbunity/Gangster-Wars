using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using SBabchuk.Runtime.Databases.Bullets;
using SBabchuk.Runtime.Databases.DefenseStore;
using SBabchuk.Runtime.Gameplay.Projectiles;
using SBabchuk.Runtime.UI;

namespace SBabchuk.Runtime.Gameplay.Barricades
{
    public class BarricadeController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("healthbar")]
        private FilledBarController _healthBar;
        
        public int CurrentHealth { get; private set; } = 10;
        public int MaxHealth { get; private set; }

        private SpriteRenderer _spriteRenderer;
        private List<Ico> _defenceIcons = new();
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ILevelFlowService _levelFlowService;
        private SignalBus _signalBus;
        
        [Inject]
        public void Construct(IAssetProvider assetProvider, IPlayerProgressService progressService, ILevelFlowService levelFlowService, SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _levelFlowService = levelFlowService;
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            var selectedDefenceId = _progressService.SelectedDefenceId;
            var shortInfo = _progressService.GetDefenceShortInfo(selectedDefenceId);
            var defenceStore = _assetProvider.DefenseStoreDatabase;
            var upgrade = defenceStore.GetUpgrade(shortInfo.Id, shortInfo.UpgradeId);
            var defence = defenceStore.GetDefense(shortInfo.Id);
            CurrentHealth = (upgrade != null) ? upgrade.Settings.Health : defence.Settings.Health;
            _defenceIcons = defence.Icons;
            MaxHealth = CurrentHealth;
            SetHealth(MaxHealth);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Bullet")
            {
                var bullet = other.gameObject.GetComponent<BaseBulletController>();
                if (bullet != null)
                {
                    TakeDamage(bullet.Damage);
                    bullet.Pop();
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (CurrentHealth - damage <= 0)
            {
                SetHealth(-CurrentHealth);
                _levelFlowService.Finish(Panels.Lose);
            }
            else
            {
                SetHealth(-damage);
            }
        }

        public void SetHealth(int value)
        {
            if (CurrentHealth + value > MaxHealth)
                CurrentHealth = MaxHealth;
            else if (CurrentHealth - value < 0)
                CurrentHealth = 0;
            else
                CurrentHealth += value;
            CheckHealth(value);
            _signalBus.Fire(new BarricadeHealthChangedSignal((float)CurrentHealth / MaxHealth));
        }

        private void CheckHealth(int value)
        {
            if (CurrentHealth >= 0.75f * MaxHealth)
                _spriteRenderer.sprite = _defenceIcons[0].Icon;
            else if (CurrentHealth >= 0.25f * MaxHealth)
                _spriteRenderer.sprite = _defenceIcons[1].Icon;
            else if (CurrentHealth > 0)
                _spriteRenderer.sprite = _defenceIcons[2].Icon;
            else
                _spriteRenderer.sprite = _defenceIcons[3].Icon;
            _healthBar.UpdateFilled((float)value / (float)MaxHealth);
        }
    }
}
