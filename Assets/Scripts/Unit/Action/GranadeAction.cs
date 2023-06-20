using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeAction : BaseAction
{
    [SerializeField] private int MaxThrowDistance = 7;
    [SerializeField] private GranadeProjectile granadeProjectilePrefab;

    public override string GetActionName()
    {
        return "Granade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitPosition = unit.GetGridPosition();

        for (int x = -MaxThrowDistance; x <= MaxThrowDistance; x++)
        {
            for (int z = -MaxThrowDistance; z <= MaxThrowDistance; z++)
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

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > MaxThrowDistance)
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
        var granadeProjectile = Instantiate(granadeProjectilePrefab, unit.GetWorldPosition() + Vector3.up * 1f, Quaternion.identity);
        granadeProjectile.Setup(gridPosition, OnGranadeBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void OnGranadeBehaviourComplete()
    {
        ActionComplete();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }
}
