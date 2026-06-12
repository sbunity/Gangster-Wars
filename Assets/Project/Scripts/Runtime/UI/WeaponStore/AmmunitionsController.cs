using System.Collections;
using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Serialization;
using SBabchuk.Runtime.Databases.MainPlayers;
using SBabchuk.Runtime.Databases.WeaponStore;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    public class AmmunitionsController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("type")]
        private ShortInfoName _type;

        [SerializeField, FormerlySerializedAs("patrons")]
        private List<Image> _patrons;

        [SerializeField, FormerlySerializedAs("buySprite")]
        private Sprite _buySprite;

        private IPlayerProgressService _progressService;
        
        [Inject]
        public void Construct(IPlayerProgressService progressService)
        {
            _progressService = progressService;
        }

        private int GetUp(int _id)
        {
            if (_type == ShortInfoName.Weapon)
            {
                return _progressService.GetWeaponShortInfo(_id).UpgradeId;
            }
            else if (_type == ShortInfoName.Defence)
            {
                return _progressService.GetDefenceShortInfo(_id).UpgradeId;
            }
            else if (_type == ShortInfoName.Personage)
            {
                return _progressService.GetPersonageShortInfo(_id).UpgradeId;
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
                _patrons[i].sprite = _buySprite;
            }
        }
    }
}
