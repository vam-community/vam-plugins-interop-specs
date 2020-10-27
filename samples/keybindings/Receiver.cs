using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Receiver : MVRScript
{
    public override void Init()
    {
        try
        {
            // Initialize your plugin here

            BroadcastActionsAvailable();
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Receiver)}.{nameof(Init)}: {e}");
        }
    }

    // This is a specialized Broadcast that targets Virt-A-Mate plugins
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

    // This method will be called whenever a keybinding plugin is initialized. List all available bindings here.
    public void OnBindingsListRequested(List<object> bindings)
    {
        var actionBindingExampleJSON = new JSONStorableAction("my-binding", () => SuperController.LogMessage($"{containingAtom?.name} received action: my-binding"));
        bindings.Add(actionBindingExampleJSON);
    }
}