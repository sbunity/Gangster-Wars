using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyDeath : MonoBehaviour
    {
        public void Play(EnemyView view)
        {
            view.SetColliderEnabled(false);
            view.SetAnimation(AnimationsName.Death);
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
        }
    }
}
