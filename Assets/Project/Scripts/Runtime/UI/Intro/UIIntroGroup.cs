using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SBabchuk.Runtime.UI.Intro
{
    public sealed class UIIntroGroup : UIIntroAnimation
    {
        [SerializeField, Min(0f)] private float _stagger = 0.05f;
        [SerializeField] private UIIntroAnimation[] _animations = Array.Empty<UIIntroAnimation>();

        protected override void ApplyHidden(UIIntroStyle style)
        {
            foreach (var animation in _animations)
                animation.Hide(style);
        }

        protected override Tween CreateShowTween(UIIntroStyle style)
        {
            var sequence = DOTween.Sequence();

            for (var i = 0; i < _animations.Length; i++)
            {
                var animation = _animations[i];
                sequence.Insert(i * _stagger + animation.ExtraDelay, animation.Play(style));
            }

            return sequence;
        }

        [ContextMenu("Collect Direct Child Animations")]
        private void CollectDirectChildAnimations()
        {
            var animations = new List<UIIntroAnimation>();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out UIIntroAnimation animation))
                    animations.Add(animation);
            }

            _animations = animations.ToArray();
        }
    }
}
