using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class EnemyAnimationControllerBase : MonoBehaviour
    {
        #region Events
        public delegate void Event();
        public static event Event OnAttacked;
        public static event Event OnDead;
        #endregion//Events

        [SpineEvent] public string fireEventName = "footstep";

        [HideInInspector]public SkeletonAnimation skltn;

        private EnemyControllerBase enemyController;

        private void OnDestroy()
        {
            if (skltn)
            skltn.state.Complete -= OnCompleteAnimation;
        }

        private void OnEnable()
        {
            //skltn.state.Complete += OnCompleteAnimation;
        }

        private void Awake()
        {
            skltn = GetComponentInChildren<SkeletonAnimation>();

            enemyController = GetComponent<EnemyControllerBase>();
        }

        private void Start()
        {
            skltn.state.Complete += OnCompleteAnimation;
            skltn.AnimationState.Event += HandleEvent;
        }

        public void SetAnimation(AnimationsName _animation)
        {
            skltn.state.SetAnimation(0, _animation.ToString(), GetLoop(_animation));
        }

        /// <summary>
        /// Встановлення зациклення певних анімацій
        /// </summary>
        /// <param name="_animation"></param>
        /// <returns></returns>
        private bool GetLoop(AnimationsName _animation)
        {
            return (_animation == AnimationsName.Walk || _animation == AnimationsName.Idle);
        }

        /// <summary>
        /// Перевырка завершеносты анымацыъ
        /// </summary>
        /// <param name="trackEntry"></param>
        public void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == AnimationsName.Attack.ToString())
            {
                enemyController.Attacked();
            }
            else if (trackEntry.Animation.Name == AnimationsName.Death.ToString())
            {
                enemyController.Dead();
            }
        }

        void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == fireEventName)
            {
                enemyController.GiveDamage();
            }
        }
    }
}
