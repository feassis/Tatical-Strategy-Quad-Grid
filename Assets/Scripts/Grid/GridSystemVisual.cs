using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private GridSystemVisualSingle gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,,] gridSystemVisualsArray;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType Type;
        public Material Material;
    }

    public enum GridVisualType
    {
        White = 0,
        Blue = 1,
        Red = 2,
        Yellow = 3,
        RedSoft = 4,
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
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

    private void Start()
    {
        var gridSize = LevelGrid.Instance.GetLevelGridSize();
        int floorAmount = LevelGrid.Instance.GetFloorAmount();
        gridSystemVisualsArray = new GridSystemVisualSingle[gridSize.width, gridSize.height, floorAmount];
        
        for (int x = 0; x < gridSize.width; x++)
        {
            for (int z = 0; z < gridSize.height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    var gridVisual = Instantiate(gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z, floor)), Quaternion.identity);
                    gridVisual.transform.parent = transform;
                    gridVisual.Hide();
                    gridSystemVisualsArray[x, z, floor] = gridVisual;
                }    
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnIsBusyChanged += UnitActionSystem_OnIsBusyChanged;

        UpdateGridVisual();
    }

    private void UnitActionSystem_OnIsBusyChanged(bool obj)
    {
        UpdateGridVisual(); ;
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridVisuals()
    {
        var gridSize = LevelGrid.Instance.GetLevelGridSize();
        int floorAmount = LevelGrid.Instance.GetFloorAmount();

        for (int x = 0; x < gridSize.width; x++)
        {
            for (int z = 0; z < gridSize.height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    gridSystemVisualsArray[x, z, floor].Hide();
                }    
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType type)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, type);
    }

    private void ShowGridPositionRangeSquared(GridPosition gridPosition, int range, GridVisualType type)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, type);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType type)
    {
        Material gridMaterial = GetGridVisualTypeMaterial(type);
        foreach (var pos in gridPositions)
        {
            gridSystemVisualsArray[pos.x, pos.z, pos.floor].Show(gridMaterial);
        }
    }

    private void UpdateGridVisual()
    {
        var unit = UnitActionSystem.Instance.GetSelectedUnit();

        if(unit == null)
        {
            return;
        }

        HideAllGridVisuals();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        GridVisualType visualType;
        switch (selectedAction)
        {
            case MoveAction moveAction:
                visualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                visualType = GridVisualType.Yellow;
                break;
            case ShootAction shootAction:
                visualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(),
                    shootAction.GetMaxRange(), GridVisualType.RedSoft);
                break;
            case GranadeAction granadeAction:
                visualType = GridVisualType.Yellow;
                break;
            case SwordAction swordAction:
                visualType = GridVisualType.Red;
                ShowGridPositionRangeSquared(selectedUnit.GetGridPosition(),
                    swordAction.GetSwordRange(), GridVisualType.RedSoft);
                break;
            case InteractAction interactAction:
                visualType = GridVisualType.Blue;
                break;
            default:
                visualType = GridVisualType.White;
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), visualType);

    }

    private Material GetGridVisualTypeMaterial(GridVisualType type)
    {
        return gridVisualTypeMaterialList.Find(g => g.Type == type).Material;
    }
}
