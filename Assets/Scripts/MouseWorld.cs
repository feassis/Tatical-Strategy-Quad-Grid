using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;

    private static MouseWorld instance;

    private void Awake()
    {
        instance = this;
    }

    public static Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.floorMask);

        instance.transform.position = raycastHit.point;

        return raycastHit.point;
    }
}
