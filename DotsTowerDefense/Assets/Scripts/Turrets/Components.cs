using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MachineGun : IComponentData{}

public struct LazerBeam : IComponentData {}

public struct FreezeGun : IComponentData {}

public struct Frozen : IComponentData {}

public struct FireTimer: IComponentData
{
    public float cooldown;
    public float countdown;
}

public struct FireBullet: IComponentData {}

public struct FireTarget : IComponentData
{
    public float3 position;
}

public struct RelativeBulletSpawnPosition : IComponentData
{
    public float3 value;
}