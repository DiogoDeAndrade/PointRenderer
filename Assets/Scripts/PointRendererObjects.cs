using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointRendererObjects : MonoBehaviour
{
    public ParticleArray    particles;
    public GameObject       pointPrefab;
    [Range(1.0f, 10.0f)]
    public float            scale = 1.0f;

    void Start()
    {
        if (particles == null) return;
        
        foreach (var p in particles.particles)
        {
            var newObject = Instantiate(pointPrefab, transform);
            newObject.transform.position = p.position;
            newObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
