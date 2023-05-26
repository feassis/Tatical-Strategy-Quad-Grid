using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private int MaxShootDistance = 7;
    [SerializeField] private float shootingStateTimer = 0.1f;
    [SerializeField] private float cooloffStateTimer = 0.5f;
    [SerializeField] private float aimingStateTimer = 0.5f;
    [SerializeField] private float aimingRotationSpeed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float unitShootHeight = 1.7f;
    [SerializeField] private LayerMask obstaclesLayerMask;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public int GetMaxRange() => MaxShootDistance;

    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Unit ShootingUnit;
    }

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    private enum State
    {
        Aiming = 0,
        Shooting = 1,
        Cooloff = 2
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
            case State.Aiming:
                Vector3 aimDirection = targetUnit.GetWorldPosition() - unit.GetWorldPosition();
                transform.forward = Vector3.Lerp(transform.forward,
                aimDirection, Time.deltaTime * aimingRotationSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
            default:
                break;
        }

        if(stateTimer <= 0)
        {
            NextStage();
        }
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs { 
            TargetUnit = targetUnit,
            ShootingUnit = unit
        });

        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            TargetUnit = targetUnit,
            ShootingUnit = unit
        });

        targetUnit.Damage(damage);
    }

    private void NextStage()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTimer;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = cooloffStateTimer; 
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -MaxShootDistance; x <= MaxShootDistance; x++)
        {
            for (int z = -MaxShootDistance; z <= MaxShootDistance; z++)
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

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (testDistance > MaxShootDistance)
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

                Vector3 unitWorldPos = LevelGrid.Instance.GetWorldPosition(unitPosition);

                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPos).normalized;
                bool hitSomething = Physics.Raycast(unitWorldPos + Vector3.up * unitShootHeight, shootDir,
                    Vector3.Distance(unitWorldPos, targetUnit.GetWorldPosition()), obstaclesLayerMask);

                if (hitSomething)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitPosition = unit.GetGridPosition();

        return GetValidActionGridPositionList(unitPosition);
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitOnGridPosition(gridPosition);
        state = State.Aiming;
        stateTimer = aimingStateTimer;
        canShootBullet = true;
        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit() => targetUnit;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit target = LevelGrid.Instance.GetUnitOnGridPosition(gridPosition);
        float hp = target.GetHeathNormalized();

        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 100 + Mathf.RoundToInt((1 - hp) * 100)
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
