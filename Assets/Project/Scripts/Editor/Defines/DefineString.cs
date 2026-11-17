using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace SBabchuk.EditorTools.Defines
{
    public sealed class DefineString
    {
        private const char Separator = ';';

        private readonly NamedBuildTarget _buildTarget;
        private readonly string _baseDefineLine;
        private readonly List<string> _definesList;

        public DefineString()
        {
            _buildTarget = DefineManager.ActiveBuildTarget;
            _definesList = PlayerSettings.GetScriptingDefineSymbols(_buildTarget)
                .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            _baseDefineLine = GetDefineLine();
        }

        public bool HasDefine(string define)
        {
            return _definesList.FindIndex(x => x == define) != -1;
        }

        public void RemoveDefine(string define)
        {
            var defineIndex = _definesList.FindIndex(x => x == define);
            if (defineIndex == -1)
                return;

            _definesList.RemoveAt(defineIndex);
        }

        public void AddDefine(string define)
        {
            if (HasDefine(define))
                return;

            _definesList.Add(define);
        }

        public string GetDefineLine()
        {
            return string.Join(Separator.ToString(), _definesList);
        }

        public bool HasChanges()
        {
            return _baseDefineLine != GetDefineLine();
        }

        public void ApplyDefines()
        {
            if (HasChanges())
                PlayerSettings.SetScriptingDefineSymbols(_buildTarget, GetDefineLine());
        }
    }
}
