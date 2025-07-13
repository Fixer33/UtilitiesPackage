using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Utilities.Editor
{
    [CustomPropertyDrawer(typeof(SceneRef))]
    public class SceneNameDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var pathProp = property.FindPropertyRelative("_fullPath");

            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;

            var dropdown = new DropdownField
            {
                label = property.displayName,
                style = { flexGrow = 1 }
            };

            // Populate dropdown choices from build settings
            var scenes = EditorBuildSettings.scenes;
            var scenePaths = new List<string>();

            foreach (var scene in scenes)
            {
                if (!scene.enabled) continue;
                scenePaths.Add(scene.path);
            }

            dropdown.choices = scenePaths;

            // Set current value
            var currentIndex = scenePaths.IndexOf(pathProp.stringValue);
            if (currentIndex >= 0)
                dropdown.SetValueWithoutNotify(scenePaths[currentIndex]);
            else
                dropdown.SetValueWithoutNotify(string.Empty);

            dropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedPath = evt.newValue;
                var selectedName = System.IO.Path.GetFileNameWithoutExtension(selectedPath);

                pathProp.stringValue = selectedPath;

                // Apply to make sure it's saved
                property.serializedObject.ApplyModifiedProperties();
            });

            root.Add(dropdown);
            return root;
        }
    }
}