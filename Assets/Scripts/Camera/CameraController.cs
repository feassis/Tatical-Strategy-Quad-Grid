using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private float cameraRotationSpeed = 100f;
    [SerializeField] private float cameraZoomSpeed = 5f;
    [SerializeField] private float zoomAmount = 5f;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private const float MIN_FOLLOW_Y = 2f;
    private const float MAX_FOLLOW_Y = 12;

    private Vector3 followOffset;

    private void Start()
    {
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        followOffset = transposer.m_FollowOffset;
    }

    private void Update()
    {
        MoveCamera();

        RotateCamera();

        Zoom();
    }

    private void MoveCamera()
    {
        Vector2 inputMovementDirection = InputManager.Instance.GetCameraMoveVector();

        inputMovementDirection.Normalize();

        Vector3 moveVector = transform.forward * inputMovementDirection.y + transform.right * inputMovementDirection.x;

        transform.position += moveVector * cameraSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;
    }

    private void Zoom()
    {
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        followOffset.y += zoomAmount * InputManager.Instance.GetCameraZoomAmount();

        followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_Y, MAX_FOLLOW_Y);

        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, cameraZoomSpeed * Time.deltaTime);
    }
}
