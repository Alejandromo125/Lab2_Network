using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreserveObject : MonoBehaviour
{
    public string specifiedSceneName; // Set this in the Inspector to the specified scene name.
    private AudioSource audioSource;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != gameObject.scene.name && scene.name != specifiedSceneName)
        {
            Destroy(audioSource.gameObject);
            Destroy(gameObject);
        }
    }

    // Use this method to load a new scene
    public void LoadNewScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
