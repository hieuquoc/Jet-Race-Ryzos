using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputController : MonoBehaviour
{
    public Transform Camera;
    [SerializeField] private float moveSpeed = 10f;

    private int holdDirection = 0; // -1 trái, 1 phải, 0 không giữ

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
    }

    void Update()
    {
        HandleInput();

        if (holdDirection != 0 && Camera != null)
        {
            Camera.localPosition += Vector3.right * holdDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleInput()
    {
        var touches = Touch.activeTouches;
        if (touches.Count == 0)
        {
            holdDirection = 0;
            return;
        }

        var touch = touches[0];
        var phase = touch.phase;

        if (phase == UnityEngine.InputSystem.TouchPhase.Began ||
            phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
            phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            float half = Screen.width * 0.5f;
            holdDirection = touch.screenPosition.x < half ? -1 : 1;
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            holdDirection = 0;
        }
    }
}


