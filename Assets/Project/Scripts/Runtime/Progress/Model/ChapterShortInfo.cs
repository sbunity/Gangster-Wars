using System;
using UnityEngine;

namespace SBabchuk.Runtime.Progress
{
    [Serializable]
    public sealed class ChapterShortInfo
    {
        [SerializeField] private int _id;
        public int Id { get => _id; set => _id = value; }

        [SerializeField] private string _name;
        public string Name { get => _name; set => _name = value; }

        [SerializeField] private mySwitch _isUnlocked;
        public mySwitch IsUnlocked { get => _isUnlocked; set => _isUnlocked = value; }

        [SerializeField] private mySwitch _isCompleted;
        public mySwitch IsCompleted { get => _isCompleted; set => _isCompleted = value; }

        public ChapterShortInfo(int id, string name, bool isUnlocked)
        {
            _id = id;
            _name = name;
            _isUnlocked = isUnlocked ? mySwitch.On : mySwitch.Off;
            _isCompleted = mySwitch.Off;
        }
    }
}
