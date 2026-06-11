using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class GangsterAnimationController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("fireEvent"), SpineEvent]
        private string _fireEvent = "";

        private SkeletonAnimation _skeletonAnimation;
        private GangsterControllerBase _controller;
        AnimationsName _currentAnimation;

        private void OnDestroy()
        {
            _skeletonAnimation.state.Complete -= OnCompleteAnimation;
            _skeletonAnimation.AnimationState.Event -= HandleEvent;
        }

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _controller = GetComponentInParent<GangsterControllerBase>();
        }

        public virtual void Start()
        {
            Subscribe();
        }

        public void Subscribe()
        {
            _skeletonAnimation.state.Complete += OnCompleteAnimation;
            _skeletonAnimation.AnimationState.Event += HandleEvent;
        }

        public void SetAnimation(AnimationsName _animation)
        {
            _currentAnimation = _animation;
            _skeletonAnimation.state.SetAnimation(0, _animation.ToString(), GetLoop(_animation));
        }

        public AnimationsName GetCurrentAnimation() 
            => _currentAnimation;

        private bool GetLoop(AnimationsName _animation) 
            => _animation == AnimationsName.Idle || _animation == AnimationsName.Shoot || _animation == AnimationsName.Reload;

        private void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == AnimationsName.Shoot.ToString())
            {
                var leader = _controller as LeaderGangsterController;
                if (leader != null && !leader.IsAttacking)
                {
                    SetAnimation(AnimationsName.Idle);
                    leader.StopShootingFinished();
                }
            }
            else if (trackEntry.Animation.Name == AnimationsName.Shoot_prev.ToString())
            {
                SetAnimation(AnimationsName.Idle);
                _controller.AttackEnded();
            }
            else if (trackEntry.Animation.Name == AnimationsName.Throwing.ToString())
            {
                SetAnimation(AnimationsName.Idle);
            }
        }

        private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == _fireEvent)
            {
                _controller.SpawnBullet();
            }
        }
    }
}
