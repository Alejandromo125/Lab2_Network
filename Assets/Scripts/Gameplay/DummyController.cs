using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public int healthPoints;
    public string username;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float moveSpeed = 5f;

    public void UpdateDummy(CharacterData data)
    {
        characterData.position = data.position;
        characterData.rotation = data.rotation;

        characterData.HealthPoints = data.HealthPoints;

        characterData.actions = data.actions;

        UpdateActualData();
    }

    public void UpdateActualData()
    {
        Debug.Log("Updating dummy data");
        transform.position = characterData.position;
        transform.rotation = characterData.rotation;
        healthPoints = characterData.HealthPoints;

        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);

    }
}
