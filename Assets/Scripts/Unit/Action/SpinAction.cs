using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    [SerializeField] private float spinAddAmount = 360f;
    private float amountYetToSpin;
    
    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float frameSpinAmount = spinAddAmount * Time.deltaTime;
        amountYetToSpin -= frameSpinAmount;
        transform.eulerAngles += new Vector3(0, frameSpinAmount, 0);

        if (amountYetToSpin < 0)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        amountYetToSpin = spinAddAmount;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 0
        };
    }
}
