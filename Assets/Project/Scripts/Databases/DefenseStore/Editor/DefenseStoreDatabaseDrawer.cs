using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SBabchuk
{
    public class DefenseStoreDatabaseDrawer
    {
        static Color defaultColor;
        static int selected = 0;
        private static int selectedMode = 0;
        static private DefenseStoreDatabase database;
        static string titleBttnVisibleUpgrade = "Show";
        static string titleBttnVisibleIcons = "Show";
        public static void Draw(DefenseStoreDatabase _database, int _selectedMode)
        {
            if (database == null)
                database = _database;
            selectedMode = _selectedMode;
            defaultColor = GUI.color;
            DrawNavigation();
        }

        public static void DrawNavigation()
        {
            GUILayout.BeginVertical("box");
            {
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Р СњР В°Р В»Р В°РЎв‚¬РЎвЂљРЎС“Р Р†Р В°Р Р…Р Р…РЎРЏ:");
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Р вЂќР С•Р В±Р В°Р Р†Р С‘РЎвЂљР С‘ Р Р…Р С•Р Р†Р С‘Р в„– Р В·Р В°Р С—Р С‘РЎРѓ"))
                    {
                        database.Defenses.Add(new Defense(database.Defenses.Count));
                        selected = database.Defenses.Count - 1;
                    }

                    if (GUILayout.Button("Р вЂ™Р С‘Р Т‘Р В°Р В»Р С‘РЎвЂљР С‘ Р Р†РЎРѓРЎвЂ“ Р В·Р В°Р С—Р С‘РЎРѓР С‘", GUILayout.Width(150)))
                    {
                        database.Defenses.Clear();
                        selected = 0;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    if (selectedMode == 1)
                    {
                        if (GUILayout.Button("<--", GUILayout.Width(50)))
                        {
                            selected = Mathf.Max(0, selected - 1);
                        }

                        if (GUILayout.Button("-->", GUILayout.Width(50)))
                        {
                            selected = Mathf.Min(database.Defenses.Count == 0 ? 0 : database.Defenses.Count - 1, selected + 1);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                if (database)
                {
                    if (database.Defenses != null)
                    {
                        if (database.Defenses.Count > 0)
                        {
                            if (selectedMode == 0)
                            {
                                foreach (Defense _defense in database.Defenses)
                                {
                                    if (DrawDefense(_defense))
                                        break;
                                }
                            }
                            else
                            {
                                DrawDefense(database.Defenses[selected]);
                            }
                        }
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public static bool DrawDefense(Defense _record)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
                    if (GUILayout.Button("Р вЂ™Р С‘Р Т‘Р В°Р В»Р С‘РЎвЂљР С‘", GUILayout.Width(75), GUILayout.Height(20)))
                    {
                        database.Defenses.Remove(_record);
                        selected = Mathf.Max(0, selected - 1);
                        return true;
                    }
                }

                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    _record.Id = EditorGUILayout.IntField("ID: ", _record.Id);
                    _record.Name = EditorGUILayout.TextField("Р СњР В°Р в„–Р СР ВµР Р…РЎС“Р Р†Р В°Р Р…Р Р…РЎРЏ: ", _record.Name);
                    Utils.CheckColor(_record.Price, 0);
                    _record.Price = EditorGUILayout.IntField("Р вЂ™Р В°РЎР‚РЎвЂљРЎвЂ“РЎРѓРЎвЂљРЎРЉ: ", _record.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_record.Settings.Health, 0);
                    _record.Settings.Health = EditorGUILayout.IntField("Р С™-РЎРѓРЎвЂљРЎРЉ Р В¶Р С‘РЎвЂљРЎвЂљРЎвЂ“Р Р† (Р В±Р ВµР В· Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р В°): ", _record.Settings.Health);
                    Utils.ChangeColor(defaultColor);
                    GUI.color = Color.green;
                    _record.CountUpgrades = EditorGUILayout.IntSlider("Р С™РЎвЂ“Р В»РЎРЉР С”РЎвЂ“РЎРѓРЎвЂљРЎРЉ Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘РЎвЂ“Р Р†: ", _record.CountUpgrades, 1, 5);
                    GUI.color = defaultColor;
                    if (GUILayout.Button(((selected != _record.Id)) ? "Show" : titleBttnVisibleUpgrade, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _record.Id)
                        {
                            if (titleBttnVisibleUpgrade == "Show")
                            {
                                titleBttnVisibleUpgrade = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleUpgrade = "Show";
                                selected = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleUpgrade = "Hide";
                            selected = _record.Id;
                        }
                    }

                    if (titleBttnVisibleUpgrade == "Hide" && selected == _record.Id)
                    {
                        EditorGUILayout.LabelField("Р вЂ Р Р…РЎвЂћР С•РЎР‚Р СР В°РЎвЂ РЎвЂ“РЎРЏ Р С—РЎР‚Р С• Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р С‘:");
                        if (_record.Upgrades != null)
                        {
                            if (_record.CountUpgrades == _record.Upgrades.Count)
                            {
                                foreach (DUpgrade _upgrade in _record.Upgrades)
                                {
                                    DrawUpgrade(_upgrade);
                                }
                            }
                            else
                            {
                                if (_record.CountUpgrades > _record.Upgrades.Count)
                                {
                                    for (int i = _record.Upgrades.Count; i < _record.CountUpgrades; i++)
                                    {
                                        _record.Upgrades.Add(new DUpgrade(_record.Upgrades.Count));
                                    }
                                }
                                else
                                {
                                    _record.Upgrades.RemoveRange(_record.Upgrades.Count - (_record.Upgrades.Count - _record.CountUpgrades), _record.Upgrades.Count - _record.CountUpgrades);
                                }
                            }
                        }
                        else
                        {
                            _record.Upgrades = new List<DUpgrade>();
                            for (int i = 0; i < _record.CountUpgrades; i++)
                            {
                                _record.Upgrades.Add(new DUpgrade(_record.Upgrades.Count));
                            }
                        }
                    }

                    GUI.color = Color.green;
                    _record.CountIcons = EditorGUILayout.IntSlider("Р С™РЎвЂ“Р В»РЎРЉР С”РЎвЂ“РЎРѓРЎвЂљРЎРЉ РЎвЂ“Р С”Р С•Р Р…Р С•Р С”: ", _record.CountIcons, 1, 4);
                    GUI.color = defaultColor;
                    if (GUILayout.Button(((selected != _record.Id)) ? "Show" : titleBttnVisibleIcons, GUILayout.Width(100), GUILayout.Height(20)))
                    {
                        if (selected == _record.Id)
                        {
                            if (titleBttnVisibleIcons == "Show")
                            {
                                titleBttnVisibleIcons = "Hide";
                            }
                            else
                            {
                                titleBttnVisibleIcons = "Show";
                                selected = -1;
                            }
                        }
                        else
                        {
                            titleBttnVisibleIcons = "Hide";
                            selected = _record.Id;
                        }
                    }

                    if (titleBttnVisibleIcons == "Hide" && selected == _record.Id)
                    {
                        EditorGUILayout.LabelField("Р вЂ Р Р…РЎвЂћР С•РЎР‚Р СР В°РЎвЂ РЎвЂ“РЎРЏ Р С—РЎР‚Р С• Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р С‘:");
                        if (_record.Icons != null)
                        {
                            if (_record.CountIcons == _record.Icons.Count)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    foreach (Ico _ico in _record.Icons)
                                    {
                                        DrawIco(_ico);
                                    }
                                }

                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                if (_record.CountIcons > _record.Icons.Count)
                                {
                                    for (int i = _record.Icons.Count; i < _record.CountIcons; i++)
                                    {
                                        _record.Icons.Add(new Ico(_record.Icons.Count));
                                    }
                                }
                                else
                                {
                                    _record.Icons.RemoveRange(_record.Icons.Count - (_record.Icons.Count - _record.CountIcons), _record.Icons.Count - _record.CountIcons);
                                }
                            }
                        }
                        else
                        {
                            _record.Icons = new List<Ico>();
                            for (int i = 0; i < _record.CountIcons; i++)
                            {
                                _record.Icons.Add(new Ico(_record.Icons.Count));
                            }
                        }
                    }
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            return false;
        }

        public static void DrawIco(Ico _record)
        {
            _record.Icon = (Sprite)EditorGUILayout.ObjectField(_record.Icon, typeof(Sprite), false, GUILayout.Width(75), GUILayout.Height(75));
        }

        public static void DrawUpgrade(DUpgrade _upgrade)
        {
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.BeginVertical();
                {
                    _upgrade.Id = EditorGUILayout.IntField("ID Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р В°: ", _upgrade.Id);
                    _upgrade.Name = EditorGUILayout.TextField("Р СњР В°Р в„–Р СР ВµР Р…РЎС“Р Р†Р В°Р Р…Р Р…РЎРЏ Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р В°: ", _upgrade.Name);
                    Utils.CheckColor(_upgrade.Price, 0);
                    _upgrade.Price = EditorGUILayout.IntField("Р вЂ™Р В°РЎР‚РЎвЂљРЎвЂ“РЎРѓРЎвЂљРЎРЉ Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р В°: ", _upgrade.Price);
                    Utils.ChangeColor(defaultColor);
                    Utils.CheckColor(_upgrade.Settings.Health, 0);
                    _upgrade.Settings.Health = EditorGUILayout.IntField("Р С™-РЎРѓРЎвЂљРЎРЉ Р В¶Р С‘РЎвЂљРЎвЂљРЎвЂ“Р Р† (Р В±Р ВµР В· Р В°Р С—Р С–РЎР‚Р ВµР в„–Р Т‘Р В°): ", _upgrade.Settings.Health);
                    Utils.ChangeColor(defaultColor);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
        }

        public static void ChangeColor(Color _color)
        {
            GUI.color = _color;
        }
    }
}
