using UnityEngine;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.Relay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Relay/Relay")]
    public class RelaySO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        //[DropdownDefined("udp", "dtls", "wss")]
#endif
        //[Tooltip("Available options: upd, dtls, wss")]
        public bool isSecure = true;

        public async Task<string> CreateRelayAsync(int maxConnections)
        {
            try
            {
                Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

                Debug.Log($"[CreateRelay] JoinCode: {joinCode}");

                //RelayServerData relayServerData = new(alloc, connectionType);
                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(
                    alloc.RelayServer.IpV4, 
                    (ushort)alloc.RelayServer.Port, 
                    alloc.AllocationIdBytes, 
                    alloc.Key, 
                    alloc.ConnectionData, 
                    alloc.ConnectionData,   // Para el host se repite
                    isSecure                  // o "udp"
                );

                NetworkManager.Singleton.StartHost();

                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);

                return string.Empty;
            }
        }

        public async Task JoinRelayAsync(string joinCode)
        {
            try
            {
                JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

                Debug.Log($"[JoinRelay] JoinCode: {joinCode}");

                //RelayServerData relayServerData = new RelayServerData(alloc, connectionType);
                //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(
                    alloc.RelayServer.IpV4, 
                    (ushort)alloc.RelayServer.Port, 
                    alloc.AllocationIdBytes, 
                    alloc.Key, 
                    alloc.ConnectionData, 
                    alloc.ConnectionData,   // Para el host se repite
                    isSecure                  // o "udp"
                );

                NetworkManager.Singleton.StartClient();

            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);
            }
        }

        public void CleanUp()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}