using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public int healthPoints;
    public string name;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float moveSpeed = 5f;

    public void UpdateDummy(CharacterData data)
    {
        characterData.transform.position = data.transform.position;
        characterData.transform.rotation = data.transform.rotation;
        characterData.transform.localScale = data.transform.localScale;

        characterData.HealthPoints = data.HealthPoints;

        characterData.actions = data.actions;
    }
}
