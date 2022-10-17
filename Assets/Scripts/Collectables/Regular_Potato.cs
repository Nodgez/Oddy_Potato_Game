using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regular_Potato : Potato
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Garbage"))
        {
            onPotatoDestroyed?.Invoke();
            Destroy(this.gameObject);
            return;
        }
        scoringTrigger.IsTriggered = true;

        renderer.enabled = false;
        collider.enabled = false;

        var pEffect = Instantiate(collectEffect, this.transform.position, Quaternion.identity);
        pEffect.Play();
        
        Destroy(pEffect.gameObject, pEffect.main.duration);
        Destroy(this.gameObject, 0.1f);

        onPotatoDestroyed?.Invoke();
    }

}
