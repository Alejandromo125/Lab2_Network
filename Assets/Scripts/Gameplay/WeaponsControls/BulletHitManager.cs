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

    public void TakeDamage(int damage, GameObject entity)
    {
        entityLife -= damage;
        Debug.Log("Object has been shot: " + entity);

        hp_Bar_Manager_.Change(-damage);

        if (entityLife <= 0)
        {
            hp_Bar_Manager_.Change(100);
        }
    }
}
