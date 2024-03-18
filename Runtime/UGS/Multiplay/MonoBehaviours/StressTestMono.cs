using System.Collections;
using System.Collections.Generic;
using d4160.Coroutines;
using d4160.UGS.Multiplay.LifecycleAPI;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StressTestMono : NetworkBehaviour
{
    public int _allocationsCount = 25;
    [SerializeField] private QueueAllocationRequestSO _queueAlloc;
    [SerializeField] private RemoveAllocationRequestSO _removeAlloc;
    public int _allocationsRate = 10;
    public float _waitEachAllocation = 0.1f;
    public float _waitEachBatchAllocation = 1f;

    [Space]
    public GameObject _networkedObj;
    public int _objsCount = 100;

    [Header("UI")]
    [SerializeField] private Button _startTestBtn;
    [SerializeField] private InputField _instancesNumberIpf;

    [SerializeField]
    private List<string> _allocationsList = new List<string>();

    private int _allocationsBatchCount;

    void Awake()
    {
        _startTestBtn.onClick.AddListener(() =>
        {
            _objsCount = int.Parse(_instancesNumberIpf.text);
            PlayersStressTest();
        });
    }

    private void OnEnable()
    {
        _instancesNumberIpf.text = _objsCount.ToString();
    }

    [Button]
    [ContextMenu("ServerStressTest")]
    public void ServerStressTest()
    {
        _queueAlloc.NewUuidForAllocation = true;

        CoroutineStarter.Instance.StartCoroutine(ServerStressTestCo());
    }

    private IEnumerator ServerStressTestCo()
    {
        _allocationsBatchCount = _allocationsRate;
        for (int i = 0; i < _allocationsCount; i++)
        {
            _queueAlloc.SendRequest((result) =>
            {
                _allocationsList.Add(result);
            });

            yield return new WaitForSeconds(_waitEachAllocation);
            _allocationsBatchCount--;

            if (_allocationsCount == 0)
            {
                yield return new WaitForSeconds(_waitEachBatchAllocation);
                _allocationsBatchCount = _allocationsRate;
            }
        }
    }

    [Button]
    [ContextMenu("RemoveServers")]
    public void RemoveServers()
    {
        CoroutineStarter.Instance.StartCoroutine(RemoveServersCo());
    }

    private IEnumerator RemoveServersCo()
    {
        _allocationsBatchCount = _allocationsRate;

        for (int i = _allocationsList.Count - 1; i >= 0; i--)
        {
            string temp = _allocationsList[i];
            _removeAlloc.AllocationId = temp;
            int iLocal = i;
            _removeAlloc.SendRequest((result) =>
            {
                _allocationsList.RemoveAt(iLocal);
            });

            yield return new WaitForSeconds(_waitEachAllocation);

            _allocationsBatchCount--;

            if (_allocationsCount == 0)
            {
                yield return new WaitForSeconds(_waitEachBatchAllocation);
                _allocationsBatchCount = _allocationsRate;
            }
        }
    }

    [Button]
    [ContextMenu("PlayersStressTest")]
    public void PlayersStressTest()
    {
        InstancePlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InstancePlayerServerRpc()
    {
        for (int i = 0; i < _objsCount; i++)
        {
            var instance = Instantiate(_networkedObj);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
        }
    }
}
