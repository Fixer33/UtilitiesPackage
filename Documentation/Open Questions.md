# com.fixer33.utilities - Open Questions

## Release hygiene
1. CHANGELOG order and dates: 1.3.0 is dated 2025-03-24 and sits above 1.0.0 dated 2025-05-21 (the older version has the later date). Which is correct?
2. Entries for 1.1.x/1.2.x are missing although package.json says 1.3.0. The 1.3.0 entry lists RandomPool, TemplateObjectPool, ObservableExtensions and two editor tools, but not `SceneRef`, `SceneNameDrawer`, `FloatExtensions`, vector/collection/button extensions, `All`/`Any`, `FBXExport`, `SamplesExtraction` or `SubAssetCreating`. Were these added in other releases without log entries?

## Dependencies and build
3. The runtime asmdef sets `overrideReferences: true` with no precompiled references, and the code uses `JetBrains.Annotations` (`[MustDisposeResource]`) and `UnityEngine.UI.Button`. It compiles in the author's setup, but it is unclear whether those references are guaranteed on all consumer setups (for example when the uGUI package is absent). Intentional?

## API design
5. `NumericTypeExtensions.cs` contains only class `FloatExtensions` (file name suggests int/double too). Are other numeric types planned?
7. `ObservableExtensions` waits by polling each frame rather than subscribing to `ValueChanged`. Deliberate? Only `bool` has `WaitFor`.
8. Random helpers behave differently on empty input: `CollectionExtensions.GetRandom` returns `default`, `RandomPool<T>.GetRandom` throws, and `EnumerableExtensions.GetRandom` on an empty sequence calls `Random.Range(0, 0)`, whose result was not verified. `EnumerableExtensions.GetRandom` also enumerates twice, which may be wrong for non-repeatable sequences. Intended?
9. `ObjectPool<T>.ReleaseAll()` releases every active item and then clears `ActiveItems`, although the release callback already removes items. Redundant, harmless?
10. `TemplateObjectPool<T>` and `TemplateGameObjectObjectPool` deactivate the template GameObject itself. Is this intended for templates that are scene objects used elsewhere?
11. `SceneRef` only stores a path; no loader or validation exists, and the drawer lists only enabled Build Settings scenes (a stale value is silently shown empty). Is a runtime helper planned? `SceneNameDrawer` has an unused local `selectedName`, and the drawer class name differs from the type it draws.

## Editor tools
12. `SubAssetCreating` removes the original asset and calls `DeleteAsset` after adding it to the parent. Whether references from other assets survive was not verified.
13. `SamplesExtraction` requires the path pattern `Assets/Samples/.+/[\d\.]+/[\w\d ]+$`. Is it only for the author's own sample workflow? It also uses `UnityEngine.Device.Application` (device-simulator wrapper) instead of `UnityEngine.Application`; intentional?

## Documentation, samples, tests
14. No tests or samples exist. Are any planned (pool, random pool weights and observable waits are testable)?
15. README does not list extensions, `SceneRef`, `All`/`Any` or the Asset-menu tools. Should it?
16. Which packages consume this one (via package.json or asmdef) cannot be determined from within this package.

## Answered

- (was 4) `#if PACKAGES_DEV` in `RandomPoolEditor` stays as is. Author confirmed: it is intentional and is set by the development project, not by the package.
- (was 6) `Observable<T>` serialization (the commented-out `[Serializable]` / `[SerializeField]`) is dead code. Author confirmed. It is documented as dead in Package Description.md; removal of the commented lines is left to a later source change.
