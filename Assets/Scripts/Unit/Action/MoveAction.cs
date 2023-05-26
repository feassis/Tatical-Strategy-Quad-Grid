using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private int maxMoveDistance = 4;
    
    private List<Vector3> positionList;
    private int currentPositionIndex;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private const float stoppingDistance = 0.1f;

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward,
                moveDirection, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;

            if(currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            
        }

        
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), targetPosition, out int pathLength);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (var gridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(gridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);

                GridPosition testGridPosition = unitPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitPosition == testGridPosition)
                {
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.isWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if(!Pathfinding.Instance.HasPath(unitPosition, testGridPosition))
                {
                    continue;
                }

                int pathfingingDistanceMultiplier = 10;

                if(Pathfinding.Instance.GetPathLength(unitPosition, testGridPosition) > maxMoveDistance * pathfingingDistanceMultiplier)
                {
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        var shootAction = unit.GetAction<ShootAction>();
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = shootAction.GetTargetCountAtPosition(gridPosition) * 5
        };
    }
}
