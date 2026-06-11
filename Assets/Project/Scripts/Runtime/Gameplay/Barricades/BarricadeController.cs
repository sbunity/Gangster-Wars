using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk
{
    public class BarricadeController : MonoBehaviour
    {
        [Header("Поточна к-сть життів")]
        public int currentHealth = 10;

        [Header("Максимальна кількість життів")]
        [HideInInspector] public int maxHealth;

        [Header("Відображення життя")]
        public FilledBarController healthbar;

        [Header("Відображення")]
        private SpriteRenderer sRenderer;

        private List<Ico> icons = new List<Ico>();
        private IAssetProvider _assetProvider;
        private IPlayerProgressService _progressService;
        private ILevelFlowService _levelFlowService;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(
            IAssetProvider assetProvider,
            IPlayerProgressService progressService,
            ILevelFlowService levelFlowService,
            SignalBus signalBus)
        {
            _assetProvider = assetProvider;
            _progressService = progressService;
            _levelFlowService = levelFlowService;
            _signalBus = signalBus;
        }

        public void Awake()
        {
            sRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            var selectedDefenceId = _progressService.SelectedDefenceId;

            DefenceShortInfo shortInfo = _progressService.GetDefenceShortInfo(selectedDefenceId);

            var defenceStore = _assetProvider.DefenseStoreDatabase;

            DUpgrade upgrade = defenceStore.GetUpgrade(shortInfo.id, shortInfo.upgradeID);

            Defense defence = defenceStore.GetDefense(shortInfo.id);

            if (upgrade != null)
            {
                currentHealth = upgrade.settings.health;
            }
            else
            {
                currentHealth = defence.settings.health;
            }

            icons = defence.icons;

            maxHealth = currentHealth;

            //CheckHealth();

            SetHealth(maxHealth);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Bullet")
            {
                BaseBulletController _bullet = other.gameObject.GetComponent<BaseBulletController>();

                if (_bullet != null) { 
                    TakeDamage(_bullet.damage);
                    _bullet.Pop();
                }
            }
        }

        /// <summary>
        /// Отримання шкоди
        /// </summary>
        /// <param name="_damage"></param>
        public void TakeDamage(int _damage)
        {
            Debug.Log("Отримання урона: " + _damage);

            if (currentHealth - _damage <= 0)
            {
                SetHealth(-currentHealth);

                _levelFlowService.Finish(Panels.Lose);
            }
            else
            {
                SetHealth(-_damage);
            }
        }

        /// <summary>
        /// Добавляє життя
        /// </summary>
        /// <param name="_value"></param>
        public void SetHealth(int _value)
        {
            if (currentHealth + _value > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else if (currentHealth - _value < 0)
            {
                currentHealth = 0;
            }
            else
            {
                currentHealth += _value;
            }

            CheckHealth(_value);

            _signalBus.Fire(new BarricadeHealthChangedSignal((float)currentHealth / maxHealth));
        }

        private void CheckHealth(int _value)
        {
            if (currentHealth >= 0.75f * maxHealth)
            {
                sRenderer.sprite = icons[0].ico;
            }
            else if(currentHealth >= 0.25f * maxHealth)
            {
                sRenderer.sprite = icons[1].ico;
            }
            else if (currentHealth > 0)
            {
                sRenderer.sprite = icons[2].ico;
            }
            else
            {
                sRenderer.sprite = icons[3].ico;
            }

            healthbar.UpdateFlled((float)_value / (float)maxHealth);
        }
    }
}
