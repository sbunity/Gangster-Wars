using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using DG.Tweening;

namespace SBabchuk
{
    public class BaseBulletController : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rigidbody2d;

        [HideInInspector]
        public Transform _parent;

        [HideInInspector]
        public Bullet properties;

        [HideInInspector]
        public int damage;

        [Header("Колайдер для зіткнення")]
        [HideInInspector]
        public Collider2D collisionCollider;

        SortingEnemy sorting;

        public virtual void Awake()
        {
            rigidbody2d = GetComponentInChildren<Rigidbody2D>();

            sorting = GetComponent<SortingEnemy>();
        }

        public virtual void Start()
        {
            _parent = transform.parent;
        }

        public virtual void Init(int _id = 0, int _damage = 0, Vector3 _position = default(Vector3), Vector3 _target = default(Vector3), float _offset = 0)
        {
            this.gameObject.SetActive(true);

            if (sorting)
                sorting.AnchorOffset = -_offset;

            transform.position = _position;

            damage = _damage;

            properties = PersistableSO.Instance.Bullet.GetBullet(_id);

            StartMove((_target != default(Vector3)) ? (Vector2)_target : SightController.sightPosition);

            if (collisionCollider == null)
            {
                collisionCollider = gameObject.AddComponent<PolygonCollider2D>();
            }

            collisionCollider.isTrigger = false;
        }

        /// <summary>
		/// Запускаєм рух
		/// </summary>
		public virtual void StartMove(Vector2 _target)
        {
            if (rigidbody2d)
            {
                rigidbody2d.velocity = (new Vector3(_target.x, _target.y, transform.position.z) - transform.position).normalized * properties.speedMove;

                RotationBullet(_target);
            }
            else
            {
                Debug.Log("rigidbody2d == null");
            }
        }

        /// <summary>
        /// Поворот пуль
        /// </summary>
        /// <param name="_target"></param>
        public virtual void RotationBullet(Vector3 _target)
        {
            Vector3 currPoint = transform.position;

            Vector3 currDir = _target - currPoint;

            currDir.Normalize();

            float rotationZ = Mathf.Atan2(currDir.y, currDir.x) * Mathf.Rad2Deg;
            Vector3 Vzero = Vector3.zero;

            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        /// <summary>
		/// Повернення в пуш
		/// </summary>
		public virtual void Pop()
        {
            transform.SetParent(_parent);

            if (collisionCollider)
                collisionCollider.isTrigger = false;

            this.gameObject.SetActive(false);

            transform.tag = "Bullet";

            LevelController.Instance.SpawnCollision(_collisionID: 7, _position: this.transform.position);
        }

        public virtual void OnBecameInvisible()
        {
            Pop();
        }
    }
}
