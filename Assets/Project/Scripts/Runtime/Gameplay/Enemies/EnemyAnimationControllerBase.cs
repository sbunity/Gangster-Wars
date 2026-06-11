using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.Serialization;

namespace SBabchuk
{
    public class EnemyAnimationControllerBase : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("fireEventName"), SpineEvent]
        private string _fireEventName = "footstep";

        [SerializeField, FormerlySerializedAs("skltn")]
        private SkeletonAnimation _skeleton;

        private EnemyControllerBase _enemyController;

        private void OnDestroy()
        {
            if (_skeleton)
                _skeleton.state.Complete -= OnCompleteAnimation;
        }

        private void OnEnable()
        {
        }

        private void Awake()
        {
            _skeleton = GetComponentInChildren<SkeletonAnimation>();
            _enemyController = GetComponent<EnemyControllerBase>();
        }

        private void Start()
        {
            _skeleton.state.Complete += OnCompleteAnimation;
            _skeleton.AnimationState.Event += HandleEvent;
        }

        public void SetAnimation(AnimationsName _animation) 
            => _skeleton.state.SetAnimation(0, _animation.ToString(), GetLoop(_animation));

        private bool GetLoop(AnimationsName _animation) 
            => _animation == AnimationsName.Walk || _animation == AnimationsName.Idle;

        public void OnCompleteAnimation(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == AnimationsName.Attack.ToString())
                _enemyController.Attacked();
            else if (trackEntry.Animation.Name == AnimationsName.Death.ToString())
                _enemyController.Dead();
        }

        void HandleEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == _fireEventName)
                _enemyController.GiveDamage();
        }
    }
}
