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
        var gridManager = GridManager.Instance;
        Entities.ForEach((Entity entity, ref Tag_GridObstacle tag, ref Translation trans) =>
        {
            //remove tag and add position to grid
            int2 gridPos = GridPosFromWorldPos(trans.Value, gridManager);
            int index = DataIndex(gridPos, gridManager);
            gridManager.data[index] = new GridPoint
            {
                traversable = false,
                exitDirection = int2.zero,
            };
            
            PostUpdateCommands.RemoveComponent(entity, typeof(Tag_GridObstacle));
        });
        RecalculatePaths(ref gridManager);
    }

    public int2 GridPosFromWorldPos(float3 worldPos)
    {
        var gridData = GridManager.Instance;
        return GridPosFromWorldPos(worldPos, gridData);
    }

    public static int DataIndex(int2 gridPos, GridManager gridData)
    {
        int index = gridPos.x + gridPos.y * gridData.gridSize.x;
        if(index > gridData.data.Length)
            Debug.LogWarning($"generated index {index} outside of grid range");
        return index;
    }
    
    public static int2 GridPosFromWorldPos(float3 worldPos, GridManager gridData)
    {
        float2 relPos = (worldPos.xz - gridData.posOffset) / gridData.tileSize;
        return math.clamp((int2)math.round(relPos), 0, gridData.gridSize);
    }

    public static bool InBounds(int2 pos, GridManager gridData)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridData.gridSize.x && pos.y < gridData.gridSize.y;
    }
    
    
    static readonly int2[] directions = 
    {
        new int2(0, 1),
        new int2(1, 0),
        new int2(0, -1),
        new int2(-1, 0),
    };

    public static void RecalculatePaths(ref GridManager gridData)
    {
        var queue = new Queue<FlowFieldPoint>();
        queue.Enqueue(new FlowFieldPoint
        {
            exitDistance = 0,
            position = gridData.exit,
        });

        var exploredTiles = new HashSet<int2> {gridData.exit};

        var order = Utility.CountArray(0, directions.Length).ToArray();
        while (queue.Count > 0)
        {
            FlowFieldPoint point = queue.Dequeue();
            order.Randomize();
            for (var i = 0; i < directions.Length; i++)
            {
                int2 dir = directions[order[i]];
                int2 neighborPos = point.position + dir;
                int index = DataIndex(neighborPos, gridData);
                if (!InBounds(neighborPos, gridData) || exploredTiles.Contains(neighborPos) ||
                    !gridData.data[index].traversable)
                    continue;

                queue.Enqueue(new FlowFieldPoint
                {
                    position = neighborPos,
                    exitDistance = point.exitDistance + 1,
                });
                exploredTiles.Add(neighborPos);
                gridData.data[index].exitDirection = -dir;
            }
        }
    }
    
    struct FlowFieldPoint
    {
        public int exitDistance;
        public int2 position;
    }
}
