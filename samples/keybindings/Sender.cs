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

    public void Update()
    {
        // This is a simplistic implementation of a keybinding plugin
        if (!Input.anyKeyDown) return;

        for (var i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                var receiver = _receivers.ElementAtOrDefault(i);
                if (receiver != null)
                {
                    SendAction(receiver, receiver.actions[0]);
                }
            }
        }
    }

    private void SendAction(Receiver receiver, string action)
    {
        if (receiver.storable == null)
        {
            _receivers.Remove(receiver);
            return;
        }
        receiver.storable.SendMessage("OnActionTriggered", action, SendMessageOptions.RequireReceiver);
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

    public void ActionsAvailable(JSONStorable storable)
    {
        var existing = _receivers.FirstOrDefault(r => r.storable == storable);
        if (existing != null) _receivers.Remove(existing);
        TryRegister(storable);
    }

    private void TryRegister(JSONStorable storable)
    {
        var actions = new List<string>();
        storable.SendMessage("PublishActions", this, SendMessageOptions.DontRequireReceiver);
        if (actions.Count > 0)
        {
            _receivers.Add(new Receiver
            {
                storable = storable,
                actions = actions
            });
        }
    }

    private class Receiver
    {
        public JSONStorable storable;
        public List<string> actions;
    }
}