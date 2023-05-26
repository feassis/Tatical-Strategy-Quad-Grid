using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderUpdater : MonoBehaviour
{
    void Start()
    {
        DestructableCrate.OnAnyDestroyed += DestructableCreate_OnAnyDestroyed;
    }

    private void DestructableCreate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructableCrate crate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(crate.GetGridPosition(), true);
    }
}
