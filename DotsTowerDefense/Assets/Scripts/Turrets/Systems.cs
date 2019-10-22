using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

public class FireTimerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref FireTimer timer) =>
        {
            timer.countdown -= UnityEngine.Time.deltaTime;
            if(timer.cooldown <= 0f)
            {
                EntityManager.AddComponent<FireBullet>(entity);
            }
        });
    }
}

public class FireBulletSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .With(GetEntityQuery(typeof(FireBullet)))
            .ForEach((ref RelativeBulletSpawnPosition spawnPos) =>
            {
                var bullet = EntityManager.CreateEntity();
                EntityManager.AddComponentData(bullet, 
                    new Translation { Value = spawnPos.value });

            });
    }
}