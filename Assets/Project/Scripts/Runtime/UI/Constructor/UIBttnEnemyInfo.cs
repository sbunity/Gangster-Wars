using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class UIBttnEnemyInfo : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("enemyID")]
        private EnemiesName _enemyId;

        [SerializeField, FormerlySerializedAs("ico")]
        private Image _icon;

        [SerializeField, FormerlySerializedAs("health")]
        private Text _health;

        [SerializeField, FormerlySerializedAs("damage")]
        private Text _damage;

        [SerializeField, FormerlySerializedAs("windowConfirm")]
        private GameObject _windowConfirm;

        [SerializeField, FormerlySerializedAs("counterController")]
        private CounterController _counterController;

        private EnemyDatabase _enemyDatabase;
        
        [Inject]
        public void Construct(IAssetProvider assetProvider)
        {
            _enemyDatabase = assetProvider.EnemyDatabase;
        }

        private void Start()
        {
            SetInfo();
        }

        private void SetInfo()
        {
            Enemy enemy = _enemyDatabase.GetEnemy((int)_enemyId);
            _icon.sprite = enemy.Icon;
            _health.text = "Р–РёС‚С‚СЏ: " + enemy.Health.ToString();
            _damage.text = "РЈСЂРѕРЅ: " + enemy.Damage.ToString();
        }

        public void Click()
        {
            _windowConfirm.SetActive(true);
            _counterController.SetEnemyID((int)_enemyId);
        }
    }
}
