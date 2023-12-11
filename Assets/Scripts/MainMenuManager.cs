using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject serverPrefab;

    private ServerUDP_Script server;
    private ClientUDP_Script client;

    private void Awake()
    {
        server = FindObjectOfType<ServerUDP_Script>();
        client= FindObjectOfType<ClientUDP_Script>();

        if (server)
            server.DestroyServer();
        if (client)
            client.DestroyClient();
    }
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
