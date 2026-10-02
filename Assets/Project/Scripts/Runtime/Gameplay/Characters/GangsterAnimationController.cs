using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.Gameplay.Characters
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

        public void PlayFireLoop(float shotsPerSecond)
        {
            _currentAnimation = AnimationsName.Shoot;
            var entry = _skeletonAnimation.state.SetAnimation(0, AnimationsName.Shoot.ToString(), true);
            entry.MixDuration = 0f;

            if (shotsPerSecond <= 0f || !TryGetFireEvents(entry.Animation, out var firstEventTime, out var eventsCount))
                return;

            entry.TimeScale = entry.Animation.Duration / eventsCount * shotsPerSecond;
            entry.TrackTime = firstEventTime;
        }

        public AnimationsName GetCurrentAnimation()
            => _currentAnimation;

        private bool TryGetFireEvents(Spine.Animation animation, out float firstEventTime, out int eventsCount)
        {
            firstEventTime = float.MaxValue;
            eventsCount = 0;

            foreach (var timeline in animation.Timelines)
            {
                if (timeline is not EventTimeline eventTimeline)
                    continue;

                for (var i = 0; i < eventTimeline.FrameCount; i++)
                {
                    if (eventTimeline.Events[i].Data.Name != _fireEvent)
                        continue;

                    eventsCount++;
                    firstEventTime = Mathf.Min(firstEventTime, eventTimeline.Frames[i]);
                }
            }

            return eventsCount > 0 && animation.Duration > 0f;
        }

        private bool GetLoop(AnimationsName _animation) 
            => _animation == AnimationsName.Idle || _animation == AnimationsName.Shoot || _animation == AnimationsName.Reload;

        private void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == AnimationsName.Shoot_prev.ToString())
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
