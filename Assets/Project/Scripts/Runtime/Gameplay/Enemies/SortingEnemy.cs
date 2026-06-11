using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBabchuk;
using Spine.Unity;

/// <summary>
/// Сортування
/// </summary>
public class SortingEnemy : MonoBehaviour
{
    private const float UnitsPerSortingOrder = 0.03f;

    [Header("Виключення сортування")]
	public bool IsStatic;

    [Header("Чи варто сортувани вкладені елементи")]
    public bool InChildren;

    [Header("Точка сортування")]
    [SerializeField] private Transform sortAnchor;

    [Header("Зміщення сортування")]
	public float AnchorOffset;

    /// <summary>
    /// Компоненти в усіх діятх включно
    /// </summary>
    [HideInInspector] public List<Renderer> renderList;

    /// <summary>
    /// Компонент SkeletonAnimation
    /// </summary>
    private SkeletonAnimation e_animation;

    /// <summary>
    /// Стартова ініціалізація
    /// </summary>
    private void Awake()
	{
        e_animation = GetComponentInChildren<SkeletonAnimation>();

        if (InChildren)
        {
            renderList = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }
        else
        {
            Renderer renderer = GetComponent<Renderer>();

            renderList = renderer != null ? new List<Renderer>() { renderer } : new List<Renderer>();
        }
    }

    /// <summary>
    /// Оновлення
    /// </summary>
    private void LateUpdate()
	{
		if (!IsStatic)
		{
			AssignSortOrder();
		}
	}

    /// <summary>
    /// Встановлення налаштувань сортування
    /// </summary>
	private void AssignSortOrder() 
	{
        int sortingOrder = GetSortOrder();

        if (e_animation)
        {
            MeshRenderer meshRenderer = e_animation.gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer)
                meshRenderer.sortingOrder = sortingOrder;
        }
       
        foreach (Renderer renderer in renderList)
        {
            if (renderer)
                renderer.sortingOrder = sortingOrder;
        }
    }

    private int GetSortOrder()
    {
        float anchorY = sortAnchor ? sortAnchor.position.y : transform.position.y + AnchorOffset;
        return -Mathf.RoundToInt(anchorY / UnitsPerSortingOrder);
    }
}
