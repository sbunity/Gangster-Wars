using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Enemies
{
    [DisallowMultipleComponent]
    public sealed class EnemyDamageFlash : MonoBehaviour, IEnemyDamageFeedback
    {
        private static readonly int FillColorId = Shader.PropertyToID("_FillColor");
        private static readonly int FillPhaseId = Shader.PropertyToID("_FillPhase");
        private const string DefaultFillShaderName = "Spine/Skeleton Fill";

        [SerializeField] private Color _flashColor = new(1f, 1f, 1f, 0.65f);
        [SerializeField, Min(0.01f)] private float _fadeOutDuration = 0.12f;
        [SerializeField] private Shader _fillShader;

        private readonly List<SkeletonFlashTarget> _skeletonTargets = new();
        private readonly List<SpriteFlashTarget> _spriteTargets = new();
        private Coroutine _flashRoutine;
        private bool _isCached;

        private void Awake()
        {
            CacheTargets();
            ApplyFlash(0f);
        }

        private void OnDisable()
        {
            ResetFeedback();
        }

        private void OnDestroy()
        {
            StopFlashRoutine();

            foreach (var target in _skeletonTargets)
                target.Dispose();

            _skeletonTargets.Clear();
            _spriteTargets.Clear();
            _isCached = false;
        }

        public void Play()
        {
            if (!isActiveAndEnabled)
                return;

            if (!_isCached)
                CacheTargets();

            StopFlashRoutine();
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        public void ResetFeedback()
        {
            StopFlashRoutine();
            ApplyFlash(0f);
        }

        private IEnumerator FlashRoutine()
        {
            var maxPhase = Mathf.Clamp01(_flashColor.a);
            ApplyFlash(maxPhase);

            var elapsed = 0f;
            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                ApplyFlash(Mathf.Lerp(maxPhase, 0f, elapsed / _fadeOutDuration));
                yield return null;
            }

            ApplyFlash(0f);
            _flashRoutine = null;
        }

        private void CacheTargets()
        {
            _fillShader ??= Shader.Find(DefaultFillShaderName);

            foreach (var target in _skeletonTargets)
                target.Dispose();

            _skeletonTargets.Clear();
            _spriteTargets.Clear();

            var skeletonRenderers = GetComponentsInChildren<SkeletonRenderer>(true);
            foreach (var skeletonRenderer in skeletonRenderers)
                _skeletonTargets.Add(new SkeletonFlashTarget(skeletonRenderer, _fillShader));

            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var spriteRenderer in spriteRenderers)
                _spriteTargets.Add(new SpriteFlashTarget(spriteRenderer));

            _isCached = true;
        }

        private void StopFlashRoutine()
        {
            if (_flashRoutine == null)
                return;

            StopCoroutine(_flashRoutine);
            _flashRoutine = null;
        }

        private void ApplyFlash(float phase)
        {
            phase = Mathf.Clamp01(phase);

            foreach (var target in _skeletonTargets)
                target.Apply(_flashColor, phase);

            foreach (var target in _spriteTargets)
                target.Apply(_flashColor, phase);
        }

        private static void DestroyMaterial(Material material)
        {
            if (material == null)
                return;

            if (Application.isPlaying)
                Destroy(material);
            else
                DestroyImmediate(material);
        }

        private sealed class SkeletonFlashTarget
        {
            private readonly SkeletonRenderer _renderer;
            private readonly List<Material> _sourceMaterials = new();
            private readonly List<Material> _flashMaterials = new();

            public SkeletonFlashTarget(SkeletonRenderer renderer, Shader fillShader)
            {
                _renderer = renderer;
                BuildMaterialOverrides(fillShader);
            }

            public void Apply(Color color, float phase)
            {
                foreach (var material in _flashMaterials)
                {
                    if (material == null)
                        continue;

                    material.SetColor(FillColorId, color);
                    material.SetFloat(FillPhaseId, phase);
                }
            }

            public void Dispose()
            {
                if (_renderer != null)
                {
                    for (var i = 0; i < _sourceMaterials.Count; i++)
                    {
                        var source = _sourceMaterials[i];
                        var flash = i < _flashMaterials.Count ? _flashMaterials[i] : null;

                        if (source != null &&
                            _renderer.CustomMaterialOverride.TryGetValue(source, out var current) &&
                            current == flash)
                        {
                            _renderer.CustomMaterialOverride.Remove(source);
                        }
                    }
                }

                foreach (var material in _flashMaterials)
                    DestroyMaterial(material);

                _sourceMaterials.Clear();
                _flashMaterials.Clear();
            }

            private void BuildMaterialOverrides(Shader fillShader)
            {
                if (_renderer == null || fillShader == null)
                    return;

                _renderer.Initialize(false);

                var meshRenderer = _renderer.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    return;

                foreach (var sourceMaterial in meshRenderer.sharedMaterials)
                {
                    if (sourceMaterial == null || _sourceMaterials.Contains(sourceMaterial))
                        continue;

                    var flashMaterial = new Material(sourceMaterial)
                    {
                        shader = fillShader,
                        name = $"{sourceMaterial.name} (Damage Flash)"
                    };

                    flashMaterial.SetColor(FillColorId, Color.clear);
                    flashMaterial.SetFloat(FillPhaseId, 0f);

                    _renderer.CustomMaterialOverride[sourceMaterial] = flashMaterial;
                    _sourceMaterials.Add(sourceMaterial);
                    _flashMaterials.Add(flashMaterial);
                }
            }
        }

        private readonly struct SpriteFlashTarget
        {
            private readonly SpriteRenderer _renderer;
            private readonly Color _baseColor;

            public SpriteFlashTarget(SpriteRenderer renderer)
            {
                _renderer = renderer;
                _baseColor = renderer != null ? renderer.color : Color.white;
            }

            public void Apply(Color flashColor, float phase)
            {
                if (_renderer == null)
                    return;

                var target = new Color(flashColor.r, flashColor.g, flashColor.b, _baseColor.a);
                _renderer.color = Color.Lerp(_baseColor, target, phase);
            }
        }
    }
}
