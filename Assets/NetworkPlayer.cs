using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem.XR;

public class NetworkPlayer : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawned");
        DisableClientInput();
    }

    public void DisableClientInput()
    {
        if (IsClient && !IsOwner) 
        {
            var clientMoveProvider = GetComponent<NetworkMoveProvider>();
            var clientTurnProvider = GetComponent<ActionBasedSnapTurnProvider>();
            var clientControllers = GetComponentsInChildren<ActionBasedController>();
            var clientHead = GetComponentInChildren<TrackedPoseDriver>();
            var clientCamera = GetComponentInChildren<Camera>();

            clientCamera.enabled = false;
            clientMoveProvider.enableInputActions = false;
            clientTurnProvider.enableTurnLeftRight = false;
            clientTurnProvider.enableTurnAround = false;
            clientHead.enabled = false;

            foreach (var controller in clientControllers)
            {
                controller.enableInputActions = false;
                controller.enableInputTracking = false;
            }
        }
    }

    public void OnSelectGrabbable(SelectEnterEventArgs eventArgs) 
    {
        Debug.Log("Grabbed");
        if (IsClient && IsOwner) 
        {
            NetworkObject networkObjectSelected = eventArgs.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null) 
            {
                RequestGrabbableOwnershipServerRpc(OwnerClientId, networkObjectSelected);
                eventArgs.interactableObject.transform.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    [ServerRpc]
    public void RequestGrabbableOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.Log($"Unable to change owner to ClientId {newOwnerClientId}");
        }
    }
}
