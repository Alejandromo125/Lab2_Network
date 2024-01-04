using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject serverPrefab;

    private ServerUDP_Script server;
    private ClientUDP_Script client;

    public AudioMixerSnapshot musicUnmutedSnapshot;
    public AudioMixerSnapshot musicMutedSnapshot;
    public AudioMixerSnapshot soundUnmutedSnapshot;
    public AudioMixerSnapshot soundMutedSnapshot;

    bool isMusicEnabled = true;
    bool isSoundEnabled = true;

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

    // Function to toggle the music on/off using a bool parameter.
    public void ToggleMusic(bool togleState)
    {
        isMusicEnabled = togleState;

        if (isMusicEnabled)
            musicUnmutedSnapshot.TransitionTo(0.1f); // Transition to the unmuted music snapshot.
        else
            musicMutedSnapshot.TransitionTo(0.1f); // Transition to the muted music snapshot.
    }
    // Function to toggle the sfx on/off using a bool parameter.
    public void ToggleSound(bool togleState)
    {
        isSoundEnabled = togleState;

        if (isSoundEnabled)
            soundUnmutedSnapshot.TransitionTo(0.1f); // Transition to the unmuted sound snapshot.
        else
            soundMutedSnapshot.TransitionTo(0.1f); // Transition to the muted sound snapshot.
    }
}
