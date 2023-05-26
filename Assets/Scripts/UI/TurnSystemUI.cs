using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        var turnSystem = TurnSystem.Instance;

        endTurnButton.onClick.AddListener(turnSystem.NextTurn);
        turnSystem.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateEnemyTurnVisual();
        UpdateEndTurnVisibility();
    }

    private void OnDestroy()
    {
        var turnSystem = TurnSystem.Instance;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        var turnSystem = TurnSystem.Instance;
        UpdateEnemyTurnVisual();
        UpdateEndTurnVisibility();

        turnText.text = $"Turn: {turnSystem.GetTurnNumber()}";
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
