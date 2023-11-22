using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitDummyManager : MonoBehaviour
{
    DummyController dummyController_;

    void Start()
    {
        dummyController_ = FindObjectOfType<DummyController>();
    }

    void Update()
    {
        // Check for entity life conditions, if needed.
    }

    public void TakeDamage(int damage, GameObject entity)
    {
        dummyController_.healthPoints -= damage;
        Debug.Log("Object has been shot: " + entity);

        // Check for entity life conditions, e.g., destroy the entity if life reaches zero.
        if (dummyController_.healthPoints <= 0)
        {
            dummyController_.healthPoints = 0;
            Destroy(entity);
        }
    }
}