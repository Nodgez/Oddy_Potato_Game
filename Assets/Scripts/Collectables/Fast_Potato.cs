using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fast_Potato : Potato
{
    public float multiplier = 2f;
    public float duration = 2f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Garbage"))
        {
            Destroy(this.gameObject);
            return;
        }
        var movement = collision.GetComponent<Movement>();
        if (movement == null)
            return;

        var pEffect = Instantiate(collectEffect, this.transform.position, Quaternion.identity);
        pEffect.Play();
        movement.StartCoroutine(SpeedUp(movement));
        Destroy(this.gameObject);
    }

    private IEnumerator SpeedUp(Movement movement)
    {
        var ogSpeed = movement.DefaultSpeed;
        var newSpeed = movement.Speed * multiplier;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            movement.Speed = Mathf.Lerp(newSpeed, ogSpeed, t);
            yield return null;
        }

        movement.Speed = movement.DefaultSpeed;
    }
}
