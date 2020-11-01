using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRendererGizmos : MonoBehaviour
{
    public ParticleArray particles;
    [Range(0.5f, 10.0f)]
    public float         size;

    private void OnDrawGizmos()
    {
        if (particles == null) return;

        Gizmos.color = Color.red;
        
        foreach (var p in particles.particles)
        {
            Gizmos.DrawSphere(p, size);
        }
    }
}
