using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class NetworkStartup : MonoBehaviour
{
    [SerializeField]
    private GameObject Desk;

    [SerializeField]
    private GameObject Cube;

    public int maxConnection = 5;
    public UnityTransport transport;
    public string joinCode;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Host()
    {

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log(newJoinCode);

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort) allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        
        Debug.Log("TryHost");
        
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager != null)
        {
            bool hostStarted = networkManager.StartHost();
            if (hostStarted)
            {
                Debug.Log("Host");
                Destroy(GameObject.Find("StartupXR"));
                Destroy(GameObject.Find("VR Canvas"));
                GameObject desk = Instantiate(Desk);
                GameObject cube = Instantiate(Cube);
                desk.GetComponent<NetworkObject>().Spawn();
                cube.GetComponent<NetworkObject>().Spawn();
            }
            else
            {
                Debug.Log("Failed");
            }
        }
    }
    public async void Join()
    {
        TMP_InputField joinCode = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode.text);


        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort) allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);


        Debug.Log("TryJoin");
        
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager != null)
        {
            bool clientStarted = networkManager.StartClient();
            Debug.Log(clientStarted);
            if (clientStarted)
            {
                Debug.Log("Joined");
                Destroy(GameObject.Find("StartupXR"));
                Destroy(GameObject.Find("VR Canvas"));
            }
            else
            {
                Debug.Log("Failed");
            }
        }
    }
}
