# Virt-A-Mate keybindings interoperability specifications

Specifications to implement keybindings in plugins, or building plugins to handle shortcuts

## Sample implementation

See the [samples/](samples/) folder.

## Basic implementation (bindings provider)

```csharp
public class Receiver : MVRScript
{
    public void Init()
    {
        // Call this when ready to receive shortcuts or when shortcuts have changed
        SuperController.singleton.BroadcastMessage("OnActionsProviderAvailable", this, SendMessageOptions.DontRequireReceiver);

    }

    public void OnDestroy()
    {
        // Call this when this plugin should not receive shortcuts anymore
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
    }

    // This method will be called when a shortcuts plugin is started
    public void OnBindingsListRequested(List<object> bindings)
    {
        // Custom binding metadata, provided by the shortcuts plugin
        bindings.Add(new Dictionary<string, string>
        {
            // For example, Keybindings supports providing a namespace value:
            { "Namespace", "MySuperPlugin" }
        });
        // Add JSONStorable bindings (see Supported action types)
        bindings.Add(new JSONStorableAction("MyAction", () => SuperController.LogMessage("Hi!")));
    }
}
```

## Supported action types

| Map/JSONStorable | Action | Bool   | Float              |
| ---------------- | ------ | ------ | ------------------ |
| Key Down         | Invoke | False  | -1 or 1            |
| Key Up           | -      | True   | 0                  |
| Thumbstick       | -      | -      | -1 .. 1            |

## Projects using this

### Keybindings plugins

- [Keybindings by Acidbubbles](https://github.com/acidbubbles/vam-vimvam)

### Plugins compatible with keybindings

- [Timeline by Acidbubbles](https://github.com/acidbubbles/vam-timeline)
- [Embody by Acidbubbles](https://github.com/acidbubbles/vam-embody) (under development)
