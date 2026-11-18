# com.fixer33.utilities - Usage Guide

Package "Utilities" 1.3.0, Unity 6000.0+. Namespaces: `Utilities`, `Utilities.Extensions`, `Utilities.FastConditions` (runtime), `Utilities.Editor` (editor). Things marked (assumed) are inferred, not read directly from code.

## 1. Installation

### Via com.fixer33.package-installer (preferred)
1. Install the `com.fixer33.package-installer` package first (it is not installed by this package).
2. Open the installer window from the menu `Tools/Fixer33/Package Installer` (this is the path in the installer's code; older notes may say Window > Fixer33).
3. Find "Utilities" (id `com.fixer33.utilities`) in the "Tools" group and install it.

### Manually by Git URL
Package Manager > **+** > **Add package from git URL**:
```
https://github.com/Fixer33/UtilitiesPackage.git
```
No other package is required (package.json declares no dependencies).

## 2. Minimal setup

- If your code is in an asmdef, add a reference to `com.fixer33.utilities`. Code in the default `Assembly-CSharp` sees it automatically (the asmdef has `autoReferenced: true`).
- Add the namespaces you need:
```csharp
using Utilities;
using Utilities.Extensions;
using Utilities.FastConditions;
```
- No scene objects, prefabs or settings assets are needed. The package has no runtime state or singletons.

## 3. Using the public API

### 3.1 Object pools
`ObjectPool<T>` (T is a class) wraps `UnityEngine.Pool.ObjectPool<T>` and tracks borrowed items in `ActiveItems`.

```csharp
var pool = new ObjectPool<Bullet>(
    createFunc: () => new Bullet(),
    actionOnGet: b => b.Reset());

Bullet b = pool.Get();          // borrow
pool.Release(b);                // return
pool.ReleaseAll();              // return everything in ActiveItems
int idle = pool.CountInactive;
foreach (var active in pool) { }   // enumerates active items
pool.Clear();                   // destroy pooled (inactive) items
```
`Get(out T item)` returns a `PooledObject<T>` you can dispose (for example with `using`) to release the item.

`TemplateObjectPool<T>` (T : Component) clones a template component next to the template (same parent), activates clones on `Get` and deactivates them on `Release` (README example):
```csharp
public MyComponent prefab;
private TemplateObjectPool<MyComponent> pool;

void Start() { pool = new TemplateObjectPool<MyComponent>(prefab); }
void Spawn()  { var item = pool.Get(); /* use */ pool.Release(item); }
```
`TemplateGameObjectObjectPool` does the same for a `GameObject` template. The second constructor argument `setAsLastSiblingOnGet` (default `true`) moves a fetched item to the end of its parent's children, useful for lists in UI layouts.

### 3.2 Weighted random pool (RandomPool<T>)
1. Create a concrete asset type (the base class is abstract):
```csharp
[CreateAssetMenu]
public class MyItemPool : RandomPool<MyItem> {}
```
2. Create the asset (Assets > Create > MyItemPool), then in the inspector use "+ Add Item", set weights; "Normalize (100%)" and the "Auto Normalize" toggle are on the toolbar.
3. Pick in code:
```csharp
public MyItemPool pool;

void Pick()
{
    if (pool.TryGetRandom(out var item)) { /* use item */ }
    // or: var item = pool.GetRandom();   // throws InvalidOperationException if empty / all weights zero
    // int i = pool.GetRandomIndex();     // -1 when invalid
    // int j = pool.GetRandomIndex(new System.Random(42));  // deterministic
}
```
Other members: `Entries` (read-only list of `Entry { item, weight }`), `TotalWeight`, `Normalize()`.

### 3.3 Observable<T>
```csharp
var health = new Observable<int>(100);
health.ValueChanged += (oldValue, newValue) => Debug.Log($"{oldValue} -> {newValue}");
health.Value = 90;   // fires the event
health.Value = 90;   // no event: value is Equal to the old one
```
Awaiting (needs `using Utilities.Extensions;`, returns `Awaitable`, polls once per frame):
```csharp
async Awaitable Run(Observable<bool> ready, Observable<float> progress, CancellationToken ct)
{
    await ready.WaitFor(true, ct);
    await progress.WaitUntilHigherThan(0.99f, cancellationToken: ct);
}
```
Available: `WaitFor` (bool only), `WaitUntil` / `WaitWhile` (predicate, for bool, float, int), `WaitUntilHigherThan` / `WaitUntilLowerThan` (float, int). `Observable<T>` is not serializable and does not show in the inspector as a field (the commented-out serialization attributes are dead code, confirmed by the author).

### 3.4 SceneRef
A serializable struct with a path string. Use it as a field and read `FullPath`; the inspector shows a dropdown of enabled Build Settings scenes.
```csharp
[SerializeField] private SceneRef nextScene;
void Go() => UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene.FullPath);
```
The package has no loader of its own; the `LoadScene` call above is plain Unity (whether a path loads is up to Build Settings).

### 3.5 Extension methods
- Enumerable / collections: `list.ForEach(x => ...)`, `list.IndexOf(item)`, `list.GetRandom()`, `components.ClearWithGameObjectDestroy()` (destroys GameObjects, then clears the collection).
- Numbers (`FloatExtensions`): `Clamp`, `Clamp01`, `Map`, `Round(digits)`, `RoundToNearest`, `Sign`, `Abs`, `Sqrt`, `Pow`, `Log`, `Ceil`, `Floor`, `RoundToInt`, `CeilToInt`, `FloorToInt`, `Lerp`, `Min`, `Max`, `ToRadians`, `ToDegrees`.
- Vectors: `v.WithX(1f)`, `v.WithY(...)`, and `WithZ` for `Vector3`.
- Buttons (`UnityEngine.UI.Button`): `AddClickListener`, `RemoveClickListener`, `RemoveAllClickListeners`.
- Null checks (`Utilities.FastConditions`): `All.IsNull(a, b)`, `All.IsNotNull(a, b)`, `Any.IsNull(a, b)`, `Any.IsNotNull(a, b)`.

## 4. Editor windows and menu paths

No standalone editor windows. Everything is in the Project window's **Assets** menu or in inspectors.

| Menu / UI | What it does |
|---|---|
| `Assets/Fixer33/Extract Animations/Export` | For selected model assets (e.g. FBX), copies embedded animation clips (names containing "preview" are skipped) to `.anim` files in a folder you choose. Overwrites clips of the same name. Remembers the last folder. |
| `Assets/Fixer33/Samples/Extract folder contents` | Enabled for one folder matching `Assets/Samples/<name>/<version>/<name>`. Moves its contents into `Assets/`, asks on conflicts (replace / abort / skip), deletes the sample folder. |
| `Assets/Fixer33/Sub-assets/Set selected object as target asset` | Marks the selected asset as parent. |
| `Assets/Fixer33/Sub-assets/Add selected assets to target asset` | Adds the selection as sub-assets; only enabled within 60 seconds of setting the target. |
| `Assets/Fixer33/Sub-assets/Add selected assets to asset by file dialog` | Same, with the parent picked in a file dialog. |
| `Assets/Fixer33/Sub-assets/Extract Assets` | Turns sub-assets back into standalone `.asset` files. |
| RandomPool inspector | Custom UI Toolkit inspector for every `RandomPool<T>` asset. |
| SceneRef field | Scene dropdown (drawer class `SceneNameDrawer`). |
| GameObject inspector header | Button "Show observable values" / "Hide observable values": lists `Observable` fields of the object's `Behaviour` components with live values in Play Mode. |

## 5. Configuration and scripting defines

- The package has no settings asset and defines no scripting symbols.
- `PACKAGES_DEV` is read (not defined) by `RandomPoolEditor`: when set, the stylesheet is loaded from `Assets/Packages/com.fixer33.utilities/Editor/RandomPoolEditor.uss`. It is meant for developing the package inside a project; normal users do not set it (author confirmed this is intentional and stays).
- Per-item pool behaviour is configured through constructor arguments (`collectionCheck`, `defaultCapacity` 10, `maxSize` 10000).
- Editor preference key `last_selected_fbx_export_path` stores the last animation export folder.

## 6. Extension points and a worked example

- `ObjectPool<T>`: override the protected virtual `OnItemCreate`, `OnItemGet`, `OnItemRelease`, `OnItemDestroy`.
- `RandomPool<T>`: subclass for a concrete type; override protected virtual `OnValidate()` (call `base.OnValidate()`).
- `Observable`: abstract base with `GetValue()`; custom subclasses are shown by the inspector visualizer (assumed, since it finds fields of type `Observable`).

Worked example: a pool of UI rows that plays a sound on spawn and reset text on release.
```csharp
public class RowPool : TemplateObjectPool<RowView>
{
    public RowPool(RowView template) : base(template, setAsLastSiblingOnGet: true) { }

    protected override void OnItemGet(RowView item)
    {
        base.OnItemGet(item);        // keep activation and sibling ordering
        item.PlaySpawnSound();
    }

    protected override void OnItemRelease(RowView item)
    {
        item.ClearText();
        base.OnItemRelease(item);    // deactivates the object
    }
}
```
Calling `base` matters: the template pools override these hooks to activate and deactivate the object.
(`RowView`, `PlaySpawnSound`, `ClearText` are your own code.)

## 7. Common pitfalls (current code)

- Template pools deactivate the template object itself in their constructor. Do not reuse a scene object as a template if it must stay active.
- Clones are created under the template's parent, so a template without a sensible parent will spawn at the scene root.
- `RandomPool<T>.GetRandom()` throws on an empty pool or when all weights are zero; `TryGetRandom` returns false. `CollectionExtensions.GetRandom` returns `default` on an empty collection; `EnumerableExtensions.GetRandom` on an empty sequence calls `Random.Range(0, 0)`, behaviour not verified here, and it enumerates the sequence twice, so avoid it on non-repeatable sequences.
- The `Observable<T>` wait helpers poll each frame rather than subscribing to `ValueChanged`; there is no `WaitFor` for float or int.
- `Observable<T>` is not serializable; do not expect it in the inspector or saved assets.
- `All`/`Any` use `is null` and therefore ignore Unity's overloaded `==`: a destroyed `UnityEngine.Object` is NOT reported as null.
- `SceneRef` stores only a path. A scene that is not enabled in Build Settings shows as empty in the dropdown, and `FullPath` may be stale.
- The two "Add selected assets" sub-asset commands delete the original asset files; whether references from other assets survive was not verified. Back up or use version control first.
- `Extract folder contents` only accepts the pattern `Assets/Samples/.+/[\d\.]+/[\w\d ]+$` and deletes the source sample folder afterwards.
- The runtime asmdef uses `overrideReferences: true` and the code uses `UnityEngine.UI.Button` and `JetBrains.Annotations`; setups without the uGUI package or those annotations may fail to compile (assumed; not verified).
- `ObjectPool<T>.ReleaseAll()` copies the active list, releases each item, then clears the list; callbacks run once per item.
- The README and CHANGELOG cover only part of the API (for example extensions, `SceneRef`, `All`/`Any`, Asset-menu tools are not listed).

## 8. Other com.fixer33.* packages

This package does not depend on, and is not depended on by, any other package that could be determined from inside it. Related packages, by name and Git URL only:
- `com.fixer33.package-installer` - installs this package.
- `com.fixer33.observable-variables` - https://github.com/Fixer33/ObservableVariablesPackage.git
- `com.fixer33.easy-events` - https://github.com/Fixer33/EasyEventsPackage.git
- `com.fixer33.modular-console` - https://github.com/Fixer33/ModularConsolePackage.git

(The installer entry for this package: https://github.com/Fixer33/UtilitiesPackage.git. Whether the other packages interoperate with it is not stated in this package; assumed no direct link.)

## 9. Remaining open questions

Open questions that affect usage:
- Q3: Are the uGUI and `JetBrains.Annotations` references guaranteed on all consumer setups given `overrideReferences: true`?
- Q7: Is polling in the `Observable` wait helpers deliberate, and why only `bool` has `WaitFor`?
- Q8: Is the different empty-input behaviour of the random helpers (throw / default / `Random.Range(0, 0)`) and the double enumeration intended?
- Q10: Is deactivating a scene template object in the template pools intended?
- Q11: Is a runtime scene-loading helper for `SceneRef` planned?
- Q12: Do references from other assets survive `SubAssetCreating` moves and deletions?
- Q13: Is `SamplesExtraction` meant only for the author's sample workflow?
- Q16: Which packages consume this one cannot be determined from inside the package.
