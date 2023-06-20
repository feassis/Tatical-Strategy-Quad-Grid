using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingLinkMonoBehavior : MonoBehaviour
{
    public Vector3 LinkPositionA;
    public Vector3 LinkPositionB;

    public PathfindingLink GetPathfindingLink() => new PathfindingLink 
    {
        GridPositionA = LevelGrid.Instance.GetGridPosition(LinkPositionA),
        GridPositionB = LevelGrid.Instance.GetGridPosition(LinkPositionB)
    };
}
