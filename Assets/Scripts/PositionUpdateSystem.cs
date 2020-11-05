using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;

public class PositionUpdateSystem : JobComponentSystem
{
    [BurstCompile]
    struct PositionUpdateJob : IJobForEach<PositionComponent>
    {
        public float dT;

        public void Execute(ref PositionComponent position)
        {
            position.position.y = position.baseY + Mathf.Sin((position.position.x + position.position.y) * dT) * 5.0f;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new PositionUpdateJob()
        {
            dT = Time.DeltaTime
        };

        return job.Schedule(this, inputDependencies);
    }
}
