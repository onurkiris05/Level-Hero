using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] public VariableJoystick variableJoystick;

    public static Vector2 Direction;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Direction = variableJoystick.Direction.normalized;
    }
}
