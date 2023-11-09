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

public class NetworkStartupCopy : MonoBehaviour
{
    [SerializeField]
    private GameObject Desk;

    [SerializeField]
    private GameObject Cube;

    private string serverIp = "196.168.1.10";
    private ushort serverPort = 7777;

    public void Host()
    {


        Debug.Log("TryHost");
        
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager != null)
        {
            networkManager.GetComponent<UnityTransport>().SetConnectionData(serverIp, serverPort, "0.0.0.0");

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
    public void Join()
    {
        Debug.Log("TryJoin");
        
        NetworkManager networkManager = NetworkManager.Singleton;

        if (networkManager != null)
        {
            networkManager.GetComponent<UnityTransport>().SetConnectionData(serverIp, serverPort);

            bool clientStarted = networkManager.StartClient();
            Debug.Log(clientStarted);
            if (clientStarted)
            {
                Debug.Log("Joined");
                // Destroy(GameObject.Find("StartupXR"));
                // Destroy(GameObject.Find("VR Canvas"));
            }
            else
            {
                Debug.Log("Failed");
            }
        }
    }
}
