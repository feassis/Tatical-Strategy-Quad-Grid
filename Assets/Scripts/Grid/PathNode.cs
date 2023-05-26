using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private bool isWalkable = true;

    private PathNode cameFromPathNode;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public int GetFCost() => fCost;

    public void SetGcost(int cost) => gCost = cost;
    public void SetHcost(int cost) => hCost = cost;

    public void CalculateFCost() => fCost = gCost + hCost;

    public void ResetCameFromPathNode() => cameFromPathNode = null;
    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;
    public PathNode GetCameFromPathNode() => cameFromPathNode;

    public GridPosition GetGridPosition() => gridPosition;

    public bool IsWalkable() => isWalkable;

    public void SetIsWalkable(bool isWalkable) => this.isWalkable = isWalkable;
}
