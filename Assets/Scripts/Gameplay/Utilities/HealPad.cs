using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPad : MonoBehaviour
{
    public GameObject healPadPrefab;
    private float timer;
    private float timer_2;
    private float timer_3;
    private float timer_4;
    private float timer_5;
    private float lastTimer;

    private float TimeTimer = 0f;
    private float startTime = 0f;

    void Start()
    {
       timer = 75f;
       timer_2 = 60f;
       timer_3 = 45f;
       timer_4 = 30f;
       timer_5 = 15f;

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        TimeTimer = Time.time - startTime;

        if (TimeTimer - lastTimer > timer_5)
        {
            Vector3 center = new Vector3(7.38f, 0.46f, -8.11f);
            healPadPrefab.transform.position = center;
        }

        if (TimeTimer - lastTimer > timer_4)
        {
            Vector3 center = new Vector3(2.23f, 0.46f, -13.14f);
            healPadPrefab.transform.position = center;
        }

        if (TimeTimer - lastTimer > timer_3)
        {
            Vector3 center = new Vector3(15.58f, 0.46f, -2.38f);
            healPadPrefab.transform.position = center;
        }

        if (TimeTimer - lastTimer > timer_2)
        {
            Vector3 outOfMap = new Vector3(2.07f, 0.46f, -2.7f);
            healPadPrefab.transform.position = outOfMap;
        }

        if (TimeTimer - lastTimer > timer)
        {
            lastTimer = TimeTimer;

            Vector3 center = new Vector3(15.49f, 0.46f, -10.7f);
            healPadPrefab.transform.position = center;
        }
    }
}
