using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Potato : MonoBehaviour
{
    [SerializeField] protected Trigger scoringTrigger;
    [SerializeField] protected ParticleSystem collectEffect;

    protected new SpriteRenderer renderer;
    protected new Collider2D collider;

    protected EventPool eventPool;

    protected virtual void Awake()
    {
        eventPool = GameObject.FindObjectOfType<EventPool>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }
}
