using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonoBehavior))]
public class PathfindingLinkMonoBehaviourEditor : Editor
{
    private void OnSceneGUI()
    {
        PathfindingLinkMonoBehavior pathfindingLinkMonoBehaviour = (PathfindingLinkMonoBehavior)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newLinkPositionA = Handles.PositionHandle(pathfindingLinkMonoBehaviour.LinkPositionA, Quaternion.identity);
        Vector3 newLinkPositionB = Handles.PositionHandle(pathfindingLinkMonoBehaviour.LinkPositionB, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pathfindingLinkMonoBehaviour, "Change Link Position");
            pathfindingLinkMonoBehaviour.LinkPositionA = newLinkPositionA;
            pathfindingLinkMonoBehaviour.LinkPositionB = newLinkPositionB;
        }

    }
}
