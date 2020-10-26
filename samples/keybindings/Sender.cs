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
        var actions = new List<string>();
        storable.SendMessage("OnActionsListRequested", actions, SendMessageOptions.DontRequireReceiver);
        if (actions.Count > 0)
        {
            _receivers.Add(new Receiver
            {
                storable = storable,
                actions = actions
            });
        }
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
        if (!Input.anyKeyDown) return;

        for (var i = 0; i < _receivers.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                var receiver = _receivers[i];
                SendAction(receiver, receiver.actions[0]);
            }
        }
    }

    private void SendAction(Receiver receiver, string action)
    {
        if (receiver.storable == null)
        {
            _receivers.Remove(receiver);
            SuperController.LogError($"{nameof(Sender)}: The receiver does not exist anymore.");
            return;
        }
        if (!receiver.storable.isActiveAndEnabled)
        {
            SuperController.LogError($"{nameof(Sender)}: The receiver {receiver.storable.containingAtom?.name ?? "(unspecified)"}/{receiver.storable.name} is disabled.");
            return;
        }
        receiver.storable.SendMessage("OnActionTriggered", action, SendMessageOptions.RequireReceiver);
    }

    private class Receiver
    {
        public JSONStorable storable;
        public List<string> actions;
    }
}