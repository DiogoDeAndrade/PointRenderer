using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PS/Particle System")]
public class ParticleArray : ScriptableObject
{
    [System.Serializable]
    public class Particle
    {
        public Vector3 position;
    }

    [HideInInspector] public List<Particle>   particles;
}
