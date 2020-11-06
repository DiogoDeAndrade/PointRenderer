using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;

public class PointRendererGS : MonoBehaviour
{
    public ParticleArray    particles;
    public Material         pointRendererMaterial;
    public float            size = 1.0f;
    public bool             index32 = false;
    public bool             ecsEnable = false;

    class MeshData
    {
        public Mesh     mesh;
        public int[]    indices;
    };

    List<MeshData>              meshes;

    [HideInInspector] public NativeArray<Vector3>    vertices;
    [HideInInspector] public JobHandle               updateJob;

    void Start()
    {
        int nParticles = particles.particles.Count;

        meshes = new List<MeshData>();

        if (ecsEnable)
        {
            vertices = new NativeArray<Vector3>(nParticles, Allocator.Persistent);
            BuildEntities();

            if (index32)
            {
                var indices = new int[nParticles];
                for (int j = 0; j < nParticles; j++)
                {
                    indices[j] = j;
                }

                var mesh = new Mesh();

                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                mesh.SetVertices(vertices);
                mesh.SetIndices(indices, MeshTopology.Points, 0);

                meshes.Add(new MeshData { mesh = mesh, indices = indices });
            }
            else
            {
                int meshIndex = 0;
                int maxIndicesPerMesh = 65535;
                int currentArraySize = 0;

                for (int i = 0; i < nParticles; i += maxIndicesPerMesh)
                {
                    var newName = "Mesh" + meshIndex++;
                    int c = Mathf.Min(maxIndicesPerMesh, nParticles - i);

                    if (c != currentArraySize)
                    {
                        currentArraySize = c;
                    }

                    var indices = new int[c];
                    for (int j = 0; j < c; j++)
                    {
                        indices[j] = j;
                    }

                    var mesh = new Mesh();

                    mesh.name = newName;
                    mesh.SetVertices(vertices.GetSubArray(i, c));
                    mesh.SetIndices(indices, MeshTopology.Points, 0);

                    meshes.Add(new MeshData { mesh = mesh, indices = indices });
                }
            }
        }
        else
        {
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

                meshes.Add(new MeshData { mesh = mesh, indices = null });
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

                    meshes.Add(new MeshData { mesh = mesh, indices = null });
                }
            }
        }

        Debug.Log("Rendering " + nParticles + " in " + meshes.Count + " meshes");
    }

    void LateUpdate()
    {
        updateJob.Complete();

        if (ecsEnable)
        {
            UpdateMeshes();
        }

        pointRendererMaterial.SetFloat("_Size", size);

        foreach (var mesh in meshes)
        {
            Graphics.DrawMesh(mesh.mesh, Vector3.zero, Quaternion.identity, pointRendererMaterial, 0);
        }
    }

    void BuildEntities()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        int nParticles = particles.particles.Count;
        for (int index = 0; index < nParticles; index++)
        {
            var particle = particles.particles[index];
            var entity = entityManager.CreateEntity(typeof(PositionComponent));

            vertices[index] = particle.position;
            entityManager.SetComponentData(entity, new PositionComponent { index = index, position = particle.position, baseY = particle.position.y });
        }
    }

    public int GetParticleCount()
    {
        return particles.particles.Count;
    }

    void UpdateMeshes()
    {
        int nParticles = particles.particles.Count;
        if (index32)
        {
            var mesh = meshes[0];

            mesh.mesh.SetVertices(vertices);
            mesh.mesh.SetIndices(mesh.indices, MeshTopology.Points, 0);
            mesh.mesh.UploadMeshData(false);
        }
        else
        {
            int meshIndex = 0;
            int maxIndicesPerMesh = 65535;

            for (int i = 0; i < nParticles; i += maxIndicesPerMesh)
            {
                int c = Mathf.Min(maxIndicesPerMesh, nParticles - i);

                var mesh = meshes[meshIndex];

                mesh.mesh.SetVertices(vertices.GetSubArray(i, c));
                mesh.mesh.SetIndices(mesh.indices, MeshTopology.Points, 0);
                mesh.mesh.UploadMeshData(false);

                meshIndex++;
            }
        }
    }

    private void OnDestroy()
    {
        if (vertices != null)
        {
            vertices.Dispose();
        }
    }
}
