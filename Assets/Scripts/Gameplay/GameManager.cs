using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ClientUDP_Script client;
    private ServerUDP_Script server;
    public DummyController[] dummies;
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
        dummies = FindObjectsOfType<DummyController>();
        client = FindObjectOfType<ClientUDP_Script>();
        server = FindObjectOfType<ServerUDP_Script>();
    }

    // Update is called once per frame
    public void UpdateData(Message message)
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

    public void UpdatePlayersData(Message message)
    {
        
        foreach (DummyController dummyController in dummies) 
        {
            if(dummyController.username == message.message) 
            {
                dummyController.UpdateDummy(message.characterData);
            }
        }
    }
}
