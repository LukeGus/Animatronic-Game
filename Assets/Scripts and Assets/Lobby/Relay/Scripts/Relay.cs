using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;

public class Relay : MonoBehaviour
{
    public static Relay Instance { get; private set; }

    private async void Start()
    {
        Instance = this;
        
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
        };
        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            // The number at the end of this line signifies the max amount of players allowed in a relay.
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);

            // You can use "dtls" or "udp" or "wss"
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            if (!NetworkManager.Singleton.IsHost)
                NetworkManager.Singleton.StartHost();

            LobbyManager.Instance.ReadyPlayerServerRpc();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            if (!NetworkManager.Singleton.IsClient)
                NetworkManager.Singleton.StartClient();
            
            LobbyManager.Instance.ReadyPlayerServerRpc();
            
            Debug.Log("Joined relay with " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}