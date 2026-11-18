# com.fixer33.utilities - Package Description

Version 1.3.0 (package.json), display name "Utilities", description "Package containing some QoL utilities". Unity 6000.0+. Author Fixer33. MIT license. Git URL: https://github.com/Fixer33/UtilitiesPackage.git

Statements marked (assumed) are inferred, not read directly from code. Unclear points are in `Open Questions.md` in this folder.

## 1. Purpose and users

A grab-bag of small quality-of-life helpers for Unity projects: an object pool wrapper, a weighted random pool asset, a lightweight observable value, a scene reference field, several extension methods, and a few Asset-menu editor tools. Users are Unity developers who install it as a dependency in their games or other packages (assumed: mainly the author, who maintains the `com.fixer33.*` package family).

## 2. Architecture and main components

Two assemblies, no runtime state, no singletons, no scenes or prefabs.

- Runtime assembly `com.fixer33.utilities` (root namespace `Utilities`, folder `Runtime/`). Sub-namespaces `Utilities.Extensions` and `Utilities.FastConditions`.
- Editor assembly `com.fixer33.utilities.Editor` (namespace `Utilities.Editor`, folder `Editor/`), Editor platform only, references the runtime assembly.

Runtime components:
- `ObjectPool<T>` (`Runtime/ObjectPool.cs`): wraps `UnityEngine.Pool.ObjectPool<T>`, implements `IObjectPool<T>` and `IEnumerable<T>` (enumerates active items). Adds an `ActiveItems` list, `ReleaseAll()`, and protected virtual hooks `OnItemCreate`, `OnItemGet`, `OnItemRelease`, `OnItemDestroy`.
- `TemplateObjectPool<T>` (`T : Component`) and `TemplateGameObjectObjectPool` (GameObjects): instantiate clones of a template under the template's parent, deactivate the template, activate items on get, deactivate on release, optionally `SetAsLastSibling` on get.
- `RandomPoolBase` (abstract ScriptableObject, editor target) and `RandomPool<T>`: weighted list of `Entry { item, weight }`, weights as percentage points, optional auto-normalise in `OnValidate`.
- `Observable` (abstract, non-generic, `GetValue()`) and `Observable<T>`: value with a `ValueChanged(oldValue, newValue)` event, fired only if `Equals` says the value changed.
- `SceneRef` (serializable struct): holds a scene path string, exposed as `FullPath`.
- `ObservableExtensions`: awaitable polling helpers for `Observable<bool/float/int>`.
- Other extension classes and `All` / `Any` null-check helpers (see section 3).

Editor components: see section 5.

## 3. Public API surface

Namespace `Utilities`:
- `ObjectPool<T> where T : class`
  - ctor `(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)`
  - `Get()`, `Get(out T)` (returns `PooledObject<T>`), `Release(T)`, `ReleaseAll()`, `Clear()`, `CountInactive`, `ActiveItems`, `GetEnumerator()`.
  - Extension points: the four protected virtual `OnItem*` methods.
- `TemplateObjectPool<T>`: ctor `(T template, bool setAsLastSiblingOnGet = true, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)`. Overrides `OnItemGet` / `OnItemRelease`.
- `TemplateGameObjectObjectPool`: same shape for `GameObject`.
- `RandomPool<T>`: abstract; subclass it (with `[CreateAssetMenu]`) to get an asset type. Members: nested `Entry` struct (`item`, `weight` with `[Min(0f)]`), `Entries` (read-only list), `TotalWeight`, `Normalize()`, `TryGetRandom(out T)`, `GetRandom()` (throws `InvalidOperationException` when empty or all weights are zero), `GetRandomIndex()` (uses UnityEngine.Random; -1 if invalid), `GetRandomIndex(System.Random)` (deterministic variant), protected virtual `OnValidate()`. Serialized private fields `autoNormalize` (default true) and `entries`; the editor finds them by those names.
- `Observable<T>`: delegate `ObservableValueChangedDelegate(T oldValue, T newValue)`, event `ValueChanged`, property `Value`, ctors `()` and `(T value)`, `GetValue()` override. `Observable.GetValue()` is abstract.
- `SceneRef`: `FullPath`. The package has no scene-loading helper for it.

Namespace `Utilities.Extensions` (all static classes):
- `ObservableExtensions`: `WaitFor(Observable<bool>, bool state, CancellationToken)`; `WaitUntil` and `WaitWhile` (predicate overloads) for bool, float, int; `WaitUntilHigherThan` and `WaitUntilLowerThan` for float and int. All return `Awaitable` and poll once per frame via `Awaitable.EndOfFrameAsync`. `Awaitable` needs Unity 2023.1+ (satisfied by the 6000.0 minimum).
- `EnumerableExtensions`: `ForEach`, `IndexOf`, `GetRandom` on `IEnumerable<T>`.
- `CollectionExtensions`: `GetRandom` on `ICollection<T>`, `ClearWithGameObjectDestroy` for `ICollection<T>` where `T : Component` and for `ICollection<GameObject>`.
- `FloatExtensions` (defined in `NumericTypeExtensions.cs`): `Clamp`, `Clamp01`, `Map`, `Round`, `RoundToNearest` (float and int overloads), `Sign`, `Abs`, `Sqrt`, `Pow`, `Log` (two overloads), `Ceil`, `Floor`, `RoundToInt`, `CeilToInt`, `FloorToInt`, `Lerp`, `Min`, `Max`, `ToRadians`, `ToDegrees`.
- `Vector2Extensions`: `WithX`, `WithY`. `Vector3Extensions`: `WithX`, `WithY`, `WithZ`.
- `ButtonExtensions` (UnityEngine.UI `Button`): `AddClickListener`, `RemoveClickListener`, `RemoveAllClickListeners`.

Namespace `Utilities.FastConditions`: `All.IsNull(params object[])`, `All.IsNotNull(...)`, `Any.IsNull(...)`, `Any.IsNotNull(...)`. They use `is null`, so they bypass Unity's overloaded `==` for destroyed objects (assumed intentional).

## 4. Dependencies

- package.json declares none.
- Runtime asmdef: `references` is empty, `overrideReferences: true` with no `precompiledReferences`, `autoReferenced: true`, no `defineConstraints`, no `versionDefines`.
- Implicit use: Unity engine core (`UnityEngine.Pool`, `Awaitable`), uGUI (`UnityEngine.UI.Button` in `ButtonExtensions`), and `JetBrains.Annotations` (`[MustDisposeResource]` in `ObjectPool.cs`). How the uGUI and annotations references resolve under this asmdef is not verified (assumed to work through Unity's default references).
- Editor asmdef references only `com.fixer33.utilities`; platform Editor.
- Define symbols: the package defines none. `RandomPoolEditor` contains `#if PACKAGES_DEV`, which switches the stylesheet load path to `Assets/Packages/com.fixer33.utilities/Editor/RandomPoolEditor.uss`; `PACKAGES_DEV` is set by the developing project, not by this package; keeping this `#if` as is is confirmed by the author.
- No other com.fixer33.* package is required.

## 5. Editor tools and menu items

All under the Project window Assets menu:
- `Assets/Fixer33/Extract Animations/Export` (`FBXExport.AnimationClipsExport`): for each selected model asset (e.g. FBX) copies its embedded `AnimationClip`s (skipping names containing "preview") into `.anim` files in a folder chosen via folder panel; overwrites existing clips with the same name; remembers the last folder in EditorPrefs key `last_selected_fbx_export_path`. Valid when the first selected object is not a folder.
- `Assets/Fixer33/Samples/Extract folder contents` (`SamplesExtraction`): valid for exactly one folder matching `Assets/Samples/<name>/<version>/<name>`. Moves all files into `Assets/` (preserving subfolders), prompting on conflicts (replace / abort / skip), then deletes the sample folder. Intended to flatten an imported UPM sample (assumed).
- `Assets/Fixer33/Sub-assets/Set selected object as target asset`, `.../Add selected assets to target asset` (only valid within 60 seconds of setting the target), `.../Add selected assets to asset by file dialog`, `.../Extract Assets` (`SubAssetCreating`): turn selected assets into sub-assets of another asset, or extract sub-assets back into standalone `.asset` files. The two add-variants delete the original asset files.

Inspectors and drawers:
- `RandomPoolEditor` (`[CustomEditor(typeof(RandomPoolBase), true)]`, UI Toolkit): toolbar with "+ Add Item", "Normalize (100%)", "Auto Normalize" toggle and a total-percentage label; a list with item field, weight slider/number and Remove button; help box. Styled by `Editor/RandomPoolEditor.uss`. It invokes `Normalize` by reflection.
- `SceneNameDrawer` (`[CustomPropertyDrawer(typeof(SceneRef))]`, UI Toolkit): dropdown of enabled scenes from Build Settings; stores the scene path in `_fullPath`.
- `ObservableValueVisualizer` (`[InitializeOnLoadMethod]`): adds a "Show observable values" / "Hide observable values" button to the GameObject inspector header; lists `Observable` fields (public or private, including auto-property backing fields) of the object's `Behaviour` components with live values in Play Mode.

## 6. Samples and tests

None. The package has no `Samples~`, tests, or documentation other than the files in this folder. The README has short usage snippets for `TemplateObjectPool<T>` and `RandomPool<T>` only.

## 7. How other projects consume it

Added in the Unity Package Manager by Git URL `https://github.com/Fixer33/UtilitiesPackage.git`. Types are used via `using Utilities;`, `using Utilities.Extensions;`, `using Utilities.FastConditions;`. Assemblies that use asmdefs must reference `com.fixer33.utilities`. It may also be installable through the `com.fixer33.package-installer` package (assumed; that package's Git URL is not stated in this package).

## 8. Current state

- Functionally complete and small; no TODO/FIXME comments in the code.
- Observable is non-serializable: the commented-out `[Serializable]` and `[SerializeField]` in `Observable.cs` are dead code (confirmed by the author); serialization is not supported and not planned.
- The CHANGELOG lists only 1.3.0 and 1.0.0 (dates in reverse order) and does not mention the extension classes, `SceneRef`, `All`/`Any`, or the Asset-menu tools.
- README covers only part of the API.
