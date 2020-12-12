using System.Collections.Generic;
using UnityEngine;

public class PluginExample : MVRScript
{
    private JSONStorableAction _actionBindingExampleJSON;

    public override void Init()
    {
        _actionBindingExampleJSON = new JSONStorableAction(
            "my-action-binding",
            () => SuperController.LogMessage($"Hi from {containingAtom.name}!")
        );

        SuperController.singleton.BroadcastMessage("OnActionsProviderAvailable", this, SendMessageOptions.DontRequireReceiver);
    }

    public void OnDestroy()
    {
        SuperController.singleton.BroadcastMessage("OnActionsProviderDestroyed", this, SendMessageOptions.DontRequireReceiver);
    }


    // This method will be called whenever a keybinding plugin is initialized. List all available bindings here.
    public void OnBindingsListRequested(List<object> bindings)
    {
        bindings.Add(_actionBindingExampleJSON);
    }
}