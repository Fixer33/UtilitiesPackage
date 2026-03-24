using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utilities.Editor
{
    public static class ObservableValueVisualizer
    {
        private static readonly Dictionary<Type, FieldInfo[]> _behaviourObservableFields = new();
        private static readonly Dictionary<GameObject, bool> _drawValuesOnObjects = new();
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= EditorOnFinishedDefaultHeaderGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += EditorOnFinishedDefaultHeaderGUI;
        }

        private static void EditorOnFinishedDefaultHeaderGUI(UnityEditor.Editor obj)
        {
            if (obj.target is not GameObject gameObject)
                return;
            
            _drawValuesOnObjects.TryAdd(gameObject, false);
            if (_drawValuesOnObjects[gameObject])
            {
                if (GUILayout.Button("Hide observable values"))
                {
                    _drawValuesOnObjects[gameObject] = false;
                    return;
                }
            }
            else
            {
                if (GUILayout.Button("Show observable values"))
                {
                    _drawValuesOnObjects[gameObject] = true;
                }
                else
                {
                    return;
                }
            }
            
            var behaviours = gameObject.GetComponents<Behaviour>();
            foreach (var behaviour in behaviours)
            {
                var type = behaviour.GetType();
                
                if (_behaviourObservableFields.TryGetValue(type, out var observableFields) == false)
                {
                    observableFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                            .Where(i => typeof(Observable).IsAssignableFrom(i.FieldType))
                            .ToArray();
                    
                    _behaviourObservableFields.Add(type, observableFields);
                }
                
                if (observableFields.Length <= 0)
                    continue;
                
                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Values only available in Play Mode.", MessageType.Info);
                    continue;
                }
                
                foreach (var observableField in observableFields)
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    var name = observableField.Name;
                    if (name.StartsWith("<") && name.Contains(">k__BackingField"))
                    {
                        name = name.Replace("<", "").Replace(">k__BackingField", "");
                    }
                    
                    name = ObjectNames.NicifyVariableName(name);

                    var value = observableField.GetValue(behaviour) as Observable;
                    GUILayout.Label(name, EditorStyles.boldLabel);
                    
                    GUILayout.FlexibleSpace();
                    
                    var valObj = value?.GetValue();
                    string valStr = valObj != null ? valObj.ToString() : "NULL";
                    
                    var valueStyle = new GUIStyle(EditorStyles.label)
                    {
                        normal = { textColor = valObj != null ? Color.cyan : Color.red },
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleRight
                    };
                    
                    GUILayout.Label(valStr, valueStyle);
                    
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}