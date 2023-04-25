using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Characters
{
    public interface ICharacterController
    {
        bool MoveEnabled { get; set; }
        bool CameraMoveEnabled { get; set; }

        void SetEnable(bool enable, bool hardMode = true);
    }
}