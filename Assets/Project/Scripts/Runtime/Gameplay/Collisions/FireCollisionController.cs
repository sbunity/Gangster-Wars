using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;

namespace SBabchuk
{
    public class FireCollisionController : CollisionController
    {
        private Tween _twn;

        private void OnDisable()
        {
            _twn?.Kill();
        }

        public override void Init(Vector3 _position, int _damage, float _radius, float _time = 0)
        {
            this.gameObject.SetActive(true);
            transform.position = _position;
            Damage = _damage;
            Radius = _radius;
            Time = _time;
            Action(Time);
        }

        private void Action(float _time)
        {
            _twn = DOVirtual.DelayedCall(Time, () =>
            {
                Pop();
            });
        }
    }
}
