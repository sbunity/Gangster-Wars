using UnityEngine;

namespace SBabchuk.Runtime.UI.WeaponStore
{
    /// <summary>
    /// Marker component for the grenade store element's "locked" view. Grenades have no
    /// lock-time purchase logic (buying happens through <see cref="UnlockGElementController"/>),
    /// so this only tags the GameObject so BSElementController can toggle it via GetComponentInChildren.
    /// </summary>
    public sealed class LockGElementController : MonoBehaviour
    {
    }
}
