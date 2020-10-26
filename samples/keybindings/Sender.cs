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