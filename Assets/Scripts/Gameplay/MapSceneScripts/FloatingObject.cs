using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatHeight = 1.0f; // Height of the floating movement.
    public float floatSpeed = 1.0f; // Speed of the floating movement.
    public bool syncOffset = false; // Checkbox to sync the offset delay.

    private float originalY;
    private float randomOffsetDelay;

    void Start()
    {
        originalY = transform.position.y;

        if (!syncOffset)
        {
            // Randomize the delay offset for each object.
            randomOffsetDelay = Random.Range(0.0f, 1.0f);
        }
    }

    void Update()
    {
        float newY = originalY + Mathf.Sin((Time.time + randomOffsetDelay) * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
