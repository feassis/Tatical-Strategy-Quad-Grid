using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;
    [SerializeField] private HealthSystem healthSystem;

    private int actionPoints = 2;
    private GridPosition currentGridPosition;

    private BaseAction[] baseActionArray;

    public BaseAction[] GetBaseActionArray() => baseActionArray;

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T) baseAction;
            }
        }

        return null;
    }

    public GridPosition GetGridPosition() => currentGridPosition;
    public bool IsEnemy() => isEnemy;

    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        Debug.Log(gridPosition);
        currentGridPosition = gridPosition;
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(currentGridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    internal void Damage(float dmg)
    {
        healthSystem.Damage(dmg);
    }

    private void Update()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (gridPosition != currentGridPosition)
        {
            var oldGridPositon = currentGridPosition;
            currentGridPosition = gridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPositon, gridPosition);
        }

    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }

        return false;
    }

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints() => actionPoints;
    public Vector3 GetWorldPosition() => transform.position;

    public float GetHeathNormalized() => healthSystem.GetHeathNormalized();

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        bool isPlayerTurn = TurnSystem.Instance.IsPlayerTurn();
        bool isUnitTurn = (IsEnemy() && !isPlayerTurn) || (!IsEnemy() && isPlayerTurn);
        
        if (!isUnitTurn)
        {
            return;
        }

        actionPoints = ACTION_POINTS_MAX;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
}
