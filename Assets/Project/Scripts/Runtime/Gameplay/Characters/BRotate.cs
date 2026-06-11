using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRotate : MonoBehaviour
{
    private SkeletonAnimation SkeletonAnim;

    private Bone GunBone;

    private Bone Hand_B1Bone;
    private Bone Hand_B2Bone;
    private Bone Hand_B3Bone;
    private Bone Hand_FBone;
    private Bone Hand_F2Bone;
    private Bone WeaponBone;

    [SpineBone]
    public string boneName;

    [SpineBone]
    public string Hand_B1BoneName;
    [SpineBone]
    public string Hand_B2BoneName;
    [SpineBone]
    public string Hand_B3BoneName;
    [SpineBone]
    public string Hand_FBoneName;
    [SpineBone]
    public string Hand_F2BoneName;
    [SpineBone]
    public string WeaponBoneName;

    [Header("Аім")]
    public Transform target;

    [Range(-180, 180)]
    public int angle;

    void Start()
    {
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        if (SkeletonAnim)
        {
            // get bones
            GunBone = SkeletonAnim.skeleton.FindBone(boneName);

            Hand_B1Bone = SkeletonAnim.skeleton.FindBone(Hand_B1BoneName);
            Hand_B2Bone = SkeletonAnim.skeleton.FindBone(Hand_B2BoneName);
            Hand_B3Bone = SkeletonAnim.skeleton.FindBone(Hand_B3BoneName);
            Hand_FBone = SkeletonAnim.skeleton.FindBone(Hand_FBoneName);
            Hand_F2Bone = SkeletonAnim.skeleton.FindBone(Hand_F2BoneName);
            WeaponBone = SkeletonAnim.skeleton.FindBone(WeaponBoneName);

            SkeletonAnim.UpdateWorld += UpdateBones;
        }
    }

    void UpdateBones(ISkeletonAnimation animated)
    {
       
        if (GunBone != null)
        {
            Rotate(GunBone);
        }

        if (Hand_B1Bone != null)
        {
            Rotate(Hand_B1Bone);
        }

        if (Hand_B2Bone != null)
        {
            Rotate(Hand_B2Bone);
        }

        if (Hand_B3Bone != null)
        {
            Rotate(Hand_B3Bone);
        }

        if (Hand_FBone != null)
        {
            Rotate(Hand_FBone);
        }

        if (Hand_F2Bone != null)
        {
            Rotate(Hand_F2Bone);
        }

        if (WeaponBone != null)
        {
            Rotate(WeaponBone);
        }
    }

    public void Rotate(Bone _GunBone)
    {
        // could be public
        const float LowerRotationBound = -180.0f;
        const float UpperRotationBound = 180.0f;

        // temp variables
        float tempRot;
        Vector3 tempVec;

        // gun bone rotation
        tempVec = Camera.main.WorldToScreenPoint(new Vector3(_GunBone.WorldX + transform.position.x, _GunBone.WorldY + transform.position.y, 0));
        //tempVec = Input.mousePosition - tempVec;
        tempVec = Camera.main.WorldToScreenPoint(target.position) - tempVec;
        tempRot = Mathf.Atan2(tempVec.y, tempVec.x * transform.localScale.x) * Mathf.Rad2Deg + angle;
        _GunBone.Rotation = Mathf.Clamp(tempRot, LowerRotationBound, UpperRotationBound) - _GunBone.Parent.WorldRotationX; 
    }

    void Flip()
    {
        // change facing flag and scale transform
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
