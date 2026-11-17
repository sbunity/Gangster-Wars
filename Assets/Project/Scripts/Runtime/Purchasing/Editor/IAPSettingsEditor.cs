using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SBabchuk.EditorTools;
using UnityEditor;
using UnityEngine;

namespace SBabchuk.Runtime.Purchasing
{
    [CustomEditor(typeof(IAPSettings))]
    public sealed class IAPSettingsEditor : Editor
    {
        private const string EnumWindowTitle = "[IAP Manager]: Product Type";
        private const float ItemSpacing = 8f;
        private const float DeleteButtonWidth = 80f;
        private const float AddButtonWidth = 48f;
        private const float AddButtonHeight = 20f;
        private const float IconButtonSize = 18f;
        private const float WarningIconOffset = 22f;

        private static Vector2 EnumWindowSize => new(300, 320);

        private SerializedProperty _storeItemsProperty;
        private SerializedProperty _useFakeStoreProperty;
        private SerializedProperty _fakeStoreModeProperty;

        private GUIContent _settingsContent;
        private GUIContent _warningContent;
        private GUIContent _addContent;

        private GenericMenu _menu;

        private void OnEnable()
        {
            _storeItemsProperty = serializedObject.FindProperty("_storeItems");
            _useFakeStoreProperty = serializedObject.FindProperty("_useFakeStore");
            _fakeStoreModeProperty = serializedObject.FindProperty("_fakeStoreMode");

            var addIcon = EditorGUIUtility.IconContent("Toolbar Plus").image;
            _settingsContent = new GUIContent(addIcon);
            _addContent = new GUIContent(" Add", addIcon, "Add new product");
            _warningContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml").image);

            _menu = new GenericMenu();
            _menu.AddItem(new GUIContent("Add Product"), false, AddNewItem);
            _menu.AddItem(new GUIContent("Open Types Manager"), false, OpenTypesWindow);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorBoxGroup.BeginBoxGroup("Settings");
            EditorGUILayout.PropertyField(_useFakeStoreProperty);
            EditorGUILayout.PropertyField(_fakeStoreModeProperty);
            EditorBoxGroup.EndBoxGroup();

            EditorBoxGroup.BeginMenuBoxGroup("Products", _menu);

            if (_storeItemsProperty.arraySize > 0)
            {
                for (var i = 0; i < _storeItemsProperty.arraySize; i++)
                {
                    var itemProperty = _storeItemsProperty.GetArrayElementAtIndex(i);
                    var androidIDProperty = itemProperty.FindPropertyRelative("_androidID");
                    var iOSIDProperty = itemProperty.FindPropertyRelative("_iOSID");
                    var productKeyTypeProperty = itemProperty.FindPropertyRelative("_productKeyType");
                    var productTypeProperty = itemProperty.FindPropertyRelative("_productType");
                    var priceProperty = itemProperty.FindPropertyRelative("_defaultUSDPrice");

                    var productKeyType = (ProductKeyType)productKeyTypeProperty.intValue;
                    var productType = (ProductType)productTypeProperty.intValue;

                    var title = string.Format("{0} ({1})", productKeyType, productType);

                    itemProperty.isExpanded = EditorBoxGroup.BeginFoldoutBoxGroup(itemProperty.isExpanded, title, out var rect);

                    if (androidIDProperty.stringValue.Length == 0 && iOSIDProperty.stringValue.Length == 0)
                        EditorGUI.LabelField(new Rect(rect.x + rect.width - WarningIconOffset, rect.y, IconButtonSize, IconButtonSize), _warningContent);

                    if (itemProperty.isExpanded)
                    {
                        var backupColor = GUI.backgroundColor;

                        EditorGUI.BeginChangeCheck();

                        if (androidIDProperty.stringValue.Length == 0)
                            GUI.backgroundColor = Color.red;

                        EditorGUILayout.PropertyField(androidIDProperty, new GUIContent("Android ID"));

                        GUI.backgroundColor = backupColor;

                        if (iOSIDProperty.stringValue.Length == 0)
                            GUI.backgroundColor = Color.red;

                        EditorGUILayout.PropertyField(iOSIDProperty, new GUIContent("iOS ID"));

                        GUI.backgroundColor = backupColor;

                        if (EditorGUI.EndChangeCheck())
                        {
                            androidIDProperty.stringValue = androidIDProperty.stringValue.Trim();
                            iOSIDProperty.stringValue = iOSIDProperty.stringValue.Trim();
                        }

                        GUILayout.Space(ItemSpacing);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(productKeyTypeProperty);
                        if (GUILayout.Button(_settingsContent, GUILayout.Width(IconButtonSize), GUILayout.Height(IconButtonSize)))
                            OpenTypesWindow();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(productTypeProperty);

                        GUILayout.Space(ItemSpacing);

                        EditorGUILayout.PropertyField(priceProperty);

                        serializedObject.ApplyModifiedProperties();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Delete", GUILayout.Width(DeleteButtonWidth)))
                        {
                            if (EditorUtility.DisplayDialog("Remove", "Are you sure you want to remove this item?", "Remove", "Cancel"))
                            {
                                DeleteItem(i);

                                GUIUtility.ExitGUI();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorBoxGroup.EndFoldoutBoxGroup();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Products list is empty!", MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(_addContent, GUILayout.Width(AddButtonWidth), GUILayout.Height(AddButtonHeight)))
                AddNewItem();

            EditorGUILayout.EndHorizontal();

            EditorBoxGroup.EndBoxGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void OpenTypesWindow()
        {
            var window = (IAPTypesWindow)EditorWindow.GetWindow(typeof(IAPTypesWindow), true);
            window.titleContent = new GUIContent(EnumWindowTitle);
            window.minSize = EnumWindowSize;
            window.maxSize = EnumWindowSize;
            window.ShowUtility();
        }

        private void DeleteItem(int index)
        {
            _storeItemsProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }

        private void AddNewItem()
        {
            var newItemIndex = _storeItemsProperty.arraySize;

            _storeItemsProperty.arraySize++;

            var newProperty = _storeItemsProperty.GetArrayElementAtIndex(newItemIndex);
            newProperty.FindPropertyRelative("_androidID").stringValue = "";
            newProperty.FindPropertyRelative("_iOSID").stringValue = "";
            newProperty.FindPropertyRelative("_productType").enumValueIndex = 0;
            newProperty.isExpanded = true;

            serializedObject.ApplyModifiedProperties();
        }

        private sealed class IAPTypesWindow : EditorWindow
        {
            private const string TypesFileName = "ProductKeyType";
            private const string EnumNamespace = "SBabchuk.Runtime.Purchasing";
            private const float UpdateButtonWidth = 100f;
            private const float DeleteButtonWidth = 70f;

            private List<EnumData> _enumDataList;

            private string _newTypeName;
            private string _itemTypeNewName;
            private int _selectedItemType;

            private string _filePath;

            private Vector2 _scrollView = Vector2.zero;

            private GUIStyle _contentStyle;

            private void OnEnable()
            {
                _enumDataList = new List<EnumData>();
                var values = (int[])Enum.GetValues(typeof(ProductKeyType));
                var names = Enum.GetNames(typeof(ProductKeyType));

                for (var i = 0; i < values.Length; i++)
                    _enumDataList.Add(new EnumData(values[i], names[i]));

                var scriptPath = AssetDatabase.FindAssets(TypesFileName + " t:MonoScript")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == TypesFileName);
                _filePath = string.IsNullOrEmpty(scriptPath) ? null : Path.GetFullPath(scriptPath);

                _selectedItemType = -1;
                _newTypeName = "";
                _itemTypeNewName = "";

                _contentStyle = new GUIStyle { padding = new RectOffset(8, 8, 8, 8) };
            }

            private void OnGUI()
            {
                EditorGUILayout.BeginVertical(_contentStyle);
                EditorGUILayout.BeginHorizontal();

                _newTypeName = EditorGUILayout.TextField(_newTypeName);

                var uniqueName = _enumDataList.FindIndex(x => x.Name == _newTypeName) == -1;

                if (GUILayout.Button("Add") && _newTypeName != "" && uniqueName)
                {
                    _enumDataList.Add(new EnumData(GetNextElementID(), _newTypeName));

                    RegenerateEnum();

                    _newTypeName = "";
                    GUI.FocusControl(null);
                }

                EditorGUILayout.EndHorizontal();

                if (!uniqueName)
                    EditorGUILayout.HelpBox("Product type name should be unique", MessageType.Warning);

                EditorGUILayout.EndVertical();

                _scrollView = EditorGUILayout.BeginScrollView(_scrollView);
                EditorGUILayout.BeginVertical(_contentStyle);

                if (_enumDataList.Count > 0)
                {
                    for (var i = 0; i < _enumDataList.Count; i++)
                    {
                        var itemTypeRect = EditorGUILayout.BeginVertical(GUI.skin.box);

                        if (i != _selectedItemType)
                        {
                            EditorGUILayout.LabelField(_enumDataList[i].Name);
                        }
                        else
                        {
                            _itemTypeNewName = EditorGUILayout.TextField(_itemTypeNewName);

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Update name", GUILayout.Width(UpdateButtonWidth)))
                            {
                                _enumDataList[i].Name = _itemTypeNewName;
                                RegenerateEnum();
                            }

                            if (GUILayout.Button("Delete", GUILayout.Width(DeleteButtonWidth)))
                            {
                                if (EditorUtility.DisplayDialog("Remove this item?", "Are you sure you want to remove " + _enumDataList[i].Name + " item?", "Remove", "Cancel"))
                                {
                                    _enumDataList.RemoveAt(i);
                                    RegenerateEnum();

                                    GUI.FocusControl(null);
                                    _selectedItemType = -1;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        if (GUI.Button(itemTypeRect, GUIContent.none, GUIStyle.none))
                        {
                            if (i == _selectedItemType)
                            {
                                _selectedItemType = -1;
                            }
                            else
                            {
                                _itemTypeNewName = _enumDataList[i].Name;
                                _selectedItemType = i;
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Enum is empty!", MessageType.Info);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }

            private int GetNextElementID()
            {
                if (_enumDataList == null || _enumDataList.Count == 0) return 0;

                var elementID = -1;
                foreach (var enumData in _enumDataList)
                {
                    if (enumData.Value > elementID)
                        elementID = enumData.Value;
                }

                return elementID + 1;
            }

            private void RegenerateEnum()
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    Debug.LogError($"[IAP Manager]: {TypesFileName}.cs was not found.");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("namespace " + EnumNamespace);
                sb.AppendLine("{");
                sb.AppendLine("    public enum ProductKeyType");
                sb.AppendLine("    {");
                foreach (var enumData in _enumDataList)
                    sb.AppendLine("        " + enumData.Name + " = " + enumData.Value + ",");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                File.WriteAllText(_filePath, sb.ToString(), new UTF8Encoding(false));

                AssetDatabase.Refresh();
            }

            private sealed class EnumData
            {
                public int Value { get; }
                public string Name { get; set; }

                public EnumData(int value, string name)
                {
                    Value = value;
                    Name = name;
                }
            }
        }
    }
}
