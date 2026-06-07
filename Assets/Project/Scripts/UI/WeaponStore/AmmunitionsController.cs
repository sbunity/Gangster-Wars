using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SBabchuk
{
    public class AmmunitionsController : MonoBehaviour
    {
        public ShortInfoName type;

        public List<Image> patrons;

        public Sprite buySprite;

        private int GetUp(int _id)
        {
            if (type == ShortInfoName.Weapon)
            {
                return PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetWeaponShortInfo(_id).upgradeID;
            }
            else if (type == ShortInfoName.Defence)
            {
                return PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetDefenceShortInfo(_id).upgradeID;
            }
            else if (type == ShortInfoName.Personage)
            {
                return PersistableSO.Instance.PlayerPrefs.PlayerPrefs.GetPersonageShortInfo(_id).upgradeID;
            }
            else
            {
                return -1;
            }
        }

        public void Initialisation(int _id)
        {
            for (int i = 0; i <= GetUp(_id); i++)
            {
                patrons[i].sprite = buySprite;
            }
        }
    }
}
