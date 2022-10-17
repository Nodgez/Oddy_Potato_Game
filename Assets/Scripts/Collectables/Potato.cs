using System;
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

    public Action onPotatoDestroyed;

    protected virtual void Awake()
    {
        eventPool = GameObject.FindObjectOfType<EventPool>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        var vpPos = Camera.main.WorldToViewportPoint(transform.position);

        if (vpPos.x < -0.1f)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.09f, vpPos.y, 10));
        if (vpPos.x > 1.1f)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.09f, vpPos.y, 10));
    }
}
