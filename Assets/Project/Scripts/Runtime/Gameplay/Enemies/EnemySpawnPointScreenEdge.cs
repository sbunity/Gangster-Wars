using UnityEngine;

namespace SBabchuk
{
    public sealed class EnemySpawnPointScreenEdge : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private float _rightOffset = 1f;

        private void Start()
        {
            PlaceRightOfScreen();
        }

        private void PlaceRightOfScreen()
        {
            var targetCamera = _camera != null ? _camera : Camera.main;

            if (targetCamera == null)
            {
                Debug.LogWarning($"{nameof(EnemySpawnPointScreenEdge)} needs a camera.", this);
                return;
            }

            var position = transform.position;
            var distanceToPointPlane = Mathf.Abs(position.z - targetCamera.transform.position.z);
            var rightScreenPoint = new Vector3(Screen.width, Screen.height * 0.5f, distanceToPointPlane);
            var rightWorldPoint = targetCamera.ScreenToWorldPoint(rightScreenPoint);

            position.x = rightWorldPoint.x + _rightOffset;
            transform.position = position;
        }
    }
}
