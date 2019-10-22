using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GridSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        GridManager gridManager = GridManager.Instance;
        Entities.WithAll(typeof(Tag_GridObstacle)).ForEach((Entity entity, ref Translation trans) =>
        {
            //remove tag and add position to grid
            int2 gridPos = gridManager.WorldToGridPos(trans.Value);
            int index = gridManager.DataIndex(gridPos);
            gridManager.data[index] = new GridPoint
            {
                traversable = false,
                exitDirection = int2.zero,
            };
            
            PostUpdateCommands.RemoveComponent(entity, typeof(Tag_GridObstacle));
        });
        gridManager.RecalculatePaths();
    }
}
