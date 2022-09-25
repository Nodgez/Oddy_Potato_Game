using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy_Potato : Potato
{
    [SerializeField] private int hitCount = 3;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Garbage"))
        {
            Destroy(this.gameObject);
            return;
        }

        scoringTrigger.IsTriggered = true;
        hitCount--;
        var pEffect = Instantiate(collectEffect, this.transform.position, Quaternion.identity);
        pEffect.Play();

        if (hitCount > 0)
            return;

        renderer.enabled = false;
        collider.enabled = false;

        Destroy(pEffect.gameObject, pEffect.main.duration);
        Destroy(this.gameObject);
    }
}
