using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct PositionComponent : IComponentData
{
    public int      index;
    public Vector3  position;
    public float    baseY;
}
