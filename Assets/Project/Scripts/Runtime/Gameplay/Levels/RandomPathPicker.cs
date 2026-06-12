using System;
using UnityEngine;

namespace SBabchuk.Runtime.Gameplay.Levels
{
    // Picks spawn-path indices without repetition, refilling the pool once exhausted.
    // Extracted from LevelController to keep its random-path behavior identical.
    public sealed class RandomPathPicker
    {
        private readonly int _pathCount;
        private int[] _available;

        public RandomPathPicker(int pathCount)
        {
            _pathCount = pathCount;
            _available = BuildPool();
        }

        public int Next()
        {
            if (_available.Length == 0)
                _available = BuildPool();

            var numToRemove = UnityEngine.Random.Range(0, _available.Length);
            var index = _available[numToRemove];
            RemoveAt(ref _available, numToRemove);
            return index;
        }

        private int[] BuildPool()
        {
            var pool = new int[_pathCount];
            for (var i = 0; i < _pathCount; i++)
                pool[i] = i;

            return pool;
        }

        private static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (var i = index; i < arr.Length - 1; i++)
                arr[i] = arr[i + 1];

            Array.Resize(ref arr, arr.Length - 1);
        }
    }
}
