using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimationEvent : MonoBehaviour
{
    public void Play(ParticleSystem particle)
    {
        Instantiate(particle, transform);
    }    
}
