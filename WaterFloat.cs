using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    public float floatAmplitude = 0.2f;
    public float floatSpeed = 2f;

    public float tiltAmplitude = 3f;
    public float tiltSpeed = 1.5f;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        float tiltX = initialRotation.x + Mathf.Sin(Time.time * tiltSpeed) * tiltAmplitude;
        float tiltZ = initialRotation.z + Mathf.Cos(Time.time * tiltSpeed) * tiltAmplitude;

        transform.eulerAngles = new Vector3(tiltX, initialRotation.y, tiltZ);
    }
}