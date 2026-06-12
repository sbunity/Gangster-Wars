using UnityEngine;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;
using SBabchuk.Runtime.Databases.Enemies;

namespace SBabchuk.Runtime.Gameplay.Characters
{
    public class SightController : MonoBehaviour, IAimService
    {
        [SerializeField, Range(-2, 2)] private float offsetY;
        [SerializeField, Range(-180, 180)] private float minForwardAngle = -70f;
        [SerializeField, Range(-180, 180)] private float maxForwardAngle = 70f;
        [SerializeField] private bool clampAimToForwardSector = true;
        [SerializeField] private Transform forwardSectorOrigin;
        [SerializeField, Range(0f, 5f)] private float minAimDistance = 1.5f;

        private bool _isTouchingFireArea;
        private ILeaderWeaponController _leaderWeaponController;
        private IHandService _handService;
        public Vector2 CurrentAimPosition { get; private set; }

        [Inject]
        public void Construct(ILeaderWeaponController leaderWeaponController, IHandService handService)
        {
            _leaderWeaponController = leaderWeaponController;
            _handService = handService;
        }

        private void Awake()
        {
            UpdateAimPosition(transform.position);
        }

        private void OnEnable()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        private void OnDisable()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        public virtual void OnTouchMove(Gesture gesture)
        {
            if (_handService != null && _handService.IsHoldingGrenade)
            {
                HoldAimForward();
                return;
            }

            var pos = gesture.GetTouchToWorldPoint(9f);
            var targetPosition = new Vector3(pos.x, pos.y + offsetY, 0);
            var canShoot = IsTargetInForwardSector(targetPosition);
            if (canShoot || clampAimToForwardSector)
            {
                UpdateAimPosition(canShoot ? ClampToMinAimDistance(targetPosition) : ClampToForwardSector(targetPosition));
            }

            if (!canShoot && _leaderWeaponController != null)
            {
                _leaderWeaponController.StopAttack();
            }
            else if (canShoot && _isTouchingFireArea && _leaderWeaponController != null && !_leaderWeaponController.IsAttacking)
            {
                _leaderWeaponController.Attack();
            }
        }

        public void OnTouchDown(Gesture gesture)
        {
            if (_handService != null && _handService.IsHoldingGrenade)
            {
                HoldAimForward();
                return;
            }

            _isTouchingFireArea = false;
            if (gesture.pickedObject)
            {
                if (gesture.pickedObject.CompareTag("FireZone") || gesture.pickedObject.CompareTag("Place") || gesture.pickedObject.CompareTag("Enemy"))
                {
                    _isTouchingFireArea = true;
                    var pos = gesture.GetTouchToWorldPoint(9f);
                    var targetPosition = new Vector3(pos.x, pos.y + offsetY, 0);
                    var canShoot = IsTargetInForwardSector(targetPosition);
                    if (canShoot || clampAimToForwardSector)
                    {
                        UpdateAimPosition(canShoot ? ClampToMinAimDistance(targetPosition) : ClampToForwardSector(targetPosition));
                    }

                    if (_leaderWeaponController != null && canShoot)
                    {
                        _leaderWeaponController.Attack();
                    }
                    else
                    {
                        _leaderWeaponController?.StopAttack();
                    }
                }
            }
        }

        public void OnTouchUp(Gesture gesture)
        {
            _isTouchingFireArea = false;
            _leaderWeaponController?.StopAttack();
        }

        private bool IsTargetInForwardSector(Vector3 targetPosition)
        {
            var direction = targetPosition - GetForwardSectorOrigin();
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return true;

            var touchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return IsAngleInForwardSector(touchAngle);
        }

        private Vector3 ClampToForwardSector(Vector3 targetPosition)
        {
            var origin = GetForwardSectorOrigin();
            var direction = targetPosition - origin;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                direction = Vector2.right;

            var distance = Mathf.Max(direction.magnitude, minAimDistance);
            var touchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var clampedAngle = ClampAngleToForwardSector(touchAngle);
            var angleRad = clampedAngle * Mathf.Deg2Rad;

            return origin + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * distance;
        }

        private Vector3 ClampToMinAimDistance(Vector3 targetPosition)
        {
            var origin = GetForwardSectorOrigin();
            var direction = targetPosition - origin;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
                direction = Vector2.right;

            var distance = Mathf.Max(direction.magnitude, minAimDistance);

            return origin + (Vector3)direction.normalized * distance;
        }

        private Vector3 GetForwardSectorOrigin()
        {
            if (forwardSectorOrigin)
                return forwardSectorOrigin.position;

            if (_leaderWeaponController != null)
                return _leaderWeaponController.GetAimOrigin();

            return transform.position;
        }

        private void HoldAimForward()
        {
            _isTouchingFireArea = false;
            _leaderWeaponController?.StopAttack();

            UpdateAimPosition(GetForwardSectorOrigin() + Vector3.right * minAimDistance);
        }

        private void UpdateAimPosition(Vector3 aimPosition)
        {
            transform.position = aimPosition;
            CurrentAimPosition = new Vector2(aimPosition.x, aimPosition.y - offsetY);
        }

        private bool IsAngleInForwardSector(float angle)
        {
            angle = NormalizeAngle(angle);
            var minAngle = NormalizeAngle(minForwardAngle);
            var maxAngle = NormalizeAngle(maxForwardAngle);
            if (minAngle <= maxAngle)
                return angle >= minAngle && angle <= maxAngle;

            return angle >= minAngle || angle <= maxAngle;
        }

        private float ClampAngleToForwardSector(float angle)
        {
            if (IsAngleInForwardSector(angle))
                return NormalizeAngle(angle);

            var minAngle = NormalizeAngle(minForwardAngle);
            var maxAngle = NormalizeAngle(maxForwardAngle);
            var minDelta = Mathf.Abs(Mathf.DeltaAngle(angle, minAngle));
            var maxDelta = Mathf.Abs(Mathf.DeltaAngle(angle, maxAngle));
            return minDelta <= maxDelta ? minAngle : maxAngle;
        }

        private float NormalizeAngle(float angle)
        {
            return Mathf.Repeat(angle + 180f, 360f) - 180f;
        }
    }
}
