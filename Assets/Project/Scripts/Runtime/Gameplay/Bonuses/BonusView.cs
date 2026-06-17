using System.Collections.Generic;
using SBabchuk.Runtime.Gameplay.Enemies;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Bonuses
{
    public sealed class BonusView : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _collectSortingOrderOffset = 100;

        private SortingEnemy _sorting;
        private readonly List<RendererSorting> _renderers = new();
        private int _minBaseOrder;
        private bool _hasCachedSorting;

        public Collider2D CollisionCollider { get; private set; }

        public void Initialize()
        {
            CollisionCollider = GetComponent<Collider2D>();

            if (_sorting == null)
                _sorting = GetComponent<SortingEnemy>();

            CacheDefaultSorting();
        }

        public void RenderAbove(Canvas targetCanvas)
        {
            if (targetCanvas == null)
                return;

            CacheDefaultSorting();

            if (_sorting != null)
                _sorting.enabled = false;

            var baseOrder = targetCanvas.sortingOrder + _collectSortingOrderOffset;
            foreach (var entry in _renderers)
            {
                if (entry.Renderer == null)
                    continue;

                entry.Renderer.sortingLayerID = targetCanvas.sortingLayerID;
                entry.Renderer.sortingOrder = baseOrder + (entry.BaseOrder - _minBaseOrder);
            }
        }

        public void RestoreSorting()
        {
            if (_sorting != null)
                _sorting.enabled = true;

            if (!_hasCachedSorting)
                return;

            foreach (var entry in _renderers)
            {
                if (entry.Renderer == null)
                    continue;

                entry.Renderer.sortingLayerID = entry.BaseLayerId;
                entry.Renderer.sortingOrder = entry.BaseOrder;
            }
        }

        private void CacheDefaultSorting()
        {
            if (_hasCachedSorting)
                return;

            var renderers = GetComponentsInChildren<SpriteRenderer>(true);
            if (renderers.Length == 0)
                return;

            _minBaseOrder = int.MaxValue;
            foreach (var renderer in renderers)
            {
                _renderers.Add(new RendererSorting(renderer, renderer.sortingLayerID, renderer.sortingOrder));
                if (renderer.sortingOrder < _minBaseOrder)
                    _minBaseOrder = renderer.sortingOrder;
            }

            _hasCachedSorting = true;
        }

        private readonly struct RendererSorting
        {
            public RendererSorting(SpriteRenderer renderer, int baseLayerId, int baseOrder)
            {
                Renderer = renderer;
                BaseLayerId = baseLayerId;
                BaseOrder = baseOrder;
            }

            public SpriteRenderer Renderer { get; }
            public int BaseLayerId { get; }
            public int BaseOrder { get; }
        }
    }
}
