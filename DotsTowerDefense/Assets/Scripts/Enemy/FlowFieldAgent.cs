using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct FlowFieldAgent : IComponentData
{
    public int2 gridPosition;
    public bool onGrid;
    public bool active;
}
