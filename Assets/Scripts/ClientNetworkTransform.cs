using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    protected override void Update()
    {
        CanCommitToTransform = IsOwner;
        base.Update();
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager.IsConnectedClient || networkManager.IsListening) {
            if (CanCommitToTransform) {
                TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
            }
        }
    }

    protected override bool OnIsServerAuthoritatitive()
    {
        return false;
    }
}
