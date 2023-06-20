using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorVisibility : MonoBehaviour
{
    [SerializeField] private float floorHeightOffset = 2f;
    [SerializeField] private bool isDinamicFloorPosition;
    [SerializeField] private List<Renderer> ignoreRenderer;
    private Renderer[] rendererArray;
    private int floor;

    private void Start()
    {
        floor = LevelGrid.Instance.GetFloor(transform.position);

        if(floor == 0 && !isDinamicFloorPosition)
        {
            this.enabled = false;
            return;
        }

        UpdateVisibility();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (isDinamicFloorPosition)
        {
            floor = LevelGrid.Instance.GetFloor(transform.position);
        }

        float cameraHeight = CameraController.Instance.GetCameraHeight();

        bool showObject = cameraHeight  > LevelGrid.Instance.GetFloorHeight() * floor + floorHeightOffset;

        if (showObject || floor == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Awake()
    {
        rendererArray = GetComponentsInChildren<Renderer>(true);
    }

    private void Show()
    {
        foreach (var renderer in rendererArray)
        {
            if (ignoreRenderer.Contains(renderer))
            {
                continue;
            }

            renderer.enabled = true;
        }
    }

    private void Hide()
    {
        foreach (var renderer in rendererArray)
        {
            if (ignoreRenderer.Contains(renderer))
            {
                continue;
            }

            renderer.enabled = false;
        }
    }
}
