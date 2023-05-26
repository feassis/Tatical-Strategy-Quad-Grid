
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private List<Unit> units;
    private IInteractable interactable;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        units = new List<Unit>();
    }

    public List<Unit> GetUnitList() => units;

    public void AddUnit(Unit unit) => units.Add(unit); 
    public void RemoveUnit(Unit unit) => units.Remove(unit); 

    public override string ToString()
    {
        string unitString = "";

        foreach (var unit in units)
        {
            unitString += unit + "\n";
        }

        return $"{gridPosition.x}, {gridPosition.z} \n {unitString}";
    }

    public bool HasAnyUnit()
    {
        return units.Count > 0;
    }

    public Unit GetUnit()
    {
        if (!HasAnyUnit())
        {
            return null;
        }

        return units[0];
    }

    public IInteractable GetInteractable() => interactable;
    public void SetInteractable(IInteractable interactable) => this.interactable = interactable;
}
