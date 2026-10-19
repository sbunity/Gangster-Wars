using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class LevelShortInfo
    {
        [SerializeField] private int _chapterId;
        public int ChapterId { get => _chapterId; set => _chapterId = value; }

        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private mySwitch _isCompleted;
        public mySwitch IsCompleted { get => _isCompleted; set => _isCompleted = value; }

        [SerializeField] private int _stars;
        public int Stars { get => _stars; set => _stars = value; }

        public LevelShortInfo(int id, int chapterId)
        {
            _id = id;
            _chapterId = chapterId;
            _isCompleted = mySwitch.Off;
            _stars = 0;
        }
    }
}
