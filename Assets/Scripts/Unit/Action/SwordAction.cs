using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    [SerializeField] private float afterHitStateTime = 0.5f;
    [SerializeField] private float beforeHitStateTime = 0.2f;
    [SerializeField] private float swordDamage = 100f;
    [SerializeField] private float aimingRotationSpeed = 10f;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;
    public static event EventHandler OnAnySwordHit;

    private int MaxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    public int GetSwordRange() => MaxSwordDistance;

    private enum State
    {
        SwingingSwordBeforeHit = 0,
        SwingingSwordAfterHit = 1,
    }


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection = targetUnit.GetWorldPosition() - unit.GetWorldPosition();
                transform.forward = Vector3.Lerp(transform.forward,
                aimDirection, Time.deltaTime * aimingRotationSpeed);

                break;
            case State.SwingingSwordAfterHit:
                break;
        }

        if (stateTimer <= 0)
        {
            NextStage();
        }
    }

    private void NextStage()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                stateTimer = afterHitStateTime;

                targetUnit.Damage(swordDamage);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);

                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 200
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitPosition = unit.GetGridPosition();

        for (int x = -MaxSwordDistance; x <= MaxSwordDistance; x++)
        {
            for (int z = -MaxSwordDistance; z <= MaxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);

                GridPosition testGridPosition = unitPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitPosition == testGridPosition)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitOnGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitOnGridPosition(gridPosition);

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        state = State.SwingingSwordBeforeHit;
        stateTimer = beforeHitStateTime;
        ActionStart(onActionComplete);
    }
}
