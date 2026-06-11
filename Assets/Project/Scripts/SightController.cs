using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBabchuk.Runtime.Services.Contracts;
using Zenject;

namespace SBabchuk
{
    public class SightController : MonoBehaviour, IAimService
    {
        [Header("Зміщення по У")]
        [Range(-2, 2)]
        public float offsetY;

        [Header("Сектор стрільби")]
        [Range(-180, 180)]
        public float minForwardAngle = -70f;

        [Range(-180, 180)]
        public float maxForwardAngle = 70f;

        [SerializeField]
        private bool clampAimToForwardSector = true;

        [Header("Точка повороту")]
        [SerializeField]
        private Transform forwardSectorOrigin;

        [SerializeField, Range(0f, 5f)]
        private float minAimDistance = 1.5f;

        private bool isTouchingFireArea;
        private ILeaderWeaponController _leaderWeaponController;
        private IHandService _handService;
        public Vector2 CurrentAimPosition { get; private set; }

        [Inject]
        private void Construct(
            ILeaderWeaponController leaderWeaponController,
            IHandService handService)
        {
            _leaderWeaponController = leaderWeaponController;
            _handService = handService;
        }

        /// <summary>
        /// Awake
        /// </summary>
        void Awake()
        {
            UpdateAimPosition(transform.position);
        }

        /// <summary>
        /// Підписались на Енейбл
        /// </summary>
        public void OnEnable()
        {
            EasyTouch.On_TouchStart += OnTouchDown;
            EasyTouch.On_TouchDown += OnTouchMove;
            EasyTouch.On_TouchUp += OnTouchUp;
        }

        /// <summary>
        /// Підписались на Дізкйбл
        /// </summary>
        public void OnDisable()
        {
            EasyTouch.On_TouchStart -= OnTouchDown;
            EasyTouch.On_TouchDown -= OnTouchMove;
            EasyTouch.On_TouchUp -= OnTouchUp;
        }

        /// <summary>
        /// Рухаєм тачом
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        virtual public void OnTouchMove(Gesture gesture)
        {
            if (_handService != null && _handService.IsHoldingGrenade)
            {
                HoldAimForward();
                return;
            }

            Vector3 pos = gesture.GetTouchToWorldPoint(9f);
            Vector3 targetPosition = new Vector3(pos.x, pos.y + offsetY, 0);
            bool canShoot = IsTargetInForwardSector(targetPosition);

            if (canShoot || clampAimToForwardSector)
            {
                UpdateAimPosition(canShoot ? ClampToMinAimDistance(targetPosition) : ClampToForwardSector(targetPosition));
            }

            if (!canShoot && _leaderWeaponController != null)
            {
                _leaderWeaponController.StopAttack();
            }
            else if (canShoot && isTouchingFireArea && _leaderWeaponController != null && !_leaderWeaponController.IsAttacking)
            {
                _leaderWeaponController.Attack();
            }
        }

        /// <summary>
        /// Нажаття на екрані
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        public void OnTouchDown(Gesture gesture)
        {
            if (_handService != null && _handService.IsHoldingGrenade)
            {
                HoldAimForward();
                return;
            }

            isTouchingFireArea = false;

            if (gesture.pickedObject)
            {
                if (gesture.pickedObject.tag == "FireZone" || gesture.pickedObject.tag == "Place" || gesture.pickedObject.tag == "Enemy")
                {
                    isTouchingFireArea = true;

                    Vector3 pos = gesture.GetTouchToWorldPoint(9f);
                    Vector3 targetPosition = new Vector3(pos.x, pos.y + offsetY, 0);
                    bool canShoot = IsTargetInForwardSector(targetPosition);

                    if (canShoot || clampAimToForwardSector)
                    {
                        UpdateAimPosition(canShoot ? ClampToMinAimDistance(targetPosition) : ClampToForwardSector(targetPosition));
                    }

                    if (_leaderWeaponController != null && canShoot)
                    {
                        _leaderWeaponController.Attack();
                    }
                    else if (_leaderWeaponController != null)
                    {
                        _leaderWeaponController.StopAttack();
                    }
                }
            }
        }

        /// <summary>
        /// Віджаття
        /// </summary>
        public void OnTouchUp(Gesture gesture)
        {
            isTouchingFireArea = false;

            if (_leaderWeaponController != null)
            {
                _leaderWeaponController.StopAttack();
            }
        }

        private bool IsTargetInForwardSector(Vector3 targetPosition)
        {
            Vector2 direction = targetPosition - GetForwardSectorOrigin();

            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return true;

            float touchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return IsAngleInForwardSector(touchAngle);
        }

        private Vector3 ClampToForwardSector(Vector3 targetPosition)
        {
            Vector3 origin = GetForwardSectorOrigin();
            Vector2 direction = targetPosition - origin;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
                direction = Vector2.right;

            float distance = Mathf.Max(direction.magnitude, minAimDistance);
            float touchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float clampedAngle = ClampAngleToForwardSector(touchAngle);
            float angleRad = clampedAngle * Mathf.Deg2Rad;

            return origin + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * distance;
        }

        private Vector3 ClampToMinAimDistance(Vector3 targetPosition)
        {
            Vector3 origin = GetForwardSectorOrigin();
            Vector2 direction = targetPosition - origin;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
                direction = Vector2.right;

            float distance = Mathf.Max(direction.magnitude, minAimDistance);
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
            isTouchingFireArea = false;

            if (_leaderWeaponController != null)
            {
                _leaderWeaponController.StopAttack();
            }

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
            float minAngle = NormalizeAngle(minForwardAngle);
            float maxAngle = NormalizeAngle(maxForwardAngle);

            if (minAngle <= maxAngle)
                return angle >= minAngle && angle <= maxAngle;

            return angle >= minAngle || angle <= maxAngle;
        }

        private float ClampAngleToForwardSector(float angle)
        {
            if (IsAngleInForwardSector(angle))
                return NormalizeAngle(angle);

            float minAngle = NormalizeAngle(minForwardAngle);
            float maxAngle = NormalizeAngle(maxForwardAngle);
            float minDelta = Mathf.Abs(Mathf.DeltaAngle(angle, minAngle));
            float maxDelta = Mathf.Abs(Mathf.DeltaAngle(angle, maxAngle));

            return minDelta <= maxDelta ? minAngle : maxAngle;
        }

        private float NormalizeAngle(float angle)
        {
            return Mathf.Repeat(angle + 180f, 360f) - 180f;
        }
    }
}
