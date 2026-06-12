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

        private int GetUp(int id)
        {
            if (_type == ShortInfoName.Weapon)
            {
                return _progressService.GetWeaponShortInfo(id)?.UpgradeId ?? -1;
            }
            else if (_type == ShortInfoName.Defence)
            {
                return _progressService.GetDefenceShortInfo(id)?.UpgradeId ?? -1;
            }
            else if (_type == ShortInfoName.Personage)
            {
                return _progressService.GetPersonageShortInfo(id)?.UpgradeId ?? -1;
            }
            else
            {
                return -1;
            }
        }

        public void Initialisation(int id)
        {
            var upgradeId = GetUp(id);
            for (int i = 0; i <= upgradeId && i < _patrons.Count; i++)
            {
                if (_patrons[i] != null)
                    _patrons[i].sprite = _buySprite;
            }
        }
    }
}
