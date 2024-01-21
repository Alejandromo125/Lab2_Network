using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialObjectTeleporter : MonoBehaviour
{
    public GameObject objectToTeleport;
    public GameObject[] teleportPoints; // Assign teleport point objects in the Inspector.
    public float teleportDelay = 2f; // Time delay between teleports.
    public float teleportDuration = 2f; // Time duration for teleporting.

    private int currentIndex = 0;

    void Start()
    {
        InvokeRepeating(nameof(TeleportObject), 0f, teleportDelay);
    }

    void TeleportObject()
    {
        if (currentIndex < teleportPoints.Length)
        {
            Vector3 teleportPosition = teleportPoints[currentIndex].transform.position;

            StartCoroutine(TeleportObjectCoroutine(objectToTeleport.transform, teleportPosition, teleportDuration));
            currentIndex++;
        }
        else
        {
            currentIndex = 0; // Reset index to go back to the first teleport point.
        }
    }

    IEnumerator TeleportObjectCoroutine(Transform objectToTeleport, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToTeleport.position;

        while (elapsedTime < seconds)
        {
            objectToTeleport.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToTeleport.position = end;
    }
}
