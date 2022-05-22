using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon
{
    [DisallowMultipleComponent]
public class InputController : MonoBehaviour
{
    public event Action<Button> ButtonDown;
    public event Action<Button> ButtonUp;

    void Update()
    {
        
    }

    public enum Button {
        MoveForward,
        MoveBackwards,
        TurnLeft,
        TurnRight,
        StrafeLeft,
        StrafeRight,
    }
}
}
