#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDownThisFrame()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPerformedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputMovementDirection = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMovementDirection.y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMovementDirection.y -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMovementDirection.x += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMovementDirection.x -= 1;
        }

        return inputMovementDirection;
#endif
    }

    public float GetCameraRotateAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotation.ReadValue<float>();
#else

        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount += 1;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount -= 1;
        }

        return rotateAmount;
#endif
    }

    public float GetCameraZoomAmount()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount -= 1;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount += 1;
        }

        return zoomAmount;
#endif
    }
}
