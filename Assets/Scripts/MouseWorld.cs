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

    public static Vector3 GetPositionOnlyVisible()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

        RaycastHit[] raycastArray = Physics.RaycastAll(ray, float.MaxValue, instance.floorMask);

        System.Array.Sort(raycastArray,
            (RaycastHit a, RaycastHit b) => Mathf.RoundToInt(a.distance - b.distance));
        foreach (var hit in raycastArray)
        {
            if(hit.transform.TryGetComponent<Renderer>(out Renderer renderer))
            {
                if (renderer.enabled)
                {
                    return hit.point;
                }
            }

        }

        return Vector3.zero;
    }
}
