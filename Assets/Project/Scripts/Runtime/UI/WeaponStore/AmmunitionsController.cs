using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SBabchuk
{
    public class AmmunitionsController : MonoBehaviour
    {
        public ShortInfoName type;

        public List<Image> patrons;

        public Sprite buySprite;
        private IPlayerProgressService _progressService;

        [Inject]
        private void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private int GetUp(int _id)
        {
            if (type == ShortInfoName.Weapon)
            {
                return _progressService.GetWeaponShortInfo(_id).upgradeID;
            }
            else if (type == ShortInfoName.Defence)
            {
                return _progressService.GetDefenceShortInfo(_id).upgradeID;
            }
            else if (type == ShortInfoName.Personage)
            {
                return _progressService.GetPersonageShortInfo(_id).upgradeID;
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
