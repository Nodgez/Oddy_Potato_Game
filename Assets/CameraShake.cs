using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float strength = 1f;
    public Cinemachine.CinemachineVirtualCamera vrCam;
    // Start is called before the first frame update
    private void Start()
    {
        EventPool eventPool = GameObject.FindObjectOfType<EventPool>();
        eventPool.AddEventToPool("Potato_Hit", ShakeCamera);
    }

    private void ShakeCamera()
    {
        StartCoroutine(CO_ShakeCamera());
    }

    private IEnumerator CO_ShakeCamera()
    {
        var noise = vrCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = strength;
        yield return new WaitForSeconds(0.16f);
        noise.m_AmplitudeGain = 0;

    }
}
