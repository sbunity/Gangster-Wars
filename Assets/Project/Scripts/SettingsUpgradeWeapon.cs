using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SettingsUpgradeWeapon
{
    [SerializeField, FormerlySerializedAs("weaponID")]
    private int _weaponId = -1;

    [SerializeField, FormerlySerializedAs("upgradeID")]
    private int _upgradeId = -1;
}
