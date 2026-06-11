using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{
    public Transform GetTransform() => transform;

    public Vector3 GetPosition() => transform.position;
}