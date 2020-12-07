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
        // Call this when ready to receive shortcuts
        Broadcast("OnActionsProviderAvailable");
    }

    public void OnDestroy()
    {
        // Call this when this plugin should not receive shortcuts anymore
        Broadcast("OnActionsProviderDestroyed");
    }

    // This is a more efficient way to broadcast than the built-in Unity method
    public void Broadcast(string method)
    {
        foreach (var atom in SuperController.singleton.GetAtoms())
        {
            foreach (var storable in atom.GetStorableIDs().Select(id => atom.GetStorableByID(id)).Where(s => s is MVRScript))
            {
                storable.SendMessage(method, this, SendMessageOptions.DontRequireReceiver);
            }
        }
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
