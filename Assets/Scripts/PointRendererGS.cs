using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PointRendererGS : MonoBehaviour
{
    public ParticleArray    particles;
    public Material         pointRendererMaterial;
    public float            size = 1.0f;
    public bool             index32 = false;
    public bool             ecsEnable = false;

    List<Mesh>          meshes;
    World               world;

    void Start()
    {
        if (ecsEnable)
        {
            BuildEntities();
        }

        meshes = new List<Mesh>();

        int nParticles = particles.particles.Count;

        if (index32)
        {
            var indices = new int[nParticles];
            var particlePositions = new Vector3[nParticles];
            for (int j = 0; j < nParticles; j++)
            {
                indices[j] = j;
                particlePositions[j] = particles.particles[j].position;
            }

            var mesh = new Mesh();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.SetVertices(particlePositions);
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            mesh.UploadMeshData(true);

            meshes.Add(mesh);
        }
        else
        {
            int meshIndex = 0;
            int maxIndicesPerMesh = 65535;
            int currentArraySize = 0;

            Vector3[] particlePositions = null;

            for (int i = 0; i < nParticles; i += maxIndicesPerMesh)
            {
                var newName = "Mesh" + meshIndex++;
                int c = Mathf.Min(maxIndicesPerMesh, nParticles - i);

                if (c != currentArraySize)
                {
                    particlePositions = new Vector3[c];
                    currentArraySize = c;
                }

                var indices = new int[c];
                for (int j = 0; j < c; j++)
                {
                    indices[j] = j;
                    particlePositions[j] = particles.particles[i + j].position;
                }

                var mesh = new Mesh();

                mesh.name = newName;
                mesh.SetVertices(particlePositions);
                mesh.SetIndices(indices, MeshTopology.Points, 0);
                mesh.UploadMeshData(true);

                meshes.Add(mesh);
            }
        }

        Debug.Log("Rendering " + nParticles + " in " + meshes.Count + " meshes");
    }

    void Update()
    {
        pointRendererMaterial.SetFloat("_Size", size);

        foreach (var mesh in meshes)
        {
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, pointRendererMaterial, 0);
        }
    }

    void BuildEntities()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var particle in particles.particles)
        {
            var entity = entityManager.CreateEntity(typeof(PositionComponent));

            entityManager.SetComponentData(entity, new PositionComponent { position = particle.position, baseY = particle.position.y });
        }
    }
}
