using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Potato : MonoBehaviour
{
    [SerializeField] protected Trigger scoringTrigger;
    [SerializeField] protected ParticleSystem collectEffect;

    protected EventPool eventPool;

    protected virtual void Awake()
    {
        eventPool = GameObject.FindObjectOfType<EventPool>();
    }
}
