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
        SuperController.singleton.BroadcastMessage(OnActionsProviderAvailable, this, SendMessageOptions.DontRequireReceiver);

    }

    public void OnDestroy()
    {
        // Call this when this plugin should not receive shortcuts anymore
        SuperController.singleton.BroadcastMessage(OnActionsProviderDestroyed, this, SendMessageOptions.DontRequireReceiver);
    }

    // This method will be called when a shortcuts plugin is started
    public void OnBindingsListRequested(List<object> bindings)
    {
        bindings.Add(new JSONStorableAction("MyAction", () => SuperController.LogMessage("Hi!")));
    }
}
```

## Supported action types

| Map/JSONStorable | Action | Bool   | Float              | Vector3   | StringChooser |
| ---------------- | ------ | ------ | ------------------ | --------- | ------------- |
| Key Down         | Invoke | Toggle | 1                  | -         | Next          |
| Key Up           | -      | -      | 0                  | -         | -             |
| Thumbstick       | -      | -      | Normalized Average | (x, y, 0) | -             |

## Projects using this

### Keybindings plugins

- [VimVam by Acidbubbles](https://github.com/acidbubbles/vam-vimvam) (under development)

### Plugins compatible with keybindings

- [Timeline by Acidbubbles](https://github.com/acidbubbles/vam-timeline) (under development)
