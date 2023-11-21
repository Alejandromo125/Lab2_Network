using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private ClientUDP_Script client;
    private ServerUDP_Script server;
    public List<DummyController> dummies;

    public GameObject playerPrefab;
    public GameObject dummyPrefab;

    public Vector3 startingPlayerPos;
    public Vector3 startingDummyPos;
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


    public void CreatePlayerAndDummy(string playerName,string dummyName)
    {
       GameObject player =  Instantiate(playerPrefab,startingPlayerPos,Quaternion.identity,null);
       player.GetComponent<PlayerController>().username = playerName;

       GameObject dummy = Instantiate(dummyPrefab,startingDummyPos,Quaternion.identity,null);
       dummy.GetComponent<DummyController>().username = dummyName;
       dummies.Add(dummy.GetComponent<DummyController>());
    }
}
