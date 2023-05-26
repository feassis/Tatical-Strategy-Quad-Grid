using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Awake()
    {
        UnitActionSystem.Instance.OnIsBusyChanged += UpdateVisibility;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnIsBusyChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(bool isBusy)
    {
        gameObject.SetActive(isBusy);
    }
}
