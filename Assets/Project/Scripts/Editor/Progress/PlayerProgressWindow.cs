using SBabchuk.Runtime.Services;
using SBabchuk.Runtime.Services.Contracts;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace SBabchuk.EditorTools
{
    public sealed class PlayerProgressWindow : EditorWindow
    {
        private const string MenuRoot = "Tools/Player Progress/";

        private ProgressEditBuffer _buffer;
        private SerializedObject _serializedBuffer;
        private IProgressEditSource _source;
        private Vector2 _scroll;
        private bool _isDirty;

        [MenuItem(MenuRoot + "Open Window", priority = 0)]
        private static void Open()
            => GetWindow<PlayerProgressWindow>("Player Progress");

        [MenuItem(MenuRoot + "Clear Save", priority = 20)]
        private static void ClearSaveMenu()
        {
            if (ConfirmClear())
                CreateFileStorage().Delete();
        }

        [MenuItem(MenuRoot + "Clear Save", true)]
        private static bool CanClearSaveMenu()
            => !EditorApplication.isPlayingOrWillChangePlaymode && CreateFileStorage().Exists;

        [MenuItem(MenuRoot + "Reveal Save Folder", priority = 21)]
        private static void RevealSaveFolder()
            => EditorUtility.RevealInFinder(SaveFileLocations.Progress);

        private void OnEnable()
        {
            _buffer = CreateInstance<ProgressEditBuffer>();
            _buffer.hideFlags = HideFlags.DontSave;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (_buffer != null)
                DestroyImmediate(_buffer);
        }

        private void OnFocus()
        {
            if (_buffer != null && !_isDirty)
                Reload();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawToolbar();
            DrawProgress();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(_source.Title, EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(SaveFileLocations.Progress, EditorStyles.miniLabel, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Reload"))
                    Reload();

                using (new EditorGUI.DisabledScope(_buffer.Progress == null || !_isDirty))
                {
                    if (GUILayout.Button("Save"))
                        Save();
                }

                using (new EditorGUI.DisabledScope(!_source.CanDelete))
                {
                    if (GUILayout.Button("Clear Save") && ConfirmClear())
                    {
                        _source.Delete();
                        Reload();
                    }
                }

                if (GUILayout.Button("Reveal"))
                    RevealSaveFolder();
            }
        }

        private void DrawProgress()
        {
            if (_buffer.Progress == null)
            {
                EditorGUILayout.HelpBox("No save file. Default progress will be created on the next launch.", MessageType.Info);
                return;
            }

            if (_isDirty)
                EditorGUILayout.HelpBox("Unsaved changes.", MessageType.Warning);

            _serializedBuffer.Update();
            using (var scroll = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                _scroll = scroll.scrollPosition;
                var property = _serializedBuffer.FindProperty(ProgressEditBuffer.ProgressPropertyName);
                EditorGUILayout.PropertyField(property, true);
            }

            if (_serializedBuffer.ApplyModifiedProperties())
                _isDirty = true;
        }

        private void Reload()
        {
            _source = CreateSource();
            _buffer.Progress = _source.TryLoad(out var progress) ? progress : null;
            _serializedBuffer = new SerializedObject(_buffer);
            _isDirty = false;
            Repaint();
        }

        private void Save()
        {
            _source.Save(_buffer.Progress);
            _isDirty = false;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
                Reload();
        }

        private static IProgressEditSource CreateSource()
        {
            if (EditorApplication.isPlaying && ProjectContext.HasInstance)
            {
                var container = ProjectContext.Instance.Container;
                return new LiveProgressEditSource(container.Resolve<IPlayerProgressRepository>(), container.Resolve<SignalBus>());
            }

            return new FileProgressEditSource(CreateFileStorage());
        }

        private static IProgressStorage CreateFileStorage()
            => new FileProgressStorage(new JsonUtilityDataSerializer(), SaveFileLocations.Progress);

        private static bool ConfirmClear()
            => EditorUtility.DisplayDialog("Clear Save", "Delete the saved player progress? Default progress will be created on the next launch.", "Delete", "Cancel");
    }
}
