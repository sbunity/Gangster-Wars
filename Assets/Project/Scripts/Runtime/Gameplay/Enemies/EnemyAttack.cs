using System;
using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyAttack : MonoBehaviour
    {
        public bool IsAttacking { get; private set; }

        private EnemyView _view;
        private Tween _attackTween;
        private bool _isDead;

        public void Initialize(EnemyView view)
        {
            _view = view;
            _isDead = false;
            IsAttacking = false;
            Stop();
        }

        public void MarkDead()
        {
            _isDead = true;
            Stop();
        }

        public void Attack()
        {
            if (_isDead)
                return;

            IsAttacking = true;
            _view.SetAnimation(AnimationsName.Attack);
        }

        public void WaitNext(float delay, Action attack)
        {
            IsAttacking = false;
            Stop();

            _attackTween = DOVirtual.DelayedCall(delay, () =>
            {
                if (!_isDead)
                    attack?.Invoke();
            }, false).OnStart(() =>
            {
                if (!_isDead)
                    _view.SetAnimation(AnimationsName.Idle);
            });
        }

        public void Stop()
        {
            _attackTween?.Kill();
            _attackTween = null;
        }
    }
}