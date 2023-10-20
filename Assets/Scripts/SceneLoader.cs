using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadServerScene()
    {
        SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
    }

    public void LoadClientScene()
    {
        SceneManager.LoadScene("ClientScene", LoadSceneMode.Additive);
    }

    public void LoadDesiredScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
