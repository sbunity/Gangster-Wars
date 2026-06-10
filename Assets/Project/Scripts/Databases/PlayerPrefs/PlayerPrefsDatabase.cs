using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create PlayerPrefsDatabase", fileName = "PlayerPrefsDatabase")]
    public class PlayerPrefsDatabase : ScriptableObject
    {
        public delegate void Upgraded();
        public static event Upgraded OnUpgraded;
        public static event Upgraded OnChangeCoin;

        public delegate void UpdateWeaponPatrons(WeaponsName _weaponsName, int _count);
        public static event UpdateWeaponPatrons OnUpdateWeaponPatrons;

        [Header("PlayerPrefs")]
        [SerializeField, HideInInspector] public PlayerPrefs PlayerPrefs = new PlayerPrefs();

        public void SetCoin(int _value)
        {
            Debug.Log("SetCoin: " + _value);
           
            PlayerPrefs.coin += _value;

            if (PlayerPrefs.coin < 0)
                PlayerPrefs.coin = 0;

            Save();

            OnChangeCoin?.Invoke();
        }

        public bool IsMusicEnabled()
        {
            return PlayerPrefs.musik == mySwitch.On;
        }

        public bool IsSoundEnabled()
        {
            return PlayerPrefs.sound == mySwitch.On;
        }

        public void SetMusicEnabled(bool _value)
        {
            PlayerPrefs.musik = _value ? mySwitch.On : mySwitch.Off;

            Save();
        }

        public void SetSoundEnabled(bool _value)
        {
            PlayerPrefs.sound = _value ? mySwitch.On : mySwitch.Off;

            Save();
        }

        /// <summary>
        /// Можливість здійснення покупки
        /// </summary>
        /// <returns></returns>
        public bool OpportunityBuy(int price)
        {
            return (PlayerPrefs.coin >= price);
        }

        /// <summary>
        /// Купляєм зброю
        /// </summary>
        public void BuyWeapon(int _id)
        {
            WeaponShortInfo weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(_id);

            Weapon _weapon = WeaponStoreDatabase.GetDatabase().GetWeapon(_id);

            weaponShortInfo.isBuy = mySwitch.On;

            SetCoin(-_weapon.price);

            Save();

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Перевірка Апгрейда
        /// </summary>
        /// <param name="_settingsUpgrade"></param>
        /// <returns></returns>
        public StateUpgrade CheckUpgradeWeapon(SettingsUpgradeWeapon _settingsUpgrade)
        {
            WeaponShortInfo _shortInfo = PlayerPrefs.GetWeaponShortInfo(_settingsUpgrade.weaponID);

            StateUpgrade state = StateUpgrade.None;

            if (_shortInfo != null)
            {
                if (_shortInfo.upgradeID == -1 || _shortInfo.upgradeID < _settingsUpgrade.upgradeID)
                {
                    state = StateUpgrade.Activated;
                }
                else
                {
                    state = StateUpgrade.Completed;
                }
            }
            else
            {
                Debug.Log("Не знайдено зброї");
            }

            return state;
        }

        /// <summary>
        /// Апгрейд зброї
        /// </summary>
        /// <param name="_settingsUpgrade"></param>
        public void UpgradeWeapon(SettingsUpgradeWeapon _settingsUpgrade)
        {
            WeaponShortInfo _weapon = PlayerPrefs.GetWeaponShortInfo(_settingsUpgrade.weaponID);

            if (_weapon != null)
            {
                if (_settingsUpgrade.upgradeID != -1)
                {
                    _weapon.id = _settingsUpgrade.upgradeID;

                    OnUpgraded?.Invoke();
                }
            }
            else
            {
                Debug.Log("Не знайдено посоха");
            }

            Save();
        }

        /// <summary>
        /// Вибираєм зброю
        /// </summary>
        public void SelectWeapon(int _id)
        {
            PlayerPrefs.selectedWeaponID = _id;

            Save();
        }

        /// <summary>
        /// Купляєм патрони
        /// </summary>
        public void BuyMagazine(int _id, bool _isFree = false)
        {
            WeaponShortInfo weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(_id);

            Weapon _weapon = WeaponStoreDatabase.GetDatabase().GetWeapon(_id);
           
            weaponShortInfo.countPatrons += _weapon.magazine;

            if (!_isFree)
                SetCoin(-_weapon.priceMagazine);

            Save();

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Купляєм патрони
        /// </summary>
        public void BuyUpgrade(int _id)
        {
            WeaponShortInfo weaponShortInfo = PlayerPrefs.GetWeaponShortInfo(_id);

            Weapon _weapon = WeaponStoreDatabase.GetDatabase().GetWeapon(_id);

            if (weaponShortInfo.upgradeID < _weapon.upgrades.Count - 1)
            {
                weaponShortInfo.upgradeID++;

                SetCoin(-WeaponStoreDatabase.GetDatabase().GetUpgrade(_id, weaponShortInfo.upgradeID).price);

                Save();

                OnUpgraded?.Invoke();
            }
        }

        /// <summary>
        /// Купляєм зброю
        /// </summary>
        public void BuyGrenade(int _id, bool _isFree = false)
        {
            GrenadeShortInfo _grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(_id);

            Grenade _grenade = BombStoreDatabase.GetDatabase().GetGrenade(_id);

            if (_grenadeShortInfo.isBuy == mySwitch.On)
            {
                if(!_isFree)
                    SetCoin(-_grenade.price);

                _grenadeShortInfo.count++;

                Save();
            }

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Вткористати гранату
        /// </summary>
        public void UseGrenade(int _id)
        {
            GrenadeShortInfo _grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(_id);

            //if (_grenadeShortInfo.isBuy == mySwitch.On)
            {
                _grenadeShortInfo.count--;

                Save();
            }

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Знайшли гранати
        /// </summary>
        public void FindGrenade(int _id, int _count = 1)
        {
            GrenadeShortInfo _grenadeShortInfo = PlayerPrefs.GetGrenadeShortInfo(_id);

            if (_grenadeShortInfo.isBuy == mySwitch.On)
            {
                _grenadeShortInfo.count += _count;

                Save();
            }

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Купляєм зброю
        /// </summary>
        public void BuyDefence(int _id)
        {
            DefenceShortInfo _shortInfo = PlayerPrefs.GetDefenceShortInfo(_id);

            Defense _defence = DefenseStoreDatabase.GetDatabase().GetDefense(_id);

            _shortInfo.isBuy = mySwitch.On;

            SetCoin(-_defence.price);

            Save();

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Вибираєм перепону
        /// </summary>
        public void SelectDefence(int _id)
        {
            PlayerPrefs.selectedDefenceID = _id;

            Save();

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Купляєм патрони
        /// </summary>
        public void BuyUpgradeDefence(int _id)
        {
            DefenceShortInfo _shortInfo = PlayerPrefs.GetDefenceShortInfo(_id);

            Defense _defence = DefenseStoreDatabase.GetDatabase().GetDefense(_id);

            if (_shortInfo.upgradeID < _defence.upgrades.Count - 1)
            {
                _shortInfo.upgradeID++;

                SetCoin(-DefenseStoreDatabase.GetDatabase().GetUpgrade(_id, _shortInfo.upgradeID).price);

                Save();

                OnUpgraded?.Invoke();
            }
        }

        /// <summary>
        /// Купляєм зброю
        /// </summary>
        public void BuyPersonage(int _id)
        {
            PersonageShortInfo _shortInfo = PlayerPrefs.GetPersonageShortInfo(_id);

            Personage _personage = MainPlayerDatabase.GetDatabase().GetPersonage(_id);

            _shortInfo.isBuy = mySwitch.On;

            SetCoin(-_personage.price);

            Save();

            OnUpgraded?.Invoke();
        }

        /// <summary>
        /// Купляєм патрони
        /// </summary>
        public void BuyUpgradePersonage(int _id)
        {
            PersonageShortInfo personageShortInfo = PlayerPrefs.GetPersonageShortInfo(_id);

            Personage _personage = MainPlayerDatabase.GetDatabase().GetPersonage(_id);

            if (personageShortInfo.upgradeID < _personage.upgrades.Count - 1)
            {
                personageShortInfo.upgradeID++;

                SetCoin(-MainPlayerDatabase.GetDatabase().GetUpgrade(_id, personageShortInfo.upgradeID).price);

                Save();

                OnUpgraded?.Invoke();
            }
        }

        /// <summary>
        /// Добавляєм патрони для певної зброї
        /// </summary>
        /// <param name="_weaponsName"></param>
        /// <returns></returns>
        public void SetPatrons(WeaponsName _weaponsName, int _value)
        {
            WeaponShortInfo _weaponShortInfo = PlayerPrefs.GetWeaponShortInfo((int)_weaponsName);

            _weaponShortInfo.countPatrons += _value;

            OnUpdateWeaponPatrons?.Invoke(_weaponsName, _weaponShortInfo.countPatrons);

            if (_weaponShortInfo.countPatrons == 0)
                OnUpgraded?.Invoke();

            Save();
        }

        /// <summary>
        /// Зберігаєм ід рівня
        /// </summary>
        /// <param name="_id"></param>
        public void SetLevel(int _id = 0)
        {
            PlayerPrefs.levelID = _id;

            Save();
        }

        /// <summary>
        /// Встановлюєм, що ми рівень пройшли
        /// </summary>
        public void SetLevelCompleted(float _value)
        {
            LevelShortInfo levelShortInfo = PlayerPrefs.GetLevelShortInfo(PlayerPrefs.levelID);

            if (levelShortInfo.isCompleted != mySwitch.On)
            {
                levelShortInfo.isCompleted = mySwitch.On;

                Save();
            }

            SetStars(levelShortInfo, _value);
        }

        /// <summary>
        /// Встановлюєм кількість зірочок
        /// </summary>
        /// <param name="_levelShortInfo"></param>
        /// <param name="_value"></param>
        public void SetStars(LevelShortInfo _levelShortInfo, float _value)
        {
            if (_value >= 0.75f)
            {
                _levelShortInfo.stars = Mathf.Max(3, _levelShortInfo.stars);
            }
            else if(_value >= 0.50f)
            {
                _levelShortInfo.stars = Mathf.Max(2, _levelShortInfo.stars);
            }
            else
            {
                _levelShortInfo.stars = Mathf.Max(1, _levelShortInfo.stars);
            }
        }

        /// <summary>
        /// Отримання бази даних
        /// </summary>
        /// <returns></returns>
        public static PlayerPrefsDatabase GetDatabase()
        {
            #if UNITY_EDITOR
            return Utils.GetAsset<PlayerPrefsDatabase>();
            #endif

            #if UNITY_ANDROID || UNITY_IPHONE
            return Utils.GetAsset2<PlayerPrefsDatabase>();
            #endif
        }

        /// <summary>
        /// Зберегти
        /// </summary>
        public void Save()
        {
            SaveSO(this);
        }

        public void SaveSO(ScriptableObject _objectsToPersist)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}_{1}.pso", "Main", _objectsToPersist.name));
            var json = JsonUtility.ToJson(_objectsToPersist);
            bf.Serialize(file, json);
            file.Close();
        }
    }

    [System.Serializable]
    public class PlayerPrefs
    {
        [Header("Чи включена музика")]
        public mySwitch musik;

        [Header("Чи включені звуки")]
        public mySwitch sound;

        [Header("К-сть монеток")]
        public int coin;

        [Header("Рівень на якому зупинились")]
        public int levelID;

        [Header("ID зброї в руках стрілка")]
        public int selectedWeaponID = 0;

        [Header("ID гранат в руках підривника")]
        public int selectedGrenadeID = 0;

        [Header("ID перепони, що встановлена")]
        public int selectedDefenceID = 0;

        [Header("Iнформація про зброю")]
        public List<WeaponShortInfo> weapons = new List<WeaponShortInfo>();

        [Header("Iнформація про зброю")]
        public List<GrenadeShortInfo> grenades = new List<GrenadeShortInfo>();

        [Header("Iнформація про перепони")]
        public List<DefenceShortInfo> defences = new List<DefenceShortInfo>();

        [Header("Iнформація про перепони")]
        public List<PersonageShortInfo> personages = new List<PersonageShortInfo>();

        [Header("Iнформація про рівні")]
        public List<LevelShortInfo> levels = new List<LevelShortInfo>();

        /// <summary>
        /// Конструктор
        /// </summary>
		public PlayerPrefs()
        {
            Debug.Log("Створюєм");
        }

        /// <summary>
        /// Повертаєм короткий опис зброї
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public WeaponShortInfo GetWeaponShortInfo(int _id)
        {
            foreach (WeaponShortInfo weapon in weapons)
            {
                if (weapon.id == _id)
                {
                    return weapon;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертаєм короткий опис зброї
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public GrenadeShortInfo GetGrenadeShortInfo(int _id)
        {
            foreach (GrenadeShortInfo grenade in grenades)
            {
                if (grenade.id == _id)
                {
                    return grenade;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертаєм короткий опис зброї
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public DefenceShortInfo GetDefenceShortInfo(int _id)
        {
            foreach (DefenceShortInfo defence in defences)
            {
                if (defence.id == _id)
                {
                    return defence;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертаєм короткий опис персонажа
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public PersonageShortInfo GetPersonageShortInfo(int _id)
        {
            foreach (PersonageShortInfo personage in personages)
            {
                if (personage.id == _id)
                {
                    return personage;
                }
            }

            return null;
        }

        /// <summary>
        /// Повертаєм короткий опис рівня
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public LevelShortInfo GetLevelShortInfo(int _id)
        {
            foreach (LevelShortInfo _info in levels)
            {
                if (_info.id == _id)
                {
                    return _info;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class WeaponShortInfo
    {
        [Header("Ідентифікатор")]
        public int id;

        [Header("Найменування")]
        public string name;

        [Header("Чи куплений")]
        public mySwitch isBuy;

        [Header("Ідентифікатор апгрейда")]
        public int upgradeID;

        [Header("Кількість патронів які знайшли")]
        public int countPatrons;

        public WeaponShortInfo(Weapon _weapon)
        {
            this.id = _weapon.id;
            this.name = _weapon.name;
            this.isBuy = mySwitch.Off;
            this.upgradeID = -1;
        }
    }

    [System.Serializable]
    public class GrenadeShortInfo
    {
        [Header("Ідентифікатор")]
        public int id;

        [Header("Чи гранати доступні для покупок")]
        public mySwitch isBuy;

        [Header("Кількість гранат цього типу")]
        public int count;

        public GrenadeShortInfo(Grenade _value)
        {
            this.id = _value.id;
            this.isBuy = mySwitch.Off;
            this.count = 0;
        }
    }

    [System.Serializable]
    public class DefenceShortInfo
    {
        [Header("Ідентифікатор")]
        public int id;

        [Header("Чи перепони доступні для покупок")]
        public mySwitch isBuy;

        public int upgradeID;

        public DefenceShortInfo(Defense _value)
        {
            this.id = _value.id;
            this.isBuy = mySwitch.Off;
            this.upgradeID = -1;
        }
    }

    [System.Serializable]
    public class PersonageShortInfo
    {
        [Header("Ідентифікатор")]
        public int id;

        [Header("Чи перепони доступні для покупок")]
        public mySwitch isBuy;

        [Header("Ідентифікатор апгрейда")]
        public int upgradeID;

        public PersonageShortInfo(Personage _value)
        {
            this.id = _value.id;
            this.isBuy = mySwitch.Off;
        }
    }

    [System.Serializable]
    public class LevelShortInfo
    {
        [Header("Ідентифікатор")]
        public int id;

        [Header("Чи рівень пройдений")]
        public mySwitch isCompleted;

        [Header("Кількість зірок(успішність проходження)")]
        public int stars;

        public LevelShortInfo(Level _value)
        {
            this.id = _value.id;
            this.isCompleted = mySwitch.Off;
            this.stars = 0;
        }
    }
}
