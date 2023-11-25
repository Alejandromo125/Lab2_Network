using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void HostGame()
    {
        SceneManager.LoadScene("ServerScene");
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
