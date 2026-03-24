using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities.Editor
{
    /// <summary>
    /// UI Toolkit custom inspector for all RandomPool<T> assets via RandomPoolBase.
    /// Allows adding/removing entries, editing items and weights, normalization and shows total percentage.
    /// </summary>
    [CustomEditor(typeof(RandomPoolBase), true)]
    public class RandomPoolEditor : UnityEditor.Editor
    {
        private SerializedProperty _entriesProp;
        private SerializedProperty _autoNormalizeProp;

        private readonly List<int> _indices = new List<int>();
        private ListView _listView;

        public override VisualElement CreateInspectorGUI()
        {
            serializedObject.Update();
            _entriesProp = serializedObject.FindProperty("entries");
            _autoNormalizeProp = serializedObject.FindProperty("autoNormalize");

            var root = new VisualElement();

            // Load simple stylesheet if present
            string loadPath = "Packages/com.fixer33.utilities/Editor/RandomPoolEditor.uss";
#if PACKAGES_DEV
            loadPath = "Assets/" + loadPath;            
#endif
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(loadPath);
            if (uss != null)
                root.styleSheets.Add(uss);

            // Header / toolbar
            var toolbar = new Toolbar();

            var addBtn = new ToolbarButton(AddEntry) { text = "+ Add Item" };
            toolbar.Add(addBtn);

            var normalizeBtn = new ToolbarButton(Normalize) { text = "Normalize (100%)" };
            toolbar.Add(normalizeBtn);

            var autoToggle = new ToolbarToggle { text = "Auto Normalize" };
            autoToggle.BindProperty(_autoNormalizeProp);
            // When toggled, OnValidate may renormalize; ensure UI sync after the change.
            autoToggle.RegisterValueChangedCallback(_ =>
            {
                // Defer to allow serialization/OnValidate to complete
                RequestAutoNormalizeRefresh();
            });
            toolbar.Add(autoToggle);

            toolbar.Add(new ToolbarSpacer());

            var totalLabel = new Label();
            totalLabel.AddToClassList("cg-randompool__total");
            toolbar.Add(totalLabel);

            root.Add(toolbar);

            // ListView for entries
            RefreshIndices();

            _listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                selectionType = SelectionType.Single,
                reorderable = false
            };

            _listView.itemsSource = _indices;
            _listView.makeItem = MakeItem;
            _listView.bindItem = (ve, i) => BindItem(ve, _indices[i]);
            _listView.unbindItem = (ve, i) => UnbindItem(ve);

            root.Add(_listView);

            // Footer help box
            var help = new HelpBox("Weights are treated as percentage points. Normalize to make them sum to 100%.", HelpBoxMessageType.Info);
            help.AddToClassList("cg-randompool__help");
            root.Add(help);

            // Update callbacks
            root.schedule.Execute(() =>
            {
                // Keep list and total in sync
                serializedObject.Update();
                if (_entriesProp != null)
                {
                    if (_indices.Count != _entriesProp.arraySize)
                    {
                        RefreshIndices();
                        _listView.itemsSource = _indices;
                        _listView.Rebuild();
                    }

                    float total = 0f;
                    for (int i = 0; i < _entriesProp.arraySize; i++)
                    {
                        var e = _entriesProp.GetArrayElementAtIndex(i);
                        total += Mathf.Max(0f, e.FindPropertyRelative("weight").floatValue);
                    }
                    totalLabel.text = $"Total: {total:0.##}%";
                    totalLabel.EnableInClassList("cg-randompool__total--ok", Mathf.Approximately(total, 100f));
                    totalLabel.EnableInClassList("cg-randompool__total--warn", !Mathf.Approximately(total, 100f));
                }
            }).Every(200);

            return root;
        }

        private void AddEntry()
        {
            serializedObject.Update();
            _entriesProp.arraySize++;
            var e = _entriesProp.GetArrayElementAtIndex(_entriesProp.arraySize - 1);
            // Default new weight to 0 to avoid unexpected shifts; user can Normalize
            e.FindPropertyRelative("weight").floatValue = 0f;
            serializedObject.ApplyModifiedProperties();
            RefreshIndices();
            ForceRefresh();
        }

        private void RemoveEntry(int index)
        {
            if (index < 0 || index >= _entriesProp.arraySize) return;
            serializedObject.Update();
            _entriesProp.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            RefreshIndices();
            ForceRefresh();
        }

        private void Normalize()
        {
            // Call Normalize() on the target (works for any T)
            foreach (var t in targets)
            {
                if (t is RandomPoolBase basePool)
                {
                    var so = new SerializedObject(basePool);
                    so.Update();
                    // Can't directly call generic method, but Normalize is defined in RandomPool<T>.
                    // Use reflection to invoke if available.
                    var m = t.GetType().GetMethod("Normalize");
                    m?.Invoke(t, null);
                    EditorUtility.SetDirty(t);
                    so.ApplyModifiedProperties();
                }
            }
            // Ensure our inspector's serializedObject sees updated values
            SyncAndRefresh(true);
        }

        private void RefreshIndices()
        {
            _indices.Clear();
            if (_entriesProp == null) return;
            for (int i = 0; i < _entriesProp.arraySize; i++)
                _indices.Add(i);
        }

        private VisualElement MakeItem()
        {
            var row = new VisualElement();
            row.AddToClassList("cg-randompool__row");

            var left = new VisualElement();
            left.AddToClassList("cg-randompool__row-left");
            row.Add(left);

            var right = new VisualElement();
            right.AddToClassList("cg-randompool__row-right");
            row.Add(right);

            // Item field
            var itemField = new PropertyField { name = "itemField", label = string.Empty };
            left.Add(itemField);

            // Weight field: slider + numeric input
            var weightContainer = new VisualElement();
            weightContainer.AddToClassList("cg-randompool__weight");

            var weightSlider = new Slider(0f, 100f) { name = "weightSlider", showInputField = false };
            weightSlider.AddToClassList("cg-randompool__weight-slider");
            weightContainer.Add(weightSlider);

            var weightNumber = new FloatField { name = "weightNumber" };
            weightNumber.AddToClassList("cg-randompool__weight-number");
            weightContainer.Add(weightNumber);

            right.Add(weightContainer);

            // Remove button
            var removeBtn = new Button { name = "removeBtn", text = "Remove" };
            removeBtn.AddToClassList("cg-randompool__remove");
            row.Add(removeBtn);

            return row;
        }

        private void BindItem(VisualElement ve, int index)
        {
            if (_entriesProp == null) return;
            var elementProp = _entriesProp.GetArrayElementAtIndex(index);

            var itemProp = elementProp.FindPropertyRelative("item");
            var weightProp = elementProp.FindPropertyRelative("weight");

            var itemField = ve.Q<PropertyField>("itemField");
            var weightSlider = ve.Q<Slider>("weightSlider");
            var weightNumber = ve.Q<FloatField>("weightNumber");

            itemField?.BindProperty(itemProp);

            // Initial sync
            float current = Mathf.Max(0f, weightProp.floatValue);
            weightSlider?.SetValueWithoutNotify(Mathf.Clamp(current, 0f, 100f));
            if (weightNumber != null)
            {
                weightNumber.formatString = "0.##";
                weightNumber.SetValueWithoutNotify(current);
            }

            // Register callbacks and store them to unbind safely later
            if (weightSlider != null)
            {
                EventCallback<ChangeEvent<float>> sliderCb = evt =>
                {
                    serializedObject.Update();
                    float v = Mathf.Clamp(evt.newValue, 0f, 100f);
                    weightProp.floatValue = v;
                    serializedObject.ApplyModifiedProperties();
                    weightNumber?.SetValueWithoutNotify(v);
                    UpdateTotalLabelSoon();
                    // If auto-normalize is on, OnValidate will rebalance weights.
                    // Defer a sync so UI reflects the post-normalized values.
                    if (_autoNormalizeProp != null && _autoNormalizeProp.boolValue)
                        RequestAutoNormalizeRefresh();
                };
                // store for unbind
                weightSlider.userData = sliderCb;
                weightSlider.RegisterValueChangedCallback(sliderCb);
            }

            if (weightNumber != null)
            {
                EventCallback<ChangeEvent<float>> numberCb = evt =>
                {
                    serializedObject.Update();
                    float v = Mathf.Clamp(evt.newValue, 0f, 100f);
                    weightProp.floatValue = v;
                    serializedObject.ApplyModifiedProperties();
                    weightSlider?.SetValueWithoutNotify(v);
                    UpdateTotalLabelSoon();
                    if (_autoNormalizeProp != null && _autoNormalizeProp.boolValue)
                        RequestAutoNormalizeRefresh();
                };
                weightNumber.userData = numberCb;
                weightNumber.RegisterValueChangedCallback(numberCb);
            }

            var removeBtn = ve.Q<Button>("removeBtn");
            if (removeBtn != null)
            {
                EventCallback<ClickEvent> clickCb = _ => RemoveEntry(index);
                removeBtn.userData = clickCb;
                removeBtn.RegisterCallback(clickCb);
            }
        }

        private void UnbindItem(VisualElement ve)
        {
            // Unregister callbacks safely
            var removeBtn = ve.Q<Button>("removeBtn");
            if (removeBtn != null && removeBtn.userData is EventCallback<ClickEvent> clickCb)
            {
                removeBtn.UnregisterCallback(clickCb);
                removeBtn.userData = null;
            }

            var weightSlider = ve.Q<Slider>("weightSlider");
            if (weightSlider != null && weightSlider.userData is EventCallback<ChangeEvent<float>> sliderCb)
            {
                weightSlider.UnregisterValueChangedCallback(sliderCb);
                weightSlider.userData = null;
            }

            var weightNumber = ve.Q<FloatField>("weightNumber");
            if (weightNumber != null && weightNumber.userData is EventCallback<ChangeEvent<float>> numberCb)
            {
                weightNumber.UnregisterValueChangedCallback(numberCb);
                weightNumber.userData = null;
            }
        }

        private void ForceRefresh()
        {
            SyncAndRefresh(false);
        }

        private void UpdateTotalLabelSoon()
        {
            // Trigger inspector repaint so scheduled label update reflects changes promptly
            Repaint();
        }

        private void SyncAndRefresh(bool refreshIndices)
        {
            // Sync serialized data from target and refresh visuals
            if (serializedObject != null)
                serializedObject.Update();

            if (refreshIndices)
                RefreshIndices();

            if (_listView != null)
            {
                _listView.itemsSource = _indices;
                _listView.Rebuild();
            }
            Repaint();
        }

        private void RequestAutoNormalizeRefresh()
        {
            // Use a delayed call to allow OnValidate/serialization to finish first
            EditorApplication.delayCall += () =>
            {
                // Guard: editor may have been destroyed
                if (this == null) return;
                SyncAndRefresh(false);
            };
        }
    }
}
