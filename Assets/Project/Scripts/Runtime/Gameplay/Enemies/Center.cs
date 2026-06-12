using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    public class Center : MonoBehaviour
    {
        public Transform GetTransform() => transform;

        public Vector3 GetPosition() => transform.position;
    }
}
