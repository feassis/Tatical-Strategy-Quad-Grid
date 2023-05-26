using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    [SerializeField] private MeshRenderer meshRenderer;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += HandleOnUnitSelectedChanged;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= HandleOnUnitSelectedChanged;
    }

    private void HandleOnUnitSelectedChanged(object sender, EventArgs empty)
    {
        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        meshRenderer.enabled = selectedUnit == unit;
    }
}
