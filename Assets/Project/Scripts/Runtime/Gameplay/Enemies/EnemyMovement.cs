using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyMovement : MonoBehaviour
    {
        private Transform _target;
        private EnemyView _view;
        private Tween _moveTween;
        private float _remainingMoveTime;
        private float _moveStartedAt;

        public bool IsMoving { get; private set; }
        public Transform Target => _target;

        public void Initialize(EnemyView view, Transform target, float moveTime)
        {
            _view = view;
            _target = target;
            _remainingMoveTime = moveTime;
            IsMoving = false;
            Stop();
        }

        public void Move()
        {
            if (_target == null)
                return;

            IsMoving = true;
            _moveStartedAt = Time.time;
            _moveTween = transform.DOMoveX(_target.position.x, _remainingMoveTime)
                .SetEase(Ease.Linear);
            _view.SetAnimation(AnimationsName.Walk);
        }

        public void Stop()
        {
            if (_moveTween != null)
            {
                _moveTween.Kill();
                _moveTween = null;
            }

            if (IsMoving)
                _remainingMoveTime = Mathf.Max(0f, _remainingMoveTime - (Time.time - _moveStartedAt));

            IsMoving = false;
        }

        public bool IsInAttackRange(float radius)
        {
            return _target != null && Mathf.Abs(transform.position.x - _target.position.x) < radius;
        }
    }
}
