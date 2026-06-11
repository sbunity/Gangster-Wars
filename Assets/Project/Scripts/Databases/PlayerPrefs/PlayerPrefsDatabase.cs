using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk
{
    [CreateAssetMenu(menuName = "Databases/Create PlayerPrefsDatabase", fileName = "PlayerPrefsDatabase")]
    public class PlayerPrefsDatabase : ScriptableObject
    {
        [Header("PlayerPrefs")]
        [SerializeField, HideInInspector] public PlayerPrefs PlayerPrefs = new PlayerPrefs();

        public bool IsMusicEnabled()
        {
            return PlayerPrefs.musik == mySwitch.On;
        }

        public bool IsSoundEnabled()
        {
            return PlayerPrefs.sound == mySwitch.On;
        }

        public bool OpportunityBuy(int price)
        {
            return PlayerPrefs.coin >= price;
        }

        public void SetStars(LevelShortInfo levelShortInfo, float value)
        {
            if (value >= 0.75f)
                levelShortInfo.stars = Mathf.Max(3, levelShortInfo.stars);
            else if (value >= 0.50f)
                levelShortInfo.stars = Mathf.Max(2, levelShortInfo.stars);
            else
                levelShortInfo.stars = Mathf.Max(1, levelShortInfo.stars);
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

        public WeaponShortInfo GetWeaponShortInfo(int id)
        {
            foreach (var weapon in weapons)
            {
                if (weapon.id == id)
                    return weapon;
            }

            return null;
        }

        public GrenadeShortInfo GetGrenadeShortInfo(int id)
        {
            foreach (var grenade in grenades)
            {
                if (grenade.id == id)
                    return grenade;
            }

            return null;
        }

        public DefenceShortInfo GetDefenceShortInfo(int id)
        {
            foreach (var defence in defences)
            {
                if (defence.id == id)
                    return defence;
            }

            return null;
        }

        public PersonageShortInfo GetPersonageShortInfo(int id)
        {
            foreach (var personage in personages)
            {
                if (personage.id == id)
                    return personage;
            }

            return null;
        }

        public LevelShortInfo GetLevelShortInfo(int id)
        {
            foreach (var info in levels)
            {
                if (info.id == id)
                    return info;
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

        public WeaponShortInfo(Weapon weapon)
        {
            id = weapon.id;
            name = weapon.name;
            isBuy = mySwitch.Off;
            upgradeID = -1;
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

        public GrenadeShortInfo(Grenade value)
        {
            id = value.id;
            isBuy = mySwitch.Off;
            count = 0;
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

        public DefenceShortInfo(Defense value)
        {
            id = value.id;
            isBuy = mySwitch.Off;
            upgradeID = -1;
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

        public PersonageShortInfo(Personage value)
        {
            id = value.id;
            isBuy = mySwitch.Off;
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

        public LevelShortInfo(Level value)
        {
            id = value.id;
            isCompleted = mySwitch.Off;
            stars = 0;
        }
    }
}
