using Spine;
using Spine.Unity;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public sealed class EnemyWeaponAim : MonoBehaviour
    {
        private const int SolveIterations = 3;

        [SerializeField] private SkeletonAnimation _skeleton;
        [SerializeField, SpineBone(dataField = nameof(_skeleton))] private string _pivotBoneName;
        [SerializeField] private BoneFollower _weaponFollower;
        [SerializeField] private Transform _muzzle;
        [SerializeField, Range(0f, 90f)] private float _maxAngle = 60f;
        [SerializeField, Min(0f)] private float _blendTime = 0.15f;

        private Bone _pivotBone;
        private Bone _weaponBone;
        private Vector3 _target;
        private bool _isAiming;
        private float _weight;

        public Vector3 MuzzlePosition => _muzzle.position;
        public Vector2 BarrelDirection => _muzzle.right;

        public void AimAt(Vector3 target)
        {
            _target = target;
            _isAiming = true;
        }

        public void Release()
            => _isAiming = false;

        private void OnEnable()
        {
            if (_skeleton != null)
                _skeleton.UpdateWorld += OnUpdateWorld;
        }

        private void OnDisable()
        {
            if (_skeleton != null)
                _skeleton.UpdateWorld -= OnUpdateWorld;

            _isAiming = false;
            _weight = 0f;
        }

        private void OnUpdateWorld(ISkeletonAnimation animated)
        {
            var step = _blendTime > 0f ? Time.deltaTime / _blendTime : 1f;
            _weight = Mathf.MoveTowards(_weight, _isAiming ? 1f : 0f, step);

            if (_weight <= 0f || !TryResolveBones())
                return;

            _pivotBone.Rotation += SolveAimAngle() * _weight;
        }

        private bool TryResolveBones()
        {
            if (_pivotBone != null && _weaponBone != null)
                return true;

            var skeleton = _skeleton.Skeleton;
            if (skeleton == null || _weaponFollower == null || _muzzle == null)
                return false;

            _pivotBone = skeleton.FindBone(_pivotBoneName);
            _weaponBone = skeleton.FindBone(_weaponFollower.boneName);
            return _pivotBone != null && _weaponBone != null;
        }

        private float SolveAimAngle()
        {
            Vector2 target = _skeleton.transform.InverseTransformPoint(_target);
            var pivot = new Vector2(_pivotBone.WorldX, _pivotBone.WorldY);
            var weaponRotation = _weaponBone.WorldRotationX;
            var weapon = new Vector2(_weaponBone.WorldX, _weaponBone.WorldY);
            var muzzleOffset = weapon + Rotate(_muzzle.localPosition, weaponRotation) - pivot;
            var barrelRotation = weaponRotation + _muzzle.localEulerAngles.z;

            var delta = 0f;
            for (var i = 0; i < SolveIterations; i++)
            {
                var muzzle = pivot + Rotate(muzzleOffset, delta);
                var barrel = Rotate(Vector2.right, barrelRotation + delta);
                delta += Vector2.SignedAngle(barrel, target - muzzle);
            }

            return Mathf.Clamp(Mathf.DeltaAngle(0f, delta), -_maxAngle, _maxAngle);
        }

        private Vector2 Rotate(Vector2 vector, float degrees)
            => Quaternion.Euler(0f, 0f, degrees) * vector;
    }
}
