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
        BroadcastActionsAvailable();
    }

    // This method should be called whenever the plugin is ready to receive shortcuts bindings
    public void BroadcastActionsAvailable()
    {
        foreach (var atom in SuperController.singleton.GetAtoms())
        {
            foreach (var storable in atom.GetStorableIDs().Select(id => atom.GetStorableByID(id)).Where(s => s is MVRScript))
            {
                storable.SendMessage("OnActionsReceiverAvailable", this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // This method will be called when a binding executor is launched
    public void OnBindingsListRequested(List<object> bindings)
    {
        bindings.Add(new JSONStorableAction());
    }
}
```

## Supported action types

| Map/JSONStorable | Action | Bool   | Float              | Vector3   | StringChooser |
| ---------------- | ------ | ------ | ------------------ | --------- | ------------- |
| Key Down         | Invoke | Toggle | 1                  | -         | Next          |
| Key Up           | -      | -      | 0                  | -         | -             |
| Mouse Down       | Invoke | Toggle | 1                  | -         | Next          |
| Mouse Up         | -      | -      | 0                  | -         | -             |
| Mouse Wheel Up   | Invoke | True   | -                  | -         | Previous      |
| Mouse Wheel Down | Invoke | False  | -                  | -         | Next          |
| Thumbstick       | -      | -      | Normalized Average | (x, y, 0) | -             |
