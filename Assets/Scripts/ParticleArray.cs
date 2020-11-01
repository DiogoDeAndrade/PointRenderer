using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PS/Particle System")]
public class ParticleArray : ScriptableObject
{
    [HideInInspector] public List<Vector3>   particles;
}
