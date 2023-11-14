using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.ExecuteOnMainThreadCoroutine(action));
        }
    }

    private IEnumerator ExecuteOnMainThreadCoroutine(Action action)
    {
        yield return null; // Wait for one frame to ensure we are on the main thread
        action.Invoke();
    }
}
