using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour
{
    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
