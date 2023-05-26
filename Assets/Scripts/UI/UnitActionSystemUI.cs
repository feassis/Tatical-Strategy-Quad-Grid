using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private ActionButtonUI actioButtonPrefab;
    [SerializeField] private Transform actionButtonHolder;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtons = new List<ActionButtonUI>();

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        Unit.OnAnyActionPointsChanged += Uniy_OnAnyActionPointsChanged;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        Unit.OnAnyActionPointsChanged -= Uniy_OnAnyActionPointsChanged;
    }

    private void Uniy_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedImage();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CrateUnitActionButtons();
        UpdateSelectedImage();
        UpdateActionPoints();
    }

    private void CrateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonHolder)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtons.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            ActionButtonUI actionButton = Instantiate(actioButtonPrefab, actionButtonHolder);
            actionButton.SetBaseAction(baseAction);
            actionButtons.Add(actionButton);
        }
    }

    private void UpdateSelectedImage()
    {
        foreach (var actionButton in actionButtons)
        {
            actionButton.UpdatedSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if(selectedUnit == null)
        {
            return;
        }

        actionPointsText.text = $"Action Points: {selectedUnit.GetActionPoints()}";
    }
}
