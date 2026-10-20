using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private CancellationTokenSource _lifetime;
        private int _busyOperations;
        private int _reloadVersion;

        private bool IsBusy => _busyOperations > 0;

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
            _lifetime = new CancellationTokenSource();
            _buffer = CreateInstance<ProgressEditBuffer>();
            _buffer.hideFlags = HideFlags.DontSave;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            _lifetime.Cancel();
            _lifetime.Dispose();
            if (_buffer != null)
                DestroyImmediate(_buffer);
        }

        private void OnFocus()
        {
            if (_buffer != null && !_isDirty && !IsBusy)
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
            using (new EditorGUI.DisabledScope(IsBusy))
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
                if (IsBusy)
                {
                    EditorGUILayout.HelpBox("Loading...", MessageType.None);
                    return;
                }

                EditorGUILayout.HelpBox("No save file. Default progress will be created on the next launch.", MessageType.Info);
                return;
            }

            if (_isDirty)
                EditorGUILayout.HelpBox("Unsaved changes.", MessageType.Warning);

            _serializedBuffer.Update();
            using (new EditorGUI.DisabledScope(IsBusy))
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
            => RunAsync(ReloadAsync).Forget();

        private void Save()
            => RunAsync(SaveAsync).Forget();

        private async UniTask ReloadAsync(CancellationToken cancellationToken)
        {
            var version = ++_reloadVersion;
            _source = CreateSource();

            var progress = await _source.LoadAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (version != _reloadVersion)
                return;

            _buffer.Progress = progress;
            _serializedBuffer = new SerializedObject(_buffer);
            _isDirty = false;
        }

        private async UniTask SaveAsync(CancellationToken cancellationToken)
        {
            await _source.SaveAsync(_buffer.Progress, cancellationToken);
            _isDirty = false;
        }

        private async UniTaskVoid RunAsync(Func<CancellationToken, UniTask> operation)
        {
            _busyOperations++;
            Repaint();

            try
            {
                await operation(_lifetime.Token);
            }
            finally
            {
                _busyOperations--;
                if (this != null)
                    Repaint();
            }
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
                var repository = container.Resolve<IPlayerProgressRepository>();
                if (repository.IsLoaded)
                    return new LiveProgressEditSource(repository, container.Resolve<SignalBus>());
            }

            return new FileProgressEditSource(CreateFileStorage());
        }

        private static FileProgressStorage CreateFileStorage()
            => new FileProgressStorage(new JsonUtilityDataSerializer(), SaveFileLocations.Progress);

        private static bool ConfirmClear()
            => EditorUtility.DisplayDialog("Clear Save", "Delete the saved player progress? Default progress will be created on the next launch.", "Delete", "Cancel");
    }
}
