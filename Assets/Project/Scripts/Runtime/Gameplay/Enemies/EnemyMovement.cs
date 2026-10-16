using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyMovement : MonoBehaviour
    {
        public bool IsMoving { get; private set; }
        public Vector2 TargetPoint => _targetPoint;
        private Vector2 _targetPoint;
        private EnemyView _view;
        private Tween _moveTween;
        private float _remainingMoveTime;
        private float _moveStartedAt;

        public void Initialize(EnemyView view, Vector2 targetPoint, float moveTime)
        {
            _view = view;
            _targetPoint = targetPoint;
            _remainingMoveTime = moveTime;
            IsMoving = false;
            Stop();
        }

        public void Move()
        {
            IsMoving = true;
            _moveStartedAt = Time.time;
            _moveTween = transform.DOMoveX(_targetPoint.x, _remainingMoveTime).SetEase(Ease.Linear);
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
            => Mathf.Abs(transform.position.x - _targetPoint.x) < radius;
    }
}