using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float timer = 2f;
    private State state;
    private enum State
    {
        WatingForEnemyTurn = 0,
        TakingTurn = 1, 
        IsBusy = 2
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WatingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.IsBusy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.IsBusy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        state = State.TakingTurn;
        timer = 2f;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if(TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
            
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestAction = null;
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointToTakeAction(baseAction))
            {
                continue;
            }

            var testAIAction = baseAction.GetBestEnemyAIAction();

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = testAIAction;
                bestAction = baseAction;
                continue;
            }

            if(testAIAction == null)
            {
                continue;
            }

            if(bestEnemyAIAction.ActionValue < testAIAction.ActionValue)
            {
                bestEnemyAIAction = testAIAction;
                bestAction = baseAction;
            }
        }

        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestAction))
        {
            bestAction.TakeAction(bestEnemyAIAction.GridPosition, onEnemyAIActionComplete);
            return true;
        }

        return false;
    }
}
