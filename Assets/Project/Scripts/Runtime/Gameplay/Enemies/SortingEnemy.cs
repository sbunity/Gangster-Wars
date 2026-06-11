using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class SortingEnemy : MonoBehaviour
{
    private const float UNITS_PER_SORTING_ORDER = 0.03f;
    [SerializeField, FormerlySerializedAs("IsStatic")]
    private bool _isStatic;

    [SerializeField, FormerlySerializedAs("InChildren")]
    private bool _inChildren;

    [SerializeField, FormerlySerializedAs("sortAnchor")]
    private Transform _sortAnchor;

    [SerializeField, FormerlySerializedAs("AnchorOffset")]
    private float _anchorOffset;
    public float AnchorOffset { get => _anchorOffset; set => _anchorOffset = value; }

    [SerializeField, FormerlySerializedAs("renderList")]
    private List<Renderer> _renderList;

    private SkeletonAnimation _skeletonAnimation;

    private void Awake()
    {
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        if (_inChildren)
        {
            _renderList = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }
        else
        {
            var objectRenderer = GetComponent<Renderer>();
            _renderList = objectRenderer != null ? new List<Renderer>
            {
                objectRenderer
            }
            : new List<Renderer>();
        }
    }

    private void LateUpdate()
    {
        if (!_isStatic)
            AssignSortOrder();
    }

    private void AssignSortOrder()
    {
        var sortingOrder = GetSortOrder();
        if (_skeletonAnimation)
        {
            var meshRenderer = _skeletonAnimation.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer)
                meshRenderer.sortingOrder = sortingOrder;
        }

        foreach (var objectRenderer in _renderList)
        {
            if (objectRenderer)
                objectRenderer.sortingOrder = sortingOrder;
        }
    }

    private int GetSortOrder()
    {
        var anchorY = _sortAnchor ? _sortAnchor.position.y : transform.position.y + _anchorOffset;
        return -Mathf.RoundToInt(anchorY / UNITS_PER_SORTING_ORDER);
    }
}
