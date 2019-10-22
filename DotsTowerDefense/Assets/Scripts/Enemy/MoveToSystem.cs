using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FlowfieldMovementSystem))]
public class MoveToSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float delta = Time.deltaTime;
        Entities.ForEach((Entity entity, ref MoveTo moveTo, ref Translation trans, ref MoveSpeed speed) =>
        {
            if (math.length(trans.Value - moveTo.target) < speed.speed * delta)
            {
                trans.Value = moveTo.target;
                EntityManager.RemoveComponent<MoveTo>(entity);
                return;
            }

            trans.Value += math.normalize(moveTo.target - trans.Value) * speed.speed * delta;
        });
    }
}
