using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayer;
    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;
    private const int MOVE_STRAIGHT_COST = 10; 
    private const int MOVE_DIAGONAL_COST = 14; 

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
        
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition p) => new PathNode(p));

        gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up, raycastOffsetDistance * 2, obstacleLayer))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startingPosition, 
        GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startingPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        var size = gridSystem.GetGridSize();

        for (int x = 0; x < size.width; x++)
        {
            for (int z = 0; z < size.height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGcost(int.MaxValue);
                pathNode.SetHcost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGcost(0);
        startNode.SetHcost(CalculateDistance(startingPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowerstFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost()
                    + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGcost(tentativeGCost);
                    neighbourNode.SetHcost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);

        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (var node in pathNodeList)
        {
            gridPositionList.Add(node.GetGridPosition());
        }

        return gridPositionList;
    }

    public int CalculateDistance(GridPosition gridPositionA, 
        GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int totalDistance = Mathf.Abs(gridPositionDistance.x) 
            + Mathf.Abs(gridPositionDistance.z);

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) 
            + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowerstFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCost = pathNodeList[0];

        foreach (var testNode in pathNodeList)
        {
            if(testNode.GetFCost() < lowestFCost.GetFCost())
            {
                lowestFCost = testNode;
            }
        }

        return lowestFCost;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighboudList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {
            neighboudList.Add(GetNode(gridPosition.x - 1, gridPosition.z));

            if(gridPosition.z - 1 >= 0)
            {
                neighboudList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if(gridPosition.z + 1 < gridSystem.GetGridSize().height)
            {
                neighboudList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }    
        }

        if(gridPosition.x + 1 < gridSystem.GetGridSize().width)
        {
            neighboudList.Add(GetNode(gridPosition.x + 1, gridPosition.z));

            if (gridPosition.z - 1 >= 0)
            {
                neighboudList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }

            if (gridPosition.z + 1 < gridSystem.GetGridSize().height)
            {
                neighboudList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }    
        }

        if (gridPosition.z - 1 >= 0)
        {
            neighboudList.Add(GetNode(gridPosition.x, gridPosition.z - 1));
        }

        if (gridPosition.z + 1 < gridSystem.GetGridSize().height)
        {
            neighboudList.Add(GetNode(gridPosition.x, gridPosition.z + 1));
        }

        return neighboudList;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    public bool isWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLegth) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLegth);
        return pathLegth;
    }
}
