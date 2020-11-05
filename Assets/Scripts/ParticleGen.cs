using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ParticleGen : MonoBehaviour
{
    public Terrain       terrain;
    public ParticleArray particles;
    public float         sampleSize;

#if UNITY_EDITOR
    [Button("Sample Terrain")]
    void SampleTerrain()
    {
        var bounds = terrain.terrainData.bounds;
        bounds.center += terrain.transform.position;

        var nParticlesX = (int)(bounds.size.x / sampleSize);
        var nParticlesZ = (int)(bounds.size.z / sampleSize);
        var nParticles = nParticlesX * nParticlesZ;

        particles.particles = new List<ParticleArray.Particle>();
        particles.particles.Capacity = nParticles;

        var t0 = Time.time;

        Vector3 p = Vector3.zero;
        for (int z = 0; z < nParticlesZ; z++)
        {
            p.z = bounds.min.z + z * sampleSize;
            for (int x = 0; x < nParticlesX; x++)
            {
                p.x = bounds.min.x + x * sampleSize;

                p.y = terrain.SampleHeight(p);

                var particle = new ParticleArray.Particle { position = p };
                particles.particles.Add(particle);
            }
        }

        var t = Time.time - t0;

        Debug.Log("Generated " + nParticles + " particles (" + nParticlesX + " x " + nParticlesZ + ") in " + t + "s");

        UnityEditor.EditorUtility.SetDirty(particles);
    }
#endif
}
