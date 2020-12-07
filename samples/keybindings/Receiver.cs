using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Receiver : MVRScript
{
    private JSONStorableAction _actionBindingExampleJSON;
    private JSONStorableFloat _floatBindingExampleJSON;

    public override void Init()
    {
        try
        {
            _actionBindingExampleJSON = new JSONStorableAction(
                "my-action-binding",
                Log
            );

            _floatBindingExampleJSON = new JSONStorableFloat(
                "my-float-binding",
                0f,
                Log,
                -1f,
                1f
            );

            Broadcast("OnActionsProviderAvailable");
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Receiver)}.{nameof(Init)}: {e}");
        }
    }

    public void OnDestroy()
    {
        Broadcast("OnActionsProviderDestroyed");
    }

    private void Log()
    {
        SuperController.LogMessage($"{containingAtom?.name} received action: my-action-binding");
    }

    private void Log(float val)
    {
        SuperController.LogMessage($"{containingAtom?.name} received float: my-float-binding = {val}");
    }

    // This is a specialized Broadcast that targets Virt-A-Mate plugins
    private void Broadcast(string method)
    {
        foreach (var atom in SuperController.singleton.GetAtoms())
        {
            foreach (var storable in atom.GetStorableIDs().Select(id => atom.GetStorableByID(id)).Where(s => s is MVRScript))
            {
                storable.SendMessage(method, this, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // This method will be called whenever a keybinding plugin is initialized. List all available bindings here.
    public void OnBindingsListRequested(List<object> bindings)
    {
        bindings.Add(_actionBindingExampleJSON);
        bindings.Add(_floatBindingExampleJSON);
    }
}