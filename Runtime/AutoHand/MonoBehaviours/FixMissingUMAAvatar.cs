using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class FixMissingUMAAvatar : MonoBehaviour
{
    [SerializeField] Animator _umaAnim;

    private void Update()
    {
        Fix();
    }

    [Button]
    private void Fix()
    {
        var obj = GameObject.Find("Hidden IK Copy (Auto Hand + VRIK requirement)");

        if (obj != null)
        {
            var anim = obj.GetComponent<Animator>();
            anim.avatar = _umaAnim.avatar;
        }
        else
        {
            Debug.Log("Missing Hidden");
        }
    }
}
