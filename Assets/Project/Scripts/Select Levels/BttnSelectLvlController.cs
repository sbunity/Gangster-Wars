using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SBabchuk
{
    public class BttnSelectLvlController : MonoBehaviour
    {
        public delegate void Initialized(int _id, mySwitch _isCompleted);
        public static event Initialized OnInitialized;

        [Header("За який рівень відповідає")]
        public LevelsName level;

        [Header("Чи пройдений")]
        public mySwitch isCompleted;
 
        [Header("Назва рівня")]
        public Text tittle;

        [Header("Кaртинка заблокованого рiвня")]
        public GameObject lockImg;

        [Header("Кількість зірочок")]
        public Image stars;

        [Header("Кількість зірочок")]
        public List<Sprite> starsSprite;

        [Header("Залешності")]
        public List<BttnSelectLvlController> predecessors = new List<BttnSelectLvlController>();

        /// <summary>
        /// База даних рівнів
        /// </summary>
        private LevelDatabase database;

        /// <summary>
        /// Властивості рівня
        /// </summary>
        private Level properties;

        /// <summary>
        /// Чи рівень закритий
        /// </summary>
        private bool isLock = true;

        private void OnEnable()
        {
            BttnSelectLvlController.OnInitialized += CheckPosterity;
        }

        private void OnDisable()
        {
            BttnSelectLvlController.OnInitialized -= CheckPosterity;
        }

        /// <summary>
        /// Стартова ініціалізація
        /// </summary>
        private void Start()
        {
            database = PersistableSO.Instance.Level;

            Init(level);
        }

        /// <summary>
        /// Витягуєм з бази властивості левела
        /// </summary>
        /// <param name="_id">Identifier.</param>
        public void Init(LevelsName _level)
        {
            properties = database.GetLevel((int)_level);

            tittle.text = properties.name;

            LevelShortInfo levelShortInfo = PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetLevelShortInfo((int)_level);

            isCompleted = levelShortInfo.isCompleted;

            OnInitialized?.Invoke((int)_level, isCompleted);

            stars.sprite = GetSpriteStar(levelShortInfo.stars);
        }

        public Sprite GetSpriteStar(int _value)
        {
            return starsSprite[_value];
        }

        public void CheckPosterity(int _id, mySwitch _mySwitch)
        {
            if (isLock)
            {
                if (predecessors.Count != 0)
                {
                    foreach (BttnSelectLvlController bttnSelectLvlController in predecessors)
                    {
                        if ((int)bttnSelectLvlController.level == _id)
                        {
                            isLock = !(_mySwitch == mySwitch.On);
                        }
                    }
                }
                else
                {
                    isLock = false;
                }
            }

            lockImg.SetActive(isLock);

            stars.enabled = !lockImg.activeSelf;
        }

        /// <summary>
        /// Старт рівня
        /// </summary>
        public void StartLevel()
        {
            if (!lockImg.activeSelf)
            {
                PersistableSO.Instance.PlayerPrefs.SetLevel((int)level);

                SceneManager.LoadScene(Scene.GameScene.ToString());
            }
        }
    }
}
