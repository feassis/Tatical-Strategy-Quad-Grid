using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private float floorHeight;
    private int floor;
    private TGridObject[,] gridObjectArray;

    public GridSystem(int width, int height, float cellSize, int floor, float floorHeight, 
        Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floor = floor;
        this.floorHeight = floorHeight;
        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridObjectArray[x, z] = createGridObject(this, new GridPosition(x, z, floor));
            }
        }      
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) *  cellSize 
            + new Vector3(0, floor,0) * floorHeight;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize),
            floor);
    }

    public void CreateDebugObjects(GridDebugObject debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, floor);
                GridDebugObject debugObj = GameObject.Instantiate(debugPrefab, 
                    GetWorldPosition(gridPosition), Quaternion.identity);

                debugObj.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0
            && gridPosition.x < width && gridPosition.z < height
            && gridPosition.floor == floor;
    }

    public (int width, int height) GetGridSize() => (width, height);
}
