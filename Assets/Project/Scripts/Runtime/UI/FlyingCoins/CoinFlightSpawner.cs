using System.Collections.Generic;
using DG.Tweening;
using SBabchuk.Runtime.Architecture;
using SBabchuk.Runtime.Services.Contracts;
using UnityEngine;
using Zenject;

namespace SBabchuk.Runtime.UI.FlyingCoins
{
    public sealed class CoinFlightSpawner : MonoBehaviour, ICoinFlightService
    {
        [Header("Toggle")]
        [SerializeField] private bool _enabled = true;

        [Header("References")]
        [SerializeField] private CoinFlightView _coinPrefab;
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _spawnRoot;

        [Header("Tuning")]
        [SerializeField, Range(1, 40)] private int _maxCoins = 12;
        [SerializeField, Range(0f, 300f)] private float _burstRadiusPixels = 70f;
        [SerializeField, Range(0.02f, 1f)] private float _burstDuration = 0.18f;
        [SerializeField, Range(0.05f, 2f)] private float _flightDuration = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _flightDurationVariance = 0.12f;
        [SerializeField, Range(0f, 0.3f)] private float _coinStagger = 0.04f;
        [SerializeField, Range(0.05f, 2f)] private float _startScale = 1f;
        [SerializeField, Range(0.05f, 2f)] private float _arriveScale = 0.6f;
        [SerializeField] private Ease _burstEase = Ease.OutQuad;
        [SerializeField] private Ease _flightEase = Ease.InQuad;

        private readonly Stack<CoinFlightView> _pool = new();
        private readonly HashSet<CoinFlightView> _inFlight = new();
        private SignalSubscriptions _signals;
        private Canvas _canvas;

        private RectTransform SpawnRoot => _spawnRoot != null ? _spawnRoot : (RectTransform)transform;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signals = new SignalSubscriptions(signalBus)
                .Add<CoinFlightRequestedSignal>(OnFlightRequested)
                .Add<CoinFlightFromScreenRequestedSignal>(OnFlightFromScreenRequested);
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable()
        {
            _signals?.Disable();
            ReturnAll();
        }

        private void OnFlightRequested(CoinFlightRequestedSignal signal)
            => Play(signal.WorldOrigin, signal.Amount);

        private void OnFlightFromScreenRequested(CoinFlightFromScreenRequestedSignal signal)
            => PlayFromScreen(signal.ScreenOrigin, signal.Amount);

        public void Play(Vector3 worldOrigin, int amount)
        {
            if (!_enabled || amount <= 0)
                return;

            var gameCamera = Camera.main;
            if (gameCamera == null)
                return;

            PlayFromScreen(gameCamera.WorldToScreenPoint(worldOrigin), amount);
        }

        public void PlayFromScreen(Vector2 screenOrigin, int amount)
        {
            if (!_enabled || amount <= 0 || _coinPrefab == null || _target == null)
                return;

            var spawnRoot = SpawnRoot;
            var canvasCamera = ResolveCanvasCamera();

            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(spawnRoot, screenOrigin, canvasCamera, out var worldStart))
                return;

            var coinCount = Mathf.Clamp(amount, 1, _maxCoins);
            var targetWorld = _target.position;

            for (var index = 0; index < coinCount; index++)
                LaunchCoin(index, screenOrigin, worldStart, targetWorld, spawnRoot, canvasCamera);
        }

        private void LaunchCoin(int index, Vector2 originScreen, Vector3 worldStart, Vector3 targetWorld, RectTransform spawnRoot, Camera canvasCamera)
        {
            var coin = GetCoin();
            coin.Rect.position = worldStart;
            coin.Rect.localScale = Vector3.one * _startScale;

            var burstScreen = originScreen + Random.insideUnitCircle * _burstRadiusPixels;
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(spawnRoot, burstScreen, canvasCamera, out var worldBurst))
                worldBurst = worldStart;

            var flightDuration = _flightDuration + Random.Range(0f, _flightDurationVariance);

            var sequence = DOTween.Sequence()
                .AppendInterval(index * _coinStagger)
                .Append(coin.Rect.DOMove(worldBurst, _burstDuration).SetEase(_burstEase))
                .Append(coin.Rect.DOMove(targetWorld, flightDuration).SetEase(_flightEase))
                .Join(coin.Rect.DOScale(Vector3.one * _arriveScale, flightDuration).SetEase(Ease.InQuad))
                .AppendCallback(() => ReturnCoin(coin));

            coin.SetTween(sequence);
        }

        private Camera ResolveCanvasCamera()
        {
            if (_canvas == null)
                _canvas = GetComponentInParent<Canvas>();

            if (_canvas == null)
                return null;

            return _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        }

        private CoinFlightView GetCoin()
        {
            var coin = _pool.Count > 0 ? _pool.Pop() : Instantiate(_coinPrefab, SpawnRoot);
            coin.transform.SetParent(SpawnRoot, false);
            coin.transform.SetAsLastSibling();
            coin.gameObject.SetActive(true);
            _inFlight.Add(coin);
            return coin;
        }

        private void ReturnCoin(CoinFlightView coin)
        {
            if (coin == null)
                return;

            coin.KillTween();
            coin.gameObject.SetActive(false);
            _inFlight.Remove(coin);
            _pool.Push(coin);
        }

        private void ReturnAll()
        {
            if (_inFlight.Count == 0)
                return;

            var snapshot = new List<CoinFlightView>(_inFlight);
            foreach (var coin in snapshot)
                ReturnCoin(coin);
        }
    }
}
