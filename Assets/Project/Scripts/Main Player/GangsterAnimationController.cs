using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class GangsterAnimationController : MonoBehaviour
    {
        private SkeletonAnimation skeletonAnimation;

        private GangsterControllerBase controller;

        [SpineEvent] public string fireEvent = "";

        AnimationsName currentAnimation;

        private void OnDestroy()
        {
            skeletonAnimation.state.Complete -= OnCompleteAnimation;
            skeletonAnimation.AnimationState.Event -= HandleEvent;
        }

        private void OnEnable()
        {
            //skltn.state.Complete += OnCompleteAnimation;
        }

        /// <summary>
        /// Перед стартова ініціалізація
        /// </summary>
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();

            controller = GetComponentInParent<GangsterControllerBase>();
        }

        /// <summary>
        /// Старт
        /// </summary>
        public virtual void Start()
        {
            Subscribe();
        }

        /// <summary>
        /// Стратова ініціалізація
        /// </summary>
        public void Subscribe()
        {
            skeletonAnimation.state.Complete += OnCompleteAnimation;
            skeletonAnimation.AnimationState.Event += HandleEvent;
        }

        /// <summary>
        /// Встановлюєм анімацію
        /// </summary>
        /// <param name="_animation"></param>
        public void SetAnimation(AnimationsName _animation)
        {
            currentAnimation = _animation;

            skeletonAnimation.state.SetAnimation(0, _animation.ToString(), GetLoop(_animation));
        }

        public AnimationsName GetCurrentAnimation()
        {
            return currentAnimation;
        }

        /// <summary>
        /// Перевіряєм зацикленість для певної анімації
        /// </summary>
        /// <param name="_animation"></param>
        /// <returns></returns>
        private bool GetLoop(AnimationsName _animation)
        {
            return  _animation == AnimationsName.Idle || _animation == AnimationsName.Shoot || _animation == AnimationsName.Reload;
        }

        /// <summary>
        /// Перевіряєм на закінчення анімації
        /// </summary>
        /// <param name="trackEntry"></param>
        private void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == AnimationsName.Reload.ToString())
            {
            }
            else if (trackEntry.Animation.Name == AnimationsName.Shoot.ToString())
            {
                //SetAnimation(AnimationsName.Idle);
            }
            else if (trackEntry.Animation.Name == AnimationsName.Shoot_prev.ToString())
            {
                SetAnimation(AnimationsName.Idle);

                controller.AttackEnded();//для снайперши, щоб вона поверталась в позицію
            }
            else if (trackEntry.Animation.Name == AnimationsName.Throwing.ToString())
            {
                SetAnimation(AnimationsName.Idle);
            }
        }

        /// <summary>
        /// Ловимо івенти
        /// </summary>
        /// <param name="trackEntry"></param>
        /// <param name="e"></param>
        private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            // Play some sound if the event named "footstep" fired.
            if (e.Data.Name == fireEvent)
            {
                controller.SpawnBullet();
            }
        }
    }
}
