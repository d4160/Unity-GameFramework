using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RpcSendTestMono : NetworkBehaviour
{
    public static string[] Properties { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.C))
        {
            RPC_SendMessage("Hey Mate from Packages!");
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Rpc_UpdateRoomProperties(Runner, new string[] { "Hello ", "World!" });
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        //if (_messages == null)
        //    _messages = FindObjectOfType<Text>();
        if (info.Source == Runner.Simulation.LocalPlayer)
            message = $"You said: {message}\n";
        else
            message = $"Some other player said: {message}\n";
        //_messages.text += message;
        Debug.Log(message);
    }

    [Rpc]
    private static void Rpc_UpdateRoomProperties(NetworkRunner runner, string[] properties, RpcInfo info = default)
    {
        Properties = properties;

        Debug.Log($"Rpc_UpdateRoomProperties(NetworkRunner, string[], RpcInfo); ActivePlayers: {runner.ActivePlayers}; Properties: {properties.Length}; Info: Source= {info.Source}, IsInvokeLocal= {info.IsInvokeLocal}, Channel= {info.Channel}");
    }
}
