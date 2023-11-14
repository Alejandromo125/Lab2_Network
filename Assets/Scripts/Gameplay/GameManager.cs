using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ClientUDP_Script client;
    private ServerUDP_Script server;
    public PlayerController[] players;
    public static GameManager instance { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        players = FindObjectsOfType<PlayerController>();
        client = FindObjectOfType<ClientUDP_Script>();
        server = FindObjectOfType<ServerUDP_Script>();
    }

    // Update is called once per frame
    private void UpdateData(Message message)
    {
        if(server)
        {
            server.HandleSendingMessages(message);
        }
        if(client)
        {
            client.SendMessageGameplay(message);
        }
    }

    public void UpdatePlayersData(PlayerController player)
    {
        foreach (PlayerController playerController in players) 
        {
            if(playerController != player) 
            {
                
            }
        }
    }
}
