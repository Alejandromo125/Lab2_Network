using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject serverPrefab;

    public void HostGame()
    {
        SceneManager.LoadScene("ServerScene");
        Instantiate(serverPrefab);

    }
    public void JoinGame()
    {
        SceneManager.LoadScene("ClientScene");

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
