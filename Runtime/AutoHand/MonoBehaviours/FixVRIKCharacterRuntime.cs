using NaughtyAttributes;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class FixVRIKCharacterRuntime : MonoBehaviour
{
    [SerializeField] private GameObject _charObj;

    private void Start()
    {
        Fix();
    }

    [Button]
    public void Fix()
    {
        _charObj.SetActive(false);
        _charObj.SetActive(true);
    }
}
