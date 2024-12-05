using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class Election : NetworkBehaviour
{
    //public NetworkVariable<bool> isLeader = new NetworkVariable<bool>(false);
    private bool isLeader = false;
    GameController gc;
    private Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //gc.StartVisualization(3);
        //Debug.Log("OwnerClientId: " + OwnerClientId);
        //IReadOnlyList<ulong> test = NetworkManager.ConnectedClientsIds;

        startButton = GameObject.FindWithTag("StartButton").GetComponent<Button>();
        
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    void OnStartButtonClick()
    {
        // Only the server can do this
        if(IsServer)
        {
            startButton.gameObject.SetActive(false);
            Debug.Log("Number of client IDs" + NetworkManager.ConnectedClientsIds.Count);
        }

        // TODO: Set the leader
        //DoSomethingServerSide(idCount, 1);
        ulong idCount = (ulong) NetworkManager.ConnectedClientsIds.Count - 1;
        if(OwnerClientId == idCount)
        {
            if(IsOwner)
            {
                isLeader = true;
                Debug.LogFormat("GameObject: {0} isLeader is set to: {1}", OwnerClientId, isLeader);
            }
        }
        else{
            Debug.Log("Failed. Client ID: " + OwnerClientId);
        }

        // TODO: This is were the algorithm starts
    }

    public void startPuzzle()
    {
        // Triggered on button press when all clients and the host have been connected
        // Access the ConnectedClientsIds and assign the client with the highest id the role of leader
    }

    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.Space))
        {
            // TEST: Send message to the first client
            callFromClientServerRpc(1, 1);
        }
    }

    [ServerRpc]
    private void callFromClientServerRpc(ulong clientId, int methodId)
    {
        DoSomethingServerSide(clientId, methodId);
    }

    private void DoSomethingServerSide(ulong clientId, int methodId)
    {
        // If not the Server/Host then return
        if (!IsServer) return;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[]{clientId}
            }
        };

        // Test
        SetLeaderClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void SetLeaderClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        //isLeader = true;
        // PLEASE WORK PLEASE PLEASE WORK PLEASE
        // GAHHHHHHHHH HOUGHH OH MY DAYS
        // NOOOOOOOOOOOOO
        Debug.LogFormat("GameObject: {0} isLeader is set to: {1}", OwnerClientId, isLeader);
    }
}
