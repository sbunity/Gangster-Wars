using System.Collections.Generic;
using UnityEngine;

namespace SBabchuk.Runtime.UI.LevelSelect
{
    public sealed class CarouselSlotLayout
    {
        private readonly List<float> _slotCenters = new();

        public float ContentWidth { get; private set; }
        public int Count => _slotCenters.Count;

        public void Clear()
        {
            _slotCenters.Clear();
            ContentWidth = 0f;
        }

        public float AddSlot(float width)
        {
            var center = ContentWidth + width * 0.5f;
            _slotCenters.Add(center);
            ContentWidth += width;
            return center;
        }

        public float GetNormalizedPosition(int index, float viewportWidth)
        {
            var scrollableWidth = ContentWidth - viewportWidth;
            if (scrollableWidth <= 0f || index < 0 || index >= _slotCenters.Count)
                return 0f;

            return Mathf.Clamp01((_slotCenters[index] - viewportWidth * 0.5f) / scrollableWidth);
        }
    }
}
