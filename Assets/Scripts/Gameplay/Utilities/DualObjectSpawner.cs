using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public GameObject[] spawnPoints; // Assign spawn point objects in the Inspector.
    public float spawnDelay = 2f; // Time delay between spawns.
    public float swapDuration = 3f; // Time duration for swapping positions.

    private void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (spawnPoints.Length < 2)
        {
            Debug.LogError("Insufficient spawn points. You need at least two spawn points.");
            return;
        }

        int indexA = Random.Range(0, spawnPoints.Length);
        int indexB;

        do
        {
            indexB = Random.Range(0, spawnPoints.Length);
        } while (indexB == indexA);

        Vector3 positionA = spawnPoints[indexA].transform.position;
        Vector3 positionB = spawnPoints[indexB].transform.position;

        Instantiate(objectToSpawn, positionA, Quaternion.identity);
        Instantiate(objectToSpawn, positionB, Quaternion.identity);

        InvokeRepeating(nameof(SwapObjects), spawnDelay, spawnDelay);
    }

    void SwapObjects()
    {
        int indexA = Random.Range(0, spawnPoints.Length);
        int indexB;

        do
        {
            indexB = Random.Range(0, spawnPoints.Length);
        } while (indexB == indexA);

        Vector3 positionA = spawnPoints[indexA].transform.position;
        Vector3 positionB = spawnPoints[indexB].transform.position;

        StartCoroutine(MoveOverSeconds(objectToSpawn.transform, positionA, swapDuration));
        StartCoroutine(MoveOverSeconds(objectToSpawn.transform, positionB, swapDuration));
    }

    IEnumerator MoveOverSeconds(Transform objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.position;

        while (elapsedTime < seconds)
        {
            objectToMove.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.position = end;
    }
}
