using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitManager : MonoBehaviour
{
    HP_Bar_Manager hp_Bar_Manager_;

    public int entityLife = 100; // Initial entity life.

    void Start()
    {
        hp_Bar_Manager_ = FindObjectOfType<HP_Bar_Manager>();
    }

    void Update()
    {
        // Check for entity life conditions, if needed.
    }

    public void TakeDamage(int damage, GameObject entity)
    {
        entityLife -= damage;
        Debug.Log("Object has been shot: " + entity);

        hp_Bar_Manager_.Change(-damage);
        
        if (entityLife <= 0)
        {
            hp_Bar_Manager_.Change(100);
        }

        // Check for entity life conditions, e.g., destroy the entity if life reaches zero.
        //if (entityLife <= 0)
        //{
        //    entityLife = 0;
        //    entity.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
        //    //Destroy(entity);
        //}
    }
}
