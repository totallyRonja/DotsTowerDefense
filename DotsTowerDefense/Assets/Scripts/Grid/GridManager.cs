using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct GridPoint
{
    public int2 exitDirection;
    public bool traversable;
}

public class GridManager : MonoBehaviour
{
    public int2 gridSize;
    public float2 tileSize;
    public float2 posOffset;
    public int2 exit;
    public GridPoint[] data;

    public static GridManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        //create and initialize buffer
        data = new GridPoint[gridSize.x * gridSize.y];
        for (var i = 0; i < data.Length; i++)
            data[i] = new GridPoint
            {
                exitDirection = int2.zero,
                traversable = true
            };

        Instance = this;
    }
    
    public GridPoint GetData(float3 worldPos)
    {
        return GetData(WorldToGridPos(worldPos));
    }

    public GridPoint GetData(int2 gridPos)
    {
        return GetData(DataIndex(gridPos));
    }

    public GridPoint GetData(int index)
    {
        return data[index];
    }

    public int DataIndex(int2 gridPos)
    {
        int index = gridPos.x + gridPos.y * gridSize.x;
        if(index > data.Length)
            Debug.LogWarning($"generated index {index} outside of grid range");
        return index;
    }
    
    public int2 WorldToGridPos(float3 worldPos)
    {
        float2 relPos = (worldPos.xz - posOffset) / tileSize;
        return math.clamp((int2)math.round(relPos), 0, gridSize);
    }
    
    public float3 GridToWorldPos(int2 gridPos)
    {
        float2 flatPos = gridPos * tileSize + posOffset;
        return new float3(flatPos.x, 0, flatPos.y);
    }

    public bool InBounds(int2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < gridSize.x && pos.y < gridSize.y;
    }
    
    static readonly int2[] Directions = 
    {
        new int2(0, 1),
        new int2(1, 0),
        new int2(0, -1),
        new int2(-1, 0),
    };

    [BurstCompile]
    public void RecalculatePaths()
    {
        var queue = new Queue<FlowFieldPoint>();
        queue.Enqueue(new FlowFieldPoint
        {
            exitDistance = 0,
            position = exit,
        });

        var exploredTiles = new HashSet<int2> {exit};

        var order = Utility.CountArray(0, Directions.Length).ToArray();
        while (queue.Count > 0)
        {
            FlowFieldPoint point = queue.Dequeue();
            order.Randomize();
            for (var i = 0; i < Directions.Length; i++)
            {
                int2 dir = Directions[order[i]];
                int2 neighborPos = point.position + dir;
                int index = DataIndex(neighborPos);
                if (!InBounds(neighborPos) || exploredTiles.Contains(neighborPos) ||
                    !data[index].traversable)
                    continue;

                queue.Enqueue(new FlowFieldPoint
                {
                    position = neighborPos,
                    exitDistance = point.exitDistance + 1,
                });
                exploredTiles.Add(neighborPos);
                data[index].exitDirection = -dir;
            }
        }
    }
    
    struct FlowFieldPoint
    {
        public int exitDistance;
        public int2 position;
    }

    void OnDrawGizmos()
    {
        //helper data
        float2 size = gridSize * tileSize;
        const float yHeight = 0.1f;

        //grid
        Gizmos.color = Color.red;
        for (var i = 0; i <= gridSize.y; i++)
        {
            var pos = new float3(posOffset.x - 0.5f, yHeight, posOffset.y + i * tileSize.y - 0.5f);
            Gizmos.DrawRay(pos, Vector3.right * size.x);
        }

        for (var i = 0; i <= gridSize.x; i++)
        {
            var pos = new float3(posOffset.x + i * tileSize.x - 0.5f, yHeight, posOffset.y - 0.5f);
            Gizmos.DrawRay(pos, Vector3.forward * size.y);
        }

        //directions and blocker
        if (data != null && data.Length == gridSize.x * gridSize.y)
        {
            for (var x = 0; x < gridSize.x; x++)
            for (var y = 0; y < gridSize.y; y++)
            {
                var pos = new Vector3(posOffset.x + x * tileSize.x, yHeight, posOffset.y + y * tileSize.y);
                int index = x + y * gridSize.x;
                GridPoint dataPoint = data[index];
                if (dataPoint.traversable)
                {
                    Gizmos.color = Color.green;
                    float2 exitDir = math.normalize(dataPoint.exitDirection) * 0.5f;
                    Gizmos.DrawRay(pos, new Vector3(exitDir.x, 0, exitDir.y));
                }
                else
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(pos, new float3(0.9f));
                }
            }
        }

        //exit pos
        Gizmos.color = Color.blue;
        float2 exitPos = posOffset + tileSize * exit;
        Gizmos.DrawSphere(new Vector3(exitPos.x, 0, exitPos.y), 0.2f);
    }
}