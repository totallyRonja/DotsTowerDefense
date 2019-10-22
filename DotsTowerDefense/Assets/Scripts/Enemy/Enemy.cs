using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour, IConvertGameObjectToEntity
{

    public float speed;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed{ speed = speed });
        dstManager.AddComponentData(entity, new FlowFieldAgent
        {
            gridPosition = int2.zero,
            active = true,
            onGrid = false,
        });
    }
}
