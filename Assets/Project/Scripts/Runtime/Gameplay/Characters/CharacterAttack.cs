using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public sealed class CharacterAttack : MonoBehaviour
    {
        private Tween _attackTween;

        public bool IsAttacking { get; private set; }

        public void StartAttack(CharacterView view, AnimationsName animation)
        {
            IsAttacking = true;
            view.SetAnimation(animation);
        }

        public void Stop()
        {
            IsAttacking = false;
            if (_attackTween != null)
            {
                _attackTween.Kill();
                _attackTween = null;
            }
        }
    }
}
