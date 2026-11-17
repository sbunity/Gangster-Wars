using UnityEditor;
using UnityEngine;

namespace SBabchuk.EditorTools
{
    public static class EditorBoxGroup
    {
        private const float GroupSpacing = 2f;

        public static Rect BeginBoxGroup(string title)
        {
            var rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

            if (!string.IsNullOrEmpty(title))
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();

            return rect;
        }

        public static bool BeginExpandBoxGroup(string title, bool value)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

            var expanded = EditorGUILayout.Foldout(value, title, true, BoldFoldout);

            EditorGUILayout.BeginVertical();

            return expanded;
        }

        public static void BeginMenuBoxGroup(string title, GenericMenu menu)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            if (GUILayout.Button(EditorGUIUtility.IconContent("_Menu"), EditorStyles.iconButton))
                menu.ShowAsContext();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
        }

        public static void EndBoxGroup()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            GUILayout.Space(GroupSpacing);
        }

        public static bool BeginFoldoutBoxGroup(bool value, string title, out Rect foldoutRect)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

            var expanded = EditorGUILayout.Foldout(value, title, true, BoldFoldout);
            foldoutRect = GUILayoutUtility.GetLastRect();

            return expanded;
        }

        public static void EndFoldoutBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

        private static GUIStyle BoldFoldout => new(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
    }
}
