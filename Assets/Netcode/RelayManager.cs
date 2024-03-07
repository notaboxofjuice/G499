using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
public class RelayManager : MonoBehaviour
{
    // Singleton Initialization
    public static RelayManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    /// <summary>
    /// Creates a relay server allocation and start a host
    /// </summary>
    /// <param name="maxConnections">The maximum amount of clients that can connect to the relay</param>
    /// <returns>The join code</returns
    public async Task<string> CreateRelay(int maxConnections = 3)
    {
        try
        {
            Debug.Log("Attempting to create Relay server");
            //Initialize the Unity Services engine
            await UnityServices.InitializeAsync();
            //Always authenticate your users beforehand
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                //If not already logged, log the user in
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // Request allocation and join code
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            // Start host
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        } catch (RelayServiceException e)
        {
            Debug.LogError("Failed to create relay allocation: " + e.Message);
            return null;
        }
    }
    /// <summary>
    /// Join a Relay server based on the JoinCode received from the Host or Server
    /// </summary>
    /// <param name="joinCode">The join code generated on the host or server</param>
    /// <returns>True if the connection was successful</returns>
    public async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            // Initialize the Unity Services engine
            await UnityServices.InitializeAsync();
            // Always authenticate your users beforehand
            if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // Join allocation
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            // Start client
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        } catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay allocation: " + e.Message);
            return false;
        }
    }
}