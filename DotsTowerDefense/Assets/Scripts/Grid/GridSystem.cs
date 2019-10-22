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
        Entities.ForEach((Entity entity, ref Tag_GridObstacle tag, ref Translation trans) =>
        {
            //remove tag and add position to grid
            int2 gridPos = gridManager.GridPosFromWorldPos(trans.Value);
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
