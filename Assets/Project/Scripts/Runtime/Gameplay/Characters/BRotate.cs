using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BRotate : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("boneName"), SpineBone]
    private string _boneName;

    [SerializeField, FormerlySerializedAs("Hand_B1BoneName"), SpineBone]
    private string _handB1BoneName;

    [SerializeField, FormerlySerializedAs("Hand_B2BoneName"), SpineBone]
    private string _handB2BoneName;

    [SerializeField, FormerlySerializedAs("Hand_B3BoneName"), SpineBone]
    private string _handB3BoneName;

    [SerializeField, FormerlySerializedAs("Hand_FBoneName"), SpineBone]
    private string _handFBoneName;

    [SerializeField, FormerlySerializedAs("Hand_F2BoneName"), SpineBone]
    private string _handF2BoneName;

    [SerializeField, FormerlySerializedAs("WeaponBoneName"), SpineBone]
    private string _weaponBoneName;

    [SerializeField, FormerlySerializedAs("target")]
    private Transform _target;

    [SerializeField, FormerlySerializedAs("angle"), Range(-180, 180)]
    private int _angle;

    private SkeletonAnimation _skeletonAnim;
    private Bone _gunBone;
    private Bone _handB1Bone;
    private Bone _handB2Bone;
    private Bone _handB3Bone;
    private Bone _handFBone;
    private Bone _handF2Bone;
    private Bone _weaponBone;

    void Start()
    {
        _skeletonAnim = GetComponent<SkeletonAnimation>();
        if (_skeletonAnim)
        {
            _gunBone = _skeletonAnim.skeleton.FindBone(_boneName);
            _handB1Bone = _skeletonAnim.skeleton.FindBone(_handB1BoneName);
            _handB2Bone = _skeletonAnim.skeleton.FindBone(_handB2BoneName);
            _handB3Bone = _skeletonAnim.skeleton.FindBone(_handB3BoneName);
            _handFBone = _skeletonAnim.skeleton.FindBone(_handFBoneName);
            _handF2Bone = _skeletonAnim.skeleton.FindBone(_handF2BoneName);
            _weaponBone = _skeletonAnim.skeleton.FindBone(_weaponBoneName);
            _skeletonAnim.UpdateWorld += UpdateBones;
        }
    }

    private void UpdateBones(ISkeletonAnimation animated)
    {
        if (_gunBone != null)
        {
            Rotate(_gunBone);
        }

        if (_handB1Bone != null)
        {
            Rotate(_handB1Bone);
        }

        if (_handB2Bone != null)
        {
            Rotate(_handB2Bone);
        }

        if (_handB3Bone != null)
        {
            Rotate(_handB3Bone);
        }

        if (_handFBone != null)
        {
            Rotate(_handFBone);
        }

        if (_handF2Bone != null)
        {
            Rotate(_handF2Bone);
        }

        if (_weaponBone != null)
        {
            Rotate(_weaponBone);
        }
    }

    private void Rotate(Bone _GunBone)
    {
        const float LOWER_ROTATION_BOUND = -180.0f;
        const float UPPER_ROTATION_BOUND = 180.0f;
        var tempVec = Camera.main.WorldToScreenPoint(new Vector3(_GunBone.WorldX + transform.position.x, _GunBone.WorldY + transform.position.y, 0));
        tempVec = Camera.main.WorldToScreenPoint(_target.position) - tempVec;
        var tempRot = Mathf.Atan2(tempVec.y, tempVec.x * transform.localScale.x) * Mathf.Rad2Deg + _angle;
        _GunBone.Rotation = Mathf.Clamp(tempRot, LOWER_ROTATION_BOUND, UPPER_ROTATION_BOUND) - _GunBone.Parent.WorldRotationX;
    }

    private void Flip()
    {
        var scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
