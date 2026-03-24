# Utilities Package

![Unity](https://img.shields.io/badge/Unity-UPM%20Package-blue)
![GitHub](https://img.shields.io/github/license/Fixer33/UtilitiesPackage)

Provides essential Quality of Life (QoL) utilities for Unity development, including enhanced object pooling, weighted random pools, and observable extensions.

## Features

- **Object Pooling**: Enhanced `ObjectPool<T>` with support for templates and tracking of active items.
- **Random Pools**: Serializable, weighted random selection system with custom editor support.
- **Observable Extensions**: Asynchronous waiting methods for `Observable<T>` values (WaitUntil, WaitWhile, etc.).
- **Inspector Tools**: Visualization for `Observable` values directly in the Unity Inspector.

## Installation

### Using UPM (Unity Package Manager)

1. Open Unity and go to **Window > Package Manager**.
2. Click the **+** button and select **Add package from git URL**.
3. Enter the repository URL:
   ```
   https://github.com/Fixer33/UtilitiesPackage.git
   ```
4. Click **Add** and wait for Unity to install the package.

## Usage

### Object Pooling

```csharp
// Using a template component
public MyComponent prefab;
private TemplateObjectPool<MyComponent> pool;

void Start() {
    pool = new TemplateObjectPool<MyComponent>(prefab);
}

void Spawn() {
    var item = pool.Get();
    // Use item...
    pool.Release(item);
}
```

### Random Pool

1. Create a class inheriting from `RandomPool<T>`:
   ```csharp
   [CreateAssetMenu]
   public class MyItemPool : RandomPool<MyItem> {}
   ```
2. Create the asset and configure weights in the Inspector.
3. Pick items in code:
   ```csharp
   public MyItemPool pool;
   void Pick() {
       if (pool.TryGetRandom(out var item)) {
           // ...
       }
   }
   ```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
