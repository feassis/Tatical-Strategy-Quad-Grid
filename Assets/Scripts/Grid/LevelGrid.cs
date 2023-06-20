using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }
    public event EventHandler OnAnyUnitMovedGridPosition;
    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int floorAmount = 1;
    [SerializeField] private float cellSize;
    [SerializeField] private float floorHeight = 3f;
    private List<GridSystem<GridObject>> gridSystemList;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystemList = new List<GridSystem<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
            var gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, floorHeight,
            (GridSystem<GridObject> g, GridPosition p) => new GridObject(g, p));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

            gridSystemList.Add(gridSystem);
        }
    }

    public float GetFloorHeight()
    {
        return floorHeight;
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    private GridSystem<GridObject> GetGridSystem(int floor) => gridSystemList[floor];

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, 
        GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }

    public int GetFloor(Vector3 worldPosition) => Mathf.RoundToInt(worldPosition.y / floorHeight);

    public GridPosition GetGridPosition(Vector3 worldPosition) 
        => GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) 
    {
        if(gridPosition.floor < 0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }

        return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }
        
    
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);

        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);

        return gridObject.GetUnit();
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    public (int width, int height) GetLevelGridSize() => GetGridSystem(0).GetGridSize();
    public int GetFloorAmount() => floorAmount;

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);

        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);

        gridObject.SetInteractable(interactable);
    }
}
