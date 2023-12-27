using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 5f;

    private float t = 0f;

    private void Update()
    {
        // Calculate the position along the path based on time and speed
        t += speed * Time.deltaTime;
        t = Mathf.Clamp01(t); // Clamp t between 0 and 1

        // Move the object towards the current position along the path
        transform.position = Vector3.Lerp(pointA, pointB, t);

        // Check if the object has reached point B
        if (t >= 1f)
        {
            // Object has reached point B, reset t to 0 to restart the movement
            t = 0f;
        }
    }
}
