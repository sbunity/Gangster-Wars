using System;
using System.Collections.Generic;
using Zenject;

namespace SBabchuk.Runtime.Architecture
{
    /// <summary>
    /// Collects SignalBus subscriptions once and toggles them on/off idempotently,
    /// so MonoBehaviours don't each re-implement the subscribe/unsubscribe + "is subscribed" flag boilerplate.
    /// Register handlers via <see cref="Add{TSignal}(Action)"/> / <see cref="Add{TSignal}(Action{TSignal})"/>,
    /// then drive them from the component lifecycle with <see cref="Enable"/> / <see cref="Disable"/>.
    /// </summary>
    public sealed class SignalSubscriptions
    {
        private readonly SignalBus _signalBus;
        private readonly List<Action> _subscribe = new();
        private readonly List<Action> _unsubscribe = new();
        private bool _active;

        public SignalSubscriptions(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public SignalSubscriptions Add<TSignal>(Action<TSignal> handler)
        {
            _subscribe.Add(() => _signalBus.Subscribe(handler));
            _unsubscribe.Add(() => _signalBus.Unsubscribe(handler));
            return this;
        }

        public SignalSubscriptions Add<TSignal>(Action handler)
        {
            _subscribe.Add(() => _signalBus.Subscribe<TSignal>(handler));
            _unsubscribe.Add(() => _signalBus.Unsubscribe<TSignal>(handler));
            return this;
        }

        public void Enable() => SetActive(true);

        public void Disable() => SetActive(false);

        private void SetActive(bool active)
        {
            if (_signalBus == null || active == _active)
                return;

            _active = active;
            var actions = active ? _subscribe : _unsubscribe;
            foreach (var action in actions)
                action();
        }
    }
}
