using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using d4160.Events;
using Unity.Services.Core;

namespace d4160.UGS.Authentication
{
    [CreateAssetMenu(menuName = "d4160/UGS/Events/RequestFailed")]
    public class RequestFailedEventSO : EventSOBase<RequestFailedException>
    {
        
    }
}