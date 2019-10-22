using System;
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