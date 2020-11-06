using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

public class PositionUpdateSystem : JobComponentSystem
{
    PointRendererGS pointRenderer;

    protected override void OnCreate()
    {
        base.OnCreate();

        pointRenderer = GameObject.FindObjectOfType<PointRendererGS>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float dT = Time.DeltaTime;
        var   vertices = pointRenderer.vertices;

        pointRenderer.updateJob = Entities.ForEach((ref PositionComponent position) =>
        {
            position.position.y = position.baseY + Mathf.Sin((position.position.x + position.position.y) * dT) * 5.0f;
            vertices[position.index] = position.position;
        }).Schedule(inputDependencies);

        return pointRenderer.updateJob;
    }
}
