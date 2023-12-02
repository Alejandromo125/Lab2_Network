using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitManager : MonoBehaviour
{
    public int entityLife = 100; // Initial entity life.

    void Start()
    {
        // Initialization, if needed.
    }

    void Update()
    {
        // Check for entity life conditions, if needed.
    }

    public void TakeDamage(int damage, GameObject entity)
    {
        entityLife -= damage;
        Debug.Log("Object has been shot: " + entity);

        // Check for entity life conditions, e.g., destroy the entity if life reaches zero.
        //if (entityLife <= 0)
        //{
        //    entityLife = 0;
        //    entity.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
        //    //Destroy(entity);
        //}
    }
}
