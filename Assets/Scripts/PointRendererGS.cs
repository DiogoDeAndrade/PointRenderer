using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRendererGS : MonoBehaviour
{
    public ParticleArray    particles;
    public Material         pointRendererMaterial;
    public float            size = 1.0f;
    public bool             index32 = false;

    List<Mesh>       meshes;

    void Start()
    {
        meshes = new List<Mesh>();

        int nParticles = particles.particles.Count;

        if (index32)
        {
            var indices = new int[nParticles];
            for (int j = 0; j < indices.Length; j++) indices[j] = j;

            var mesh = new Mesh();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(particles.particles);
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            mesh.UploadMeshData(true);

            meshes.Add(mesh);
        }
        else
        {
            int meshIndex = 0;
            int maxIndicesPerMesh = 65535;

            for (int i = 0; i < nParticles; i += maxIndicesPerMesh)
            {
                var newName = "Mesh" + meshIndex++;
                int c = Mathf.Min(maxIndicesPerMesh, nParticles - i);

                var indices = new int[c];
                for (int j = 0; j < c; j++) indices[j] = j;

                var mesh = new Mesh();

                mesh.name = newName;
                mesh.SetVertices(particles.particles.GetRange(i, c));

                if (maxIndicesPerMesh > 65535)
                {
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                }

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
}
