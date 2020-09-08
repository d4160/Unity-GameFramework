using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lean.Pool
{
    public class LeanPoolActions : MonoBehaviour
    {
        public virtual void Despawn()
        {
            LeanPool.Despawn(gameObject);
        }
    }
}