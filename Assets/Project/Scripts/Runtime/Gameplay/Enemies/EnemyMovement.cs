using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyMovement : MonoBehaviour
    {
        public bool IsMoving { get; private set; }
        public Transform Target => _target;
        private Transform _target;
        private EnemyView _view;
        private Tween _moveTween;
        private float _remainingMoveTime;
        private float _moveStartedAt;

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
            _moveTween = transform.DOMoveX(_target.position.x, _remainingMoveTime).SetEase(Ease.Linear);
            _view.SetAnimation(AnimationsName.Walk);
        }

        public void Stop()
        {
            _moveTween?.Kill();
            _moveTween = null;

            if (IsMoving)
                _remainingMoveTime = Mathf.Max(0f, _remainingMoveTime - (Time.time - _moveStartedAt));
            
            IsMoving = false;
        }

        public bool IsInAttackRange(float radius) 
            => _target != null && Mathf.Abs(transform.position.x - _target.position.x) < radius;
    }
}