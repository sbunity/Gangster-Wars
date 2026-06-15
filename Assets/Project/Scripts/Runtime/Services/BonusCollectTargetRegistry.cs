using System.Collections.Generic;
using SBabchuk.Runtime.Services.Contracts;

namespace SBabchuk.Runtime.Services
{
    public sealed class BonusCollectTargetRegistry : IBonusCollectTargetRegistry
    {
        private readonly Dictionary<Key, IBonusCollectTarget> _targets = new();

        public void Register(ShortInfoName kind, int id, IBonusCollectTarget target)
        {
            if (target == null)
                return;

            _targets[new Key(kind, id)] = target;
        }

        public void Unregister(ShortInfoName kind, int id, IBonusCollectTarget target)
        {
            var key = new Key(kind, id);
            if (_targets.TryGetValue(key, out var registeredTarget) && ReferenceEquals(registeredTarget, target))
                _targets.Remove(key);
        }

        public bool TryGet(ShortInfoName kind, int id, out IBonusCollectTarget target)
        {
            return _targets.TryGetValue(new Key(kind, id), out target) && target != null;
        }

        private readonly struct Key
        {
            private readonly ShortInfoName _kind;
            private readonly int _id;

            public Key(ShortInfoName kind, int id)
            {
                _kind = kind;
                _id = id;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((int)_kind * 397) ^ _id;
                }
            }

            public override bool Equals(object obj)
                => obj is Key other && other._kind == _kind && other._id == _id;
        }
    }
}
