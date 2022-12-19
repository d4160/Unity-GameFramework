using Fusion;
using Fusion.Photon.Realtime;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/Player")]
public class PhotonFusionPlayerSO : ScriptableObject
{
    public CustomAuthenticationType authType;
    public string userId;
    public NetworkPrefabRef playerPrefab;

    private AuthenticationValues _auth;

    public AuthenticationValues AuthValues => _auth ??= new($"{userId}{Random.Range(1, int.MaxValue)}")
    {
        AuthType = authType
    };

    [Button]
    public void ReInitializeAuthValues()
    {
        _auth = new(userId);
    }
}
