using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private Button actionButton;
    [SerializeField] private GameObject selectedAction;

    private BaseAction action;
    public void SetBaseAction(BaseAction baseAction)
    {
        action = baseAction;
        actionButtonText.text = baseAction.GetActionName().ToUpper();
        actionButton.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
            });
    }

    public void UpdatedSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedAction.SetActive(selectedBaseAction == action);
    }
}
