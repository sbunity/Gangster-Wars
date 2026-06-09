using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    public class BarricadeController : MonoBehaviour
    {
        public delegate void Updatevalue(float _value);
        public static event Updatevalue OnUpdateHealth;

        public delegate void GameOver(Panels _panel);
        public static event GameOver OnGameOver;

        public static BarricadeController Instance;
        
        [Header("Поточна к-сть життів")]
        public int currentHealth = 10;

        [Header("Максимальна кількість життів")]
        [HideInInspector] public int maxHealth;

        [Header("Відображення життя")]
        public FilledBarController healthbar;

        [Header("Відображення")]
        private SpriteRenderer sRenderer;

        private List<Ico> icons = new List<Ico>();

        public void Awake()
        {
            if (Instance == null)
                Instance = this;

            sRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            DefenceShortInfo shortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetDefenceShortInfo(PersistableSO.Instance.PlayerPrefs.PlayerPrefs.selectedDefenceID);

            DUpgrade upgrade = PersistableSO.Instance.DefenceStore.GetUpgrade(shortInfo.id, shortInfo.upgradeID);

            Defense defence = PersistableSO.Instance.DefenceStore.GetDefense(shortInfo.id);

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

                OnGameOver?.Invoke(Panels.Lose);
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

            OnUpdateHealth?.Invoke((float)currentHealth / maxHealth);
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
