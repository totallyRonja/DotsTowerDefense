using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FlowfieldMovementSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.WithNone(typeof(MoveTo)).WithAll(typeof(MoveSpeed)).
                ForEach((Entity entity, ref FlowFieldAgent agent) =>
        {
            if(!agent.active)
                return;

            GridManager grid = GridManager.Instance;

            if (!agent.onGrid)
            {
                var trans = EntityManager.GetComponentData<Translation>(entity);
                int2 gridPos = grid.WorldToGridPos(trans.Value);
                //TODO: check if grid pos is blocked and move to closest unblocked tile
                agent.gridPosition = gridPos;
                agent.onGrid = true;
            }

            int2 direction = grid.GetData(agent.gridPosition).exitDirection;
            agent.gridPosition += direction;
            EntityManager.AddComponentData(entity, new MoveTo{target = grid.GridToWorldPos(agent.gridPosition)});
        });
    }
}
