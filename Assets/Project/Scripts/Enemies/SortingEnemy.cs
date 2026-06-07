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
    [Header("Виключення сортування")]
	public bool IsStatic;

    [Header("Чи варто сортувани вкладені елементи")]
    public bool InChildren;

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
    private void Start()
	{
        e_animation = GetComponentInChildren<SkeletonAnimation>();

        if (InChildren)
        {
            renderList = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }
        else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                renderList = new List<Renderer>(){ spriteRenderer };
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
        if (e_animation)
        {
            e_animation.gameObject.GetComponent<MeshRenderer>().sortingOrder = -Mathf.RoundToInt((transform.position.y + AnchorOffset) / 0.03f);
        }
       
        foreach (Renderer renderer in renderList)
        {
            renderer.sortingOrder = -Mathf.RoundToInt((transform.position.y + AnchorOffset) / 0.03f);
        }
    }
}
