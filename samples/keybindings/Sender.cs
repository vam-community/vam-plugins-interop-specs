using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sender : MVRScript
{
    private readonly List<Receiver> _receivers = new List<Receiver>();

    public override void Init()
    {
        try
        {
            AcquireAllAvailableBroadcastingPlugins();
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Receiver)}.{nameof(Init)}: {e}");
        }
    }

    public void AcquireAllAvailableBroadcastingPlugins()
    {
        foreach (var atom in SuperController.singleton.GetAtoms())
        {
            foreach (var storable in atom.GetStorableIDs().Select(id => atom.GetStorableByID(id)).Where(s => s is MVRScript))
            {
                TryRegister(storable);
            }
        }
    }

    public void OnActionsReceiverAvailable(JSONStorable storable)
    {
        var existing = _receivers.FirstOrDefault(r => r.storable == storable);
        if (existing != null) _receivers.Remove(existing);
        TryRegister(storable);
    }

    private void TryRegister(JSONStorable storable)
    {
        var bindings = new List<object>();
        storable.SendMessage("OnBindingsListRequested", bindings, SendMessageOptions.DontRequireReceiver);
        if (bindings.Count > 0)
        {
            var actions = new List<JSONStorableAction>();
            var floats = new List<JSONStorableFloat>();
            foreach (var binding in bindings)
            {
                if (!(TryAdd(actions, binding) || TryAdd(floats, binding)))
                    SuperController.LogError($"{nameof(Sender)}: Received unknown binding type {binding.GetType()} from {storable.name} in atom {storable.containingAtom?.name ?? "(no containing atom)"}.");
            }
            _receivers.Add(new Receiver
            {
                storable = storable,
                actions = actions,
                floats = floats
            });
        }
    }

    private static bool TryAdd<T>(List<T> list, object binding) where T : class
    {
        var typed = binding as T;
        if (typed != null)
        {
            list.Add(typed);
            return true;
        }
        return false;
    }

    public void Update()
    {
        try
        {
            ProcessKeys();
        }
        catch (Exception e)
        {
            SuperController.LogError($"{nameof(Receiver)}.{nameof(Update)}: {e}");
        }
    }

    // This is a simplistic implementation of a keybinding plugin
    // We just map keys 1-9 to a plugin's first action in the order
    // they were registered
    private void ProcessKeys()
    {
        if(_receivers.Count == 0) return;

        if (Input.anyKeyDown)
        {
            for (var i = 0; i < _receivers.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    var receiver = _receivers[i];
                    if (receiver.actions.Count > 0 && ValidateReceiver(receiver))
                        receiver.actions[0].actionCallback.DynamicInvoke();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var receiver = _receivers[0];
            if (receiver.floats.Count > 0 && ValidateReceiver(receiver))
                receiver.floats[0].val = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            var receiver = _receivers[0];
            if (receiver.floats.Count > 0 && ValidateReceiver(receiver))
                receiver.floats[0].val = 0f;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var receiver = _receivers[0];
            if (receiver.floats.Count > 0 && ValidateReceiver(receiver))
                receiver.floats[0].val = 1f;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            var receiver = _receivers[0];
            if (receiver.floats.Count > 0 && ValidateReceiver(receiver))
                receiver.floats[0].val = 0f;
        }
    }

    private bool ValidateReceiver(Receiver receiver)
    {
        if (receiver.storable == null)
        {
            _receivers.Remove(receiver);
            SuperController.LogError($"{nameof(Sender)}: The receiver does not exist anymore.");
            return false;
        }
        if (!receiver.storable.isActiveAndEnabled)
        {
            SuperController.LogError($"{nameof(Sender)}: The receiver {receiver.storable.containingAtom?.name ?? "(unspecified)"}/{receiver.storable.name} is disabled.");
            return false;
        }
        return true;
    }

    private class Receiver
    {
        public JSONStorable storable;
        public List<JSONStorableAction> actions;
        public List<JSONStorableFloat> floats;
    }
}