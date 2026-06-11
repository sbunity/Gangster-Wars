using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class UIBttnEnemyInfo : MonoBehaviour
    {
        [Header("ІД юніта")]
        public EnemiesName enemyID;

        [Header("Іконка")]
        public Image ico;

        [Header("Життя")]
        public Text health;

        [Header("Урон")]
        public Text damage;

        [Header("ВІкно із вибором кількості")]
        public GameObject windowConfirm;

        [Header("Кількість")]
        public CounterController counterController;

        private EnemyDatabase enemyDatabase;

        [Inject]
        private void Construct(IAssetProvider assetProvider)
        {
            enemyDatabase = assetProvider.EnemyDatabase;
        }

        private void Start()
        {
            SetInfo();
        }

        private void SetInfo()
        {
            Enemy enemy = enemyDatabase.GetEnemy((int)enemyID);

            ico.sprite = enemy.ico;

            health.text = "Життя: " + enemy.health.ToString();

            damage.text = "Урон: " + enemy.damage.ToString();
        }

        public void Click()
        {
            windowConfirm.SetActive(true);

            counterController.SetEnemyID((int)enemyID);
        }
    }
}
