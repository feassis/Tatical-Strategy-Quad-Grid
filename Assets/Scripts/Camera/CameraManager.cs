using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera actionCamera;
    [SerializeField] private float characterShoulderHeight = 1.7f;
    [SerializeField] private float shoulderOffset = 0.5f;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        HideActionCamera();
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                Vector3 sholderHeight = Vector3.up * characterShoulderHeight;
                Unit shooterUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 shootDir = targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition();

                Vector3 sholderOffset = Quaternion.Euler(0, 90, 0) * shootDir.normalized * shoulderOffset;
                Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + sholderOffset
                    + sholderHeight + shootDir.normalized * -1;

                actionCamera.gameObject.transform.position = actionCameraPosition;

                actionCamera.gameObject.transform.LookAt(targetUnit.GetWorldPosition() + sholderHeight);
                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCamera.gameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCamera.gameObject.SetActive(false);
    }
}
