## [1.3.1] - 2026-11-20

### Changed
- Added package documentation: `Package Description`, `Usage Guide` and `Open Questions` in the `Documentation` folder.
- README now links to the package description and open questions.

## [1.3.0] - 2025-03-24

### Added
- `ObservableExtensions`: Added async waiting methods for `Observable<T>` (WaitUntil, WaitWhile, WaitUntilHigherThan, etc.).
- `TemplateObjectPool<T>` and `TemplateGameObjectObjectPool`: New specialized object pools that use a template object for instantiation.
- `RandomPool<T>`: A serializable weighted random pool ScriptableObject.
- `ObservableValueVisualizer`: Editor utility to visualize observable values in the inspector.
- `RandomPoolEditor`: Custom editor for the new `RandomPool` system.

## [1.0.0] - 2025-05-21

### Added
- Created first version of package