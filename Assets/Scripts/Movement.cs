using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private float defaultSpeed = 0;
    public float Speed
    {
        get{ return speed; }
        set { speed = value; }
    }
    public float DefaultSpeed { 
        get { return defaultSpeed; }
    }

    private void Start()
    {
        defaultSpeed = speed;
    }

    public void MultiplySpeed(float multiplier)
    {
        this.speed *= multiplier;
    }

    // Update is called once per frame
    void Update()
    {
        var xAxis = Input.GetAxis("Horizontal");
        var delta = new Vector3(xAxis * speed, 0, 0) * Time.deltaTime;
        transform.position += delta;

        var currentScale = transform.localScale.x;
        var targetScale = Sign(xAxis) == 0 ? currentScale : Sign(xAxis) < 0 ? 1 : -1;
        var newScale = currentScale + (targetScale * Time.deltaTime * speed);
        transform.localScale = new Vector3(Mathf.Clamp(newScale, -1f, 1f), 1, 1);

        var leftRotation = Quaternion.Euler(0, 0, -5f);
        var rightRotation = Quaternion.Euler(0, 0, 5f);
        transform.rotation = Quaternion.Slerp(leftRotation, rightRotation, xAxis * 0.5f + 0.5f);

        var vpPos = Camera.main.WorldToViewportPoint(transform.position);

        if (vpPos.x < -0.1f)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1.09f, 0, 10));
        if (vpPos.x > 1.1f)
            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.09f, 0, 10));
    }

    int Sign(float val)
    {
        if (Mathf.Approximately(0, val))
            return 0;
        else if (val > 0)
            return 1;
        else
            return -1;
    }
}
